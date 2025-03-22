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
    public partial class AddItem : Page
    {
        private IInventoryService _inventoryService;
        private IAIService _aiService;
        private string _uploadedImagePath = string.Empty;
        private string _imageBase64 = string.Empty;

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
            _aiService = DependencyContainer.AIService;

            if (!IsPostBack)
            {
                // Load categories for dropdown
                LoadCategories();

                // Set default quantity to 1
                txtQuantity.Text = "1";
            }
            else
            {
                // If this is a postback, restore image from hidden field
                if (!string.IsNullOrEmpty(hdnImageData.Value))
                {
                    string imageData = hdnImageData.Value;

                    // If it's a data URL, extract the base64 part
                    if (imageData.StartsWith("data:image/"))
                    {
                        // Extract the base64 data from the data URI
                        _imageBase64 = imageData.Split(',')[1];

                        // Display the image preview
                        imgPreview.ImageUrl = imageData;
                        imgPreview.Style["display"] = "block";
                        uploadIcon.Visible = false;
                    }
                    else
                    {
                        _imageBase64 = imageData;
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
        }

        protected void btnGenerateTags_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(GenerateTagsAsync));
        }

        private async Task GenerateTagsAsync()
        {
            // Check if image is uploaded
            if (!HasUploadedImage())
            {
                ShowNotification("Please upload an image first to generate tags.", "bg-yellow-50 text-yellow-800 border-yellow-400");
                return;
            }

            try
            {
                // Get current category if selected
                string categoryName = null;
                if (ddlCategory.SelectedIndex > 0)
                {
                    categoryName = ddlCategory.SelectedItem.Text;
                }

                // Parse existing tags if any
                List<string> existingTags = null;
                if (!string.IsNullOrWhiteSpace(txtTags.Text))
                {
                    existingTags = txtTags.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim())
                        .ToList();
                }

                // Display loading status
                divAIResults.Visible = true;
                litAIStatus.Text = "Generating tags...";

                // Call the AI service to generate tags
                var tags = await _aiService.GenerateTagsAsync(_imageBase64, existingTags, categoryName);

                if (tags != null && tags.Count > 0)
                {
                    // Bind tags to the repeater
                    rptrTagSuggestions.DataSource = tags;
                    rptrTagSuggestions.DataBind();
                    divTagSuggestions.Visible = true;

                    litAIStatus.Text = $"Generated {tags.Count} tags based on image content.";
                }
                else
                {
                    divTagSuggestions.Visible = false;
                    litAIStatus.Text = "Could not generate tags. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error generating tags: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
            }
        }

        protected void btnSuggestCategory_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(SuggestCategoryAsync));
        }

        private async Task SuggestCategoryAsync()
        {
            // Check if image is uploaded
            if (!HasUploadedImage())
            {
                ShowNotification("Please upload an image first to suggest categories.", "bg-yellow-50 text-yellow-800 border-yellow-400");
                return;
            }

            try
            {
                // Get all category names
                List<string> categoryNames = _inventoryService.GetAllCategories()
                    .Select(c => c.Name)
                    .ToList();

                if (categoryNames.Count == 0)
                {
                    ShowNotification("No categories found in the system. Please add categories first.", "bg-yellow-50 text-yellow-800 border-yellow-400");
                    return;
                }

                // Display loading status
                divAIResults.Visible = true;
                litAIStatus.Text = "Analyzing image and suggesting categories...";

                // Call the AI service to suggest categories
                var categorySuggestions = await _aiService.SuggestCategoryAsync(_imageBase64, categoryNames);

                if (categorySuggestions != null && categorySuggestions.Count > 0)
                {
                    // Convert to a list of category suggestion models for the repeater
                    var suggestions = new List<CategorySuggestionModel>();
                    foreach (var suggestion in categorySuggestions)
                    {
                        // Find the category ID by name
                        var category = _inventoryService.GetAllCategories()
                            .FirstOrDefault(c => c.Name.Equals(suggestion.Key, StringComparison.OrdinalIgnoreCase));

                        if (category != null)
                        {
                            suggestions.Add(new CategorySuggestionModel
                            {
                                Category = suggestion.Key,
                                CategoryId = category.CategoryID,
                                Confidence = suggestion.Value
                            });
                        }
                    }

                    // Bind suggestions to the repeater
                    rptrCategorySuggestions.DataSource = suggestions.OrderByDescending(s => s.Confidence).Take(5);
                    rptrCategorySuggestions.DataBind();
                    divCategorySuggestions.Visible = true;

                    litAIStatus.Text = $"Suggested {suggestions.Count} categories based on image content.";
                }
                else
                {
                    divCategorySuggestions.Visible = false;
                    litAIStatus.Text = "Could not suggest categories. Please try again or select manually.";
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error suggesting categories: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
            }
        }

        protected void btnGenerateDescription_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(GenerateDescriptionAsync));
        }

        private async Task GenerateDescriptionAsync()
        {
            // Check if image is uploaded
            if (!HasUploadedImage())
            {
                ShowNotification("Please upload an image first to generate a description.", "bg-yellow-50 text-yellow-800 border-yellow-400");
                return;
            }

            // Check if item name is provided
            if (string.IsNullOrWhiteSpace(txtItemName.Text))
            {
                ShowNotification("Please enter an item name first to generate a description.", "bg-yellow-50 text-yellow-800 border-yellow-400");
                return;
            }

            try
            {
                // Get current category if selected
                string categoryName = null;
                if (ddlCategory.SelectedIndex > 0)
                {
                    categoryName = ddlCategory.SelectedItem.Text;
                }

                // Parse existing tags if any
                List<string> tags = new List<string>();
                if (!string.IsNullOrWhiteSpace(txtTags.Text))
                {
                    tags = txtTags.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(t => t.Trim())
                        .ToList();
                }

                // Display loading status
                divAIResults.Visible = true;
                litAIStatus.Text = "Generating description...";

                // Debug log
                System.Diagnostics.Debug.WriteLine($"Calling AI service with: Item={txtItemName.Text}, Tags={tags.Count}, Category={categoryName}, HasImage={!string.IsNullOrEmpty(_imageBase64)}");

                // Call the AI service to generate a description
                var description = await _aiService.GenerateDescriptionAsync(_imageBase64, txtItemName.Text, tags, categoryName);

                if (!string.IsNullOrWhiteSpace(description))
                {
                    // Update the description field
                    txtDescription.Text = description;
                    litAIStatus.Text = "Generated description based on image and item details.";
                }
                else
                {
                    litAIStatus.Text = "Could not generate a description. Please try again or enter manually.";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Description generation error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ShowNotification($"Error generating description: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
                litAIStatus.Text = "Could not generate a description due to an error. Please try again later.";
            }
        }

        protected void lnkSelectCategory_Command(object sender, CommandEventArgs e)
        {
            // Set the selected category in the dropdown
            string categoryId = e.CommandArgument.ToString();
            ddlCategory.SelectedValue = categoryId;

            ShowNotification("Category selected.", "bg-green-50 text-green-800 border-green-400");
        }

        protected void lnkSelectTag_Command(object sender, CommandEventArgs e)
        {
            // Add the selected tag to the text box
            string tag = e.CommandArgument.ToString();

            // Check if the tag is already in the text box
            if (string.IsNullOrWhiteSpace(txtTags.Text))
            {
                txtTags.Text = tag;
            }
            else
            {
                // Check if the tag already exists
                var existingTags = txtTags.Text.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(t => t.Trim())
                    .ToList();

                if (!existingTags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                {
                    txtTags.Text += ", " + tag;
                }
            }

            ShowNotification("Tag added.", "bg-green-50 text-green-800 border-green-400");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
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
                                string filePath = Server.MapPath("~/Uploads/Items/") + fileName;

                                // Create directory if it doesn't exist
                                string directory = Path.GetDirectoryName(filePath);
                                if (!Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }

                                // Save file
                                fileUpload.SaveAs(filePath);

                                // Set image path
                                item.ImagePath = "~/Uploads/Items/" + fileName;
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
                    int itemId = _inventoryService.CreateItem(item);

                    if (itemId > 0)
                    {
                        // Add stock history
                        if (quantity > 0)
                        {
                            ItemHistory history = new ItemHistory
                            {
                                ItemID = itemId,
                                QuantityChanged = quantity,
                                PreviousQuantity = 0,
                                NewQuantity = quantity,
                                ChangeType = "StockIn",
                                Notes = string.IsNullOrEmpty(stockNotes) ? "Initial stock" : stockNotes,
                                ChangedBy = userId,
                                ChangedDate = DateTime.Now
                            };

                            _inventoryService.AddItemHistory(history);
                        }

                        // Redirect to inventory page
                        Response.Redirect("~/Pages/Dashboard/Inventory/Inventory.aspx?success=add");
                    }
                    else
                    {
                        ShowNotification("Failed to add item. Please try again.", "bg-red-50 text-red-800 border-red-400");
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
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Redirect to login page
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

    public class CategorySuggestionModel
    {
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public double Confidence { get; set; }
    }
}