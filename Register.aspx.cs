using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Manajemen_Inventaris
{
    public partial class Register : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If user is already logged in, redirect to dashboard
            if (Session["UserID"] != null)
            {
                Response.Redirect("~/Dashboard.aspx");
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

            // Check if username already exists
            if (UsernameExists(username))
            {
                lblMessage.Text = "Username sudah digunakan. Silakan pilih username lain.";
                lblMessage.Visible = true;
                return;
            }

            // Register user
            if (RegisterUser(username, email, company, password))
            {
                // Redirect to login page
                Response.Redirect("~/Login.aspx?registered=true");
            }
            else
            {
                lblMessage.Text = "Terjadi kesalahan saat mendaftar. Silakan coba lagi.";
                lblMessage.Visible = true;
            }
        }

        private bool UsernameExists(string username)
        {
            bool exists = false;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @Username", connection);
                    cmd.Parameters.AddWithValue("@Username", username);

                    connection.Open();
                    int count = (int)cmd.ExecuteScalar();
                    exists = (count > 0);
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error checking username: " + ex.Message);
            }

            return exists;
        }

        private bool RegisterUser(string username, string email, string company, string password)
        {
            bool success = false;

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(
                        "INSERT INTO Users (Username, Password, Email, Company) " +
                        "VALUES (@Username, @Password, @Email, @Company)", connection);

                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); // In a real app, use hashed passwords
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Company", company);

                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    success = (rowsAffected > 0);
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error registering user: " + ex.Message);
            }

            return success;
        }
    }
}