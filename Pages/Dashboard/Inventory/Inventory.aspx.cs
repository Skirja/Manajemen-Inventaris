using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Services;

namespace Manajemen_Inventaris.Pages.Dashboard.Inventory
{
    public partial class Inventory : System.Web.UI.Page
    {
        private IInventoryService _inventoryService;
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

            if (!IsPostBack)
            {
                // Display username
                if (Session["Username"] != null)
                {
                    litUsername.Text = Session["Username"].ToString();
                }

                LoadCategories();
                LoadItems();
                CheckLowStockItems();
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
            List<Item> items;

            // Check if filtering by category
            int categoryId = Convert.ToInt32(ddlCategory.SelectedValue);
            if (categoryId > 0)
            {
                items = _inventoryService.GetItemsByCategoryId(categoryId);
            }
            else
            {
                items = _inventoryService.GetAllItems();
            }

            // Check if searching
            string searchTerm = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                items = items.Where(i =>
                    i.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (i.Description != null && i.Description.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (i.AITags != null && i.AITags.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                ).ToList();
            }

            // Apply sorting
            SortItems(ref items);

            // Bind to grid
            gvItems.DataSource = items;
            gvItems.DataBind();
        }

        private void SortItems(ref List<Item> items)
        {
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

        private void CheckLowStockItems()
        {
            List<Item> lowStockItems = _inventoryService.GetLowStockItems(LOW_STOCK_THRESHOLD);
            if (lowStockItems.Count > 0)
            {
                pnlLowStock.Visible = true;
                litLowStockMessage.Text = $"You have {lowStockItems.Count} item(s) with low stock (5 or fewer units). Please consider restocking soon.";
            }
            else
            {
                pnlLowStock.Visible = false;
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

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadItems();
        }

        protected void gvItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int itemId = Convert.ToInt32(e.CommandArgument);
            hdnItemId.Value = itemId.ToString();

            switch (e.CommandName)
            {
                case "ViewItem":
                    ShowItemDetails(itemId);
                    break;
                case "EditItem":
                    // Redirect to edit page
                    Response.Redirect($"~/Pages/Dashboard/Inventory/EditItem.aspx?id={itemId}");
                    break;
                case "AdjustStock":
                    ShowStockAdjustment(itemId);
                    break;
                case "DeleteItem":
                    DeleteItem(itemId);
                    break;
            }
        }

        protected void gvItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Highlight low stock items
                Label lblQuantity = (Label)e.Row.FindControl("lblQuantity");
                if (lblQuantity != null)
                {
                    int quantity = Convert.ToInt32(lblQuantity.Text);
                    if (quantity <= LOW_STOCK_THRESHOLD)
                    {
                        lblQuantity.CssClass = "text-red-600 font-medium";
                    }
                }
            }
        }

        private void ShowItemDetails(int itemId)
        {
            Item item = _inventoryService.GetItemById(itemId);
            if (item != null)
            {
                litItemName.Text = item.Name;
                litItemDescription.Text = item.Description ?? "No description available";
                litItemCategory.Text = item.CategoryName;
                litItemQuantity.Text = item.Quantity.ToString();
                litItemAITags.Text = item.AITags ?? "No AI tags available";

                // Get username for created by and modified by
                var userRepository = DependencyContainer.UserRepository;
                var createdByUser = userRepository.GetById(item.CreatedBy);
                var modifiedByUser = userRepository.GetById(item.LastModifiedBy);

                litItemCreatedBy.Text = createdByUser?.Username ?? "Unknown";
                litItemCreatedDate.Text = item.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss");
                litItemModifiedBy.Text = modifiedByUser?.Username ?? "Unknown";
                litItemModifiedDate.Text = item.LastModifiedDate.ToString("yyyy-MM-dd HH:mm:ss");

                // Set image
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    imgItemDetail.ImageUrl = item.ImagePath;
                }
                else
                {
                    imgItemDetail.ImageUrl = "~/Uploads/no-image.png";
                }

                pnlItemDetails.Visible = true;
            }
        }

        private void ShowStockAdjustment(int itemId)
        {
            Item item = _inventoryService.GetItemById(itemId);
            if (item != null)
            {
                litAdjustItemName.Text = item.Name;
                litAdjustCurrentQuantity.Text = item.Quantity.ToString();
                txtQuantityChange.Text = string.Empty;
                txtNotes.Text = string.Empty;
                ddlAdjustmentType.SelectedIndex = 0; // Default to "Stock In"

                pnlStockAdjustment.Visible = true;
            }
        }

        private void DeleteItem(int itemId)
        {
            bool success = _inventoryService.DeleteItem(itemId);
            if (success)
            {
                // Reload items
                LoadItems();
                CheckLowStockItems();
            }
            else
            {
                // Show error message
                ScriptManager.RegisterStartupScript(this, GetType(), "deleteError",
                    "alert('Failed to delete item. Please try again.');", true);
            }
        }

        protected void btnCloseDetails_Click(object sender, EventArgs e)
        {
            pnlItemDetails.Visible = false;
        }

        protected void btnSaveAdjustment_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int itemId = Convert.ToInt32(hdnItemId.Value);
                int quantityChange = Convert.ToInt32(txtQuantityChange.Text);
                string notes = txtNotes.Text.Trim();

                // Check if stock in or stock out
                if (ddlAdjustmentType.SelectedValue == "out")
                {
                    quantityChange = -quantityChange; // Make negative for stock out
                }

                // Get current user ID
                int userId = Convert.ToInt32(Session["UserID"]);

                bool success = _inventoryService.AdjustItemQuantity(itemId, quantityChange, notes, userId);
                if (success)
                {
                    pnlStockAdjustment.Visible = false;
                    LoadItems();
                    CheckLowStockItems();
                }
                else
                {
                    // Show error message
                    ScriptManager.RegisterStartupScript(this, GetType(), "adjustError",
                        "alert('Failed to adjust stock. Please check that you have sufficient quantity for stock out operations.');", true);
                }
            }
        }

        protected void btnCancelAdjustment_Click(object sender, EventArgs e)
        {
            pnlStockAdjustment.Visible = false;
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Dashboard/Inventory/AddItem.aspx");
        }

        protected void btnManageCategories_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Pages/Dashboard/Inventory/ManageCategories.aspx");
        }
    }
}