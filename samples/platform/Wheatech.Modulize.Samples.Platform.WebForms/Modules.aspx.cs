using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace Wheatech.Modulize.Samples.Platform.WebForms
{
    public partial class Modules : System.Web.UI.Page
    {
        #region License

        private class LicenseData
        {
            public string FullName { get; set; }

            public string ShortName { get; set; }

            public string Url { get; set; }

            public Regex Pattern { get; set; }
        }

        private static readonly List<LicenseData> _licenses = new List<LicenseData>()
        {
            new LicenseData
            {
                FullName = "Academic Free License 3.0",
                ShortName = "AFL-3.0",
                Url = "https://opensource.org/licenses/AFL-3.0",
                Pattern = new Regex(@"^AFL((\-|\s+)3(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "GNU Affero General Public License 3.0",
                ShortName = "AGPL-3.0",
                Url = "https://opensource.org/licenses/AGPL-3.0",
                Pattern = new Regex(@"^AGPL((\-|\s+)3(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Adaptive Public License 1.0",
                ShortName = "APL-1.0",
                Url = "https://opensource.org/licenses/APL-1.0",
                Pattern = new Regex(@"^APL((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Apache License 2.0",
                ShortName = "Apache-2.0",
                Url = "https://opensource.org/licenses/Apache-2.0",
                Pattern = new Regex(@"^Apache((\-|\s+)2(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Apple Public Source License 2.0",
                ShortName = "APSL-2.0",
                Url = "https://opensource.org/licenses/APSL-2.0",
                Pattern = new Regex(@"^APSL((\-|\s+)2(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Artistic License 2.0",
                ShortName = "Artistic-2.0",
                Url = "https://opensource.org/licenses/Artistic-2.0",
                Pattern = new Regex(@"^Artistic((\-|\s+)2(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Attribution Assurance License",
                ShortName = "AAL",
                Url = "https://opensource.org/licenses/AAL",
                Pattern = new Regex("^AAL$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "The BSD 3-Clause License",
                ShortName = "BSD-3",
                Url = "https://opensource.org/licenses/BSD-3-Clause",
                Pattern = new Regex(@"^BSD((\-|\s+)3((\-|\s+)Clause)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "The BSD 2-Clause License",
                ShortName = "BSD-2",
                Url = "https://opensource.org/licenses/BSD-2-Clause",
                Pattern = new Regex(@"^BSD(\-|\s+)2((\-|\s+)Clause)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Boost Software License 1.0",
                ShortName = "BSL-1.0",
                Url = "https://opensource.org/licenses/BSL-1.0",
                Pattern = new Regex(@"^BSL((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "CeCILL License 2.1",
                ShortName = "CECILL-2.1",
                Url = "https://opensource.org/licenses/CECILL-2.1",
                Pattern = new Regex(@"^CECILL((\-|\s+)2\.1)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Computer Associates Trusted Open Source License 1.1",
                ShortName = "CATOSL-1.1",
                Url = "https://opensource.org/licenses/CATOSL-1.1",
                Pattern = new Regex(@"^CATOSL((\-|\s+)1\.1)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Common Development and Distribution License 1.0",
                ShortName = "CDDL-1.0",
                Url = "https://opensource.org/licenses/CDDL-1.0",
                Pattern = new Regex(@"^CDDL((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Common Public Attribution License 1.0",
                ShortName = "CPAL-1.0",
                Url = "https://opensource.org/licenses/CPAL-1.0",
                Pattern = new Regex(@"^CPAL((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "CUA Office Public License 1.0",
                ShortName = "CUA-OPL 1.0",
                Url = "https://opensource.org/licenses/CUA-OPL-1.0",
                Pattern = new Regex(@"^CUA\-OPL((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "EU DataGrid Software License",
                ShortName = "EUDatagrid",
                Url = "https://opensource.org/licenses/EUDatagrid",
                Pattern = new Regex("^EUDatagrid$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Eclipse Public License 1.0",
                ShortName = "EPL-1.0",
                Url = "https://opensource.org/licenses/EPL-1.0",
                Pattern = new Regex(@"^EPL((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "eCos License 2.0",
                ShortName = "eCos-2.0",
                Url = "https://opensource.org/licenses/eCos-2.0",
                Pattern = new Regex(@"^eCos((\-|\s+)2(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Educational Community License 2.0",
                ShortName = "ECL-2.0",
                Url = "https://opensource.org/licenses/ECL-2.0",
                Pattern = new Regex(@"^ECL((\-|\s+)2(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Eiffel Forum License 2.0",
                ShortName = "EFL-2.0",
                Url = "https://opensource.org/licenses/EFL-2.0",
                Pattern = new Regex(@"^EFL((\-|\s+)2(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Entessa Public License",
                ShortName = "Entessa",
                Url = "https://opensource.org/licenses/Entessa",
                Pattern = new Regex(@"^Entessa((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "European Union Public License 1.1",
                ShortName = "EUPL-1.1",
                Url = "https://joinup.ec.europa.eu/sites/default/files/eupl1.1.-licence-en_0.pdf",
                Pattern = new Regex(@"^EUPL((\-|\s+)1\.1)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Fair License",
                ShortName = "Fair",
                Url = "http://fairlicense.org/",
                Pattern = new Regex(@"^Fair(\s+License)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Frameworx License",
                ShortName = "Frameworx",
                Url = "https://opensource.org/licenses/Frameworx-1.0",
                Pattern = new Regex(@"^Frameworx(\s+License)?((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Free Public License",
                ShortName = "FPL",
                Url = "https://opensource.org/licenses/FPL-1.0.0",
                Pattern = new Regex(@"^FPL((\-|\s+)1(\.0){0,2})?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Free Public License",
                ShortName = "FPL",
                Url = "https://opensource.org/licenses/FPL-1.0.0",
                Pattern = new Regex(@"^Free\s+Public\s+License((\-|\s+)1(\.0){0,2})?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "GNU General Public License 2.0",
                ShortName = "GPL-2.0",
                Url = "https://opensource.org/licenses/GPL-2.0",
                Pattern = new Regex(@"^GPL(\-|\s+)2(\.0)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "GNU General Public License 2.1",
                ShortName = "GPL-2.0",
                Url = "https://opensource.org/licenses/LGPL-2.1",
                Pattern = new Regex(@"^GPL(\-|\s+)2\.1$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "GNU General Public License 3.0",
                ShortName = "GPL-3.0",
                Url = "https://opensource.org/licenses/GPL-3.0",
                Pattern = new Regex(@"^GPL((\-|\s+)3(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "GNU Lesser General Public License 2.1",
                ShortName = "LGPL-2.1",
                Url = "https://opensource.org/licenses/LGPL-2.1",
                Pattern = new Regex(@"^LGPL(\-|\s+)2\.1$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "GNU Lesser General Public License 3.0",
                ShortName = "LGPL-3.0",
                Url = "https://opensource.org/licenses/LGPL-3.0",
                Pattern = new Regex(@"^LGPL((\-|\s+)3(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Historical Permission Notice and Disclaimer",
                ShortName = "HPND",
                Url = "https://opensource.org/licenses/HPND",
                Pattern = new Regex("^HPND$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "IBM Public License 1.0",
                ShortName = "IPL-1.0",
                Url = "https://opensource.org/licenses/IPL-1.0",
                Pattern = new Regex(@"^IPL((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "IPA Font License",
                ShortName = "IPA",
                Url = "https://opensource.org/licenses/IPA",
                Pattern = new Regex("^IPA$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "ISC License",
                ShortName = "ISC",
                Url = "https://opensource.org/licenses/ISC",
                Pattern = new Regex("^ISC$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "LaTeX Project Public License 1.3c",
                ShortName = "LPPL-1.3c",
                Url = "https://opensource.org/licenses/LPPL-1.3c",
                Pattern = new Regex(@"^LPPL((\-|\s+)1\.3c)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Licence Libre du Québec – Permissive 1.1",
                ShortName = "LiLiQ-P 1.1",
                Url = "https://opensource.org/licenses/LiLiQ-P-1.1",
                Pattern = new Regex(@"^LiLiQ\-P((\-|\s+)1\.1)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Licence Libre du Québec – Réciprocité 1.1",
                ShortName = "LiLiQ-R 1.1",
                Url = "https://opensource.org/licenses/LiLiQ-R-1.1",
                Pattern = new Regex(@"^LiLiQ\-R((\-|\s+)1\.1)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Licence Libre du Québec – Réciprocité forte 1.1",
                ShortName = "LiLiQ-R+ 1.1",
                Url = "https://opensource.org/licenses/LiLiQ-Rplus-1.1",
                Pattern = new Regex(@"^LiLiQ\-R\+((\-|\s+)1\.1)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Lucent Public License 1.02",
                ShortName = "LPL-1.02",
                Url = "https://opensource.org/licenses/LPL-1.02",
                Pattern = new Regex(@"^LPL((\-|\s+)1\.02)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "MirOS Licence",
                ShortName = "MirOS",
                Url = "https://opensource.org/licenses/MirOS",
                Pattern = new Regex(@"^MirOS(\s+License)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Microsoft Public License",
                ShortName = "MS-PL",
                Url = "https://opensource.org/licenses/MS-PL",
                Pattern = new Regex(@"^MS\-PL$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Microsoft Reciprocal License",
                ShortName = "MS-RL",
                Url = "https://opensource.org/licenses/MS-RL",
                Pattern = new Regex(@"^MS\-RL$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "MIT License",
                ShortName = "MIT",
                Url = "https://opensource.org/licenses/MIT",
                Pattern = new Regex(@"^MIT(\s+License)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Motosoto License",
                ShortName = "Motosoto",
                Url = "https://opensource.org/licenses/Motosoto",
                Pattern = new Regex(@"^Motosoto(\s+License)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Mozilla Public License 2.0",
                ShortName = "MPL-2.0",
                Url = "https://opensource.org/licenses/MPL-2.0",
                Pattern = new Regex(@"^MPL((\-|\s+)2(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Multics License",
                ShortName = "Multics",
                Url = "https://opensource.org/licenses/Multics",
                Pattern = new Regex(@"^Multics(\s+License)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "NASA Open Source Agreement 1.3",
                ShortName = "NASA-1.3",
                Url = "https://opensource.org/licenses/NASA-1.3",
                Pattern = new Regex(@"^NASA((\-|\s+)1\.3)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "NTP License",
                ShortName = "NTP",
                Url = "https://opensource.org/licenses/NTP",
                Pattern = new Regex(@"^NTP(\s+License)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Naumen Public License",
                ShortName = "Naumen",
                Url = "https://opensource.org/licenses/Naumen",
                Pattern = new Regex(@"^Naumen(\s+License)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Nethack General Public License",
                ShortName = "NGPL",
                Url = "https://opensource.org/licenses/NGPL",
                Pattern = new Regex("^NGPL$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Nokia Open Source License",
                ShortName = "Nokia",
                Url = "https://opensource.org/licenses/Nokia",
                Pattern = new Regex("^Nokia$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Non-Profit Open Software License 3.0",
                ShortName = "NPOSL-3.0",
                Url = "https://opensource.org/licenses/NPOSL-3.0",
                Pattern = new Regex(@"^NPOSL((\-|\s+)3(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "OCLC Research Public License 2.0",
                ShortName = "OCLC-2.0",
                Url = "https://opensource.org/licenses/OCLC-2.0",
                Pattern = new Regex(@"^OCLC((\-|\s+)2(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Open Group Test Suite License",
                ShortName = "OGTSL",
                Url = "https://opensource.org/licenses/OGTSL",
                Pattern = new Regex("^OGTSL$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Open Software License 3.0",
                ShortName = "OSL-3.0",
                Url = "https://opensource.org/licenses/OSL-3.0",
                Pattern = new Regex(@"^OSL((\-|\s+)3(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "OSET Public License version 2.1",
                ShortName = "OPL-2.1",
                Url = "https://opensource.org/licenses/OPL-2.1",
                Pattern = new Regex(@"^OPL((\-|\s+)2\.1)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "PHP License 3.0",
                ShortName = "PHP-3.0",
                Url = "https://opensource.org/licenses/PHP-3.0",
                Pattern = new Regex(@"^PHP((\-|\s+)3(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "The PostgreSQL License",
                ShortName = "PostgreSQL",
                Url = "https://opensource.org/licenses/PostgreSQL",
                Pattern = new Regex(@"^PostgreSQL(\s+License)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Python License",
                ShortName = "Python",
                Url = "https://opensource.org/licenses/Python-2.0",
                Pattern = new Regex(@"^Python(\s+License)?((\-|\s+)2(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "CNRI portion of the multi-part Python License",
                ShortName = "CNRI-Python",
                Url = "https://opensource.org/licenses/CNRI-Python",
                Pattern = new Regex(@"^CNRI\-Python$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Q Public License",
                ShortName = "QPL-1.0",
                Url = "https://opensource.org/licenses/QPL-1.0",
                Pattern = new Regex(@"^QPL((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "RealNetworks Public Source License 1.0",
                ShortName = "RPSL-1.0",
                Url = "https://opensource.org/licenses/RPSL-1.0",
                Pattern = new Regex(@"^RPSL((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Reciprocal Public License 1.5",
                ShortName = "RPL-1.5",
                Url = "https://opensource.org/licenses/RPL-1.5",
                Pattern = new Regex(@"^RPL((\-|\s+)1\.5)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Ricoh Source Code Public License",
                ShortName = "RSCPL",
                Url = "https://opensource.org/licenses/RSCPL",
                Pattern = new Regex(@"^RSCPL$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "SIL OPEN FONT LICENSE",
                ShortName = "OFL-1.1",
                Url = "https://opensource.org/licenses/OFL-1.1",
                Pattern = new Regex(@"^OFL((\-|\s+)1\.1)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Simple Public License 2.0",
                ShortName = "SimPL-2.0",
                Url = "https://opensource.org/licenses/SimPL-2.0",
                Pattern = new Regex(@"^SimPL((\-|\s+)2(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Sleepycat License",
                ShortName = "Sleepycat",
                Url = "https://opensource.org/licenses/Sleepycat",
                Pattern = new Regex(@"^Sleepycat(\s+License)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Sun Public License 1.0",
                ShortName = "SPL-1.0",
                Url = "https://opensource.org/licenses/SPL-1.0",
                Pattern = new Regex(@"^SPL((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Sybase Open Watcom Public License 1.0",
                ShortName = "Watcom-1.0",
                Url = "https://opensource.org/licenses/Watcom-1.0",
                Pattern = new Regex(@"^Watcom((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "University of Illinois/NCSA Open Source License",
                ShortName = "NCSA",
                Url = "https://opensource.org/licenses/NCSA",
                Pattern = new Regex(@"^NCSA$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Universal Permissive License",
                ShortName = "UPL",
                Url = "https://opensource.org/licenses/UPL",
                Pattern = new Regex(@"^UPL$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Vovida Software License 1.0",
                ShortName = "VSL-1.0",
                Url = "https://opensource.org/licenses/VSL-1.0",
                Pattern = new Regex(@"^VSL((\-|\s+)1(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "W3C® SOFTWARE NOTICE AND LICENSE",
                ShortName = "W3C",
                Url = "https://opensource.org/licenses/W3C",
                Pattern = new Regex(@"^W3C$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "wxWindows Library License",
                ShortName = "WXwindows",
                Url = "https://opensource.org/licenses/WXwindows",
                Pattern = new Regex(@"^WXwindows$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "Zope Public License 2.0",
                ShortName = "ZPL-2.0",
                Url = "https://opensource.org/licenses/ZPL-2.0",
                Pattern = new Regex(@"^ZPL((\-|\s+)2(\.0)?)?$", RegexOptions.IgnoreCase)
            },
            new LicenseData
            {
                FullName = "zlib/libpng license",
                ShortName = "Zlib",
                Url = "https://opensource.org/licenses/Zlib",
                Pattern = new Regex(@"^Zlib$", RegexOptions.IgnoreCase)
            }
        };

        #endregion

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
                        var license = _licenses.First(x => x.Pattern.IsMatch(module.License));
                        if (license != null)
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
    }
}