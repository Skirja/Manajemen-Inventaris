using System;
using System.IO;
using System.Web;
using System.Web.UI;

namespace Manajemen_Inventaris
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is authenticated
            if (Session["UserID"] == null)
            {
                // If not on the login page, redirect to login
                string currentPage = Path.GetFileName(Request.Url.AbsolutePath);
                if (!currentPage.Equals("Login.aspx", StringComparison.OrdinalIgnoreCase) &&
                    !currentPage.Equals("Register.aspx", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Redirect("~/Login.aspx");
                }
            }
        }
    }
}