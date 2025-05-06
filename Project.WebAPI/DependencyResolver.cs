using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Ninject;

namespace Project.WebAPI
{
    public class DependencyResolver : IServiceProvider
    {
        private readonly IKernel _kernel;

        public DependencyResolver(IKernel kernel)
        {
            _kernel = kernel;
        }

        public object GetService(Type serviceType)
        {
            try
            {
                return _kernel.Get(serviceType);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public static class NinjectExtensions
    {
        public static void AddNinject(this IServiceCollection services, IKernel kernel)
        {
            services.AddSingleton<IServiceProvider>(new DependencyResolver(kernel));
        }
    }
} 