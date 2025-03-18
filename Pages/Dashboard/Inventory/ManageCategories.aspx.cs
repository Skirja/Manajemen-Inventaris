using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Services;

namespace Manajemen_Inventaris.Pages.Dashboard.Inventory
{
    public partial class ManageCategories : Page
    {
        private IInventoryService _inventoryService;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is authenticated
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Pages/Auth/Login.aspx");
                return;
            }

            // Display username
            if (Session["Username"] != null)
            {
                litUsername.Text = Session["Username"].ToString();
            }

            _inventoryService = DependencyContainer.InventoryService;

            if (!IsPostBack)
            {
                LoadCategories();
            }
        }

        private void LoadCategories(string searchTerm = "")
        {
            List<CategoryWithItemCount> categories = _inventoryService.GetCategoriesWithItemCount();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(searchTerm))
            {
                categories = categories.Where(c =>
                    c.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (c.Description != null && c.Description.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            }

            // Bind to grid
            gvCategories.DataSource = categories;
            gvCategories.DataBind();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int categoryId = Convert.ToInt32(hdnCategoryId.Value);
                string name = txtCategoryName.Text.Trim();
                string description = txtDescription.Text.Trim();

                // Get current user ID
                int userId = Convert.ToInt32(Session["UserID"]);

                bool success;
                if (categoryId == 0) // Add new category
                {
                    // Create new category
                    var category = new Category
                    {
                        Name = name,
                        Description = description,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now
                    };

                    success = _inventoryService.AddCategory(category) > 0;

                    if (success)
                    {
                        ShowMessage("Category added successfully.", "text-green-600");
                    }
                    else
                    {
                        ShowMessage("Failed to add category. Please try again.", "text-red-600");
                    }
                }
                else // Update existing category
                {
                    // Get existing category
                    var category = _inventoryService.GetCategoryById(categoryId);
                    if (category != null)
                    {
                        category.Name = name;
                        category.Description = description;

                        success = _inventoryService.UpdateCategory(category);

                        if (success)
                        {
                            ShowMessage("Category updated successfully.", "text-green-600");
                        }
                        else
                        {
                            ShowMessage("Failed to update category. Please try again.", "text-red-600");
                        }
                    }
                    else
                    {
                        ShowMessage("Category not found.", "text-red-600");
                        success = false;
                    }
                }

                if (success)
                {
                    // Reset form
                    ResetForm();

                    // Reload categories
                    LoadCategories();
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        protected void gvCategories_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int categoryId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "EditCategory")
            {
                // Load category for editing
                var category = _inventoryService.GetCategoryById(categoryId);
                if (category != null)
                {
                    // Set form fields
                    hdnCategoryId.Value = category.CategoryID.ToString();
                    txtCategoryName.Text = category.Name;
                    txtDescription.Text = category.Description;

                    // Update UI
                    litFormTitle.Text = "Edit Category";
                    btnSave.Text = "Update Category";
                    btnCancel.Visible = true;

                    // Hide any previous messages
                    lblMessage.Visible = false;
                }
            }
            else if (e.CommandName == "DeleteCategory")
            {
                // Delete category
                bool success = _inventoryService.DeleteCategory(categoryId);
                if (success)
                {
                    ShowMessage("Category deleted successfully.", "text-green-600");
                    LoadCategories();
                }
                else
                {
                    ShowMessage("Failed to delete category. It may be in use by inventory items.", "text-red-600");
                }
            }
        }

        protected void btnSearchCategory_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearchCategory.Text.Trim();
            LoadCategories(searchTerm);
        }

        protected void lnkBackToInventory_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Dashboard/Inventory/Inventory.aspx");
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Redirect to login page
            Response.Redirect("~/Pages/Auth/Login.aspx");
        }

        private void ResetForm()
        {
            // Reset form fields
            hdnCategoryId.Value = "0";
            txtCategoryName.Text = string.Empty;
            txtDescription.Text = string.Empty;

            // Reset UI
            litFormTitle.Text = "Add New Category";
            btnSave.Text = "Save Category";
            btnCancel.Visible = false;
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "mt-4 block text-sm font-medium " + cssClass;
            lblMessage.Visible = true;
        }
    }
}