﻿using Wheatech.Modulize.Samples.Settings.Services;

namespace Wheatech.Modulize.Samples.Settings.WebForms
{
    public class Startup
    {
        public void Configuration(ISettingsService service)
        {
            service.RegisterField(new SettingsField { Key = "DefaultPassword", Name = "Default Password" });
        }
    }
}