using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Manajemen_Inventaris
{
    public partial class Login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // If user is already logged in, redirect to dashboard
            if (Session["UserID"] != null)
            {
                Response.Redirect("~/Dashboard.aspx");
            }

            if (!IsPostBack)
            {
                // Check if user just registered
                if (Request.QueryString["registered"] == "true")
                {
                    lblMessage.Text = "Pendaftaran berhasil! Silakan login dengan akun baru Anda.";
                    lblMessage.CssClass = "text-green-500 text-sm text-center block";
                    lblMessage.Visible = true;
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            // Validate credentials
            if (ValidateUser(username, password))
            {
                // Redirect to dashboard
                Response.Redirect("~/Dashboard.aspx");
            }
            else
            {
                // Show error message
                lblMessage.Text = "Username atau password salah. Silakan coba lagi.";
                lblMessage.Visible = true;
            }
        }

        private bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            try
            {
                // Get connection string from web.config
                string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Create command
                    SqlCommand cmd = new SqlCommand("SELECT UserID, Username FROM Users WHERE Username = @Username AND Password = @Password", connection);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); // In a real app, use hashed passwords

                    // Open connection
                    connection.Open();

                    // Execute command
                    SqlDataReader reader = cmd.ExecuteReader();

                    // Check if user exists
                    if (reader.Read())
                    {
                        // Store user info in session
                        Session["UserID"] = reader["UserID"].ToString();
                        Session["Username"] = reader["Username"].ToString();

                        isValid = true;
                    }

                    // Close reader
                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Log error
                lblMessage.Text = "Terjadi kesalahan saat login. Silakan coba lagi.";
                lblMessage.Visible = true;

                // In a real app, log the exception
                System.Diagnostics.Debug.WriteLine("Login error: " + ex.Message);
            }

            return isValid;
        }
    }
}