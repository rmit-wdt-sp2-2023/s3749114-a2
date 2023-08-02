using Autofac;
using CustomerApplication.Services;
using BankLibrary.Models;

namespace CustomerApplication.Tests.Modules;

public class BackendModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);

        // Register types for DI (Dependency Injection).

        //builder.RegisterInstance<AccountService>(new AccountService);
        //builder.RegisterType<AccountService>();
    }
}


