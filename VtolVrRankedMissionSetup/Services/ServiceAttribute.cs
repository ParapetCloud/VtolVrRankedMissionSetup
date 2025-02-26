using Microsoft.Extensions.DependencyInjection;
using System;

namespace VtolVrRankedMissionSetup.Services
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ServiceAttribute : Attribute
    {
        public Type? InterfaceType { get; set; }
        public ServiceLifetime Lifetime { get; }

        public ServiceAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}
