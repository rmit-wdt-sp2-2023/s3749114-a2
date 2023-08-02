using Autofac;

namespace CustomerApplication.Tests.Base;

public abstract class BaseTest : IDisposable
{
    private static readonly object s_containerLock = new();

    private IContainer _container;

    protected ContainerBuilder Builder { get; }

    protected BaseTest() => Builder = new ContainerBuilder();

    protected IContainer Container
    {
        get
        {
            if (_container == null)
                BuildContainer();
            return _container;
        }
    }

    private void BuildContainer()
    {
        lock(s_containerLock)
        {
            _container ??= Builder.Build();
        }
    }

    public virtual void Dispose()
    {
        Container.Dispose();
        GC.SuppressFinalize(this);
    }
}