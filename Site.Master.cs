using System;
using System.IO;
using System.Web.UI;

namespace Manajemen_Inventaris
{
    public partial class Site : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if user is authenticated
                if (Session["UserID"] == null)
                {
                    // Hide navigation for unauthenticated users except on login and register pages
                    string currentPage = Path.GetFileName(Request.Path);
                    if (!currentPage.Equals("Login.aspx", StringComparison.OrdinalIgnoreCase) &&
                        !currentPage.Equals("Register.aspx", StringComparison.OrdinalIgnoreCase))
                    {
                        Response.Redirect("~/Pages/Auth/Login.aspx");
                    }
                }
            }
        }
    }
}