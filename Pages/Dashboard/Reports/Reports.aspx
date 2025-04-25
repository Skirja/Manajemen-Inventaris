<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" Async="true"
    CodeBehind="Reports.aspx.cs" Inherits="Manajemen_Inventaris.Pages.Dashboard.Reports.Reports" %>

    <asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
        <!-- Add Chart.js for visualizations -->
        <script src="https://cdn.jsdelivr.net/npm/chart.js@3.7.1/dist/chart.min.js"></script>
        <script>
            function saveScrollPosition() {
                var scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop;
                document.getElementById('<%= hdnScrollPosition.ClientID %>').value = scrollPosition;
            }

            function restoreScrollPosition() {
                var savedPosition = document.getElementById('<%= hdnScrollPosition.ClientID %>').value;
                if (savedPosition && savedPosition !== '') {
                    window.scrollTo(0, parseInt(savedPosition));
                }
            }

            // Register with the ScriptManager for AJAX support
            function pageLoad() {
                restoreScrollPosition();
            }

            // Add event listeners
            if (window.addEventListener) {
                window.addEventListener('load', restoreScrollPosition, false);
                window.addEventListener('scroll', saveScrollPosition, false);
            } else if (window.attachEvent) {
                window.attachEvent('onload', restoreScrollPosition);
                window.attachEvent('onscroll', saveScrollPosition);
            }
        </script>
    </asp:Content>

    <asp:Content ID="NavContent" ContentPlaceHolderID="NavContent" runat="server">
        <a href="~/Pages/Dashboard/Dashboard.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">Dashboard</a>
        <a href="~/Pages/Dashboard/Inventory/Inventory.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">Inventory</a>
        <a href="~/Pages/Dashboard/Reports/Reports.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-indigo-500 text-sm font-medium text-gray-900">Reports</a>
    </asp:Content>

    <asp:Content ID="LoginStatusContent" ContentPlaceHolderID="LoginStatusContent" runat="server">
        <div class="flex items-center space-x-4">
            <span class="text-sm text-gray-700">Welcome, <asp:Literal ID="litUsername" runat="server"></asp:Literal>
            </span>
            <asp:LinkButton ID="btnLogout" runat="server" OnClick="btnLogout_Click"
                CssClass="text-sm text-red-600 hover:text-red-300">Logout</asp:LinkButton>
        </div>
    </asp:Content>

    <asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
        <asp:HiddenField ID="hdnScrollPosition" runat="server" Value="0" />

        <!-- Page Header -->
        <div class="bg-white shadow overflow-hidden sm:rounded-lg mb-8">
            <div class="px-6 py-6 sm:px-8 flex justify-between items-center border-b border-gray-200">
                <div>
                    <h3 class="text-xl leading-6 font-medium text-gray-900">Inventory Reports</h3>
                    <p class="mt-2 max-w-2xl text-sm text-gray-500">Generate insights and analytics about inventory
                        status, movements, and trends</p>
                </div>
            </div>

            <!-- Filter Section -->
            <div class="px-6 py-5 sm:px-8 bg-gray-50">
                <div class="grid grid-cols-1 md:grid-cols-4 gap-6">
                    <!-- Date Range Selection -->
                    <div>
                        <label for="<%= txtStartDate.ClientID %>"
                            class="block text-sm font-medium text-gray-700 mb-2">Start Date</label>
                        <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date"
                            CssClass="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
                        </asp:TextBox>
                    </div>
                    <div>
                        <label for="<%= txtEndDate.ClientID %>" class="block text-sm font-medium text-gray-700 mb-2">End
                            Date</label>
                        <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date"
                            CssClass="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
                        </asp:TextBox>
                    </div>
                    <!-- Category Filter -->
                    <div>
                        <label for="<%= ddlCategory.ClientID %>"
                            class="block text-sm font-medium text-gray-700 mb-2">Category</label>
                        <asp:DropDownList ID="ddlCategory" runat="server"
                            CssClass="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md">
                            <asp:ListItem Text="All Categories" Value="0" />
                        </asp:DropDownList>
                    </div>
                    <!-- Generate Button -->
                    <div class="flex items-end">
                        <asp:Button ID="btnGenerateReport" runat="server" Text="Generate Report"
                            OnClick="btnGenerateReport_Click"
                            CssClass="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" />
                    </div>
                </div>
            </div>

            <!-- Dashboard Overview Section -->
            <div class="border-t border-gray-200 px-6 py-5 sm:px-8">
                <h4 class="text-lg font-semibold text-gray-700 mb-4">Dashboard Overview</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                    <div class="bg-indigo-50 p-4 rounded-lg shadow">
                        <h5 class="text-sm font-semibold text-indigo-700">Total Items</h5>
                        <asp:Label ID="lblTotalItems" runat="server"
                            CssClass="text-3xl font-bold text-indigo-800 mt-2 block">0</asp:Label>
                    </div>
                    <div class="bg-green-50 p-4 rounded-lg shadow">
                        <h5 class="text-sm font-semibold text-green-700">Categories</h5>
                        <asp:Label ID="lblTotalCategories" runat="server"
                            CssClass="text-3xl font-bold text-green-800 mt-2 block">0</asp:Label>
                    </div>
                    <div class="bg-yellow-50 p-4 rounded-lg shadow">
                        <h5 class="text-sm font-semibold text-yellow-700">Total Stock</h5>
                        <asp:Label ID="lblTotalStock" runat="server"
                            CssClass="text-3xl font-bold text-yellow-800 mt-2 block">0</asp:Label>
                    </div>
                    <div class="bg-red-50 p-4 rounded-lg shadow">
                        <h5 class="text-sm font-semibold text-red-700">Low Stock Items</h5>
                        <asp:Label ID="lblLowStock" runat="server"
                            CssClass="text-3xl font-bold text-red-800 mt-2 block">0</asp:Label>
                    </div>
                </div>
            </div>

            <!-- Inventory Movement Reports -->
            <div class="border-t border-gray-200 px-6 py-5 sm:px-8">
                <h4 class="text-lg font-semibold text-gray-700 mb-4">Inventory Movement Report</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <!-- Movement Summary -->
                    <div class="bg-white p-4 rounded-lg shadow">
                        <h5 class="text-md font-semibold text-gray-700 mb-3">Movement Summary</h5>
                        <canvas id="inventoryMovementChart" width="400" height="300"></canvas>
                        <asp:Literal ID="litInventoryMovementChartData" runat="server"></asp:Literal>
                    </div>
                    <!-- Daily Movement -->
                    <div class="bg-white p-4 rounded-lg shadow">
                        <h5 class="text-md font-semibold text-gray-700 mb-3">Daily Movement</h5>
                        <canvas id="dailyMovementChart" width="400" height="300"></canvas>
                        <asp:Literal ID="litDailyMovementChartData" runat="server"></asp:Literal>
                    </div>
                </div>

                <!-- Movement Details Table -->
                <div class="mt-6">
                    <h5 class="text-md font-semibold text-gray-700 mb-3">Movement Details</h5>
                    <div class="bg-white shadow overflow-hidden border-b border-gray-200 sm:rounded-lg">
                        <asp:GridView ID="gvMovementDetails" runat="server" AutoGenerateColumns="false"
                            CssClass="min-w-full divide-y divide-gray-200" HeaderStyle-CssClass="bg-gray-50"
                            RowStyle-CssClass="bg-white divide-y divide-gray-200"
                            AlternatingRowStyle-CssClass="bg-gray-50 divide-y divide-gray-200" GridLines="None"
                            EmptyDataText="No movement data found for the selected period."
                            EmptyDataRowStyle-CssClass="text-center py-8 text-gray-500 text-sm">
                            <Columns>
                                <asp:BoundField DataField="Date" HeaderText="Date"
                                    DataFormatString="{0:yyyy-MM-dd HH:mm}"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm text-gray-500" />
                                <asp:TemplateField HeaderText="Action"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    <ItemTemplate>
                                        <span class='<%# GetActionClass(Eval("ChangeType").ToString()) %>'>
                                            <%# GetActionTypeDisplay(Eval("ChangeType").ToString()) %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ItemName" HeaderText="Item"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm text-gray-900" />
                                <asp:BoundField DataField="QuantityDisplay" HeaderText="Quantity Change"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm text-gray-500" />
                                <asp:BoundField DataField="Notes" HeaderText="Notes"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm text-gray-500" />
                                <asp:BoundField DataField="ChangedByUsername" HeaderText="User"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm text-gray-500" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <!-- Category Analysis -->
            <div class="border-t border-gray-200 px-6 py-5 sm:px-8">
                <h4 class="text-lg font-semibold text-gray-700 mb-4">Category Analysis</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <!-- Category Distribution -->
                    <div class="bg-white p-4 rounded-lg shadow">
                        <h5 class="text-md font-semibold text-gray-700 mb-3">Category Distribution</h5>
                        <canvas id="categoryDistributionChart" width="400" height="300"></canvas>
                        <asp:Literal ID="litCategoryDistributionChartData" runat="server"></asp:Literal>
                    </div>
                    <!-- Category Table -->
                    <div class="bg-white p-4 rounded-lg shadow">
                        <h5 class="text-md font-semibold text-gray-700 mb-3">Category Details</h5>
                        <div class="overflow-x-auto">
                            <table class="min-w-full divide-y divide-gray-200">
                                <thead class="bg-gray-50">
                                    <tr>
                                        <th scope="col"
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Category
                                        </th>
                                        <th scope="col"
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Items
                                        </th>
                                        <th scope="col"
                                            class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                            Percentage
                                        </th>
                                    </tr>
                                </thead>
                                <tbody class="bg-white divide-y divide-gray-200">
                                    <asp:Repeater ID="rptCategories" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td
                                                    class="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                                                    <%# Eval("Name") %>
                                                </td>
                                                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                                    <%# Eval("ItemCount") %>
                                                </td>
                                                <td class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                                    <%# string.Format("{0:P1}", Convert.ToDouble(Eval("Percentage"))) %>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>

            <!-- AI Recognition Statistics -->
            <div class="border-t border-gray-200 px-6 py-5 sm:px-8">
                <h4 class="text-lg font-semibold text-gray-700 mb-4">AI Recognition Statistics</h4>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <!-- AI Tags Usage -->
                    <div class="bg-white p-4 rounded-lg shadow">
                        <h5 class="text-md font-semibold text-gray-700 mb-3">AI Tags Usage</h5>
                        <canvas id="aiTagsChart" width="400" height="300"></canvas>
                        <asp:Literal ID="litAiTagsChartData" runat="server"></asp:Literal>
                    </div>
                    <!-- Image Usage -->
                    <div class="bg-white p-4 rounded-lg shadow">
                        <h5 class="text-md font-semibold text-gray-700 mb-3">Image Usage</h5>
                        <canvas id="imageUsageChart" width="400" height="300"></canvas>
                        <asp:Literal ID="litImageUsageChartData" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>

            <!-- Export Options -->
            <div class="border-t border-gray-200 px-6 py-5 sm:px-8">
                <h4 class="text-lg font-semibold text-gray-700 mb-4">Export Report</h4>
                <div class="flex space-x-4">
                    <asp:Button ID="btnExportPDF" runat="server" Text="Export to PDF" OnClick="btnExportPDF_Click"
                        CssClass="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500" />
                    <asp:Button ID="btnExportExcel" runat="server" Text="Export to Excel" OnClick="btnExportExcel_Click"
                        CssClass="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500" />
                    <asp:Button ID="btnExportCSV" runat="server" Text="Export to CSV" OnClick="btnExportCSV_Click"
                        CssClass="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-yellow-600 hover:bg-yellow-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-yellow-500" />
                </div>
            </div>
        </div>
    </asp:Content>