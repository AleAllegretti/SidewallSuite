﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BeltsPack
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Syncfusion licence
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjE4MjIwQDMyMzAyZTMxMmUzMGpzM0x4STQyOEJtNXNZY3lhMHF0OXJJTTlLK085QVBoZDNVSnVLeUdQdkk9");
        }
    }
}
