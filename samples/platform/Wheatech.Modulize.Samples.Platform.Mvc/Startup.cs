﻿using Wheatech.Activation;
using Wheatech.Modulize.SQLite;

namespace Wheatech.Modulize.Samples.Platform.Mvc
{
    public class Startup
    {
        public void ConfigurationCompleted(IActivatingEnvironment environment, IModuleConfiguration configuration, IModuleContainer container)
        {
            configuration.PersistWithSQLite("App_Data/modulize.db");
            container.Start(environment);
        }
    }
}