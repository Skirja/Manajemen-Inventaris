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
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using System.Globalization;
using ClosedXML.Excel;

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
                ddlCategory.Items.Add(new System.Web.UI.WebControls.ListItem(category.Name, category.CategoryID.ToString()));
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

            // Apply category filter if selected
            if (_categoryId > 0)
            {
                // If a specific category is selected, only show that category
                categoryDistribution = categoryDistribution
                    .Where(c => c.CategoryID == _categoryId)
                    .ToList();
            }

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

            // Add placeholder data if no categories with items exist
            if (categoryLabels.Length == 1)
            {
                categoryLabels.Append("'No Data'");
                categoryData.Append("1");
                categoryColors.Append("'rgba(209, 213, 219, 0.8)'"); // Gray color for no data
            }
            else
            {
                // Remove trailing commas
                categoryLabels.Length--;
                categoryData.Length--;
                categoryColors.Length--;
            }

            // Close arrays
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
                                    }},
                                    tooltip: {{
                                        callbacks: {{
                                            label: function(context) {{
                                                if (context.label === 'No Data') {{
                                                    return 'No data available for selected filters';
                                                }}
                                                return context.label + ': ' + context.raw;
                                            }}
                                        }}
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
            try
            {
                // Create a new PDF document
                Document document = new Document(PageSize.A4, 36, 36, 54, 36);

                // Set the response content type and headers for PDF download
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", $"attachment;filename=InventoryReport_{_startDate:yyyy-MM-dd}_to_{_endDate:yyyy-MM-dd}.pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);

                // Create a PdfWriter that writes to the output stream
                PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

                // Open the document for writing
                document.Open();

                // Add document metadata
                document.AddTitle("Inventory Management Report");
                document.AddAuthor("Manajemen Inventaris System");
                document.AddCreator("Manajemen Inventaris System");

                // Add report title
                iTextSharp.text.Font titleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 18, iTextSharp.text.Font.BOLD, new BaseColor(79, 70, 229));
                Paragraph title = new Paragraph("Inventory Management Report", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                title.SpacingAfter = 10;
                document.Add(title);

                // Add date range and filter info
                iTextSharp.text.Font subtitleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);
                Paragraph dateRange = new Paragraph($"Period: {_startDate:MMMM dd, yyyy} to {_endDate:MMMM dd, yyyy}", subtitleFont);
                dateRange.Alignment = Element.ALIGN_CENTER;
                document.Add(dateRange);

                // Add category filter if specified
                if (_categoryId > 0)
                {
                    string categoryName = ddlCategory.SelectedItem.Text;
                    Paragraph categoryFilter = new Paragraph($"Category: {categoryName}", subtitleFont);
                    categoryFilter.Alignment = Element.ALIGN_CENTER;
                    categoryFilter.SpacingAfter = 20;
                    document.Add(categoryFilter);
                }
                else
                {
                    Paragraph categoryFilter = new Paragraph("Category: All Categories", subtitleFont);
                    categoryFilter.Alignment = Element.ALIGN_CENTER;
                    categoryFilter.SpacingAfter = 20;
                    document.Add(categoryFilter);
                }

                // Add dashboard overview section
                iTextSharp.text.Font sectionTitleFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 14, iTextSharp.text.Font.BOLD, new BaseColor(79, 70, 229));
                Paragraph overviewTitle = new Paragraph("Dashboard Overview", sectionTitleFont);
                overviewTitle.SpacingBefore = 15;
                overviewTitle.SpacingAfter = 10;
                document.Add(overviewTitle);

                // Create a table for dashboard metrics
                PdfPTable overviewTable = new PdfPTable(4);
                overviewTable.WidthPercentage = 100;
                overviewTable.SpacingAfter = 20;

                // Add header cells
                iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE);

                PdfPCell headerCell = new PdfPCell(new Phrase("Total Items", headerFont));
                headerCell.BackgroundColor = new BaseColor(79, 70, 229);
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell.Padding = 5f;
                overviewTable.AddCell(headerCell);

                headerCell = new PdfPCell(new Phrase("Categories", headerFont));
                headerCell.BackgroundColor = new BaseColor(16, 185, 129);
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell.Padding = 5f;
                overviewTable.AddCell(headerCell);

                headerCell = new PdfPCell(new Phrase("Total Stock", headerFont));
                headerCell.BackgroundColor = new BaseColor(245, 158, 11);
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell.Padding = 5f;
                overviewTable.AddCell(headerCell);

                headerCell = new PdfPCell(new Phrase("Low Stock Items", headerFont));
                headerCell.BackgroundColor = new BaseColor(239, 68, 68);
                headerCell.HorizontalAlignment = Element.ALIGN_CENTER;
                headerCell.Padding = 5f;
                overviewTable.AddCell(headerCell);

                // Add data cells
                iTextSharp.text.Font dataFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);

                PdfPCell dataCell = new PdfPCell(new Phrase(lblTotalItems.Text, dataFont));
                dataCell.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCell.Padding = 5f;
                overviewTable.AddCell(dataCell);

                dataCell = new PdfPCell(new Phrase(lblTotalCategories.Text, dataFont));
                dataCell.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCell.Padding = 5f;
                overviewTable.AddCell(dataCell);

                dataCell = new PdfPCell(new Phrase(lblTotalStock.Text, dataFont));
                dataCell.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCell.Padding = 5f;
                overviewTable.AddCell(dataCell);

                dataCell = new PdfPCell(new Phrase(lblLowStock.Text, dataFont));
                dataCell.HorizontalAlignment = Element.ALIGN_CENTER;
                dataCell.Padding = 5f;
                overviewTable.AddCell(dataCell);

                document.Add(overviewTable);

                // Add movement report section
                Paragraph movementTitle = new Paragraph("Inventory Movement Report", sectionTitleFont);
                movementTitle.SpacingBefore = 15;
                movementTitle.SpacingAfter = 10;
                document.Add(movementTitle);

                // Get movement statistics and generate table
                var movementStats = _inventoryService.GetInventoryMovementStats(_startDate, _endDate);

                PdfPTable movementTable = new PdfPTable(5);
                movementTable.WidthPercentage = 100;
                movementTable.SpacingAfter = 20;

                // Add header row
                movementTable.AddCell(CreateHeaderCell("Stock In", new BaseColor(79, 70, 229)));
                movementTable.AddCell(CreateHeaderCell("Stock Out", new BaseColor(239, 68, 68)));
                movementTable.AddCell(CreateHeaderCell("Created", new BaseColor(16, 185, 129)));
                movementTable.AddCell(CreateHeaderCell("Updated", new BaseColor(59, 130, 246)));
                movementTable.AddCell(CreateHeaderCell("Deleted", new BaseColor(245, 158, 11)));

                // Add data row
                movementTable.AddCell(CreateDataCell(movementStats["StockIn"].ToString()));
                movementTable.AddCell(CreateDataCell(movementStats["StockOut"].ToString()));
                movementTable.AddCell(CreateDataCell(movementStats["Created"].ToString()));
                movementTable.AddCell(CreateDataCell(movementStats["Updated"].ToString()));
                movementTable.AddCell(CreateDataCell(movementStats["Deleted"].ToString()));

                document.Add(movementTable);

                // Add movement details
                iTextSharp.text.Font subheaderFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY);
                Paragraph detailsTitle = new Paragraph("Movement Details", subheaderFont);
                detailsTitle.SpacingBefore = 10;
                detailsTitle.SpacingAfter = 10;
                document.Add(detailsTitle);

                // Get movement details
                var movementDetails = _inventoryService.GetItemHistoryForReporting(_startDate, _endDate, _categoryId);

                if (movementDetails.Count > 0)
                {
                    // Create table for movement details
                    PdfPTable detailsTable = new PdfPTable(6);
                    detailsTable.WidthPercentage = 100;
                    detailsTable.SpacingAfter = 20;

                    // Set column widths
                    float[] widths = new float[] { 2f, 1.5f, 2f, 1f, 2.5f, 1.5f };
                    detailsTable.SetWidths(widths);

                    // Add header row
                    detailsTable.AddCell(CreateStandardHeaderCell("Date"));
                    detailsTable.AddCell(CreateStandardHeaderCell("Action"));
                    detailsTable.AddCell(CreateStandardHeaderCell("Item"));
                    detailsTable.AddCell(CreateStandardHeaderCell("Quantity"));
                    detailsTable.AddCell(CreateStandardHeaderCell("Notes"));
                    detailsTable.AddCell(CreateStandardHeaderCell("User"));

                    // Add data rows
                    iTextSharp.text.Font detailsRowFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);
                    bool alternateRow = false;

                    foreach (var item in movementDetails)
                    {
                        BaseColor rowColor = alternateRow ? new BaseColor(249, 250, 251) : BaseColor.WHITE;
                        alternateRow = !alternateRow;

                        detailsTable.AddCell(CreateDetailCell(item.ChangedDate.ToString("yyyy-MM-dd HH:mm"), detailsRowFont, rowColor));
                        detailsTable.AddCell(CreateDetailCell(GetActionTypeDisplay(item.ChangeType), detailsRowFont, rowColor));
                        detailsTable.AddCell(CreateDetailCell(item.ItemName, detailsRowFont, rowColor));
                        detailsTable.AddCell(CreateDetailCell(FormatQuantityChange(item.ChangeType, item.QuantityChanged), detailsRowFont, rowColor));
                        detailsTable.AddCell(CreateDetailCell(item.Notes ?? "", detailsRowFont, rowColor));
                        detailsTable.AddCell(CreateDetailCell(item.ChangedByUsername, detailsRowFont, rowColor));
                    }

                    document.Add(detailsTable);
                }
                else
                {
                    iTextSharp.text.Font italicFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.ITALIC, BaseColor.GRAY);
                    Paragraph noData = new Paragraph("No movement data found for the selected period.", italicFont);
                    noData.Alignment = Element.ALIGN_CENTER;
                    noData.SpacingAfter = 20;
                    document.Add(noData);
                }

                // Add page break before category analysis
                document.NewPage();

                // Add category analysis section
                Paragraph categoryTitle = new Paragraph("Category Analysis", sectionTitleFont);
                categoryTitle.SpacingBefore = 15;
                categoryTitle.SpacingAfter = 10;
                document.Add(categoryTitle);

                // Get category distribution
                var categoryDistribution = _inventoryService.GetCategoryDistribution();

                // Apply category filter if selected
                if (_categoryId > 0)
                {
                    categoryDistribution = categoryDistribution
                        .Where(c => c.CategoryID == _categoryId)
                        .ToList();
                }

                // Calculate total items for percentage
                int totalItems = categoryDistribution.Sum(c => c.ItemCount);

                if (categoryDistribution.Count > 0 && totalItems > 0)
                {
                    // Create table for category details
                    PdfPTable categoryTable = new PdfPTable(3);
                    categoryTable.WidthPercentage = 100;
                    categoryTable.SpacingAfter = 20;

                    // Set column widths
                    float[] catWidths = new float[] { 2f, 1f, 1f };
                    categoryTable.SetWidths(catWidths);

                    // Add header row
                    categoryTable.AddCell(CreateStandardHeaderCell("Category"));
                    categoryTable.AddCell(CreateStandardHeaderCell("Items"));
                    categoryTable.AddCell(CreateStandardHeaderCell("Percentage"));

                    // Add data rows
                    iTextSharp.text.Font categoryRowFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);
                    bool alternateRow = false;

                    foreach (var category in categoryDistribution)
                    {
                        BaseColor rowColor = alternateRow ? new BaseColor(249, 250, 251) : BaseColor.WHITE;
                        alternateRow = !alternateRow;

                        double percentage = (double)category.ItemCount / totalItems;

                        categoryTable.AddCell(CreateDetailCell(category.Name, categoryRowFont, rowColor));
                        categoryTable.AddCell(CreateDetailCell(category.ItemCount.ToString(), categoryRowFont, rowColor, Element.ALIGN_CENTER));
                        categoryTable.AddCell(CreateDetailCell(string.Format("{0:P1}", percentage), categoryRowFont, rowColor, Element.ALIGN_CENTER));
                    }

                    document.Add(categoryTable);
                }
                else
                {
                    iTextSharp.text.Font italicFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.ITALIC, BaseColor.GRAY);
                    Paragraph noData = new Paragraph("No category data found for the selected filters.", italicFont);
                    noData.Alignment = Element.ALIGN_CENTER;
                    noData.SpacingAfter = 20;
                    document.Add(noData);
                }

                // Add AI statistics section
                Paragraph aiTitle = new Paragraph("AI Recognition Statistics", sectionTitleFont);
                aiTitle.SpacingBefore = 15;
                aiTitle.SpacingAfter = 10;
                document.Add(aiTitle);

                // Get AI stats
                var aiStats = _inventoryService.GetAIRecognitionStats(_startDate, _endDate);

                // Add AI Tags Usage subsection
                Paragraph tagsTitle = new Paragraph("AI Tags Usage", subheaderFont);
                tagsTitle.SpacingBefore = 10;
                tagsTitle.SpacingAfter = 5;
                document.Add(tagsTitle);

                PdfPTable tagsTable = new PdfPTable(2);
                tagsTable.WidthPercentage = 70;
                tagsTable.HorizontalAlignment = Element.ALIGN_CENTER;
                tagsTable.SpacingAfter = 20;

                // Add header
                tagsTable.AddCell(CreateCustomHeaderCell("Status", new BaseColor(79, 70, 229)));
                tagsTable.AddCell(CreateCustomHeaderCell("Count", new BaseColor(79, 70, 229)));

                // Add data rows
                iTextSharp.text.Font aiRowFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);

                tagsTable.AddCell(CreateDetailCell("Items with AI Tags", aiRowFont, BaseColor.WHITE));
                tagsTable.AddCell(CreateDetailCell(aiStats["WithAITags"].ToString(), aiRowFont, BaseColor.WHITE, Element.ALIGN_CENTER));

                tagsTable.AddCell(CreateDetailCell("Items without AI Tags", aiRowFont, BaseColor.WHITE));
                tagsTable.AddCell(CreateDetailCell(aiStats["WithoutAITags"].ToString(), aiRowFont, BaseColor.WHITE, Element.ALIGN_CENTER));

                document.Add(tagsTable);

                // Add Image Usage subsection
                Paragraph imageTitle = new Paragraph("Image Usage", subheaderFont);
                imageTitle.SpacingBefore = 10;
                imageTitle.SpacingAfter = 5;
                document.Add(imageTitle);

                PdfPTable imageTable = new PdfPTable(2);
                imageTable.WidthPercentage = 70;
                imageTable.HorizontalAlignment = Element.ALIGN_CENTER;
                imageTable.SpacingAfter = 20;

                // Add header
                imageTable.AddCell(CreateCustomHeaderCell("Status", new BaseColor(16, 185, 129)));
                imageTable.AddCell(CreateCustomHeaderCell("Count", new BaseColor(16, 185, 129)));

                // Add data rows
                imageTable.AddCell(CreateDetailCell("Items with Images", aiRowFont, BaseColor.WHITE));
                imageTable.AddCell(CreateDetailCell(aiStats["WithImages"].ToString(), aiRowFont, BaseColor.WHITE, Element.ALIGN_CENTER));

                imageTable.AddCell(CreateDetailCell("Items without Images", aiRowFont, BaseColor.WHITE));
                imageTable.AddCell(CreateDetailCell(aiStats["WithoutImages"].ToString(), aiRowFont, BaseColor.WHITE, Element.ALIGN_CENTER));

                document.Add(imageTable);

                // Add footer
                iTextSharp.text.Font footerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 8, iTextSharp.text.Font.ITALIC, BaseColor.GRAY);
                Paragraph footer = new Paragraph("Generated on " + DateTime.Now.ToString("MMMM dd, yyyy HH:mm:ss"), footerFont);
                footer.Alignment = Element.ALIGN_CENTER;
                footer.SpacingBefore = 30;
                document.Add(footer);

                // Close the document
                document.Close();

                // End the response to prevent other content from being written
                Response.End();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    $"alert('Error exporting to PDF: {ex.Message}');", true);
                System.Diagnostics.Debug.WriteLine($"PDF Export Error: {ex.Message}");
            }
        }

        private PdfPCell CreateHeaderCell(string text, BaseColor backgroundColor)
        {
            iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
            PdfPCell cell = new PdfPCell(new Phrase(text, headerFont));
            cell.BackgroundColor = backgroundColor;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5f;
            cell.BorderColor = BaseColor.WHITE;
            cell.BorderWidth = 1f;
            return cell;
        }

        private PdfPCell CreateStandardHeaderCell(string text)
        {
            iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
            PdfPCell cell = new PdfPCell(new Phrase(text, headerFont));
            cell.BackgroundColor = new BaseColor(79, 70, 229);
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5f;
            cell.BorderColor = BaseColor.WHITE;
            cell.BorderWidth = 0.5f;
            return cell;
        }

        private PdfPCell CreateCustomHeaderCell(string text, BaseColor backgroundColor)
        {
            iTextSharp.text.Font headerFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 10, iTextSharp.text.Font.BOLD, BaseColor.WHITE);
            PdfPCell cell = new PdfPCell(new Phrase(text, headerFont));
            cell.BackgroundColor = backgroundColor;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5f;
            cell.BorderColor = BaseColor.WHITE;
            cell.BorderWidth = 0.5f;
            return cell;
        }

        private PdfPCell CreateDataCell(string text)
        {
            iTextSharp.text.Font cellFont = new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 12, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);
            PdfPCell cell = new PdfPCell(new Phrase(text, cellFont));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.BorderWidth = 0.5f;
            return cell;
        }

        private PdfPCell CreateDetailCell(string text, iTextSharp.text.Font font, BaseColor backgroundColor, int alignment = Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = backgroundColor;
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Padding = 5f;
            cell.BorderColor = BaseColor.LIGHT_GRAY;
            cell.BorderWidth = 0.5f;
            return cell;
        }

        protected void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a new Excel workbook
                using (var workbook = new XLWorkbook())
                {
                    // Add worksheets for each section
                    var overviewSheet = workbook.Worksheets.Add("Dashboard Overview");
                    var movementSheet = workbook.Worksheets.Add("Inventory Movement");
                    var categorySheet = workbook.Worksheets.Add("Category Analysis");
                    var aiStatsSheet = workbook.Worksheets.Add("AI Recognition");

                    // Add report title and metadata to each worksheet
                    AddReportHeaderToWorksheet(overviewSheet);
                    AddReportHeaderToWorksheet(movementSheet);
                    AddReportHeaderToWorksheet(categorySheet);
                    AddReportHeaderToWorksheet(aiStatsSheet);

                    // Generate Dashboard Overview sheet
                    GenerateOverviewSection(overviewSheet);

                    // Generate Inventory Movement sheet
                    GenerateMovementSection(movementSheet);

                    // Generate Category Analysis sheet
                    GenerateCategorySection(categorySheet);

                    // Generate AI Recognition Stats sheet
                    GenerateAIStatsSection(aiStatsSheet);

                    // Auto-fit columns for better readability
                    foreach (var worksheet in workbook.Worksheets)
                    {
                        worksheet.Columns().AdjustToContents();
                    }

                    // Set content type and headers for Excel download
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", $"attachment;filename=InventoryReport_{_startDate:yyyy-MM-dd}_to_{_endDate:yyyy-MM-dd}.xlsx");
                    
                    // Save the workbook to the response stream
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        workbook.SaveAs(memoryStream);
                        memoryStream.WriteTo(Response.OutputStream);
                    }
                    
                    Response.End();
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    $"alert('Error exporting to Excel: {ex.Message}');", true);
                System.Diagnostics.Debug.WriteLine($"Excel Export Error: {ex.Message}");
            }
        }

        private void AddReportHeaderToWorksheet(IXLWorksheet worksheet)
        {
            // Add report title
            worksheet.Cell("A1").Value = "Inventory Management Report";
            worksheet.Cell("A1").Style
                .Font.SetBold(true)
                .Font.SetFontSize(16)
                .Font.SetFontColor(XLColor.FromHtml("#4F46E5"))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range("A1:H1").Merge();

            // Add date range
            worksheet.Cell("A2").Value = $"Period: {_startDate:MMMM dd, yyyy} to {_endDate:MMMM dd, yyyy}";
            worksheet.Cell("A2").Style
                .Font.SetFontSize(12)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range("A2:H2").Merge();

            // Add category filter
            string categoryText = _categoryId > 0 ? ddlCategory.SelectedItem.Text : "All Categories";
            worksheet.Cell("A3").Value = $"Category: {categoryText}";
            worksheet.Cell("A3").Style
                .Font.SetFontSize(12)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range("A3:H3").Merge();

            // Add empty row for spacing
            worksheet.Cell("A4").Value = "";
        }

        private void GenerateOverviewSection(IXLWorksheet worksheet)
        {
            // Add section title
            worksheet.Cell("A5").Value = "Dashboard Overview";
            worksheet.Cell("A5").Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.FromHtml("#4F46E5"))
                .Font.SetFontSize(14);
            worksheet.Range("A5:D5").Merge();

            // Add header row for metrics
            var headerRow = worksheet.Row(6);
            headerRow.Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Cell("A6").Value = "Total Items";
            worksheet.Cell("A6").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#4F46E5"));

            worksheet.Cell("B6").Value = "Categories";
            worksheet.Cell("B6").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#10B981"));

            worksheet.Cell("C6").Value = "Total Stock";
            worksheet.Cell("C6").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F59E0B"));

            worksheet.Cell("D6").Value = "Low Stock Items";
            worksheet.Cell("D6").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EF4444"));

            // Add data row
            worksheet.Cell("A7").Value = lblTotalItems.Text;
            worksheet.Cell("B7").Value = lblTotalCategories.Text;
            worksheet.Cell("C7").Value = lblTotalStock.Text;
            worksheet.Cell("D7").Value = lblLowStock.Text;

            // Style data row
            worksheet.Range("A7:D7").Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Gray);
        }

        private void GenerateMovementSection(IXLWorksheet worksheet)
        {
            // Add section title
            worksheet.Cell("A6").Value = "Inventory Movement Summary";
            worksheet.Cell("A6").Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.FromHtml("#4F46E5"))
                .Font.SetFontSize(14);

            // Get movement statistics
            var movementStats = _inventoryService.GetInventoryMovementStats(_startDate, _endDate);

            // Add header row for movement summary
            var headerRow = worksheet.Row(7);
            headerRow.Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            worksheet.Cell("A7").Value = "Stock In";
            worksheet.Cell("A7").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#4F46E5"));

            worksheet.Cell("B7").Value = "Stock Out";
            worksheet.Cell("B7").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#EF4444"));

            worksheet.Cell("C7").Value = "Created";
            worksheet.Cell("C7").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#10B981"));

            worksheet.Cell("D7").Value = "Updated";
            worksheet.Cell("D7").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#3B82F6"));

            worksheet.Cell("E7").Value = "Deleted";
            worksheet.Cell("E7").Style.Fill.SetBackgroundColor(XLColor.FromHtml("#F59E0B"));

            // Add data row
            worksheet.Cell("A8").Value = movementStats["StockIn"];
            worksheet.Cell("B8").Value = movementStats["StockOut"];
            worksheet.Cell("C8").Value = movementStats["Created"];
            worksheet.Cell("D8").Value = movementStats["Updated"];
            worksheet.Cell("E8").Value = movementStats["Deleted"];

            // Style data row
            worksheet.Range("A8:E8").Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Gray);

            // Add Movement Details section
            worksheet.Cell("A10").Value = "Movement Details";
            worksheet.Cell("A10").Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.FromHtml("#4F46E5"))
                .Font.SetFontSize(14);

            // Get movement details
            var movementDetails = _inventoryService.GetItemHistoryForReporting(_startDate, _endDate, _categoryId);

            // If there are movement details, create a table
            if (movementDetails.Count > 0)
            {
                // Add header row for details table
                var detailsHeaderRow = worksheet.Row(11);
                detailsHeaderRow.Style
                    .Font.SetBold(true)
                    .Font.SetFontColor(XLColor.White)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell("A11").Value = "Date";
                worksheet.Cell("B11").Value = "Action";
                worksheet.Cell("C11").Value = "Item";
                worksheet.Cell("D11").Value = "Quantity";
                worksheet.Cell("E11").Value = "Notes";
                worksheet.Cell("F11").Value = "User";

                // Style header row
                worksheet.Range("A11:F11").Style
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#4F46E5"));

                // Add data rows
                int rowIndex = 12;
                bool alternateRow = false;

                foreach (var item in movementDetails)
                {
                    worksheet.Cell($"A{rowIndex}").Value = item.ChangedDate.ToString("yyyy-MM-dd HH:mm");
                    worksheet.Cell($"B{rowIndex}").Value = GetActionTypeDisplay(item.ChangeType);
                    worksheet.Cell($"C{rowIndex}").Value = item.ItemName;
                    worksheet.Cell($"D{rowIndex}").Value = FormatQuantityChange(item.ChangeType, item.QuantityChanged);
                    worksheet.Cell($"E{rowIndex}").Value = item.Notes ?? "";
                    worksheet.Cell($"F{rowIndex}").Value = item.ChangedByUsername;

                    // Apply alternating row styling
                    if (alternateRow)
                    {
                        worksheet.Range($"A{rowIndex}:F{rowIndex}").Style
                            .Fill.SetBackgroundColor(XLColor.FromHtml("#F9FAFB"));
                    }

                    // Style based on action type
                    XLColor actionColor = XLColor.Black;
                    switch (item.ChangeType?.ToLower())
                    {
                        case "create":
                        case "created":
                            actionColor = XLColor.FromHtml("#10B981"); // Green
                            break;
                        case "update":
                        case "updated":
                            actionColor = XLColor.FromHtml("#3B82F6"); // Blue
                            break;
                        case "delete":
                        case "deleted":
                            actionColor = XLColor.FromHtml("#EF4444"); // Red
                            break;
                        case "stockin":
                        case "adjust_increase":
                            actionColor = XLColor.FromHtml("#4F46E5"); // Indigo
                            break;
                        case "stockout":
                        case "adjust_decrease":
                            actionColor = XLColor.FromHtml("#F59E0B"); // Yellow
                            break;
                    }

                    worksheet.Cell($"B{rowIndex}").Style.Font.SetFontColor(actionColor);

                    rowIndex++;
                    alternateRow = !alternateRow;
                }

                // Create Excel table for filtering
                var table = worksheet.Range($"A11:F{rowIndex - 1}").CreateTable("MovementDetailsTable");
                table.Theme = XLTableTheme.TableStyleMedium2;
            }
            else
            {
                // No data message
                worksheet.Cell("A11").Value = "No movement data found for the selected period.";
                worksheet.Cell("A11").Style
                    .Font.SetItalic(true)
                    .Font.SetFontColor(XLColor.Gray)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range("A11:F11").Merge();
            }
        }

        private void GenerateCategorySection(IXLWorksheet worksheet)
        {
            // Add section title
            worksheet.Cell("A6").Value = "Category Analysis";
            worksheet.Cell("A6").Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.FromHtml("#4F46E5"))
                .Font.SetFontSize(14);

            // Get category distribution
            var categoryDistribution = _inventoryService.GetCategoryDistribution();

            // Apply category filter if selected
            if (_categoryId > 0)
            {
                categoryDistribution = categoryDistribution
                    .Where(c => c.CategoryID == _categoryId)
                    .ToList();
            }

            // Calculate total items for percentage
            int totalItems = categoryDistribution.Sum(c => c.ItemCount);

            if (categoryDistribution.Count > 0 && totalItems > 0)
            {
                // Add header row for category table
                var headerRow = worksheet.Row(7);
                headerRow.Style
                    .Font.SetBold(true)
                    .Font.SetFontColor(XLColor.White)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                worksheet.Cell("A7").Value = "Category";
                worksheet.Cell("B7").Value = "Items";
                worksheet.Cell("C7").Value = "Percentage";

                // Style header row
                worksheet.Range("A7:C7").Style
                    .Fill.SetBackgroundColor(XLColor.FromHtml("#4F46E5"));

                // Add data rows
                int rowIndex = 8;
                bool alternateRow = false;

                foreach (var category in categoryDistribution)
                {
                    double percentage = (double)category.ItemCount / totalItems;

                    worksheet.Cell($"A{rowIndex}").Value = category.Name;
                    worksheet.Cell($"B{rowIndex}").Value = category.ItemCount;
                    worksheet.Cell($"C{rowIndex}").Value = percentage;
                    worksheet.Cell($"C{rowIndex}").Style.NumberFormat.Format = "0.0%";

                    // Apply alternating row styling
                    if (alternateRow)
                    {
                        worksheet.Range($"A{rowIndex}:C{rowIndex}").Style
                            .Fill.SetBackgroundColor(XLColor.FromHtml("#F9FAFB"));
                    }

                    rowIndex++;
                    alternateRow = !alternateRow;
                }

                // Create Excel table for filtering
                var table = worksheet.Range($"A7:C{rowIndex - 1}").CreateTable("CategoryTable");
                table.Theme = XLTableTheme.TableStyleMedium2;

                // Add total row
                worksheet.Cell($"A{rowIndex}").Value = "TOTAL";
                worksheet.Cell($"A{rowIndex}").Style.Font.SetBold(true);
                worksheet.Cell($"B{rowIndex}").Value = totalItems;
                worksheet.Cell($"B{rowIndex}").Style.Font.SetBold(true);
                worksheet.Cell($"C{rowIndex}").Value = 1.0;
                worksheet.Cell($"C{rowIndex}").Style
                    .Font.SetBold(true)
                    .NumberFormat.Format = "0.0%";
            }
            else
            {
                // No data message
                worksheet.Cell("A7").Value = "No category data found for the selected filters.";
                worksheet.Cell("A7").Style
                    .Font.SetItalic(true)
                    .Font.SetFontColor(XLColor.Gray)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                worksheet.Range("A7:C7").Merge();
            }
        }

        private void GenerateAIStatsSection(IXLWorksheet worksheet)
        {
            // Add section title
            worksheet.Cell("A6").Value = "AI Recognition Statistics";
            worksheet.Cell("A6").Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.FromHtml("#4F46E5"))
                .Font.SetFontSize(14);

            // Get AI stats
            var aiStats = _inventoryService.GetAIRecognitionStats(_startDate, _endDate);

            // AI Tags Usage subsection
            worksheet.Cell("A8").Value = "AI Tags Usage";
            worksheet.Cell("A8").Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.FromHtml("#4F46E5"));

            // Add header row for tags table
            worksheet.Cell("A9").Value = "Status";
            worksheet.Cell("B9").Value = "Count";
            worksheet.Range("A9:B9").Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.FromHtml("#4F46E5"))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Add data rows for tags
            worksheet.Cell("A10").Value = "Items with AI Tags";
            worksheet.Cell("B10").Value = aiStats["WithAITags"];
            worksheet.Cell("A11").Value = "Items without AI Tags";
            worksheet.Cell("B11").Value = aiStats["WithoutAITags"];

            // Style data rows
            worksheet.Range("A10:B11").Style
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Gray);
            worksheet.Range("B10:B11").Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Image Usage subsection
            worksheet.Cell("A13").Value = "Image Usage";
            worksheet.Cell("A13").Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.FromHtml("#10B981"));

            // Add header row for image table
            worksheet.Cell("A14").Value = "Status";
            worksheet.Cell("B14").Value = "Count";
            worksheet.Range("A14:B14").Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.FromHtml("#10B981"))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Add data rows for image usage
            worksheet.Cell("A15").Value = "Items with Images";
            worksheet.Cell("B15").Value = aiStats["WithImages"];
            worksheet.Cell("A16").Value = "Items without Images";
            worksheet.Cell("B16").Value = aiStats["WithoutImages"];

            // Style data rows
            worksheet.Range("A15:B16").Style
                .Border.SetOutsideBorder(XLBorderStyleValues.Thin)
                .Border.SetOutsideBorderColor(XLColor.Gray);
            worksheet.Range("B15:B16").Style
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

            // Add timestamp
            worksheet.Cell("A18").Value = $"Generated on {DateTime.Now:MMMM dd, yyyy HH:mm:ss}";
            worksheet.Cell("A18").Style
                .Font.SetItalic(true)
                .Font.SetFontColor(XLColor.Gray)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
            worksheet.Range("A18:B18").Merge();
        }

        protected void btnExportCSV_Click(object sender, EventArgs e)
        {
            try
            {
                // Create a unique temp directory for our CSV files
                string tempFolder = Path.Combine(Path.GetTempPath(), "InventoryReports_" + Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempFolder);

                try
                {
                    // Generate individual CSV files
                    GenerateOverviewCSV(tempFolder);
                    GenerateMovementCSV(tempFolder);
                    GenerateCategoryCSV(tempFolder);
                    GenerateAIStatsCSV(tempFolder);
                    GenerateReadMeText(tempFolder);

                    // Set the output content type and headers for CSV
                    Response.Clear();
                    Response.ContentType = "text/csv";
                    Response.AddHeader("content-disposition", $"attachment;filename=InventoryReport_{_startDate:yyyy-MM-dd}_to_{_endDate:yyyy-MM-dd}.csv");
                    
                    // Since we can't easily create a ZIP due to assembly reference issues,
                    // we'll combine all CSVs into a single CSV with section headers
                    using (StreamWriter writer = new StreamWriter(Response.OutputStream))
                    {
                        // Add Overview data
                        writer.WriteLine("=========================================================");
                        writer.WriteLine("INVENTORY MANAGEMENT REPORT - DASHBOARD OVERVIEW");
                        writer.WriteLine("=========================================================");
                        writer.WriteLine($"Date Range: {_startDate:yyyy-MM-dd} to {_endDate:yyyy-MM-dd}");
                        string categoryText = _categoryId > 0 ? ddlCategory.SelectedItem.Text : "All Categories";
                        writer.WriteLine($"Category Filter: {categoryText}");
                        writer.WriteLine();
                        
                        writer.WriteLine("Metric,Value");
                        writer.WriteLine($"Total Items,{lblTotalItems.Text}");
                        writer.WriteLine($"Total Categories,{lblTotalCategories.Text}");
                        writer.WriteLine($"Total Stock,{lblTotalStock.Text}");
                        writer.WriteLine($"Low Stock Items,{lblLowStock.Text}");
                        
                        writer.WriteLine();
                        writer.WriteLine("Movement Statistics,Count");
                        var movementStats = _inventoryService.GetInventoryMovementStats(_startDate, _endDate);
                        writer.WriteLine($"Stock In,{movementStats["StockIn"]}");
                        writer.WriteLine($"Stock Out,{movementStats["StockOut"]}");
                        writer.WriteLine($"Created,{movementStats["Created"]}");
                        writer.WriteLine($"Updated,{movementStats["Updated"]}");
                        writer.WriteLine($"Deleted,{movementStats["Deleted"]}");
                        
                        // Add Inventory Movement data
                        writer.WriteLine();
                        writer.WriteLine("=========================================================");
                        writer.WriteLine("INVENTORY MOVEMENT DETAILS");
                        writer.WriteLine("=========================================================");
                        writer.WriteLine();
                        
                        writer.WriteLine("Date,Action,Item,Quantity,Notes,User");
                        var movementDetails = _inventoryService.GetItemHistoryForReporting(_startDate, _endDate, _categoryId);
                        foreach (var item in movementDetails)
                        {
                            writer.WriteLine(
                                $"{EscapeCsvField(item.ChangedDate.ToString("yyyy-MM-dd HH:mm:ss"))}," +
                                $"{EscapeCsvField(GetActionTypeDisplay(item.ChangeType))}," +
                                $"{EscapeCsvField(item.ItemName)}," +
                                $"{EscapeCsvField(FormatQuantityChange(item.ChangeType, item.QuantityChanged))}," +
                                $"{EscapeCsvField(item.Notes ?? "")}," +
                                $"{EscapeCsvField(item.ChangedByUsername)}"
                            );
                        }
                        
                        // Add Category Analysis data
                        writer.WriteLine();
                        writer.WriteLine("=========================================================");
                        writer.WriteLine("CATEGORY ANALYSIS");
                        writer.WriteLine("=========================================================");
                        writer.WriteLine();
                        
                        writer.WriteLine("Category,Items,Percentage");
                        var categoryDistribution = _inventoryService.GetCategoryDistribution();
                        if (_categoryId > 0)
                        {
                            categoryDistribution = categoryDistribution
                                .Where(c => c.CategoryID == _categoryId)
                                .ToList();
                        }
                        
                        int totalItems = categoryDistribution.Sum(c => c.ItemCount);
                        foreach (var category in categoryDistribution)
                        {
                            double percentage = totalItems > 0 ? (double)category.ItemCount / totalItems : 0;
                            writer.WriteLine(
                                $"{EscapeCsvField(category.Name)}," +
                                $"{category.ItemCount}," +
                                $"{percentage.ToString("P1", CultureInfo.InvariantCulture)}"
                            );
                        }
                        
                        writer.WriteLine(
                            $"{EscapeCsvField("TOTAL")}," +
                            $"{totalItems}," +
                            $"{(totalItems > 0 ? "100.0%" : "0.0%")}"
                        );
                        
                        // Add AI Stats data
                        writer.WriteLine();
                        writer.WriteLine("=========================================================");
                        writer.WriteLine("AI RECOGNITION STATISTICS");
                        writer.WriteLine("=========================================================");
                        writer.WriteLine();
                        
                        writer.WriteLine("AI TAGS USAGE");
                        writer.WriteLine("Status,Count");
                        var aiStats = _inventoryService.GetAIRecognitionStats(_startDate, _endDate);
                        writer.WriteLine($"Items with AI Tags,{aiStats["WithAITags"]}");
                        writer.WriteLine($"Items without AI Tags,{aiStats["WithoutAITags"]}");
                        
                        writer.WriteLine();
                        writer.WriteLine("IMAGE USAGE");
                        writer.WriteLine("Status,Count");
                        writer.WriteLine($"Items with Images,{aiStats["WithImages"]}");
                        writer.WriteLine($"Items without Images,{aiStats["WithoutImages"]}");
                        
                        writer.WriteLine();
                        writer.WriteLine("=========================================================");
                        writer.WriteLine($"Report generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                        writer.WriteLine("=========================================================");
                    }
                    
                    Response.End();
                }
                finally
                {
                    // Clean up temp directory and files
                    try
                    {
                        if (Directory.Exists(tempFolder))
                        {
                            Directory.Delete(tempFolder, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error cleaning up temp files: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "alertMessage",
                    $"alert('Error exporting to CSV: {ex.Message}');", true);
                System.Diagnostics.Debug.WriteLine($"CSV Export Error: {ex.Message}");
            }
        }

        private void GenerateReadMeText(string folderPath)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("INVENTORY MANAGEMENT REPORT");
            sb.AppendLine("==========================");
            sb.AppendLine();
            sb.AppendLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"Date Range: {_startDate:yyyy-MM-dd} to {_endDate:yyyy-MM-dd}");
            
            string categoryText = _categoryId > 0 ? ddlCategory.SelectedItem.Text : "All Categories";
            sb.AppendLine($"Category Filter: {categoryText}");
            sb.AppendLine();
            sb.AppendLine("Files included in this package:");
            sb.AppendLine("1. Overview.csv - Dashboard overview metrics");
            sb.AppendLine("2. InventoryMovement.csv - Detailed inventory movement history");
            sb.AppendLine("3. CategoryAnalysis.csv - Category distribution analysis");
            sb.AppendLine("4. AIStats.csv - AI recognition statistics");
            sb.AppendLine();
            sb.AppendLine("For any issues with this report, please contact the system administrator.");

            File.WriteAllText(Path.Combine(folderPath, "README.txt"), sb.ToString());
        }

        private void GenerateOverviewCSV(string folderPath)
        {
            string filePath = Path.Combine(folderPath, "Overview.csv");
            
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Write header
                writer.WriteLine("Metric,Value");
                
                // Write data
                writer.WriteLine($"Total Items,{lblTotalItems.Text}");
                writer.WriteLine($"Total Categories,{lblTotalCategories.Text}");
                writer.WriteLine($"Total Stock,{lblTotalStock.Text}");
                writer.WriteLine($"Low Stock Items,{lblLowStock.Text}");

                // Add movement stats summary
                writer.WriteLine();
                writer.WriteLine("Movement Statistics,Count");
                
                var movementStats = _inventoryService.GetInventoryMovementStats(_startDate, _endDate);
                writer.WriteLine($"Stock In,{movementStats["StockIn"]}");
                writer.WriteLine($"Stock Out,{movementStats["StockOut"]}");
                writer.WriteLine($"Created,{movementStats["Created"]}");
                writer.WriteLine($"Updated,{movementStats["Updated"]}");
                writer.WriteLine($"Deleted,{movementStats["Deleted"]}");
            }
        }

        private void GenerateMovementCSV(string folderPath)
        {
            string filePath = Path.Combine(folderPath, "InventoryMovement.csv");
            
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Write header
                writer.WriteLine("Date,Action,Item,Quantity,Notes,User");
                
                // Get movement details
                var movementDetails = _inventoryService.GetItemHistoryForReporting(_startDate, _endDate, _categoryId);
                
                // Write data rows
                foreach (var item in movementDetails)
                {
                    writer.WriteLine(
                        $"{EscapeCsvField(item.ChangedDate.ToString("yyyy-MM-dd HH:mm:ss"))}," +
                        $"{EscapeCsvField(GetActionTypeDisplay(item.ChangeType))}," +
                        $"{EscapeCsvField(item.ItemName)}," +
                        $"{EscapeCsvField(FormatQuantityChange(item.ChangeType, item.QuantityChanged))}," +
                        $"{EscapeCsvField(item.Notes ?? "")}," +
                        $"{EscapeCsvField(item.ChangedByUsername)}"
                    );
                }
            }
        }

        private void GenerateCategoryCSV(string folderPath)
        {
            string filePath = Path.Combine(folderPath, "CategoryAnalysis.csv");
            
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Write header
                writer.WriteLine("Category,Items,Percentage");
                
                // Get category distribution
                var categoryDistribution = _inventoryService.GetCategoryDistribution();
                
                // Apply category filter if selected
                if (_categoryId > 0)
                {
                    categoryDistribution = categoryDistribution
                        .Where(c => c.CategoryID == _categoryId)
                        .ToList();
                }
                
                // Calculate total items for percentage
                int totalItems = categoryDistribution.Sum(c => c.ItemCount);
                
                // Write data rows
                foreach (var category in categoryDistribution)
                {
                    double percentage = totalItems > 0 ? (double)category.ItemCount / totalItems : 0;
                    writer.WriteLine(
                        $"{EscapeCsvField(category.Name)}," +
                        $"{category.ItemCount}," +
                        $"{percentage.ToString("P1", CultureInfo.InvariantCulture)}"
                    );
                }
                
                // Add total row
                writer.WriteLine(
                    $"{EscapeCsvField("TOTAL")}," +
                    $"{totalItems}," +
                    $"{(totalItems > 0 ? "100.0%" : "0.0%")}"
                );
            }
        }

        private void GenerateAIStatsCSV(string folderPath)
        {
            string filePath = Path.Combine(folderPath, "AIStats.csv");
            
            using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                // Get AI stats
                var aiStats = _inventoryService.GetAIRecognitionStats(_startDate, _endDate);
                
                // Write AI Tags section
                writer.WriteLine("AI TAGS USAGE");
                writer.WriteLine("Status,Count");
                writer.WriteLine($"Items with AI Tags,{aiStats["WithAITags"]}");
                writer.WriteLine($"Items without AI Tags,{aiStats["WithoutAITags"]}");
                
                // Add space between sections
                writer.WriteLine();
                
                // Write Image Usage section
                writer.WriteLine("IMAGE USAGE");
                writer.WriteLine("Status,Count");
                writer.WriteLine($"Items with Images,{aiStats["WithImages"]}");
                writer.WriteLine($"Items without Images,{aiStats["WithoutImages"]}");
            }
        }

        private string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return "";
            
            // Check if the field contains special characters that need escaping
            bool needsEscaping = field.Contains(",") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r");
            
            if (needsEscaping)
            {
                // Double up any quotes and surround with quotes
                return "\"" + field.Replace("\"", "\"\"") + "\"";
            }
            
            return field;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            _userService.Logout();
            Response.Redirect("~/Pages/Auth/Login.aspx");
        }
    }
}