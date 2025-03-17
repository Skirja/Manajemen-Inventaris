<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs" Inherits="Manajemen_Inventaris.Pages.Dashboard.Dashboard" %>

    <asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    </asp:Content>

    <asp:Content ID="NavContent" ContentPlaceHolderID="NavContent" runat="server">
        <a href="~/Pages/Dashboard/Dashboard.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-indigo-500 text-sm font-medium text-gray-900">Dashboard</a>
        <a href="~/Pages/Dashboard/Inventory.aspx" runat="server"
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
                CssClass="text-sm text-indigo-600 hover:text-indigo-500">Logout</asp:LinkButton>
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
                        <p class="text-3xl font-bold text-indigo-800 mt-2">0</p>
                    </div>
                    <div class="bg-green-50 p-4 rounded-lg shadow">
                        <h4 class="text-lg font-semibold text-green-700">Categories</h4>
                        <p class="text-3xl font-bold text-green-800 mt-2">0</p>
                    </div>
                    <div class="bg-yellow-50 p-4 rounded-lg shadow">
                        <h4 class="text-lg font-semibold text-yellow-700">Recent Uploads</h4>
                        <p class="text-3xl font-bold text-yellow-800 mt-2">0</p>
                    </div>
                    <div class="bg-red-50 p-4 rounded-lg shadow">
                        <h4 class="text-lg font-semibold text-red-700">Low Stock</h4>
                        <p class="text-3xl font-bold text-red-800 mt-2">0</p>
                    </div>
                </div>

                <div class="p-4">
                    <h4 class="text-lg font-semibold text-gray-700 mb-4">Recent Activity</h4>
                    <div class="bg-gray-50 p-4 rounded-lg">
                        <p class="text-gray-500 text-center">No recent activity</p>
                    </div>
                </div>
            </div>
        </div>
    </asp:Content>