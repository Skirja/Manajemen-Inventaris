using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Manajemen_Inventaris.Pages.Dashboard
{
    public partial class Dashboard : System.Web.UI.Page
    {
        private readonly IInventoryService _inventoryService;
        private readonly IUserService _userService;

        public Dashboard()
        {
            _inventoryService = DependencyContainer.InventoryService;
            _userService = DependencyContainer.UserService;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UserId"] != null)
                {
                    string username = Session["Username"]?.ToString() ?? "User";
                    litUsername.Text = username;

                    LoadDashboardStatistics();
                    LoadRecentActivity();
                }
                else
                {
                    Response.Redirect("~/Pages/Auth/Login.aspx");
                }
            }
        }

        private void LoadDashboardStatistics()
        {
            try
            {
                // Get total items count
                int totalItems = _inventoryService.GetAllItems().Count;
                lblTotalItems.Text = totalItems.ToString();

                // Get categories count
                int categoriesCount = _inventoryService.GetAllCategories().Count;
                lblTotalCategories.Text = categoriesCount.ToString();

                // Get recent uploads (items added in the last 7 days)
                var recentItems = _inventoryService.GetRecentItemsByDays(7);
                lblRecentUploads.Text = recentItems.Count.ToString();

                // Get low stock items count
                var lowStockItems = _inventoryService.GetLowStockItems();
                lblLowStock.Text = lowStockItems.Count.ToString();
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error loading dashboard statistics: {ex.Message}");
            }
        }

        private void LoadRecentActivity()
        {
            try
            {
                // Get recent activity (last 10 item history entries)
                var recentActivity = _inventoryService.GetRecentItemHistory(10);

                // Map to a simpler view model for the repeater
                var activityItems = recentActivity.Select(h => new
                {
                    Date = h.ChangedDate,
                    ActionType = GetActionTypeDisplay(h.ChangeType),
                    ItemName = h.ItemName,
                    Quantity = FormatQuantityChange(h.ChangeType, h.QuantityChanged),
                    Username = h.ChangedByUsername
                }).ToList();

                rptRecentActivity.DataSource = activityItems;
                rptRecentActivity.DataBind();
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error loading recent activity: {ex.Message}");
            }
        }

        private string GetActionTypeDisplay(string actionType)
        {
            switch (actionType?.ToLower())
            {
                case "create":
                case "created":
                    return "Added";
                case "update":
                case "updated":
                    return "Updated";
                case "delete":
                case "deleted":
                    return "Deleted";
                case "stockin":
                case "adjust_increase":
                    return "Increased";
                case "stockout":
                case "adjust_decrease":
                    return "Decreased";
                default:
                    return actionType;
            }
        }

        private string FormatQuantityChange(string actionType, int quantityChange)
        {
            if (actionType?.ToLower() == "stockin" || actionType?.ToLower() == "adjust_increase")
                return $"+{quantityChange}";
            else if (actionType?.ToLower() == "stockout" || actionType?.ToLower() == "adjust_decrease")
                return $"-{Math.Abs(quantityChange)}";
            else
                return quantityChange.ToString();
        }

        protected string GetActionClass(string actionType)
        {
            switch (actionType?.ToLower())
            {
                case "added":
                    return "px-2 py-1 text-xs font-medium rounded-full bg-green-100 text-green-800";
                case "updated":
                    return "px-2 py-1 text-xs font-medium rounded-full bg-blue-100 text-blue-800";
                case "deleted":
                    return "px-2 py-1 text-xs font-medium rounded-full bg-red-100 text-red-800";
                case "increased":
                    return "px-2 py-1 text-xs font-medium rounded-full bg-indigo-100 text-indigo-800";
                case "decreased":
                    return "px-2 py-1 text-xs font-medium rounded-full bg-yellow-100 text-yellow-800";
                default:
                    return "px-2 py-1 text-xs font-medium rounded-full bg-gray-100 text-gray-800";
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            _userService.Logout();
            Response.Redirect("~/Pages/Auth/Login.aspx");
        }
    }
}