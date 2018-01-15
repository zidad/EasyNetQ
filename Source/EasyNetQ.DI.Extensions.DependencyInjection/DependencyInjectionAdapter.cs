using System;
using Microsoft.Extensions.DependencyInjection;

namespace EasyNetQ.DI
{
    public class DependencyInjectionAdapter : IContainer, IDisposable
    {
        readonly IServiceCollection serviceCollection;
        ServiceProvider container;

        public DependencyInjectionAdapter(IServiceCollection serviceCollection = null)
        {
            this.serviceCollection = serviceCollection ?? new ServiceCollection();
            this.serviceCollection.AddSingleton(this);
        }

        public ServiceProvider Container 
        { 
            get => container;
            set => container = value;
        }

        public IServiceRegister Register<TService>(Func<IServiceProvider, TService> serviceCreator) where TService : class
        {
            if (serviceCreator == null)  throw new ArgumentNullException("serviceCreator");
            serviceCollection.AddSingleton<TService>(serviceCreator(this));
            container = serviceCollection.BuildServiceProvider();
            return this;
        }

        public IServiceRegister Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            serviceCollection.AddSingleton<TService, TImplementation>();
            container = serviceCollection.BuildServiceProvider();
            return this;
        }

        public TService Resolve<TService>() where TService : class
        {
            return (TService) Container.GetService(typeof(TService));
        }

        public void Dispose()
        {
            container?.Dispose();
        }
    }
}
