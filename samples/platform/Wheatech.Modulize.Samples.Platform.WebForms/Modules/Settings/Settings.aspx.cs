using System;
using System.Web.UI.WebControls;
using Wheatech.Modulize.Samples.Platform.Common;
using Wheatech.Modulize.Samples.Settings.Services;

namespace Wheatech.Modulize.Samples.Settings.WebForms
{
    public partial class Settings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                repeaterFields.DataSource = this.GetService<ISettingsService>().GetFields();
                repeaterFields.DataBind();
            }
        }

        protected void buttonSave_OnClick(object sender, EventArgs e)
        {
            foreach (RepeaterItem item in repeaterFields.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    var textKey = (TextBox)item.FindControl("textKey");
                    var textValue = (TextBox)item.FindControl("textValue");
                    var value = textValue.Text.Trim();
                    this.GetService<ISettingsService>().Set(textKey.Text, string.IsNullOrEmpty(value) ? null : value);
                }
            }
        }

        protected void repeaterFields_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var textValue = (TextBox) e.Item.FindControl("textValue");
                textValue.Text = this.GetService<ISettingsService>().Get(((SettingsField) e.Item.DataItem).Key);
            }
        }
    }
}