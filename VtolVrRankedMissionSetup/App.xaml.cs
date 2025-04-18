﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.UI.Xaml;
using VtolVrRankedMissionSetup.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VtolVrRankedMissionSetup
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        static App()
        {
            Services = CreateServices();
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();
            m_window.Activate();
        }

        private static IServiceProvider CreateServices()
        {
            ServiceCollection collection = new();

            IEnumerable<Type> services = typeof(App).Assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<ServiceAttribute>() != null);

            foreach (Type service in services)
            {
                ServiceAttribute attr = service.GetCustomAttribute<ServiceAttribute>()!;

                collection.Add(new ServiceDescriptor(attr.InterfaceType ?? service, service, attr.Lifetime));
            }

            return collection.BuildServiceProvider();
        }

        private Window? m_window;
    }
}
