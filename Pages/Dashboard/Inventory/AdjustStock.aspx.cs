using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Services;

namespace Manajemen_Inventaris.Pages.Dashboard.Inventory
{
    public partial class AdjustStock : Page
    {
        private readonly IInventoryService _inventoryService;
        private readonly IUserService _userService;
        private int _itemId;
        private Item _currentItem;

        public AdjustStock()
        {
            _inventoryService = DependencyContainer.InventoryService;
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
                LoadItemHistory();
            }
        }

        private void LoadItemData()
        {
            try
            {
                // Load item details
                _currentItem = _inventoryService.GetItemById(_itemId);
                if (_currentItem == null)
                {
                    ShowNotification("Item not found. It may have been deleted.", "bg-red-50 text-red-800 border-red-400");
                    return;
                }

                // Set item details on the form
                lblItemName.Text = _currentItem.Name;
                lblCategory.Text = _currentItem.CategoryName;
                lblCurrentQuantity.Text = _currentItem.Quantity.ToString();
                lblLastModified.Text = $"By {_userService.GetUserById(_currentItem.LastModifiedBy)?.Username ?? "Unknown"} at {_currentItem.LastModifiedDate.ToString("yyyy-MM-dd HH:mm")}";

                // Set item image
                if (!string.IsNullOrEmpty(_currentItem.ImagePath))
                {
                    imgItem.ImageUrl = _currentItem.ImagePath;
                }
                else
                {
                    imgItem.ImageUrl = "~/Uploads/no-image.png";
                }
            }
            catch (Exception ex)
            {
                ShowNotification($"Error loading item: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
            }
        }

        private void LoadItemHistory()
        {
            try
            {
                // Load item history
                List<ItemHistory> history = _inventoryService.GetItemHistory(_itemId);

                // Transform data for display
                var displayHistory = new List<object>();
                foreach (var record in history)
                {
                    displayHistory.Add(new
                    {
                        Date = record.ChangedDate,
                        Type = FormatChangeType(record.ChangeType),
                        QuantityChanged = record.QuantityChanged,
                        QuantityBefore = record.PreviousQuantity,
                        QuantityAfter = record.NewQuantity,
                        Notes = record.Notes,
                        CreatedBy = record.ChangedByUsername
                    });
                }

                gvItemHistory.DataSource = displayHistory;
                gvItemHistory.DataBind();
            }
            catch (Exception ex)
            {
                ShowNotification($"Error loading item history: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
            }
        }

        private string FormatChangeType(string changeType)
        {
            switch (changeType)
            {
                case "StockIn":
                    return "Stock In";
                case "StockOut":
                    return "Stock Out";
                case "Created":
                    return "Item Created";
                default:
                    return changeType;
            }
        }

        protected void btnAdjust_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input
                if (!int.TryParse(txtQuantity.Text, out int quantityChange) || quantityChange <= 0)
                {
                    ShowNotification("Please enter a valid quantity.", "bg-red-50 text-red-800 border-red-400");
                    return;
                }

                // Get current user ID
                int userId = _userService.GetLoggedInUser().UserID;

                // Ensure we have a valid current item
                if (_currentItem == null)
                {
                    // Try to reload the item
                    _currentItem = _inventoryService.GetItemById(_itemId);
                    if (_currentItem == null)
                    {
                        ShowNotification("Item not found. It may have been deleted.", "bg-red-50 text-red-800 border-red-400");
                        return;
                    }
                }

                // Get notes
                string notes = txtNotes.Text.Trim();
                if (string.IsNullOrEmpty(notes))
                {
                    notes = "Stock adjustment";
                }

                // Adjust quantity based on selected action
                string adjustmentType = ddlAdjustmentType.SelectedValue;
                if (adjustmentType == "StockOut")
                {
                    // If stock out, make quantity negative
                    quantityChange = -quantityChange;

                    // Check if we have enough stock
                    if (_currentItem.Quantity + quantityChange < 0)
                    {
                        ShowNotification($"Not enough stock. Current quantity is {_currentItem.Quantity}.", "bg-red-50 text-red-800 border-red-400");
                        return;
                    }
                }

                // Log the adjustment attempt
                System.Diagnostics.Debug.WriteLine($"Adjusting item {_itemId} quantity by {quantityChange}, current qty: {_currentItem.Quantity}, notes: {notes}, by user: {userId}");

                // Perform adjustment
                bool success = _inventoryService.AdjustItemQuantity(_itemId, quantityChange, notes, userId);

                if (success)
                {
                    // Clear form and reload data
                    txtQuantity.Text = string.Empty;
                    txtNotes.Text = string.Empty;
                    LoadItemData();
                    LoadItemHistory();

                    // Show success message
                    ShowNotification("Stock adjusted successfully.", "bg-green-50 text-green-800 border-green-400");
                }
                else
                {
                    ShowNotification("Failed to adjust stock. Please try again.", "bg-red-50 text-red-800 border-red-400");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AdjustStock.btnAdjust_Click: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ShowNotification($"Error: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
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
            pnlNotification.CssClass = $"mb-6 rounded-md p-4 border {cssClass}";
            lblNotification.Text = message;
            pnlNotification.Visible = true;
        }
    }
}