using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Services;

namespace Manajemen_Inventaris.Pages.Dashboard.Inventory
{
    public partial class AddItem : Page
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
                // Load categories for dropdown
                LoadCategories();

                // Set default quantity to 1
                txtQuantity.Text = "1";
            }
        }

        private void LoadCategories()
        {
            // Get all categories
            List<Category> categories = _inventoryService.GetAllCategories();

            // Add default option
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new ListItem("-- Select Category --", "0"));

            // Add categories to dropdown
            foreach (Category category in categories)
            {
                ddlCategory.Items.Add(new ListItem(category.Name, category.CategoryID.ToString()));
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Get form data
                string name = txtItemName.Text.Trim();
                string description = txtDescription.Text.Trim();
                int categoryId = Convert.ToInt32(ddlCategory.SelectedValue);
                int quantity = Convert.ToInt32(txtQuantity.Text.Trim());
                string tags = txtTags.Text.Trim();
                string stockNotes = txtStockNotes.Text.Trim();

                // Get current user ID
                int userId = Convert.ToInt32(Session["UserID"]);

                try
                {
                    // Create new item
                    Item item = new Item
                    {
                        Name = name,
                        Description = description,
                        CategoryID = categoryId,
                        Quantity = quantity,
                        AITags = tags,
                        CreatedBy = userId,
                        CreatedDate = DateTime.Now,
                        LastModifiedBy = userId,
                        LastModifiedDate = DateTime.Now
                    };

                    // Process image upload if provided
                    if (fileUpload.HasFile)
                    {
                        // Check file extension
                        string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();
                        if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                        {
                            // Check file size (max 5MB)
                            if (fileUpload.PostedFile.ContentLength <= 5242880) // 5MB in bytes
                            {
                                // Generate unique filename
                                string fileName = Guid.NewGuid().ToString() + fileExtension;
                                string uploadPath = Server.MapPath("~/Uploads/") + fileName;

                                // Save file
                                fileUpload.SaveAs(uploadPath);

                                // Set image path in item
                                item.ImagePath = "~/Uploads/" + fileName;

                                // TODO: If tags are empty, process image with AI to generate tags
                                if (string.IsNullOrEmpty(tags))
                                {
                                    // This would be implemented with an AI service integration
                                    // For now, we'll just set a placeholder
                                    item.AITags = "auto-generated,tags,placeholder";
                                }
                            }
                            else
                            {
                                ShowMessage("Error: File size exceeds the 5MB limit.", "text-red-600");
                                return;
                            }
                        }
                        else
                        {
                            ShowMessage("Error: Only .jpg, .jpeg, and .png files are allowed.", "text-red-600");
                            return;
                        }
                    }
                    else
                    {
                        // No image uploaded, use default
                        item.ImagePath = "~/Uploads/no-image.png";
                    }

                    // Save item to database
                    int itemId = _inventoryService.CreateItem(item);

                    if (itemId > 0)
                    {
                        // If quantity > 0 and stock notes provided, add to item history
                        if (quantity > 0 && !string.IsNullOrEmpty(stockNotes))
                        {
                            ItemHistory history = new ItemHistory
                            {
                                ItemID = itemId,
                                ChangeType = "Created",
                                QuantityChanged = quantity,
                                PreviousQuantity = 0,
                                NewQuantity = quantity,
                                Notes = "Initial stock: " + stockNotes,
                                ChangedBy = userId,
                                ChangedDate = DateTime.Now
                            };

                            _inventoryService.AddItemHistory(history);
                        }

                        // Show success message and redirect
                        ShowMessage("Item added successfully!", "text-green-600");

                        // Clear form for new entry or redirect to inventory
                        ClearForm();

                        // Optional: Redirect to inventory page after short delay
                        string script = "setTimeout(function() { window.location.href = '" +
                            ResolveUrl("~/Pages/Dashboard/Inventory/Inventory.aspx") + "'; }, 2000);";
                        ScriptManager.RegisterStartupScript(this, GetType(), "redirectScript", script, true);
                    }
                    else
                    {
                        ShowMessage("Error adding item. Please try again.", "text-red-600");
                    }
                }
                catch (Exception ex)
                {
                    ShowMessage("Error: " + ex.Message, "text-red-600");
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Dashboard/Inventory/Inventory.aspx");
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

        private void ClearForm()
        {
            txtItemName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            ddlCategory.SelectedValue = "0";
            txtQuantity.Text = "1";
            txtTags.Text = string.Empty;
            txtStockNotes.Text = string.Empty;
            imgPreview.ImageUrl = "~/Uploads/no-image.png";

            // Hide message after a delay
            string script = "setTimeout(function() { document.getElementById('" +
                lblMessage.ClientID + "').style.display = 'none'; }, 5000);";
            ScriptManager.RegisterStartupScript(this, GetType(), "hideMessage", script, true);
        }

        private void ShowMessage(string message, string cssClass)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "mt-4 block text-sm font-medium " + cssClass;
            lblMessage.Visible = true;
        }
    }
}