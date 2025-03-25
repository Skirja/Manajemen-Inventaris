<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs" Inherits="Manajemen_Inventaris.Pages.Dashboard.Dashboard" %>

    <asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    </asp:Content>

    <asp:Content ID="NavContent" ContentPlaceHolderID="NavContent" runat="server">
        <a href="~/Pages/Dashboard/Dashboard.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-indigo-500 text-sm font-medium text-gray-900">Dashboard</a>
        <a href="~/Pages/Dashboard/Inventory/Inventory.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">Inventory</a>
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

    <asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="bg-white shadow overflow-hidden sm:rounded-lg">
            <div class="px-4 py-5 sm:px-6">
                <h3 class="text-lg leading-6 font-medium text-gray-900">Dashboard</h3>
                <p class="mt-1 max-w-2xl text-sm text-gray-500">Welcome to the Inventory Management System</p>
                <asp:Label ID="lblWelcome" runat="server" CssClass="text-lg font-medium text-indigo-600 mt-2 block">
                </asp:Label>
            </div>
            <div class="border-t border-gray-200">
                <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 p-4">
                    <div class="bg-indigo-50 p-4 rounded-lg shadow">
                        <h4 class="text-lg font-semibold text-indigo-700">Total Items</h4>
                        <asp:Label ID="lblTotalItems" runat="server"
                            CssClass="text-3xl font-bold text-indigo-800 mt-2 block">0</asp:Label>
                    </div>
                    <div class="bg-green-50 p-4 rounded-lg shadow">
                        <h4 class="text-lg font-semibold text-green-700">Categories</h4>
                        <asp:Label ID="lblTotalCategories" runat="server"
                            CssClass="text-3xl font-bold text-green-800 mt-2 block">0</asp:Label>
                    </div>
                    <div class="bg-yellow-50 p-4 rounded-lg shadow">
                        <h4 class="text-lg font-semibold text-yellow-700">Recent Uploads</h4>
                        <asp:Label ID="lblRecentUploads" runat="server"
                            CssClass="text-3xl font-bold text-yellow-800 mt-2 block">0</asp:Label>
                    </div>
                    <div class="bg-red-50 p-4 rounded-lg shadow">
                        <h4 class="text-lg font-semibold text-red-700">Low Stock</h4>
                        <asp:Label ID="lblLowStock" runat="server"
                            CssClass="text-3xl font-bold text-red-800 mt-2 block">0</asp:Label>
                    </div>
                </div>

                <div class="p-4">
                    <h4 class="text-lg font-semibold text-gray-700 mb-4">Recent Activity</h4>
                    <div class="bg-gray-50 rounded-lg">
                        <asp:Repeater ID="rptRecentActivity" runat="server">
                            <HeaderTemplate>
                                <div class="border-b border-gray-200">
                                    <div class="grid grid-cols-12 bg-gray-100 py-2 px-4 rounded-t-lg">
                                        <div class="col-span-3 font-medium text-gray-600 text-sm">Date</div>
                                        <div class="col-span-2 font-medium text-gray-600 text-sm">Action</div>
                                        <div class="col-span-3 font-medium text-gray-600 text-sm">Item</div>
                                        <div class="col-span-2 font-medium text-gray-600 text-sm">Quantity</div>
                                        <div class="col-span-2 font-medium text-gray-600 text-sm">User</div>
                                    </div>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div class="border-b border-gray-200 hover:bg-gray-50">
                                    <div class="grid grid-cols-12 py-3 px-4">
                                        <div class="col-span-3 text-sm text-gray-500">
                                            <%# Eval("Date", "{0:yyyy-MM-dd HH:mm}" ) %>
                                        </div>
                                        <div class="col-span-2 text-sm font-medium">
                                            <span class='<%# GetActionClass(Eval("ActionType").ToString()) %>'>
                                                <%# Eval("ActionType") %>
                                            </span>
                                        </div>
                                        <div class="col-span-3 text-sm text-gray-900">
                                            <%# Eval("ItemName") %>
                                        </div>
                                        <div class="col-span-2 text-sm text-gray-500">
                                            <%# Eval("Quantity") %>
                                        </div>
                                        <div class="col-span-2 text-sm text-gray-500">
                                            <%# Eval("Username") %>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Panel ID="pnlNoActivity" runat="server"
                                    Visible='<%# rptRecentActivity.Items.Count == 0 %>'>
                                    <div class="text-gray-500 text-center py-8">No recent activity</div>
                                </asp:Panel>
                            </FooterTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </div>
    </asp:Content>