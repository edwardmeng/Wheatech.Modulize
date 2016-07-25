using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;
using Wheatech.Modulize.Samples.Platform.Common;

namespace Wheatech.Modulize.Samples.Platform.WebForms
{
    public partial class Modules : System.Web.UI.Page
    {
        private bool RequireRestart
        {
            get { return (bool?)ViewState["RequiresRestart"] ?? false; }
            set { ViewState["RequiresRestart"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadModules();
                LoadFeatures();
            }
        }

        private void LoadModules()
        {
            repeaterModules.DataSource = Modulizer.GetModules().OrderBy(module => module.ModuleName, StringComparer.CurrentCultureIgnoreCase);
            repeaterModules.DataBind();
        }

        private void LoadFeatures()
        {
            repeaterFeatures.DataSource = Modulizer.GetFeatures().OrderBy(feature => feature.FeatureName, StringComparer.CurrentCultureIgnoreCase);
            repeaterFeatures.DataBind();
        }

        protected string GetModuleErrors(ModuleErrors errors)
        {
            if (errors.HasFlag(ModuleErrors.IncompatibleHost))
            {
                return "The module version is incompatible with hosting application.";
            }
            if (errors.HasFlag(ModuleErrors.ForbiddenFeatures))
            {
                return "All the features underlying the module have been forbidden.";
            }
            return null;
        }

        protected string GetFeatureErrors(FeatureDescriptor feature, FeatureErrors errors)
        {
            if (errors.HasFlag(FeatureErrors.ForbiddenModule))
            {
                return $"The module '{feature.Module.ModuleName}' has been forbidden.";
            }
            if (errors.HasFlag(FeatureErrors.UninstallModule))
            {
                return $"The module '{feature.Module.ModuleName}' has not been installed.";
            }
            if (errors.HasFlag(FeatureErrors.MissingDependency))
            {
                var missingDependencies = from dependency in feature.Dependencies
                                          where Modulizer.GetFeature(dependency.FeatureId) == null
                                          select dependency.FeatureId;
                return $"The dependency features are missing: {string.Join(", ", missingDependencies)}";
            }
            if (errors.HasFlag(FeatureErrors.ForbiddenDependency))
            {
                var forbiddenDependencies = from dependency in feature.Dependencies
                                            let dependencyFeature = Modulizer.GetFeature(dependency.FeatureId)
                                            where dependencyFeature != null && dependencyFeature.Errors != FeatureErrors.None
                                            select dependencyFeature?.FeatureName;
                return $"The dependency features have been forbidden: {string.Join(", ", forbiddenDependencies)}";
            }
            if (errors.HasFlag(FeatureErrors.IncompatibleDependency))
            {
                var incompatibleDependencies = from dependency in feature.Dependencies
                                               let dependencyFeature = Modulizer.GetFeature(dependency.FeatureId)
                                               where dependencyFeature != null && dependencyFeature.Errors == FeatureErrors.None && dependency.Version != null && !dependency.Version.Match(dependencyFeature.Module.ModuleVersion)
                                               select dependencyFeature?.FeatureName;
                return $"The dependency features are incompatible: {string.Join(", ", incompatibleDependencies)}";
            }
            if (errors.HasFlag(FeatureErrors.DisabledDependency))
            {
                var disabledDependencies = from dependency in feature.Dependencies
                                           let dependencyFeature = Modulizer.GetFeature(dependency.FeatureId)
                                           where dependencyFeature != null && dependencyFeature.EnableState == FeatureEnableState.RequireEnable && dependencyFeature.Errors == FeatureErrors.None
                                           select dependencyFeature?.FeatureName;
                return $"The dependency features have not been enabled: {string.Join(", ", disabledDependencies)}";
            }
            return null;
        }

        protected void repeaterModules_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var module = (ModuleDescriptor)e.Item.DataItem;
                var panelFeatures = e.Item.FindControl("panelFeatures");
                var linkLicense = (HyperLink)e.Item.FindControl("linkLicense");
                var labelLicense = e.Item.FindControl("labelLicense");
                linkLicense.Visible = false;
                labelLicense.Visible = false;
                if (!string.IsNullOrWhiteSpace(module.License))
                {
                    if (module.License.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || module.License.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    {
                        linkLicense.NavigateUrl = module.License;
                        linkLicense.Visible = true;
                    }
                    else
                    {
                        LicenseData license;
                        if (LicenseHelper.Matches(module.License,out license))
                        {
                            linkLicense.NavigateUrl = license.Url;
                            linkLicense.Text = license.ShortName;
                            linkLicense.ToolTip = license.FullName;
                            linkLicense.Visible = true;
                        }
                        else
                        {
                            labelLicense.Visible = true;
                        }
                    }
                }
                if (module.Features.Count == 0 || (module.Features.Count == 1 && module.Features[0].FeatureId == module.ModuleId))
                {
                    panelFeatures.Visible = false;
                }
                else
                {
                    var repeaterModuleFeatures = (Repeater)e.Item.FindControl("repeaterModuleFeatures");
                    repeaterModuleFeatures.DataSource = module.Features;
                    repeaterModuleFeatures.DataBind();
                }
            }
        }

        protected void repeaterModules_OnItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var moduleId = e.CommandArgument.ToString();
            switch (e.CommandName)
            {
                case "Install":
                    Modulizer.InstallModules(moduleId);
                    break;
                case "Uninstall":
                    Modulizer.UninstallModules(moduleId);
                    break;
            }
            RequireRestart = true;
            LoadModules();
            LoadFeatures();
        }

        protected void repeaterFeatures_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var feature = (FeatureDescriptor)e.Item.DataItem;
                var repeaterFeatureDependencies = (Repeater)e.Item.FindControl("repeaterFeatureDependencies");
                repeaterFeatureDependencies.DataSource = feature.Dependencies;
                repeaterFeatureDependencies.DataBind();
            }
        }

        protected void repeaterFeatures_OnItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var featureId = e.CommandArgument.ToString();
            switch (e.CommandName)
            {
                case "Enable":
                    Modulizer.EnableFeatures(featureId);
                    break;
                case "Disable":
                    Modulizer.DisableFeatures(featureId);
                    break;
            }
            LoadModules();
            LoadFeatures();
        }

        protected void buttonRestart_OnClick(object sender, EventArgs e)
        {
            RequireRestart = false;
            typeof(HttpRuntime).GetMethod("ShutdownAppDomain", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public, null,
                new[] { typeof(ApplicationShutdownReason), typeof(string) }, null).Invoke(null, new object[] { ApplicationShutdownReason.ChangeInGlobalAsax, "Change in modules" });
            ClientScript.RegisterStartupScript(typeof(Modules), "Restarted", $"window.setTimeout(function(){{document.getElementById('{buttonRefresh.ClientID}').click();}},1000);", true);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            panelWarning.Visible = RequireRestart;
        }

        protected void buttonRefresh_OnClick(object sender, EventArgs e)
        {
            LoadModules();
            LoadFeatures();
        }
    }
}