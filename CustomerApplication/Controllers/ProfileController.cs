using System.ComponentModel.DataAnnotations;
using CustomerApplication.Filters;
using CustomerApplication.Models;
using CustomerApplication.Services;
using CustomerApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CustomerApplication.Mappers;

namespace CustomerApplication.Controllers;

[AuthorizeCustomer]
public class ProfileController : Controller
{
    private readonly CustomerService _customerService;

    private readonly LoginService _loginService;

    private int CustomerID => HttpContext.Session.GetInt32(nameof(Customer.CustomerID)).Value;

    public ProfileController(CustomerService customerService, LoginService loginService)
    {
        _customerService = customerService;
        _loginService = loginService;
    }

    public IActionResult Index()
    {
        ViewBag.DisplayEditDetailsSuccess = false;
        ViewBag.DisplayChangePasswordSuccess = false;
        ViewBag.DisplayUploadProfilePictureSuccess = false;
        ViewBag.DisplayRemoveProfilePictureSuccess = false;

        return View(ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));
    }

    public IActionResult EditDetails() => View(ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));

    [HttpPost]
    public IActionResult SubmitEditDetails(ProfileViewModel profileVM)
    {
        List<ValidationResult> errors = _customerService.UpdateCustomer(
            CustomerID, profileVM.Name, profileVM.TFN, profileVM.Address,
            profileVM.City, profileVM.State, profileVM.PostCode, profileVM.Mobile);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(EditDetails), profileVM);

        ViewBag.DisplayEditDetailsSuccess = true;
        ViewBag.DisplayChangePasswordSuccess = false;
        ViewBag.DisplayUploadProfilePictureSuccess = false;
        ViewBag.DisplayRemoveProfilePictureSuccess = false;

        return View(nameof(Index), ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));
    }

    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [HttpPost]
    public IActionResult SubmitChangePassword(ChangePasswordViewModel changePassVM)
    {
        List<ValidationResult> errors = _loginService.ChangePassword(
            CustomerID, changePassVM.OldPassword, changePassVM.NewPassword, changePassVM.ConfirmPassword);

        if (errors is not null)
            foreach (ValidationResult e in errors)
                ModelState.AddModelError(e.MemberNames.First(), e.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(ChangePassword), changePassVM);

        ViewBag.DisplayEditDetailsSuccess = false;
        ViewBag.DisplayChangePasswordSuccess = true;
        ViewBag.DisplayUploadProfilePictureSuccess = false;
        ViewBag.DisplayRemoveProfilePictureSuccess = false;

        return View(nameof(Index), ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));
    }

    public IActionResult UploadProfilePicture() => View();

    [HttpPost]
    public IActionResult SubmitUploadProfilePicture(UploadProfilePictureViewModel uploadProfilePictureVM)
    {
        (ValidationResult error, string fileName) = _customerService.UploadProfilePicture(
            CustomerID, uploadProfilePictureVM.ProfileImage);

        if (error is not null)
            ModelState.AddModelError(error.MemberNames.First(), error.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(UploadProfilePicture), uploadProfilePictureVM);

        HttpContext.Session.SetString(nameof(Customer.ProfilePicture), fileName);

        ViewBag.DisplayEditDetailsSuccess = false;
        ViewBag.DisplayChangePasswordSuccess = false;
        ViewBag.DisplayUploadProfilePictureSuccess = true;
        ViewBag.DisplayRemoveProfilePictureSuccess = false;

        return View(nameof(Index), ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));
    }

    public IActionResult RemoveProfilePicture()
    {
        List<ValidationResult> errors = _customerService.RemoveProfilePicture(CustomerID);

        if (errors is not null)
        {
            ViewBag.DisplayEditDetailsSuccess = false;
            ViewBag.DisplayChangePasswordSuccess = false;
            ViewBag.DisplayUploadProfilePictureSuccess = false;
            ViewBag.DisplayRemoveProfilePictureSuccess = false;

            return View(nameof(Index), ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));

        }

        HttpContext.Session.Remove(nameof(Customer.ProfilePicture));

        ViewBag.DisplayEditDetailsSuccess = false;
        ViewBag.DisplayChangePasswordSuccess = false;
        ViewBag.DisplayUploadProfilePictureSuccess = false;
        ViewBag.DisplayRemoveProfilePictureSuccess = true;

        return View(nameof(Index), ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));
    }
}