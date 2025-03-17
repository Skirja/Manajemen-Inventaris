using System;
using System.Web.UI;
using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Services;

namespace Manajemen_Inventaris.Pages.Auth
{
    public partial class Register : Page
    {
        private readonly IAuthService _authService;

        public Register()
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
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string company = txtCompany.Text.Trim();
            string password = txtPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;
            bool agreeTerms = chkAgree.Checked;

            // Validate input
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(company) || string.IsNullOrEmpty(password))
            {
                lblMessage.Text = "Semua field harus diisi.";
                lblMessage.Visible = true;
                return;
            }

            if (password != confirmPassword)
            {
                lblMessage.Text = "Password dan konfirmasi password tidak sama.";
                lblMessage.Visible = true;
                return;
            }

            if (!agreeTerms)
            {
                lblMessage.Text = "Anda harus menyetujui Syarat dan Ketentuan untuk melanjutkan.";
                lblMessage.Visible = true;
                return;
            }

            // Check if username is available
            if (!_authService.IsUsernameAvailable(username))
            {
                lblMessage.Text = "Username sudah digunakan. Silakan pilih username lain.";
                lblMessage.Visible = true;
                return;
            }

            try
            {
                // Register user
                User user = _authService.Register(username, password, email, company);

                if (user != null)
                {
                    // Redirect to login page
                    Response.Redirect("~/Pages/Auth/Login.aspx?registered=true");
                }
                else
                {
                    lblMessage.Text = "Terjadi kesalahan saat mendaftar. Silakan coba lagi.";
                    lblMessage.Visible = true;
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Terjadi kesalahan saat mendaftar. Silakan coba lagi.";
                lblMessage.Visible = true;
                System.Diagnostics.Debug.WriteLine("Registration error: " + ex.Message);
            }
        }
    }
}