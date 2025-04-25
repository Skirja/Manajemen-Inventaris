<%@ Page Title="Manage Categories" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="ManageCategories.aspx.cs" Inherits="Manajemen_Inventaris.Pages.Dashboard.Inventory.ManageCategories" %>

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
                    <h3 class="text-xl leading-6 font-medium text-gray-900">Category Management</h3>
                    <p class="mt-2 max-w-2xl text-sm text-gray-500">Create, edit, and delete categories for your
                        inventory items</p>
                </div>
                <div>
                    <asp:LinkButton ID="lnkBackToInventory" runat="server" OnClick="lnkBackToInventory_Click"
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

            <div class="px-6 py-5 sm:px-8 grid grid-cols-1 md:grid-cols-3 gap-6">
                <!-- Add/Edit Category Panel -->
                <div class="md:col-span-1 bg-white shadow rounded-lg p-6">
                    <h4 class="text-lg font-medium text-gray-900 mb-4">
                        <asp:Literal ID="litFormTitle" runat="server" Text="Add New Category"></asp:Literal>
                    </h4>

                    <div class="mb-4">
                        <label for="<%= txtCategoryName.ClientID %>"
                            class="block text-sm font-medium text-gray-700 mb-2">Category Name</label>
                        <asp:TextBox ID="txtCategoryName" runat="server"
                            CssClass="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md py-2 px-3">
                        </asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCategoryName" runat="server"
                            ControlToValidate="txtCategoryName" ErrorMessage="Category name is required."
                            CssClass="text-red-500 text-xs mt-1" Display="Dynamic" ValidationGroup="CategoryForm">
                        </asp:RequiredFieldValidator>
                    </div>

                    <div class="mb-4">
                        <label for="<%= txtDescription.ClientID %>"
                            class="block text-sm font-medium text-gray-700 mb-2">Description</label>
                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="3"
                            CssClass="mt-1 focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md py-2 px-3">
                        </asp:TextBox>
                    </div>

                    <div class="mt-6 flex items-center justify-between">
                        <asp:Button ID="btnSave" runat="server" Text="Save Category" OnClick="btnSave_Click"
                            ValidationGroup="CategoryForm"
                            CssClass="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" />

                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                            CausesValidation="false"
                            CssClass="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md shadow-sm text-gray-700 bg-white hover:bg-gray-50 focus:outline-none"
                            Visible="false" />
                    </div>

                    <asp:Label ID="lblMessage" runat="server" CssClass="mt-4 block text-sm font-medium" Visible="false">
                    </asp:Label>
                    <asp:HiddenField ID="hdnCategoryId" runat="server" Value="0" />
                </div>

                <!-- Categories List Panel -->
                <div class="md:col-span-2 bg-white shadow rounded-lg p-6">
                    <h4 class="text-lg font-medium text-gray-900 mb-4">Categories</h4>

                    <div class="mb-4">
                        <div class="relative rounded-md shadow-sm">
                            <asp:TextBox ID="txtSearchCategory" runat="server" placeholder="Search categories..."
                                CssClass="focus:ring-indigo-500 focus:border-indigo-500 block w-full pr-10 sm:text-sm border-gray-300 rounded-md py-2 px-3">
                            </asp:TextBox>
                            <div class="absolute inset-y-0 right-0 flex items-center pr-3">
                                <asp:LinkButton ID="btnSearchCategory" runat="server" OnClick="btnSearchCategory_Click"
                                    CausesValidation="false">
                                    <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-gray-400" fill="none"
                                        viewBox="0 0 24 24" stroke="currentColor">
                                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                            d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                                    </svg>
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>

                    <div class="overflow-hidden border-b border-gray-200 sm:rounded-lg">
                        <asp:GridView ID="gvCategories" runat="server" AutoGenerateColumns="false"
                            CssClass="min-w-full divide-y divide-gray-200" HeaderStyle-CssClass="bg-gray-50"
                            RowStyle-CssClass="bg-white divide-y divide-gray-200"
                            AlternatingRowStyle-CssClass="bg-gray-50 divide-y divide-gray-200" GridLines="None"
                            OnRowCommand="gvCategories_RowCommand" EmptyDataText="No categories found."
                            EmptyDataRowStyle-CssClass="text-center py-8 text-gray-500 text-lg">
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="CATEGORY NAME"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900" />

                                <asp:BoundField DataField="Description" HeaderText="DESCRIPTION"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider"
                                    ItemStyle-CssClass="px-6 py-4 text-sm text-gray-500" />

                                <asp:TemplateField HeaderText="ITEMS"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    <ItemTemplate>
                                        <div class="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                                            <span
                                                class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800">
                                                <%# Eval("ItemCount") %>
                                            </span>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField HeaderText="ACTIONS"
                                    HeaderStyle-CssClass="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    <ItemTemplate>
                                        <div class="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                                            <asp:LinkButton ID="lnkEdit" runat="server" CommandName="EditCategory"
                                                CommandArgument='<%# Eval("CategoryID") %>'
                                                CssClass="text-indigo-600 hover:text-indigo-900 mr-4">Edit
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="lnkDelete" runat="server" CommandName="DeleteCategory"
                                                CommandArgument='<%# Eval("CategoryID") %>'
                                                CssClass="text-red-600 hover:text-red-900"
                                                OnClientClick='<%# "return confirm(\"Are you sure you want to delete the category " + Eval("Name") + "? This will remove the category from all associated items.\");" %>'>
                                                Delete
                                            </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>
    </asp:Content>