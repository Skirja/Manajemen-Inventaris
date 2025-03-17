using System;
using System.Web.UI;

namespace Manajemen_Inventaris
{
    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Redirect to Login.aspx
            if (!IsPostBack)
            {
                Response.Redirect("~/Login.aspx");
            }
        }
    }
}