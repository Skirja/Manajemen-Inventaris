using System;
using System.Web.UI;

namespace Manajemen_Inventaris.Pages.Dashboard
{
    public partial class Dashboard : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is logged in
            if (Session["UserID"] == null)
            {
                // Redirect to login page if not logged in
                Response.Redirect("~/Pages/Auth/Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                // Display welcome message
                string username = Session["Username"].ToString();
                lblWelcome.Text = $"Selamat datang, {username}!";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Redirect to login page
            Response.Redirect("~/Pages/Auth/Login.aspx");
        }
    }
}