<%@ Page Title="Add Item" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="AddItem.aspx.cs" Inherits="Manajemen_Inventaris.Pages.Dashboard.Inventory.AddItem" %>

    <asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
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

    <asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="bg-white shadow overflow-hidden sm:rounded-lg mb-8">
            <div class="px-6 py-6 sm:px-8 flex justify-between items-center border-b border-gray-200">
                <div>
                    <h3 class="text-xl leading-6 font-medium text-gray-900">Add New Item</h3>
                    <p class="mt-2 max-w-2xl text-sm text-gray-500">Add a new item to your inventory</p>
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

            <div class="px-6 py-5 sm:px-8">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                    <!-- Item Information Form -->
                    <div class="space-y-6">
                        <div class="bg-white shadow overflow-hidden sm:rounded-lg p-6">
                            <h4 class="text-lg font-medium text-gray-900 mb-4">Item Information</h4>

                            <div class="mb-4">
                                <label for="<%= txtItemName.ClientID %>"
                                    class="block text-sm font-medium text-gray-700 mb-2">Item Name *</label>
                                <asp:TextBox ID="txtItemName" runat="server"
                                    CssClass="focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md py-2 px-3">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvItemName" runat="server"
                                    ControlToValidate="txtItemName" ErrorMessage="Item name is required."
                                    CssClass="text-red-500 text-xs mt-1" Display="Dynamic" ValidationGroup="ItemForm">
                                </asp:RequiredFieldValidator>
                            </div>

                            <div class="mb-4">
                                <label for="<%= txtDescription.ClientID %>"
                                    class="block text-sm font-medium text-gray-700 mb-2">Description</label>
                                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4"
                                    CssClass="focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md py-2 px-3">
                                </asp:TextBox>
                            </div>

                            <div class="mb-4">
                                <label for="<%= ddlCategory.ClientID %>"
                                    class="block text-sm font-medium text-gray-700 mb-2">Category *</label>
                                <asp:DropDownList ID="ddlCategory" runat="server"
                                    CssClass="mt-1 block w-full py-2 px-3 border border-gray-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rfvCategory" runat="server"
                                    ControlToValidate="ddlCategory" InitialValue="0"
                                    ErrorMessage="Please select a category." CssClass="text-red-500 text-xs mt-1"
                                    Display="Dynamic" ValidationGroup="ItemForm">
                                </asp:RequiredFieldValidator>
                            </div>

                            <div class="mb-4">
                                <label for="<%= txtQuantity.ClientID %>"
                                    class="block text-sm font-medium text-gray-700 mb-2">Initial Quantity *</label>
                                <asp:TextBox ID="txtQuantity" runat="server" TextMode="Number" min="0"
                                    CssClass="focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md py-2 px-3">
                                </asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvQuantity" runat="server"
                                    ControlToValidate="txtQuantity" ErrorMessage="Initial quantity is required."
                                    CssClass="text-red-500 text-xs mt-1" Display="Dynamic" ValidationGroup="ItemForm">
                                </asp:RequiredFieldValidator>
                                <asp:RangeValidator ID="rvQuantity" runat="server" ControlToValidate="txtQuantity"
                                    Type="Integer" MinimumValue="0" MaximumValue="99999"
                                    ErrorMessage="Quantity must be between 0 and 99999."
                                    CssClass="text-red-500 text-xs mt-1" Display="Dynamic" ValidationGroup="ItemForm">
                                </asp:RangeValidator>
                            </div>
                        </div>
                    </div>

                    <!-- Item Image Upload -->
                    <div class="space-y-6">
                        <div class="bg-white shadow overflow-hidden sm:rounded-lg p-6">
                            <h4 class="text-lg font-medium text-gray-900 mb-4">Item Image</h4>

                            <div class="mb-6">
                                <label class="block text-sm font-medium text-gray-700 mb-2">Upload Image</label>
                                <div
                                    class="mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-gray-300 border-dashed rounded-md">
                                    <div class="space-y-1 text-center">
                                        <asp:Image ID="imgPreview" runat="server" ImageUrl="~/Uploads/no-image.png"
                                            CssClass="mx-auto h-32 w-32 object-cover mb-3" />
                                        <div class="flex text-sm text-gray-600">
                                            <label for="<%= fileUpload.ClientID %>"
                                                class="relative cursor-pointer bg-white rounded-md font-medium text-indigo-600 hover:text-indigo-500 focus-within:outline-none focus-within:ring-2 focus-within:ring-offset-2 focus-within:ring-indigo-500">
                                                <span>Upload a file</span>
                                                <asp:FileUpload ID="fileUpload" runat="server" CssClass="sr-only"
                                                    onchange="showPreview(this)" />
                                            </label>
                                            <p class="pl-1">or drag and drop</p>
                                        </div>
                                        <p class="text-xs text-gray-500">
                                            PNG, JPG, JPEG up to 5MB
                                        </p>
                                    </div>
                                </div>
                            </div>

                            <div class="mb-4">
                                <label for="<%= txtTags.ClientID %>"
                                    class="block text-sm font-medium text-gray-700 mb-2">Tags</label>
                                <asp:TextBox ID="txtTags" runat="server"
                                    placeholder="Enter comma-separated tags or leave empty for AI tagging"
                                    CssClass="focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md py-2 px-3">
                                </asp:TextBox>
                                <p class="text-xs text-gray-500 mt-1">
                                    Leave empty to auto-generate tags using AI when an image is uploaded
                                </p>
                            </div>

                            <div class="mb-4">
                                <label class="block text-sm font-medium text-gray-700 mb-2">Stock Information</label>
                                <asp:TextBox ID="txtStockNotes" runat="server" TextMode="MultiLine" Rows="3"
                                    placeholder="Optional notes about the initial stock"
                                    CssClass="focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md py-2 px-3">
                                </asp:TextBox>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Form Actions -->
                <div class="mt-6 flex justify-end space-x-3">
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                        CausesValidation="false"
                        CssClass="inline-flex justify-center py-2 px-4 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" />

                    <asp:Button ID="btnSave" runat="server" Text="Save Item" OnClick="btnSave_Click"
                        ValidationGroup="ItemForm"
                        CssClass="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" />
                </div>

                <asp:Label ID="lblMessage" runat="server" CssClass="mt-4 block text-sm font-medium" Visible="false">
                </asp:Label>
            </div>
        </div>

        <script type="text/javascript">
            function showPreview(input) {
                if (input.files && input.files[0]) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        document.getElementById('<%= imgPreview.ClientID %>').src = e.target.result;
                    }
                    reader.readAsDataURL(input.files[0]);
                }
            }
        </script>
    </asp:Content>