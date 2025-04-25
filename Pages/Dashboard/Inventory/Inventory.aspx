<%@ Page Title="Inventory" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" Async="true"
    CodeBehind="Inventory.aspx.cs" Inherits="Manajemen_Inventaris.Pages.Dashboard.Inventory.Inventory" %>

    <asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    </asp:Content>

    <asp:Content ID="NavContent" ContentPlaceHolderID="NavContent" runat="server">
        <a href="~/Pages/Dashboard/Dashboard.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">Dashboard</a>
        <a href="~/Pages/Dashboard/Inventory/Inventory.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-indigo-500 text-sm font-medium text-gray-900">Inventory</a>
        <a href="~/Pages/Dashboard/Reports/Reports.aspx" runat="server"
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

    <asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="bg-white shadow overflow-hidden sm:rounded-lg mb-8">
            <div class="px-6 py-6 sm:px-8 flex justify-between items-center border-b border-gray-200">
                <div>
                    <h3 class="text-xl leading-6 font-medium text-gray-900">Inventory Management</h3>
                    <p class="mt-2 max-w-2xl text-sm text-gray-500">View and manage your inventory items</p>
                </div>
                <div class="flex space-x-4">
                    <asp:Button ID="btnAddItem" runat="server" Text="Add New Item" OnClick="btnAddItem_Click"
                        CssClass="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" />
                    <asp:Button ID="btnManageCategories" runat="server" Text="Manage Categories"
                        OnClick="btnManageCategories_Click"
                        CssClass="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md shadow-sm text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" />
                </div>
            </div>

            <!-- Filter and Search Section -->
            <div class="px-6 py-5 sm:px-8 bg-gray-50">
                <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <!-- Category Filter -->
                    <div>
                        <label for="<%= ddlCategory.ClientID %>"
                            class="block text-sm font-medium text-gray-700 mb-2">Category</label>
                        <asp:DropDownList ID="ddlCategory" runat="server" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged"
                            CssClass="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md">
                            <asp:ListItem Text="All Categories" Value="0" />
                        </asp:DropDownList>
                    </div>

                    <!-- Sort By -->
                    <div>
                        <label for="<%= ddlSort.ClientID %>" class="block text-sm font-medium text-gray-700 mb-2">Sort
                            By</label>
                        <asp:DropDownList ID="ddlSort" runat="server" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlSort_SelectedIndexChanged"
                            CssClass="block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md">
                            <asp:ListItem Text="Name (A-Z)" Value="NameAsc" />
                            <asp:ListItem Text="Name (Z-A)" Value="NameDesc" />
                            <asp:ListItem Text="Quantity (Low-High)" Value="QuantityAsc" />
                            <asp:ListItem Text="Quantity (High-Low)" Value="QuantityDesc" />
                            <asp:ListItem Text="Date Added (Newest)" Value="DateDesc" />
                            <asp:ListItem Text="Date Added (Oldest)" Value="DateAsc" />
                        </asp:DropDownList>
                    </div>

                    <!-- Search -->
                    <div>
                        <label for="<%= txtSearch.ClientID %>"
                            class="block text-sm font-medium text-gray-700 mb-2">Search</label>
                        <div class="flex rounded-md shadow-sm">
                            <asp:TextBox ID="txtSearch" runat="server" placeholder="Search items..."
                                CssClass="focus:ring-indigo-500 focus:border-indigo-500 flex-1 block w-full rounded-none rounded-l-md sm:text-sm border-gray-300 pl-3">
                            </asp:TextBox>
                            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click"
                                CssClass="-ml-px relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-r-md text-gray-700 bg-gray-50 hover:bg-gray-100 focus:outline-none focus:ring-1 focus:ring-indigo-500 focus:border-indigo-500" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="border-t border-gray-200">
                <!-- Notification Panel -->
                <asp:Panel ID="pnlNotification" runat="server" Visible="false" CssClass="mb-6 mx-6 mt-6">
                    <div class="rounded-md p-4">
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
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlLowStock" runat="server" Visible="false"
                    CssClass="bg-yellow-50 border-l-4 border-yellow-400 p-4 m-6">
                    <div class="flex">
                        <div class="flex-shrink-0">
                            <svg class="h-5 w-5 text-yellow-400" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"
                                fill="currentColor">
                                <path fill-rule="evenodd"
                                    d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z"
                                    clip-rule="evenodd" />
                            </svg>
                        </div>
                        <div class="ml-3">
                            <p class="text-sm text-yellow-700">
                                <asp:Label ID="lblLowStock" runat="server"></asp:Label>
                            </p>
                        </div>
                    </div>
                </asp:Panel>

                <div class="px-6 py-4 sm:px-8">
                    <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="false"
                        CssClass="min-w-full divide-y divide-gray-200" HeaderStyle-CssClass="bg-gray-50"
                        RowStyle-CssClass="bg-white divide-y divide-gray-200"
                        AlternatingRowStyle-CssClass="bg-gray-50 divide-y divide-gray-200" GridLines="None"
                        OnRowCommand="gvItems_RowCommand" OnRowDataBound="gvItems_RowDataBound"
                        EmptyDataText="No items found."
                        EmptyDataRowStyle-CssClass="text-center py-8 text-gray-500 text-lg">
                        <Columns>
                            <asp:TemplateField HeaderText="Image"
                                HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                <ItemTemplate>
                                    <div class="px-6 py-4 whitespace-nowrap">
                                        <asp:Image ID="imgItem" runat="server"
                                            ImageUrl='<%# Eval("ImagePath") != null && !string.IsNullOrEmpty(Eval("ImagePath").ToString()) ? Eval("ImagePath") : "~/Uploads/no-image.png" %>'
                                            CssClass="h-16 w-16 object-cover rounded-md" />
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Name"
                                HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                <ItemTemplate>
                                    <div class="px-6 py-4">
                                        <div class="text-sm font-medium text-gray-900">
                                            <%# Eval("Name") %>
                                        </div>
                                        <div class="text-sm text-gray-500">
                                            <%# Eval("Description") %>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Category"
                                HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                <ItemTemplate>
                                    <div class="px-6 py-4 whitespace-nowrap">
                                        <span
                                            class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-indigo-100 text-indigo-800">
                                            <%# Eval("CategoryName") %>
                                        </span>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Quantity"
                                HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                <ItemTemplate>
                                    <div class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                        <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("Quantity") %>'
                                            CssClass='<%# Convert.ToInt32(Eval("Quantity")) <= 5 ? "text-red-600 font-medium" : "" %>'>
                                        </asp:Label>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Actions"
                                HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                <ItemTemplate>
                                    <div class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                        <asp:LinkButton ID="lnkView" runat="server" CommandName="ViewItem"
                                            CommandArgument='<%# Eval("ItemID") %>'
                                            CssClass="text-indigo-600 hover:text-indigo-900 mr-4">View</asp:LinkButton>
                                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditItem"
                                            CommandArgument='<%# Eval("ItemID") %>'
                                            CssClass="text-blue-600 hover:text-blue-900 mr-4">Edit</asp:LinkButton>
                                        <asp:LinkButton ID="lnkAdjust" runat="server" CommandName="AdjustStock"
                                            CommandArgument='<%# Eval("ItemID") %>'
                                            CssClass="text-green-600 hover:text-green-900 mr-4">Adjust Stock
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="DeleteItem"
                                            CommandArgument='<%# Eval("ItemID") %>'
                                            CssClass="text-red-600 hover:text-red-900"
                                            OnClientClick="return confirm('Are you sure you want to delete this item?');">
                                            Delete</asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>

        <!-- Item Details Modal -->
        <asp:Panel ID="pnlItemDetails" runat="server" CssClass="fixed inset-0 overflow-y-auto" Visible="false">
            <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
                <div class="fixed inset-0 transition-opacity" aria-hidden="true">
                    <div class="absolute inset-0 bg-gray-500 opacity-75"></div>
                </div>
                <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>
                <div
                    class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
                    <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
                        <div class="sm:flex sm:items-start">
                            <div class="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left w-full">
                                <h3 class="text-lg leading-6 font-medium text-gray-900" id="modal-title">
                                    <asp:Literal ID="litItemName" runat="server"></asp:Literal>
                                </h3>
                                <div class="mt-4 grid grid-cols-1 gap-4">
                                    <div class="flex justify-center">
                                        <asp:Image ID="imgItemDetail" runat="server"
                                            CssClass="h-48 w-48 object-cover rounded-md" />
                                    </div>
                                    <div>
                                        <p class="text-sm text-gray-500 font-medium">Description:</p>
                                        <p class="text-sm text-gray-900">
                                            <asp:Literal ID="litItemDescription" runat="server"></asp:Literal>
                                        </p>
                                    </div>
                                    <div>
                                        <p class="text-sm text-gray-500 font-medium">Category:</p>
                                        <p class="text-sm text-gray-900">
                                            <asp:Literal ID="litItemCategory" runat="server"></asp:Literal>
                                        </p>
                                    </div>
                                    <div>
                                        <p class="text-sm text-gray-500 font-medium">Quantity:</p>
                                        <p class="text-sm text-gray-900">
                                            <asp:Literal ID="litItemQuantity" runat="server"></asp:Literal>
                                        </p>
                                    </div>
                                    <div>
                                        <p class="text-sm text-gray-500 font-medium">AI Tags:</p>
                                        <p class="text-sm text-gray-900">
                                            <asp:Literal ID="litItemAITags" runat="server"></asp:Literal>
                                        </p>
                                    </div>
                                    <div>
                                        <p class="text-sm text-gray-500 font-medium">Added By:</p>
                                        <p class="text-sm text-gray-900">
                                            <asp:Literal ID="litItemCreatedBy" runat="server"></asp:Literal>
                                        </p>
                                    </div>
                                    <div>
                                        <p class="text-sm text-gray-500 font-medium">Added On:</p>
                                        <p class="text-sm text-gray-900">
                                            <asp:Literal ID="litItemCreatedDate" runat="server"></asp:Literal>
                                        </p>
                                    </div>
                                    <div>
                                        <p class="text-sm text-gray-500 font-medium">Last Modified By:</p>
                                        <p class="text-sm text-gray-900">
                                            <asp:Literal ID="litItemModifiedBy" runat="server"></asp:Literal>
                                        </p>
                                    </div>
                                    <div>
                                        <p class="text-sm text-gray-500 font-medium">Last Modified On:</p>
                                        <p class="text-sm text-gray-900">
                                            <asp:Literal ID="litItemModifiedDate" runat="server"></asp:Literal>
                                        </p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
                        <asp:Button ID="btnCloseDetails" runat="server" Text="Close" OnClick="btnCloseDetails_Click"
                            CssClass="w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <!-- Stock Adjustment Modal -->
        <asp:Panel ID="pnlStockAdjustment" runat="server" CssClass="fixed inset-0 overflow-y-auto" Visible="false">
            <div class="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
                <div class="fixed inset-0 transition-opacity" aria-hidden="true">
                    <div class="absolute inset-0 bg-gray-500 opacity-75"></div>
                </div>
                <span class="hidden sm:inline-block sm:align-middle sm:h-screen" aria-hidden="true">&#8203;</span>
                <div
                    class="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
                    <div class="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
                        <div class="sm:flex sm:items-start">
                            <div class="mt-3 text-center sm:mt-0 sm:ml-4 sm:text-left w-full">
                                <h3 class="text-lg leading-6 font-medium text-gray-900" id="modal-title">
                                    Adjust Stock: <asp:Literal ID="litAdjustItemName" runat="server"></asp:Literal>
                                </h3>
                                <div class="mt-4">
                                    <p class="text-sm text-gray-500 mb-4">Current Quantity: <asp:Literal
                                            ID="litAdjustCurrentQuantity" runat="server"></asp:Literal>
                                    </p>
                                    <div class="mt-4">
                                        <label for="<%= ddlAdjustmentType.ClientID %>"
                                            class="block text-sm font-medium text-gray-700 mb-2">Adjustment Type</label>
                                        <asp:DropDownList ID="ddlAdjustmentType" runat="server"
                                            CssClass="mt-1 block w-full pl-3 pr-10 py-2 text-base border-gray-300 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm rounded-md">
                                            <asp:ListItem Text="Stock In (Add)" Value="in" />
                                            <asp:ListItem Text="Stock Out (Remove)" Value="out" />
                                        </asp:DropDownList>
                                    </div>
                                    <div class="mt-4">
                                        <label for="<%= txtQuantityChange.ClientID %>"
                                            class="block text-sm font-medium text-gray-700 mb-2">Quantity</label>
                                        <asp:TextBox ID="txtQuantityChange" runat="server" TextMode="Number" min="1"
                                            CssClass="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md">
                                        </asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvQuantityChange" runat="server"
                                            ControlToValidate="txtQuantityChange" ErrorMessage="Quantity is required."
                                            CssClass="text-red-500 text-xs mt-1" Display="Dynamic"
                                            ValidationGroup="StockAdjustment"></asp:RequiredFieldValidator>
                                        <asp:RegularExpressionValidator ID="revQuantityChange" runat="server"
                                            ControlToValidate="txtQuantityChange"
                                            ErrorMessage="Please enter a valid number."
                                            CssClass="text-red-500 text-xs mt-1" Display="Dynamic"
                                            ValidationGroup="StockAdjustment" ValidationExpression="^\d+$">
                                        </asp:RegularExpressionValidator>
                                    </div>
                                    <div class="mt-4">
                                        <label for="<%= txtNotes.ClientID %>"
                                            class="block text-sm font-medium text-gray-700 mb-2">Notes</label>
                                        <asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Rows="3"
                                            CssClass="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md">
                                        </asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvNotes" runat="server"
                                            ControlToValidate="txtNotes" ErrorMessage="Notes are required."
                                            CssClass="text-red-500 text-xs mt-1" Display="Dynamic"
                                            ValidationGroup="StockAdjustment"></asp:RequiredFieldValidator>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="bg-gray-50 px-4 py-3 sm:px-6 sm:flex sm:flex-row-reverse">
                        <asp:Button ID="btnSaveAdjustment" runat="server" Text="Save" OnClick="btnSaveAdjustment_Click"
                            ValidationGroup="StockAdjustment"
                            CssClass="w-full inline-flex justify-center rounded-md border border-transparent shadow-sm px-4 py-2 bg-indigo-600 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:ml-3 sm:w-auto sm:text-sm" />
                        <asp:Button ID="btnCancelAdjustment" runat="server" Text="Cancel"
                            OnClick="btnCancelAdjustment_Click" CausesValidation="false"
                            CssClass="mt-3 w-full inline-flex justify-center rounded-md border border-gray-300 shadow-sm px-4 py-2 bg-white text-base font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 sm:mt-0 sm:ml-3 sm:w-auto sm:text-sm" />
                    </div>
                </div>
            </div>
        </asp:Panel>

        <asp:HiddenField ID="hdnItemId" runat="server" />
    </asp:Content>