using System;
using System.Web.UI;
using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Services;

namespace Manajemen_Inventaris.Pages.Auth
{
    public partial class Login : Page
    {
        private readonly IAuthService _authService;

        public Login()
        {
            _authService = DependencyContainer.AuthService;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // If user is already logged in, redirect to dashboard
            if (Session["UserID"] != null)
            {
                Response.Redirect("~/Pages/Dashboard/Dashboard.aspx");
            }

            if (!IsPostBack)
            {
                // Check if user just registered
                if (Request.QueryString["registered"] == "true")
                {
                    lblMessage.Text = "Pendaftaran berhasil! Silakan login dengan akun baru Anda.";
                    lblMessage.CssClass = "text-green-500 text-base text-center block";
                    lblMessage.Visible = true;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // Validate credentials
            User user = _authService.Authenticate(username, password);

            if (user != null)
            {
                // Store user info in session
                Session["UserID"] = user.UserID;
                Session["Username"] = user.Username;

                // Redirect to dashboard
                Response.Redirect("~/Pages/Dashboard/Dashboard.aspx");
            }
            else
            {
                // Show error message
                lblMessage.Text = "Username atau password salah. Silakan coba lagi.";
                lblMessage.Visible = true;
            }
        }
    }
}