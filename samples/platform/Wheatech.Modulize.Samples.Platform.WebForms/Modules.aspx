<%@ Page Language="C#" Title="Modules & Features" AutoEventWireup="true" CodeBehind="Modules.aspx.cs" MasterPageFile="~/Site.Master" Inherits="Wheatech.Modulize.Samples.Platform.WebForms.Modules" %>

<%@ Import Namespace="Wheatech.Modulize" %>

<asp:Content  ContentPlaceHolderID="header" runat="server">
    <style>
        .media-list {
            border: 1px solid #eaeaea;
            border-bottom: none;
        }

        .media {
            overflow: hidden;
            padding: 1.2em 1.4em;
            border-bottom: 1px solid #eaeaea;
            margin-top: 0;
        }

            .media.enabled {
                background: #FFF;
            }

            .media.disabled {
                background: #f3f3f3;
            }

            .media.installed {
                background: #FFF;
            }

            .media.uninstalled {
                background: #f3f3f3;
            }

            .media .forbidden {
                color: #d9534f;
                margin-left: 20px;
            }

        .media-heading {
            font-size: 1.308em;
        }

            .media-heading .tools {
                float: right;
            }

        .media-description {
            margin-top: 10px;
            margin-bottom: 10px;
        }

        .media-footer > div {
            display: inline-block;
        }
    </style>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="main">
        <ul class="nav nav-tabs" role="tablist" id="tabs-prices">
            <li role="presentation" class="active">
                <a href="#tabpane-modules" aria-controls="tabpane-modules" role="tab" data-toggle="tab" class="active">Modules</a>
            </li>
            <li role="presentation">
                <a href="#tabpane-features" aria-controls="tabpane-features" role="tab" data-toggle="tab">Features</a>
            </li>
        </ul>
        <div class="tab-content">
            <div role="tabpanel" class="tab-pane fade in active" id="tabpane-modules">
                <ul class="media-list">
                    <asp:Repeater runat="server" ID="repeaterModules" ItemType="Wheatech.Modulize.ModuleDescriptor" OnItemDataBound="repeaterModules_OnItemDataBound">
                        <ItemTemplate>
                            <li class="media <%# Item.InstallState == ModuleInstallState.Installed?"installed":"uninstalled" %>">
                                <div class="media-body">
                                    <div class="media-heading">
                                        <%# HttpUtility.HtmlEncode(Item.ModuleName) %>
                                            -
                                        <%# HttpUtility.HtmlEncode(Item.ModuleVersion) %>
                                        <asp:Label runat="server" CssClass="forbidden" Text='<%# GetModuleErrors(Item.Errors) %>' Visible='<%# Item.Errors != ModuleErrors.None %>'></asp:Label>
                                        <div class="tools">
                                            <asp:Button runat="server" ID="buttonInstall" CssClass="btn btn-xs btn-link" Text="Install" CommandName="Install" CommandArgument='<%# Item.ModuleId %>'
                                                Visible='<%# Item.InstallState == ModuleInstallState.RequireInstall && Item.HasInstallers &&Item.Errors == ModuleErrors.None %>'></asp:Button>
                                            <asp:Button runat="server" ID="buttonUninstall" CssClass="btn btn-xs btn-link" Text="Uninstall" CommandName="Uninstall" CommandArgument='<%# Item.ModuleId %>'
                                                Visible='<%# Item.InstallState == ModuleInstallState.Installed && Item.HasUninstallers &&Item.Errors == ModuleErrors.None %>'
                                                OnClientClick="return confirm('Are you sure you want to uninstall this module?')"></asp:Button>
                                        </div>
                                    </div>
                                    <asp:Panel runat="server" CssClass="media-description" Visible='<%# !string.IsNullOrWhiteSpace(Item.Description) %>'>
                                        <%# HttpUtility.HtmlEncode(Item.Description).Replace("\n","<br/>").Replace(" ","&nbsp;") %>
                                    </asp:Panel>
                                    <div class="media-footer">
                                        <asp:Panel runat="server" ID="panelFeatures">
                                            Features:
                                           
                                            <asp:Repeater runat="server" ID="repeaterModuleFeatures" ItemType="Wheatech.Modulize.FeatureDescriptor">
                                                <ItemTemplate>
                                                    <asp:Label runat="server" Text='<%# HttpUtility.HtmlEncode(Item.FeatureName) %>'></asp:Label>
                                                </ItemTemplate>
                                                <SeparatorTemplate>, </SeparatorTemplate>
                                            </asp:Repeater>
                                            <asp:Label runat="server" Text="|" Visible='<%# (Item.Authors!=null&&Item.Authors.Length>0)||!string.IsNullOrWhiteSpace(Item.WebSite)||!string.IsNullOrWhiteSpace(Item.License) %>'></asp:Label>
                                        </asp:Panel>
                                        <asp:Panel runat="server" Visible='<%# Item.Authors!=null&&Item.Authors.Length>0 %>'>
                                            Authors：<%# Item.Authors == null?null:HttpUtility.HtmlEncode(string.Join(" ",Item.Authors)) %>
                                            <asp:Label runat="server" Text="|" Visible='<%# !string.IsNullOrWhiteSpace(Item.WebSite)||!string.IsNullOrWhiteSpace(Item.License) %>'></asp:Label>
                                        </asp:Panel>
                                        <asp:Panel runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Item.WebSite) %>'>
                                            WebSite：<asp:HyperLink runat="server" Text='<%# HttpUtility.HtmlEncode(Item.WebSite) %>' NavigateUrl='<%# Item.WebSite %>'></asp:HyperLink>
                                            <asp:Label runat="server" Text="|" Visible='<%# !string.IsNullOrWhiteSpace(Item.License) %>'></asp:Label>
                                        </asp:Panel>
                                        <asp:Panel runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Item.License) %>'>
                                            License:
                                            <asp:HyperLink runat="server" ID="linkLicense" Text='<%# HttpUtility.HtmlEncode(Item.License) %>' Target="_blank"></asp:HyperLink>
                                            <asp:Label runat="server" ID="labelLicense" Text='<%# HttpUtility.HtmlEncode(Item.License) %>'></asp:Label>
                                        </asp:Panel>
                                    </div>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
            <div role="tabpanel" class="tab-pane fade in" id="tabpane-features">
                <ul class="media-list">
                    <asp:Repeater runat="server" ID="repeaterFeatures" ItemType="Wheatech.Modulize.FeatureDescriptor" OnItemDataBound="repeaterFeatures_OnItemDataBound">
                        <ItemTemplate>
                            <li class="media <%# Item.EnableState == FeatureEnableState.Enabled?"enabled":"disabled" %>">
                                <div class="media-body">
                                    <div class="media-heading">
                                        <%# HttpUtility.HtmlEncode(Item.FeatureName) %>
                                        <asp:Label runat="server" mode="Encode" CssClass="forbidden" Text='<%# GetFeatureErrors(Item, Item.Errors) %>' Visible='<%# Item.Errors!= FeatureErrors.None %>'></asp:Label>
                                        <div class="tools">
                                            <asp:Button runat="server" ID="buttonEnable" CssClass="btn btn-xs btn-link" Text="Enable" CommandName="Enable" CommandArgument='<%# Item.FeatureId %>'
                                                Visible='<%# Item.EnableState == FeatureEnableState.RequireEnable && Item.Errors== FeatureErrors.None && Item.CanEnable%>'></asp:Button>
                                            <asp:Button runat="server" ID="buttonDisable" CssClass="btn btn-xs btn-link" Text="Disable" CommandName="Disable" CommandArgument='<%# Item.FeatureId %>'
                                                Visible='<%# Item.EnableState == FeatureEnableState.Enabled && Item.Errors== FeatureErrors.None &&Item.CanDisable %>'
                                                OnClientClick="return confirm('Are you sure you want to disable this feature?')"></asp:Button>
                                        </div>
                                    </div>
                                    <asp:Panel runat="server" CssClass="media-description" Visible='<%# !string.IsNullOrWhiteSpace(Item.Description) %>'>
                                        <%# HttpUtility.HtmlEncode(Item.Description).Replace("\n","<br/>").Replace(" ","&nbsp;") %>
                                    </asp:Panel>
                                    <asp:Panel runat="server" CssClass="media-footer" Visible='<%# Item.Dependencies.Count>0 %>'>
                                        Dependencies：
                                        <asp:Repeater runat="server" ID="repeaterFeatureDependencies" ItemType="Wheatech.Modulize.DependencyDescriptor">
                                            <ItemTemplate>
                                                <asp:Label runat="server" Mode="Encode" Text='<%# Modulizer.GetFeatures().SingleOrDefault(x=>x.FeatureId==Item.FeatureId)?.FeatureName??Item.FeatureId %>'></asp:Label>
                                            </ItemTemplate>
                                            <SeparatorTemplate>, </SeparatorTemplate>
                                        </asp:Repeater>
                                    </asp:Panel>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </div>
        </div>
</asp:Content>
