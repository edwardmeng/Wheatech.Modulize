<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="Wheatech.Modulize.Samples.Settings.WebForms.Settings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="header" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="main" runat="server">
    <asp:Repeater runat="server" ID="repeaterFields" ItemType="Wheatech.Modulize.Samples.Settings.Services.SettingsField" OnItemDataBound="repeaterFields_OnItemDataBound">
        <ItemTemplate>
            <div style="width: 200px; float: left">
                <%# Server.HtmlEncode(Item.Name) %>
            </div>
            <div class="width: calc(100% - 200px); float:left">
                <asp:TextBox runat="server" ID="textKey" Visible="False" Text='<%# Item.Key %>'></asp:TextBox>
                <asp:TextBox runat="server" CssClass="form-control" Style="max-width: 250px;" ID="textValue"></asp:TextBox>
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <asp:Button runat="server" Text="Save" CssClass="btn btn-primary" ID="buttonSave" OnClick="buttonSave_OnClick" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="footer" runat="server">
</asp:Content>
