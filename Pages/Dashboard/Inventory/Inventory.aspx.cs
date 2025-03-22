using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Services;

namespace Manajemen_Inventaris.Pages.Dashboard.Inventory
{
    public partial class Inventory : System.Web.UI.Page
    {
        private IInventoryService _inventoryService;
        private IAIService _aiService;
        private const int LOW_STOCK_THRESHOLD = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Check if user is authenticated
            if (Session["UserID"] == null)
            {
                Response.Redirect("~/Pages/Auth/Login.aspx");
                return;
            }

            _inventoryService = DependencyContainer.InventoryService;
            _aiService = DependencyContainer.AIService;

            if (!IsPostBack)
            {
                // Display username
                if (Session["Username"] != null)
                {
                    litUsername.Text = Session["Username"].ToString();
                }

                // Check for success message from add or edit operations
                if (!string.IsNullOrEmpty(Request.QueryString["success"]))
                {
                    string successType = Request.QueryString["success"];

                    if (successType == "add")
                    {
                        ShowNotification("Item added successfully.", "bg-green-50 text-green-800 border-green-400");
                    }
                    else if (successType == "edit")
                    {
                        ShowNotification("Item updated successfully.", "bg-green-50 text-green-800 border-green-400");
                    }
                    else if (successType == "delete")
                    {
                        ShowNotification("Item deleted successfully.", "bg-green-50 text-green-800 border-green-400");
                    }
                }

                LoadCategories();
                LoadItems();
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Redirect to login page
            Response.Redirect("~/Pages/Auth/Login.aspx");
        }

        private void LoadCategories()
        {
            // Clear existing items except the "All Categories" option
            ddlCategory.Items.Clear();
            ddlCategory.Items.Add(new ListItem("All Categories", "0"));

            // Load categories from database
            List<Category> categories = _inventoryService.GetAllCategories();
            foreach (Category category in categories)
            {
                ddlCategory.Items.Add(new ListItem(category.Name, category.CategoryID.ToString()));
            }
        }

        private void LoadItems()
        {
            try
            {
                // Get categories for the dropdown
                var categories = _inventoryService.GetAllCategories();

                // First time loading, bind categories dropdown
                if (!IsPostBack)
                {
                    ddlCategory.Items.Clear();
                    ddlCategory.Items.Add(new ListItem("All Categories", "0"));
                    foreach (var category in categories)
                    {
                        ddlCategory.Items.Add(new ListItem(category.Name, category.CategoryID.ToString()));
                    }
                }

                // Get items based on filters
                List<Item> items;

                // Check if category filter is applied
                int categoryId = Convert.ToInt32(ddlCategory.SelectedValue);
                string searchTerm = txtSearch.Text.Trim();

                if (categoryId > 0)
                {
                    // Filter by category
                    items = _inventoryService.GetItemsByCategoryId(categoryId);

                    // Apply search filter if any
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        items = items.Where(i =>
                            i.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            (i.Description != null && i.Description.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (i.AITags != null && i.AITags.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                        ).ToList();
                    }
                }
                else
                {
                    // Get all items
                    items = _inventoryService.GetAllItems();

                    // Apply search filter if any
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        items = items.Where(i =>
                            i.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            (i.Description != null && i.Description.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (i.AITags != null && i.AITags.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                        ).ToList();
                    }
                }

                // Apply sorting
                SortItems(ref items);

                // Bind items to grid
                gvItems.DataSource = items;
                gvItems.DataBind();

                // Check for low stock items
                CheckLowStockItems(items);
            }
            catch (Exception ex)
            {
                // Show error notification
                ShowNotification($"Error loading items: {ex.Message}", "bg-red-50 text-red-800 border-red-400");
            }
        }

        private void SortItems(ref List<Item> items)
        {
            // Get sort option
            string sortOption = ddlSort.SelectedValue;

            switch (sortOption)
            {
                case "NameAsc":
                    items = items.OrderBy(i => i.Name).ToList();
                    break;
                case "NameDesc":
                    items = items.OrderByDescending(i => i.Name).ToList();
                    break;
                case "QuantityAsc":
                    items = items.OrderBy(i => i.Quantity).ToList();
                    break;
                case "QuantityDesc":
                    items = items.OrderByDescending(i => i.Quantity).ToList();
                    break;
                case "DateDesc":
                    items = items.OrderByDescending(i => i.CreatedDate).ToList();
                    break;
                case "DateAsc":
                    items = items.OrderBy(i => i.CreatedDate).ToList();
                    break;
                default:
                    items = items.OrderBy(i => i.Name).ToList();
                    break;
            }
        }

        private void CheckLowStockItems(List<Item> items)
        {
            // Check if any items are below the threshold
            var lowStockItems = items.Where(i => i.Quantity < LOW_STOCK_THRESHOLD).ToList();

            if (lowStockItems.Count > 0)
            {
                // Show low stock warning
                pnlLowStock.Visible = true;

                if (lowStockItems.Count == 1)
                {
                    lblLowStock.Text = $"The item '{lowStockItems[0].Name}' is running low on stock ({lowStockItems[0].Quantity} remaining). Consider restocking soon.";
                }
                else
                {
                    lblLowStock.Text = $"{lowStockItems.Count} items are running low on stock. Consider restocking soon.";
                }
            }
            else
            {
                // Hide low stock warning
                pnlLowStock.Visible = false;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            RegisterAsyncTask(new PageAsyncTask(PerformSearchAsync));
        }

        private async Task PerformSearchAsync()
        {
            // Get search term
            string searchTerm = txtSearch.Text.Trim();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                try
                {
                    // Get current category name if selected
                    string currentCategory = null;
                    if (ddlCategory.SelectedIndex > 0)
                    {
                        currentCategory = ddlCategory.SelectedItem.Text;
                    }

                    // Call AI service to enhance the search
                    string enhancedSearch = await _aiService.EnhanceSearchAsync(searchTerm, currentCategory);

                    // If enhanced search is different from original, show notification
                    if (enhancedSearch != searchTerm)
                    {
                        ShowNotification($"Search enhanced with related terms: {enhancedSearch}", "bg-blue-50 text-blue-800 border-blue-400");

                        // Use the enhanced search term
                        // We'll search for each term separately and combine results
                        var searchTerms = enhancedSearch.Split(new string[] { " OR " }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(t => t.Trim())
                            .ToList();

                        List<Item> allItems;

                        // Check if filtering by category
                        int categoryId = Convert.ToInt32(ddlCategory.SelectedValue);
                        if (categoryId > 0)
                        {
                            allItems = _inventoryService.GetItemsByCategoryId(categoryId);
                        }
                        else
                        {
                            allItems = _inventoryService.GetAllItems();
                        }

                        // Search for each term and combine results
                        var results = new List<Item>();
                        foreach (var term in searchTerms)
                        {
                            var termResults = allItems.Where(i =>
                                i.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                (i.Description != null && i.Description.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                (i.AITags != null && i.AITags.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                            ).ToList();

                            // Add to results if not already there
                            foreach (var item in termResults)
                            {
                                if (!results.Any(r => r.ItemID == item.ItemID))
                                {
                                    results.Add(item);
                                }
                            }
                        }

                        // Apply sorting
                        SortItems(ref results);

                        // Bind to grid
                        gvItems.DataSource = results;
                        gvItems.DataBind();
                    }
                    else
                    {
                        // Just perform regular search if AI didn't enhance it
                        LoadItems();
                    }
                }
                catch (Exception ex)
                {
                    // If AI service fails, fall back to regular search
                    System.Diagnostics.Debug.WriteLine($"Error in AI search enhancement: {ex.Message}");
                    LoadItems();
                }
            }
            else
            {
                // If search term is empty, just load all items
                LoadItems();
            }
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadItems();
        }

        protected void ddlSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadItems();
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Dashboard/Inventory/AddItem.aspx");
        }

        protected void btnManageCategories_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Dashboard/Inventory/ManageCategories.aspx");
        }

        protected void gvItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int itemId = Convert.ToInt32(e.CommandArgument);

            if (e.CommandName == "ViewItem")
            {
                // Navigate to view/edit page
                Response.Redirect($"~/Pages/Dashboard/Inventory/EditItem.aspx?id={itemId}");
            }
            else if (e.CommandName == "DeleteItem")
            {
                // Delete the item
                bool success = _inventoryService.DeleteItem(itemId);

                if (success)
                {
                    // Show success message
                    ShowNotification("Item deleted successfully.", "bg-green-50 text-green-800 border-green-400");
                }
                else
                {
                    // Show error message
                    ShowNotification("Failed to delete item. Please try again.", "bg-red-50 text-red-800 border-red-400");
                }

                // Reload items
                LoadItems();
            }
        }

        protected void gvItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Get the item
                Item item = (Item)e.Row.DataItem;

                // Check if low stock
                if (item.Quantity < LOW_STOCK_THRESHOLD)
                {
                    // Add CSS class for low stock
                    e.Row.CssClass = "bg-yellow-50";
                }
            }
        }

        private void ShowNotification(string message, string cssClass)
        {
            lblNotification.Text = message;
            pnlNotification.CssClass = $"mb-6 {cssClass}";
            pnlNotification.Visible = true;
        }

        protected void btnCloseDetails_Click(object sender, EventArgs e)
        {
            // Hide the item details panel
            pnlItemDetails.Visible = false;
        }

        protected void btnSaveAdjustment_Click(object sender, EventArgs e)
        {
            // Get the values from the form
            int itemId = Convert.ToInt32(Request.QueryString["itemAdjust"]);
            int quantityChange = Convert.ToInt32(txtQuantityChange.Text);
            string notes = txtNotes.Text;
            int userId = Convert.ToInt32(Session["UserID"]);

            // Determine if it's a stock in or stock out operation
            if (ddlAdjustmentType.SelectedValue == "out")
            {
                quantityChange = -quantityChange; // Make it negative for stock out
            }

            // Adjust the quantity
            bool success = _inventoryService.AdjustItemQuantity(itemId, quantityChange, notes, userId);

            if (success)
            {
                // Hide the adjustment panel
                pnlStockAdjustment.Visible = false;

                // Show success message
                ShowNotification("Stock adjustment saved successfully.", "bg-green-50 text-green-800 border-green-400");

                // Reload items
                LoadItems();
            }
            else
            {
                // Show error message
                ShowNotification("Error adjusting stock. Please try again.", "bg-red-50 text-red-800 border-red-400");
            }
        }

        protected void btnCancelAdjustment_Click(object sender, EventArgs e)
        {
            // Hide the stock adjustment panel
            pnlStockAdjustment.Visible = false;
        }
    }
}