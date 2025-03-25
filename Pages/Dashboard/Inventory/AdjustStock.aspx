<%@ Page Title="Adjust Stock" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AdjustStock.aspx.cs" Inherits="Manajemen_Inventaris.Pages.Dashboard.Inventory.AdjustStock" %>

    <asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
        <script type="text/javascript">
            window.addEventListener('scroll', function () {
                document.getElementById('<%= hdnScrollPosition.ClientID %>').value = window.scrollY;
            });

            window.addEventListener('load', function () {
                var scrollPosition = document.getElementById('<%= hdnScrollPosition.ClientID %>').value;
                if (scrollPosition) {
                    window.scrollTo(0, parseInt(scrollPosition));
                }
            });
        </script>
    </asp:Content>

    <asp:Content ID="NavContent" ContentPlaceHolderID="NavContent" runat="server">
        <a href="~/Pages/Dashboard/Dashboard.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">Dashboard</a>
        <a href="~/Pages/Dashboard/Inventory/Inventory.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-indigo-500 text-sm font-medium text-gray-900">Inventory</a>
        <a href="~/Pages/Dashboard/Upload.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">Upload</a>
        <a href="~/Pages/Dashboard/Reports.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">Reports</a>
    </asp:Content>

    <asp:Content ID="LoginStatusContent" ContentPlaceHolderID="LoginStatusContent" runat="server">
        <div class="flex items-center space-x-4">
            <span class="text-sm text-gray-700">Welcome, <asp:Literal ID="litUsername" runat="server"></asp:Literal>
            </span>
            <asp:LinkButton ID="btnLogout" runat="server" OnClick="btnLogout_Click"
                CssClass="text-sm text-red-600 hover:text-red-300">Logout</asp:LinkButton>
        </div>
    </asp:Content>

    <asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
        <!-- Hidden field for scroll position -->
        <asp:HiddenField ID="hdnScrollPosition" runat="server" />
        <asp:HiddenField ID="hdnItemId" runat="server" />

        <div class="bg-white shadow overflow-hidden sm:rounded-lg mb-8">
            <div class="px-6 py-6 sm:px-8 flex justify-between items-center border-b border-gray-200">
                <div>
                    <h3 class="text-xl leading-6 font-medium text-gray-900">Adjust Item Stock</h3>
                    <p class="mt-2 max-w-2xl text-sm text-gray-500">Update stock quantity for the selected item</p>
                </div>
                <div>
                    <asp:LinkButton ID="lnkBackToInventory" runat="server" OnClick="lnkBackToInventory_Click"
                        CausesValidation="false"
                        CssClass="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md shadow-sm text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                        </svg>
                        Back to Inventory
                    </asp:LinkButton>
                </div>
            </div>

            <!-- Notification Panel -->
            <div class="px-6 py-5 sm:px-8">
                <asp:Panel ID="pnlNotification" runat="server" CssClass="mb-6 rounded-md p-4 border" Visible="false">
                    <div class="flex">
                        <div class="flex-shrink-0">
                            <svg class="h-5 w-5" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                                fill="currentColor">
                                <path fill-rule="evenodd"
                                    d="M18 10a8 8 0 11-16 0 8 8 0 0116 0zm-7-4a1 1 0 11-2 0 1 1 0 012 0zM9 9a1 1 0 000 2v3a1 1 0 001 1h1a1 1 0 100-2v-3a1 1 0 00-1-1H9z"
                                    clip-rule="evenodd" />
                            </svg>
                        </div>
                        <div class="ml-3">
                            <asp:Label ID="lblNotification" runat="server" CssClass="text-sm"></asp:Label>
                        </div>
                    </div>
                </asp:Panel>

                <!-- Item Details -->
                <div class="bg-white rounded-lg shadow-md p-6 mb-6">
                    <h2 class="text-xl font-semibold text-gray-800 mb-4">Item Details</h2>

                    <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                        <div>
                            <div class="mb-4">
                                <label class="block text-gray-700 text-sm font-semibold mb-2">Item Name</label>
                                <asp:Label ID="lblItemName" runat="server"
                                    CssClass="block w-full px-4 py-2 bg-gray-100 rounded-md text-gray-800">
                                </asp:Label>
                            </div>

                            <div class="mb-4">
                                <label class="block text-gray-700 text-sm font-semibold mb-2">Category</label>
                                <asp:Label ID="lblCategory" runat="server"
                                    CssClass="block w-full px-4 py-2 bg-gray-100 rounded-md text-gray-800">
                                </asp:Label>
                            </div>
                        </div>

                        <div>
                            <div class="mb-4">
                                <label class="block text-gray-700 text-sm font-semibold mb-2">Current
                                    Quantity</label>
                                <asp:Label ID="lblCurrentQuantity" runat="server"
                                    CssClass="block w-full px-4 py-2 bg-gray-100 rounded-md text-gray-800 font-bold">
                                </asp:Label>
                            </div>

                            <div class="mb-4">
                                <label class="block text-gray-700 text-sm font-semibold mb-2">Last Modified</label>
                                <asp:Label ID="lblLastModified" runat="server"
                                    CssClass="block w-full px-4 py-2 bg-gray-100 rounded-md text-gray-600 text-sm">
                                </asp:Label>
                            </div>
                        </div>
                    </div>

                    <div class="mb-4">
                        <asp:Image ID="imgItem" runat="server" CssClass="max-h-48 rounded-md border border-gray-200" />
                    </div>
                </div>

                <!-- Stock Adjustment Form -->
                <div class="bg-white rounded-lg shadow-md p-6 mb-6">
                    <h2 class="text-xl font-semibold text-gray-800 mb-4">Adjust Stock Quantity</h2>

                    <div class="mb-6">
                        <label class="block text-gray-700 text-sm font-semibold mb-2">Adjustment Type</label>
                        <asp:DropDownList ID="ddlAdjustmentType" runat="server" AutoPostBack="false"
                            CssClass="block w-full md:w-1/2 px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500">
                            <asp:ListItem Text="Stock In (Add)" Value="StockIn" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="Stock Out (Remove)" Value="StockOut"></asp:ListItem>
                        </asp:DropDownList>
                    </div>

                    <div class="mb-6">
                        <label class="block text-gray-700 text-sm font-semibold mb-2">Quantity to Adjust</label>
                        <asp:TextBox ID="txtQuantity" runat="server" type="number" min="1" step="1"
                            CssClass="block w-full md:w-1/2 px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500">
                        </asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvQuantity" runat="server" ControlToValidate="txtQuantity"
                            CssClass="text-red-500 text-sm mt-1" Display="Dynamic" ErrorMessage="Quantity is required.">
                        </asp:RequiredFieldValidator>
                        <asp:RangeValidator ID="rvQuantity" runat="server" ControlToValidate="txtQuantity"
                            CssClass="text-red-500 text-sm mt-1" Display="Dynamic" Type="Integer" MinimumValue="1"
                            MaximumValue="999999" ErrorMessage="Quantity must be a positive number.">
                        </asp:RangeValidator>
                    </div>

                    <div class="mb-6">
                        <label class="block text-gray-700 text-sm font-semibold mb-2">Notes</label>
                        <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="3"
                            CssClass="block w-full px-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                            placeholder="Enter reason for adjustment or any other notes">
                        </asp:TextBox>
                    </div>

                    <div class="flex flex-col sm:flex-row gap-4">
                        <asp:Button ID="btnAdjust" runat="server" Text="Apply Adjustment" OnClick="btnAdjust_Click"
                            CssClass="bg-blue-600 hover:bg-blue-700 text-white py-2 px-6 rounded-md font-medium focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2" />

                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                            CausesValidation="false"
                            CssClass="bg-gray-200 hover:bg-gray-300 text-gray-800 py-2 px-6 rounded-md font-medium focus:outline-none focus:ring-2 focus:ring-gray-500 focus:ring-offset-2" />
                    </div>
                </div>

                <!-- Stock History -->
                <div class="bg-white rounded-lg shadow-md p-6 mb-6">
                    <h2 class="text-xl font-semibold text-gray-800 mb-4">Stock History</h2>

                    <div class="overflow-x-auto">
                        <asp:GridView ID="gvItemHistory" runat="server" AutoGenerateColumns="false"
                            CssClass="min-w-full divide-y divide-gray-200" HeaderStyle-CssClass="bg-gray-50"
                            RowStyle-CssClass="bg-white divide-y divide-gray-200"
                            AlternatingRowStyle-CssClass="bg-gray-50 divide-y divide-gray-200" GridLines="None"
                            EmptyDataText="No history records found."
                            EmptyDataRowStyle-CssClass="text-center py-8 text-gray-500 text-lg">
                            <Columns>
                                <asp:BoundField DataField="Date" HeaderText="Date"
                                    DataFormatString="{0:yyyy-MM-dd HH:mm}"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm text-gray-500" />

                                <asp:BoundField DataField="Type" HeaderText="Action Type"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900" />

                                <asp:BoundField DataField="QuantityChanged" HeaderText="Quantity Changed"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm text-gray-500" />

                                <asp:BoundField DataField="QuantityBefore" HeaderText="Before"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm text-gray-500" />

                                <asp:BoundField DataField="QuantityAfter" HeaderText="After"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm text-gray-500" />

                                <asp:BoundField DataField="Notes" HeaderText="Notes"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 text-sm text-gray-500" />

                                <asp:BoundField DataField="CreatedBy" HeaderText="By User"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm text-gray-500" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </asp:Content>