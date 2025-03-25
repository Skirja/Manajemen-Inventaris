using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using System.Threading.Tasks;
using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Services;

namespace Manajemen_Inventaris.Pages.Dashboard.Inventory
{
    public partial class EditItem : Page
    {
        private readonly IInventoryService _inventoryService;
        private readonly IAIService _aiService;
        private readonly IUserService _userService;
        private string _uploadedImagePath = string.Empty;
        private string _imageBase64 = string.Empty;
        private int _itemId = 0;
        private Item _currentItem = null;

        public EditItem()
        {
            _inventoryService = DependencyContainer.InventoryService;
            _aiService = DependencyContainer.AIService;
            _userService = DependencyContainer.UserService;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is authenticated
            if (!_userService.IsLoggedIn())
            {
                Response.Redirect("~/Pages/Auth/Login.aspx");
                return;
            }

            // Set username in header
            User currentUser = _userService.GetLoggedInUser();
            litUsername.Text = currentUser.Username;

            // Get item ID from query string
            if (!int.TryParse(Request.QueryString["id"], out _itemId) || _itemId <= 0)
            {
                ShowNotification("Invalid item ID. Please select a valid item.", "bg-red-50 text-red-800 border-red-400");
                return;
            }

            hdnItemId.Value = _itemId.ToString();

            if (!IsPostBack)
            {
                LoadItemData();
                LoadCategories();
                LoadItemHistory();
            }
            else
            {
                // Restore image if it was uploaded
                if (!string.IsNullOrEmpty(hdnImageData.Value))
                {
                    imgPreview.ImageUrl = hdnImageData.Value;
                    imgPreview.Style["display"] = "block";
                    uploadIcon.Visible = false;
                    _imageBase64 = hdnImageData.Value;
                }
            }
        }

        private void LoadItemData()
        {
            // Get item by ID
            _currentItem = _inventoryService.GetItemById(_itemId);

            if (_currentItem == null)
            {
                // Item not found, redirect back to inventory
                Response.Redirect("~/Pages/Dashboard/Inventory/Inventory.aspx?error=notfound");
                return;
            }

            // Fill form fields with item data
            txtItemName.Text = _currentItem.Name;
            txtDescription.Text = _currentItem.Description;
            txtQuantity.Text = _currentItem.Quantity.ToString();
            txtTags.Text = _currentItem.AITags;

            // Check if item has an image
            if (!string.IsNullOrEmpty(_currentItem.ImagePath))
            {
                string imagePath = Server.MapPath(_currentItem.ImagePath);
                if (File.Exists(imagePath))
                {
                    try
                    {
                        // Load the image file and convert to base64
                        byte[] imageBytes = File.ReadAllBytes(imagePath);
                        string base64String = Convert.ToBase64String(imageBytes);

                        // Determine MIME type from file extension
                        string mimeType = "image/jpeg"; // Default
                        string extension = Path.GetExtension(_currentItem.ImagePath).ToLower();
                        if (extension == ".png")
                        {
                            mimeType = "image/png";
                        }
                        else if (extension == ".jpeg" || extension == ".jpg")
                        {
                            mimeType = "image/jpeg";
                        }

                        // Create data URI
                        string imageDataUri = $"data:{mimeType};base64,{base64String}";

                        // Display the image
                        imgPreview.ImageUrl = imageDataUri;
                        imgPreview.Style["display"] = "block";
                        uploadIcon.Visible = false;

                        // Store in hidden field for persistence
                        hdnImageData.Value = imageDataUri;
                        _imageBase64 = base64String;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                    }
                }
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

            // Select the item's category
            if (_currentItem != null && _currentItem.CategoryID > 0)
            {
                ddlCategory.SelectedValue = _currentItem.CategoryID.ToString();
            }
        }

        private void LoadItemHistory()
        {
            // Get item history
            List<ItemHistory> history = _inventoryService.GetItemHistory(_itemId);

            // Bind to grid view
            gvItemHistory.DataSource = history;
            gvItemHistory.DataBind();
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                try
                {
                    // Make sure we have the current item
                    if (_currentItem == null)
                    {
                        _currentItem = _inventoryService.GetItemById(_itemId);
                        if (_currentItem == null)
                        {
                            ShowNotification("Item not found. It may have been deleted.", "bg-red-50 text-red-800 border-red-400");
                            return;
                        }
                    }

                    // Get form data
                    string name = txtItemName.Text.Trim();
                    string description = txtDescription.Text.Trim();

                    // Truncate description if it's too long for the database
                    const int maxDescriptionLength = 8000; // NVARCHAR(MAX) has a practical limit around 8000 chars in SQL Server
                    if (description.Length > maxDescriptionLength)
                    {
                        description = description.Substring(0, maxDescriptionLength);
                        ShowNotification("Description was truncated as it was too long.", "bg-yellow-50 text-yellow-800 border-yellow-400");
                    }

                    int categoryId = Convert.ToInt32(ddlCategory.SelectedValue);
                    int quantity = Convert.ToInt32(txtQuantity.Text.Trim());
                    string tags = txtTags.Text.Trim();

                    // Get current user ID
                    int userId = Convert.ToInt32(Session["UserID"]);

                    // Update item properties
                    _currentItem.Name = name;
                    _currentItem.Description = description;
                    _currentItem.CategoryID = categoryId;
                    _currentItem.Quantity = quantity;
                    _currentItem.AITags = tags;
                    _currentItem.LastModifiedBy = userId;
                    _currentItem.LastModifiedDate = DateTime.Now;

                    // Process image upload if a new file is provided
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
                                string filePath = Server.MapPath("~/Uploads/Items/") + fileName;

                                // Create directory if it doesn't exist
                                string directory = Path.GetDirectoryName(filePath);
                                if (!Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }

                                // Save file
                                fileUpload.SaveAs(filePath);

                                // Delete old image if it exists
                                if (!string.IsNullOrEmpty(_currentItem.ImagePath))
                                {
                                    string oldImagePath = Server.MapPath(_currentItem.ImagePath);
                                    if (File.Exists(oldImagePath))
                                    {
                                        try
                                        {
                                            File.Delete(oldImagePath);
                                        }
                                        catch (Exception ex)
                                        {
                                            System.Diagnostics.Debug.WriteLine($"Error deleting old image: {ex.Message}");
                                        }
                                    }
                                }

                                // Set new image path
                                _currentItem.ImagePath = "~/Uploads/Items/" + fileName;
                            }
                            else
                            {
                                ShowNotification("Image size must be less than 5MB.", "bg-red-50 text-red-800 border-red-400");
                                return;
                            }
                        }
                        else
                        {
                            ShowNotification("Only JPG, JPEG, and PNG files are allowed.", "bg-red-50 text-red-800 border-red-400");
                            return;
                        }
                    }

                    // Save the item
                    bool success = _inventoryService.UpdateItem(_currentItem);

                    if (success)
                    {
                        // Redirect to inventory page
                        Response.Redirect("~/Pages/Dashboard/Inventory/Inventory.aspx?success=edit");
                    }
                    else
                    {
                        ShowNotification("Failed to update item. Please try again.", "bg-red-50 text-red-800 border-red-400");
                    }
                }
                catch (Exception ex)
                {
                    ShowNotification($"Error: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
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
            _userService.Logout();
            Response.Redirect("~/Pages/Auth/Login.aspx");
        }

        private void ShowNotification(string message, string cssClass)
        {
            lblNotification.Text = message;
            pnlNotification.CssClass = $"mb-6 {cssClass}";
            pnlNotification.Visible = true;
        }

        private bool HasUploadedImage()
        {
            // Check if an image is already uploaded or if a new file is being uploaded
            if (!string.IsNullOrEmpty(_imageBase64))
            {
                return true;
            }

            if (fileUpload.HasFile)
            {
                try
                {
                    // Get the file extension
                    string fileExtension = Path.GetExtension(fileUpload.FileName).ToLower();

                    // Validate file type
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png")
                    {
                        // Convert to base64
                        using (Stream fs = fileUpload.PostedFile.InputStream)
                        {
                            using (BinaryReader br = new BinaryReader(fs))
                            {
                                byte[] imageBytes = br.ReadBytes((int)fs.Length);
                                _imageBase64 = Convert.ToBase64String(imageBytes);

                                // Display the preview on the server side as well
                                string base64String = "data:image/" + fileExtension.Replace(".", "") + ";base64," + _imageBase64;
                                imgPreview.ImageUrl = base64String;
                                imgPreview.Style["display"] = "block";
                                uploadIcon.Visible = false;

                                // Store in hidden field for persistence across postbacks
                                hdnImageData.Value = base64String;

                                return true;
                            }
                        }
                    }
                    else
                    {
                        ShowNotification("Only JPG, JPEG, and PNG files are allowed for AI analysis.", "bg-yellow-50 text-yellow-800 border-yellow-400");
                    }
                }
                catch (Exception ex)
                {
                    ShowNotification($"Error processing image: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
                }
            }

            return false;
        }
    }
}