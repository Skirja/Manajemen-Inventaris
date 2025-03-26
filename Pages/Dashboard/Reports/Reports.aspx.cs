using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Manajemen_Inventaris.Models;
using Manajemen_Inventaris.Services;
using System.IO;
using System.Web.UI.HtmlControls;

namespace Manajemen_Inventaris.Pages.Dashboard.Reports
{
    public partial class Reports : System.Web.UI.Page
    {
        private readonly IInventoryService _inventoryService;
        private readonly IUserService _userService;
        private DateTime _startDate;
        private DateTime _endDate;
        private int _categoryId;

        public Reports()
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

                    // Initialize default date range (last 30 days)
                    txtStartDate.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                    txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

                    // Load categories for dropdown
                    LoadCategories();

                    // Generate initial report
                    GenerateReport();
                }
                else
                {
                    Response.Redirect("~/Pages/Auth/Login.aspx");
                }
            }
        }

        private void LoadCategories()
        {
            // Add "All Categories" option first (already in aspx)
            // Now add the actual categories
            var categories = _inventoryService.GetAllCategories();
            foreach (var category in categories)
            {
                ddlCategory.Items.Add(new ListItem(category.Name, category.CategoryID.ToString()));
            }
        }

        protected void btnGenerateReport_Click(object sender, EventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            try
            {
                // Parse date range and category filter
                if (DateTime.TryParse(txtStartDate.Text, out _startDate) &&
                    DateTime.TryParse(txtEndDate.Text, out _endDate))
                {
                    // Ensure end date is not earlier than start date
                    if (_endDate < _startDate)
                    {
                        _endDate = _startDate.AddDays(1);
                        txtEndDate.Text = _endDate.ToString("yyyy-MM-dd");
                    }

                    // Parse category ID
                    _categoryId = Convert.ToInt32(ddlCategory.SelectedValue);

                    // Load dashboard overview
                    LoadDashboardOverview();

                    // Load inventory movement report
                    LoadInventoryMovementReport();

                    // Load category analysis
                    LoadCategoryAnalysis();

                    // Load AI recognition statistics
                    LoadAIRecognitionStatistics();
                }
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error generating report: {ex.Message}");
            }
        }

        private void LoadDashboardOverview()
        {
            // Get total items
            var items = _inventoryService.GetAllItems();
            lblTotalItems.Text = items.Count.ToString();

            // Get total categories
            var categories = _inventoryService.GetAllCategories();
            lblTotalCategories.Text = categories.Count.ToString();

            // Calculate total stock
            int totalStock = items.Sum(i => i.Quantity);
            lblTotalStock.Text = totalStock.ToString();

            // Get low stock items
            var lowStockItems = _inventoryService.GetLowStockItems();
            lblLowStock.Text = lowStockItems.Count.ToString();
        }

        private void LoadInventoryMovementReport()
        {
            // Get movement statistics
            var movementStats = _inventoryService.GetInventoryMovementStats(_startDate, _endDate);

            // Format for chart.js
            string labels = $"['Stock In', 'Stock Out', 'Created', 'Updated', 'Deleted']";
            string data = $"[{movementStats["StockIn"]}, {movementStats["StockOut"]}, {movementStats["Created"]}, {movementStats["Updated"]}, {movementStats["Deleted"]}]";
            string colors = "['#4F46E5', '#EF4444', '#10B981', '#3B82F6', '#F59E0B']";

            // Generate chart.js script for movement summary
            string inventoryMovementChartScript = $@"
                <script>
                    document.addEventListener('DOMContentLoaded', function() {{
                        var ctx = document.getElementById('inventoryMovementChart').getContext('2d');
                        var inventoryMovementChart = new Chart(ctx, {{
                            type: 'doughnut',
                            data: {{
                                labels: {labels},
                                datasets: [{{
                                    data: {data},
                                    backgroundColor: {colors},
                                    borderWidth: 1
                                }}]
                            }},
                            options: {{
                                responsive: true,
                                plugins: {{
                                    legend: {{
                                        position: 'bottom',
                                    }}
                                }}
                            }}
                        }});
                    }});
                </script>";
            litInventoryMovementChartData.Text = inventoryMovementChartScript;

            // Get daily movement data
            var dailyMovements = _inventoryService.GetInventoryMovementsByDate(_startDate, _endDate, _categoryId);

            // Format for chart.js
            StringBuilder dateLabels = new StringBuilder("[");
            StringBuilder movementData = new StringBuilder("[");

            // Sort by date
            var sortedDailyMovements = dailyMovements.OrderBy(d => d.Key).ToList();

            foreach (var movement in sortedDailyMovements)
            {
                dateLabels.Append($"'{movement.Key.ToString("MM/dd")}',");
                movementData.Append($"{movement.Value},");
            }

            // Remove trailing commas and close arrays
            if (dateLabels.Length > 1) dateLabels.Length--;
            if (movementData.Length > 1) movementData.Length--;
            dateLabels.Append("]");
            movementData.Append("]");

            // Generate chart.js script for daily movement
            string dailyMovementChartScript = $@"
                <script>
                    document.addEventListener('DOMContentLoaded', function() {{
                        var ctx = document.getElementById('dailyMovementChart').getContext('2d');
                        var dailyMovementChart = new Chart(ctx, {{
                            type: 'line',
                            data: {{
                                labels: {dateLabels},
                                datasets: [{{
                                    label: 'Movements',
                                    data: {movementData},
                                    backgroundColor: 'rgba(79, 70, 229, 0.2)',
                                    borderColor: 'rgba(79, 70, 229, 1)',
                                    borderWidth: 2,
                                    tension: 0.3
                                }}]
                            }},
                            options: {{
                                responsive: true,
                                scales: {{
                                    y: {{
                                        beginAtZero: true,
                                        ticks: {{
                                            precision: 0
                                        }}
                                    }}
                                }}
                            }}
                        }});
                    }});
                </script>";
            litDailyMovementChartData.Text = dailyMovementChartScript;

            // Load movement details table
            var movementDetails = _inventoryService.GetItemHistoryForReporting(_startDate, _endDate, _categoryId);

            // Prepare data for gridview with formatted quantity display
            var movementItems = movementDetails.Select(m => new
            {
                Date = m.ChangedDate,
                ChangeType = m.ChangeType,
                ItemName = m.ItemName,
                QuantityDisplay = FormatQuantityChange(m.ChangeType, m.QuantityChanged),
                m.Notes,
                m.ChangedByUsername
            }).ToList();

            gvMovementDetails.DataSource = movementItems;
            gvMovementDetails.DataBind();
        }

        private void LoadCategoryAnalysis()
        {
            // Get category distribution
            var categoryDistribution = _inventoryService.GetCategoryDistribution();

            // Calculate total items for percentage
            int totalItems = categoryDistribution.Sum(c => c.ItemCount);

            // Add percentage to each category
            var categoriesWithPercentage = categoryDistribution.Select(c => new
            {
                c.Name,
                c.ItemCount,
                Percentage = totalItems > 0 ? (double)c.ItemCount / totalItems : 0
            }).ToList();

            // Bind to repeater
            rptCategories.DataSource = categoriesWithPercentage;
            rptCategories.DataBind();

            // Format for chart.js
            StringBuilder categoryLabels = new StringBuilder("[");
            StringBuilder categoryData = new StringBuilder("[");
            StringBuilder categoryColors = new StringBuilder("[");

            // Colors for pie chart
            string[] colorArray = {
                "rgba(79, 70, 229, 0.8)",   // Indigo
                "rgba(16, 185, 129, 0.8)",  // Green
                "rgba(239, 68, 68, 0.8)",   // Red
                "rgba(59, 130, 246, 0.8)",  // Blue
                "rgba(245, 158, 11, 0.8)",  // Yellow
                "rgba(139, 92, 246, 0.8)",  // Purple
                "rgba(236, 72, 153, 0.8)",  // Pink
                "rgba(6, 182, 212, 0.8)",   // Cyan
                "rgba(249, 115, 22, 0.8)",  // Orange
                "rgba(16, 185, 129, 0.8)"   // Teal
            };

            int colorIndex = 0;
            foreach (var category in categoryDistribution)
            {
                if (category.ItemCount > 0) // Only include categories with items
                {
                    categoryLabels.Append($"'{category.Name}',");
                    categoryData.Append($"{category.ItemCount},");
                    categoryColors.Append($"'{colorArray[colorIndex % colorArray.Length]}',");
                    colorIndex++;
                }
            }

            // Remove trailing commas and close arrays
            if (categoryLabels.Length > 1) categoryLabels.Length--;
            if (categoryData.Length > 1) categoryData.Length--;
            if (categoryColors.Length > 1) categoryColors.Length--;
            categoryLabels.Append("]");
            categoryData.Append("]");
            categoryColors.Append("]");

            // Generate chart.js script for category distribution
            string categoryChartScript = $@"
                <script>
                    document.addEventListener('DOMContentLoaded', function() {{
                        var ctx = document.getElementById('categoryDistributionChart').getContext('2d');
                        var categoryDistributionChart = new Chart(ctx, {{
                            type: 'pie',
                            data: {{
                                labels: {categoryLabels},
                                datasets: [{{
                                    data: {categoryData},
                                    backgroundColor: {categoryColors},
                                    borderWidth: 1
                                }}]
                            }},
                            options: {{
                                responsive: true,
                                plugins: {{
                                    legend: {{
                                        position: 'bottom',
                                    }}
                                }}
                            }}
                        }});
                    }});
                </script>";
            litCategoryDistributionChartData.Text = categoryChartScript;
        }

        private void LoadAIRecognitionStatistics()
        {
            // Get AI stats
            var aiStats = _inventoryService.GetAIRecognitionStats(_startDate, _endDate);

            // Generate chart.js script for AI tags usage
            string aiTagsChartScript = $@"
                <script>
                    document.addEventListener('DOMContentLoaded', function() {{
                        var ctx = document.getElementById('aiTagsChart').getContext('2d');
                        var aiTagsChart = new Chart(ctx, {{
                            type: 'bar',
                            data: {{
                                labels: ['With AI Tags', 'Without AI Tags'],
                                datasets: [{{
                                    label: 'Items',
                                    data: [{aiStats["WithAITags"]}, {aiStats["WithoutAITags"]}],
                                    backgroundColor: [
                                        'rgba(79, 70, 229, 0.6)',
                                        'rgba(209, 213, 219, 0.6)'
                                    ],
                                    borderColor: [
                                        'rgba(79, 70, 229, 1)',
                                        'rgba(209, 213, 219, 1)'
                                    ],
                                    borderWidth: 1
                                }}]
                            }},
                            options: {{
                                responsive: true,
                                scales: {{
                                    y: {{
                                        beginAtZero: true,
                                        ticks: {{
                                            precision: 0
                                        }}
                                    }}
                                }},
                                plugins: {{
                                    legend: {{
                                        display: false
                                    }}
                                }}
                            }}
                        }});
                    }});
                </script>";
            litAiTagsChartData.Text = aiTagsChartScript;

            // Generate chart.js script for image usage
            string imageUsageChartScript = $@"
                <script>
                    document.addEventListener('DOMContentLoaded', function() {{
                        var ctx = document.getElementById('imageUsageChart').getContext('2d');
                        var imageUsageChart = new Chart(ctx, {{
                            type: 'bar',
                            data: {{
                                labels: ['With Images', 'Without Images'],
                                datasets: [{{
                                    label: 'Items',
                                    data: [{aiStats["WithImages"]}, {aiStats["WithoutImages"]}],
                                    backgroundColor: [
                                        'rgba(16, 185, 129, 0.6)',
                                        'rgba(209, 213, 219, 0.6)'
                                    ],
                                    borderColor: [
                                        'rgba(16, 185, 129, 1)',
                                        'rgba(209, 213, 219, 1)'
                                    ],
                                    borderWidth: 1
                                }}]
                            }},
                            options: {{
                                responsive: true,
                                scales: {{
                                    y: {{
                                        beginAtZero: true,
                                        ticks: {{
                                            precision: 0
                                        }}
                                    }}
                                }},
                                plugins: {{
                                    legend: {{
                                        display: false
                                    }}
                                }}
                            }}
                        }});
                    }});
                </script>";
            litImageUsageChartData.Text = imageUsageChartScript;
        }

        protected string GetActionTypeDisplay(string actionType)
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

        protected string GetActionClass(string actionType)
        {
            switch (actionType?.ToLower())
            {
                case "create":
                case "created":
                    return "px-2 py-1 text-xs font-medium rounded-full bg-green-100 text-green-800";
                case "update":
                case "updated":
                    return "px-2 py-1 text-xs font-medium rounded-full bg-blue-100 text-blue-800";
                case "delete":
                case "deleted":
                    return "px-2 py-1 text-xs font-medium rounded-full bg-red-100 text-red-800";
                case "stockin":
                case "adjust_increase":
                    return "px-2 py-1 text-xs font-medium rounded-full bg-indigo-100 text-indigo-800";
                case "stockout":
                case "adjust_decrease":
                    return "px-2 py-1 text-xs font-medium rounded-full bg-yellow-100 text-yellow-800";
                default:
                    return "px-2 py-1 text-xs font-medium rounded-full bg-gray-100 text-gray-800";
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

        protected void btnExportPDF_Click(object sender, EventArgs e)
        {
            // In a real implementation, this would generate a PDF report
            // For now, just create a simple alert
            ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                "alert('PDF export functionality would be implemented here.');", true);
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            // In a real implementation, this would generate an Excel report
            // For now, just create a simple alert
            ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                "alert('Excel export functionality would be implemented here.');", true);
        }

        protected void btnExportCSV_Click(object sender, EventArgs e)
        {
            // In a real implementation, this would generate a CSV report
            // For now, just create a simple alert
            ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                "alert('CSV export functionality would be implemented here.');", true);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            _userService.Logout();
            Response.Redirect("~/Pages/Auth/Login.aspx");
        }
    }
}