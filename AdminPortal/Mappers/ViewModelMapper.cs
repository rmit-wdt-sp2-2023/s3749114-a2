using AdminPortal.ViewModels;
using BankLibrary.Models;

namespace AdminPortal.Mappers;

public static class ViewModelMapper
{
    public static LoginProfileViewModel LoginProfile(Login login)
    {
        return new LoginProfileViewModel()
        {
            LoginID = login.LoginID,
            CustomerID = login.CustomerID,
            LoginStatus = login.LoginStatus
        };
    }

}

