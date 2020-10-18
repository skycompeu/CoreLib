using CoreLib.DependencyInjection;
using CoreLib.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Di
{

    public class Container
    {

        ServiceProvider _provider;
        ServiceCollection _services;

        public Container()
        {
            _services = new ServiceCollection();
            _provider = _services.BuildServiceProvider();
        }

        public object GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }

        public TService GetService<TService>()
        {
            return _provider.GetService<TService>();
        }

        /// <summary>
        /// Scoped lifetime services are created once per request within the scope. It is equivalent to a singleton in the current scope. For example, in MVC it creates one instance for each HTTP request, but it uses the same instance in the other calls within the same web request.
        /// </summary>
        public void AddScoped(Type serviceType)
        {
            _services.AddScoped(serviceType);
            Build();
        }

        /*        public void AddScoped<TClass>() {
                    _services.AddScoped(typeof(TClass));
                    Build();
                }*/

        public void AddScoped<TClass>()
        {
            _services.AddScoped(typeof(TClass));
            Build();
        }

        /// <summary>
        /// Scoped lifetime services are created once per request within the scope. It is equivalent to a singleton in the current scope. For example, in MVC it creates one instance for each HTTP request, but it uses the same instance in the other calls within the same web request.
        /// </summary>
        public void AddScoped<TService, TClassImpl>()
           where TService : class
           where TClassImpl : class, TService
        {
            _services.AddScoped<TService, TClassImpl>();
            Build();
        }


        /// <summary>
        /// Transient lifetime services are created each time they are requested. This lifetime works best for lightweight, stateless services.
        /// </summary>
        public void AddTransient(Type serviceType)
        {
            _services.AddTransient(serviceType);
            Build();
        }

        /// <summary>
        /// Transient lifetime services are created each time they are requested. This lifetime works best for lightweight, stateless services.
        /// </summary>
        /// <typeparam name="TService">Interfejs</typeparam>
        /// <typeparam name="TImplementation">Klasa</typeparam>
        public void AddTransient<TService, TClassImpl>()
           where TService : class
           where TClassImpl : class, TService
        {
            _services.AddTransient<TService, TClassImpl>();
            Build();
        }

        /// <summary>
        /// Singleton which creates a single instance throughout the application. It creates the instance for the first time and reuses the same object in the all calls.
        /// </summary>
        public void AddSingleton(Type serviceType)
        {
            _services.AddSingleton(serviceType);
            Build();
        }

        /// <summary>
        /// Singleton which creates a single instance throughout the application. It creates the instance for the first time and reuses the same object in the all calls.
        /// </summary>
        /// <typeparam name="TService">Interfejs</typeparam>
        /// <typeparam name="TImplementation">Klasa</typeparam>
        public void AddSingleton<TService, TClassImpl>()
           where TService : class
           where TClassImpl : class, TService
        {
            _services.AddSingleton<TService, TClassImpl>();
            Build();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="module"></param>
        public void RegisterModule(Module module)
        {
            if (_services.IsNotNull())
            {
                if (module.IsNotNull())
                {
                    module.RegisterServices(this);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void Build()
        {
            if (_services.IsNotNull())
            {
                _provider = _services.BuildServiceProvider();
            }
        }
    }
}
