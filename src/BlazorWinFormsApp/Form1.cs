// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebView.WindowsForms;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;
using WebviewAppShared;

namespace BlazorWinFormsApp
{
    public partial class Form1 : System.Windows.Forms.Form
    {

        public Form1()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddWindowsFormsBlazorWebView();

            serviceCollection
                    .AddBlazorise(options =>
                    {
                        options.Immediate = true;
                    })
                    .AddBootstrapProviders()
                    .AddFontAwesomeIcons();

            InitializeComponent();

            blazorWebView1.HostPage = @"wwwroot\index.html";
            blazorWebView1.Services = serviceCollection.BuildServiceProvider();
            blazorWebView1.RootComponents.Add<App>("#app");
        }
    }

    //Helpful guide on Winforms: https://docs.microsoft.com/en-us/aspnet/core/blazor/hybrid/tutorials/windows-forms?view=aspnetcore-6.0
}
