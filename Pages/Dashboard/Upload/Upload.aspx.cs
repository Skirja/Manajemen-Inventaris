using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Services;

namespace Manajemen_Inventaris.Pages.Dashboard.Upload
{
    /// <summary>
    /// Upload page for batch image processing and AI recognition
    /// </summary>
    public partial class Upload : Page
    {
        private IAIService _aiService;
        private IInventoryService _inventoryService;
        private string _imageBase64 = string.Empty;
        private string _uploadedImagePath = string.Empty;
        private List<BatchProcessItem> _batchItems = new List<BatchProcessItem>();

        /// <summary>
        /// Page load event
        /// </summary>
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

            // Initialize services
            _aiService = DependencyContainer.AIService;
            _inventoryService = DependencyContainer.InventoryService;

            if (!IsPostBack)
            {
                // Initialize the page
                ClearResults();
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
                    }
                    else
                    {
                        _imageBase64 = imageData;
                    }
                }

                // Restore batch items from session if exists
                if (Session["BatchItems"] != null)
                {
                    _batchItems = (List<BatchProcessItem>)Session["BatchItems"];
                }
            }
        }

        /// <summary>
        /// Process the uploaded image
        /// </summary>
        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                // Check if there's an image in the hidden field (drag & drop)
                if (!string.IsNullOrEmpty(hdnImageData.Value))
                {
                    // Image data already in _imageBase64, set in Page_Load
                    ProcessUploadedImage();
                    return;
                }

                // Check if there's a file uploaded via the file upload control
                if (fileUpload.HasFile)
                {
                    foreach (HttpPostedFile uploadedFile in fileUpload.PostedFiles)
                    {
                        if (IsValidImageFile(uploadedFile))
                        {
                            // Read the file as bytes
                            using (BinaryReader reader = new BinaryReader(uploadedFile.InputStream))
                            {
                                byte[] fileData = reader.ReadBytes(uploadedFile.ContentLength);
                                _imageBase64 = Convert.ToBase64String(fileData);

                                // Save the file to server (optional, for now we're just using base64)
                                string fileName = Path.GetFileName(uploadedFile.FileName);
                                string extension = Path.GetExtension(fileName);
                                string newFileName = Guid.NewGuid().ToString() + extension;
                                string uploadDir = "~/Uploads/Items/";
                                string filePath = Server.MapPath(uploadDir + newFileName);

                                // Ensure directory exists
                                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                                uploadedFile.SaveAs(filePath);

                                _uploadedImagePath = uploadDir + newFileName;

                                // Store the image in hidden field for postbacks
                                hdnImageData.Value = _imageBase64;

                                // Process the image
                                ProcessUploadedImage();
                                break; // Process only first image for now
                            }
                        }
                        else
                        {
                            ShowNotification("Please upload a valid image file (jpg, png, gif).", "bg-yellow-50 text-yellow-800 border-yellow-400");
                        }
                    }
                }
                else
                {
                    ShowNotification("Please select an image to upload.", "bg-yellow-50 text-yellow-800 border-yellow-400");
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error processing image: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
            }
        }

        /// <summary>
        /// Process the uploaded image with AI services
        /// </summary>
        private void ProcessUploadedImage()
        {
            // Show processing indicator
            pnlNoResults.Visible = false;
            pnlProcessing.Visible = true;
            pnlResults.Visible = false;
            UpdatePanelResults.Update();

            // Register async task for image processing
            RegisterAsyncTask(new PageAsyncTask(ProcessImageAsync));
        }

        /// <summary>
        /// Process image with AI services asynchronously
        /// </summary>
        private async Task ProcessImageAsync()
        {
            try
            {
                // Set image preview
                if (!string.IsNullOrEmpty(_imageBase64))
                {
                    string imageUrl = string.IsNullOrEmpty(_uploadedImagePath)
                        ? "data:image/jpeg;base64," + _imageBase64
                        : _uploadedImagePath;

                    imgRecognitionPreview.ImageUrl = imageUrl;
                }

                // Get all category names for suggestion
                List<string> categoryNames = _inventoryService.GetAllCategories()
                    .Select(c => c.Name)
                    .ToList();

                if (categoryNames.Count == 0)
                {
                    ShowNotification("No categories found in the system. Please add categories first.", "bg-yellow-50 text-yellow-800 border-yellow-400");
                    pnlProcessing.Visible = false;
                    pnlNoResults.Visible = true;
                    return;
                }

                // Process tasks in parallel
                var suggestCategoryTask = _aiService.SuggestCategoryAsync(_imageBase64, categoryNames);
                var generateTagsTask = _aiService.GenerateTagsAsync(_imageBase64);

                // Wait for all tasks to complete
                await Task.WhenAll(suggestCategoryTask, generateTagsTask);

                // Get results
                var categorySuggestions = await suggestCategoryTask;
                var tags = await generateTagsTask;

                // Process category suggestions
                if (categorySuggestions != null && categorySuggestions.Count > 0)
                {
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

                    // Sort by confidence (descending)
                    suggestions = suggestions.OrderByDescending(s => s.Confidence).ToList();

                    // Bind to repeater
                    rptrCategorySuggestions.DataSource = suggestions;
                    rptrCategorySuggestions.DataBind();
                }

                // Process tags
                if (tags != null && tags.Count > 0)
                {
                    rptrTagSuggestions.DataSource = tags;
                    rptrTagSuggestions.DataBind();
                }

                // Hide processing indicator and show results
                pnlProcessing.Visible = false;
                pnlResults.Visible = true;
            }
            catch (Exception ex)
            {
                ShowNotification($"Error processing image: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
                pnlProcessing.Visible = false;
                pnlNoResults.Visible = true;
            }
        }

        /// <summary>
        /// Start batch processing
        /// </summary>
        protected void btnProcessBatch_Click(object sender, EventArgs e)
        {
            if (fileUpload.HasFiles)
            {
                try
                {
                    _batchItems.Clear();

                    // Process each file in a batch
                    foreach (HttpPostedFile uploadedFile in fileUpload.PostedFiles)
                    {
                        if (IsValidImageFile(uploadedFile))
                        {
                            // Add to batch items
                            _batchItems.Add(new BatchProcessItem
                            {
                                FileName = Path.GetFileName(uploadedFile.FileName),
                                Status = "Pending",
                                ProcessedTime = DateTime.Now
                            });
                        }
                    }

                    // Store batch items in session
                    Session["BatchItems"] = _batchItems;

                    // Display batch status
                    litProgress.Text = $"0 of {_batchItems.Count} processed";
                    pnlProgressBar.Style["width"] = "0%";
                    rptrBatchResults.DataSource = _batchItems;
                    rptrBatchResults.DataBind();

                    pnlBatchStatus.Visible = true;

                    // Start processing the batch
                    RegisterAsyncTask(new PageAsyncTask(ProcessBatchAsync));

                    ShowNotification($"Started processing {_batchItems.Count} files. Please wait...", "bg-blue-50 text-blue-800 border-blue-400");
                }
                catch (Exception ex)
                {
                    ShowNotification($"Error starting batch process: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
                }
            }
            else
            {
                ShowNotification("Please select files to process in batch.", "bg-yellow-50 text-yellow-800 border-yellow-400");
            }
        }

        /// <summary>
        /// Process a batch of images asynchronously
        /// </summary>
        private async Task ProcessBatchAsync()
        {
            if (_batchItems.Count == 0)
                return;

            // Get all category names for suggestion
            List<string> categoryNames = _inventoryService.GetAllCategories()
                .Select(c => c.Name)
                .ToList();

            try
            {
                int processedCount = 0;

                // Process each file in the batch
                foreach (var batchItem in _batchItems)
                {
                    // Skip already processed items
                    if (batchItem.Status != "Pending")
                        continue;

                    // Update status
                    batchItem.Status = "Processing";
                    UpdateBatchDisplay(processedCount, _batchItems.Count);

                    try
                    {
                        // Find the file in the posted files
                        foreach (HttpPostedFile uploadedFile in fileUpload.PostedFiles)
                        {
                            if (Path.GetFileName(uploadedFile.FileName) == batchItem.FileName)
                            {
                                // Read the file as bytes
                                using (BinaryReader reader = new BinaryReader(uploadedFile.InputStream))
                                {
                                    byte[] fileData = reader.ReadBytes(uploadedFile.ContentLength);
                                    string base64 = Convert.ToBase64String(fileData);

                                    // Process with AI
                                    var categorySuggestions = await _aiService.SuggestCategoryAsync(base64, categoryNames);

                                    // Get best category suggestion
                                    if (categorySuggestions != null && categorySuggestions.Count > 0)
                                    {
                                        var bestSuggestion = categorySuggestions.OrderByDescending(s => s.Value).First();
                                        batchItem.Category = bestSuggestion.Key;
                                        batchItem.Confidence = bestSuggestion.Value;
                                    }

                                    // Save the file to server
                                    string extension = Path.GetExtension(batchItem.FileName);
                                    string newFileName = Guid.NewGuid().ToString() + extension;
                                    string uploadDir = "~/Uploads/Items/";
                                    string filePath = Server.MapPath(uploadDir + newFileName);

                                    // Ensure directory exists
                                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                                    uploadedFile.SaveAs(filePath);

                                    batchItem.FilePath = uploadDir + newFileName;
                                    batchItem.Status = "Completed";
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        batchItem.Status = "Failed";
                        batchItem.ErrorMessage = ex.Message;
                    }

                    // Update progress
                    processedCount++;
                    batchItem.ProcessedTime = DateTime.Now;
                    UpdateBatchDisplay(processedCount, _batchItems.Count);

                    // Short delay to avoid UI locking
                    await Task.Delay(100);
                }

                // Update session
                Session["BatchItems"] = _batchItems;

                // Show completion notification
                ShowNotification($"Batch processing completed. {processedCount} files processed.", "bg-green-50 text-green-800 border-green-400");
            }
            catch (Exception ex)
            {
                ShowNotification($"Error in batch processing: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
            }
        }

        /// <summary>
        /// Update the batch display with current progress
        /// </summary>
        private void UpdateBatchDisplay(int processed, int total)
        {
            int percentage = (int)Math.Round((double)processed / total * 100);
            litProgress.Text = $"{processed} of {total} processed";
            pnlProgressBar.Style["width"] = $"{percentage}%";

            rptrBatchResults.DataSource = _batchItems;
            rptrBatchResults.DataBind();
        }

        /// <summary>
        /// Add the processed image to inventory
        /// </summary>
        protected void btnAddToInventory_Click(object sender, EventArgs e)
        {
            // Redirect to AddItem page with parameters for image data
            if (!string.IsNullOrEmpty(_uploadedImagePath) || !string.IsNullOrEmpty(_imageBase64))
            {
                if (!string.IsNullOrEmpty(_uploadedImagePath))
                {
                    // Use server-saved image path
                    Response.Redirect($"~/Pages/Dashboard/Inventory/AddItem.aspx?imagePath={Server.UrlEncode(_uploadedImagePath)}");
                }
                else
                {
                    // Store base64 in session and redirect
                    Session["UploadedImageBase64"] = _imageBase64;
                    Response.Redirect("~/Pages/Dashboard/Inventory/AddItem.aspx?fromUpload=true");
                }
            }
            else
            {
                ShowNotification("No image has been processed to add to inventory.", "bg-yellow-50 text-yellow-800 border-yellow-400");
            }
        }

        /// <summary>
        /// Select a category from the suggestions
        /// </summary>
        protected void lnkSelectCategory_Command(object sender, CommandEventArgs e)
        {
            int categoryId = Convert.ToInt32(e.CommandArgument);

            // Store selected category in session and redirect to AddItem
            Session["SelectedCategoryId"] = categoryId;

            if (!string.IsNullOrEmpty(_uploadedImagePath))
            {
                // Use server-saved image path
                Response.Redirect($"~/Pages/Dashboard/Inventory/AddItem.aspx?imagePath={Server.UrlEncode(_uploadedImagePath)}&categoryId={categoryId}");
            }
            else if (!string.IsNullOrEmpty(_imageBase64))
            {
                // Store base64 in session and redirect
                Session["UploadedImageBase64"] = _imageBase64;
                Response.Redirect($"~/Pages/Dashboard/Inventory/AddItem.aspx?fromUpload=true&categoryId={categoryId}");
            }
        }

        /// <summary>
        /// Clear button click event
        /// </summary>
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearResults();
        }

        /// <summary>
        /// Cancel batch processing
        /// </summary>
        protected void btnCancelBatch_Click(object sender, EventArgs e)
        {
            _batchItems.Clear();
            Session["BatchItems"] = null;
            pnlBatchStatus.Visible = false;
            ShowNotification("Batch processing cancelled.", "bg-blue-50 text-blue-800 border-blue-400");
        }

        /// <summary>
        /// Back to dashboard button click event
        /// </summary>
        protected void lnkBackToDashboard_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Dashboard/Dashboard.aspx");
        }

        /// <summary>
        /// Logout button click event
        /// </summary>
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Pages/Auth/Login.aspx");
        }

        /// <summary>
        /// Clear all results and reset the form
        /// </summary>
        private void ClearResults()
        {
            // Reset form
            _imageBase64 = string.Empty;
            _uploadedImagePath = string.Empty;
            hdnImageData.Value = string.Empty;

            // Hide results panels
            pnlProcessing.Visible = false;
            pnlResults.Visible = false;
            pnlNoResults.Visible = true;
            pnlBatchStatus.Visible = false;

            // Clear notification
            pnlNotification.Visible = false;
        }

        /// <summary>
        /// Show a notification message
        /// </summary>
        private void ShowNotification(string message, string cssClass)
        {
            pnlNotification.CssClass = $"px-6 py-4 sm:px-8 {cssClass}";
            lblNotification.Text = message;
            pnlNotification.Visible = true;
        }

        /// <summary>
        /// Check if the uploaded file is a valid image
        /// </summary>
        private bool IsValidImageFile(HttpPostedFile file)
        {
            if (file == null || file.ContentLength == 0)
                return false;

            // Check if the file extension is allowed
            string extension = Path.GetExtension(file.FileName).ToLower();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

            if (!allowedExtensions.Contains(extension))
                return false;

            // Check content type
            string contentType = file.ContentType.ToLower();
            if (!contentType.StartsWith("image/"))
                return false;

            return true;
        }

        /// <summary>
        /// Get CSS class for status badge
        /// </summary>
        protected string GetStatusClass(string status)
        {
            switch (status)
            {
                case "Completed":
                    return "bg-green-100 text-green-800";
                case "Failed":
                    return "bg-red-100 text-red-800";
                case "Processing":
                    return "bg-yellow-100 text-yellow-800";
                case "Pending":
                default:
                    return "bg-gray-100 text-gray-800";
            }
        }
    }

    /// <summary>
    /// Represents an item in the batch processing queue
    /// </summary>
    [Serializable]
    public class BatchProcessItem
    {
        public string FileName { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public double Confidence { get; set; }
        public string FilePath { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ProcessedTime { get; set; }
    }
}