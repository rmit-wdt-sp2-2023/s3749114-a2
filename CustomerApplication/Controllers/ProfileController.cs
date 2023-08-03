using System.ComponentModel.DataAnnotations;
using CustomerApplication.Filters;
using BankLibrary.Models;
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

    public IActionResult Index() => View(ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));

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

        ViewBag.DisplayChangePasswordSuccess = true;

        return View(nameof(Index), ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));
    }

    public IActionResult UploadProfilePicture() => View();

    [HttpPost]
    public IActionResult SubmitUploadProfilePicture(UploadProfilePictureViewModel uploadPictureVM)
    {
        ValidationResult error = _customerService.UploadProfilePicture(
            CustomerID, uploadPictureVM.ProfileImage);

        if (error is not null)
            ModelState.AddModelError(error.MemberNames.First(), error.ErrorMessage);

        if (!ModelState.IsValid)
            return View(nameof(UploadProfilePicture), uploadPictureVM);

        ViewBag.DisplayUploadProfilePictureSuccess = true;

        return View(nameof(Index), ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));
    }

    public IActionResult RemoveProfilePicture()
    {
        List<ValidationResult> errors = _customerService.RemoveProfilePicture(CustomerID);

        if (errors is not null)
            return View(nameof(Index), ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));

        ViewBag.DisplayRemoveProfilePictureSuccess = true;

        return View(nameof(Index), ViewModelMapper.Profile(_customerService.GetCustomer(CustomerID)));
    }

    public ActionResult ProfilePicture()
    {
        ProfilePicture profilePicture = _customerService.GetProfilePicture(CustomerID);

        return new FileContentResult(profilePicture.Image, profilePicture.ContentType)
        {
            FileDownloadName = profilePicture.FileName
        };
    }
}