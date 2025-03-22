<%@ Page Title="Add Item" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" Async="true"
    CodeBehind="AddItem.aspx.cs" Inherits="Manajemen_Inventaris.Pages.Dashboard.Inventory.AddItem" %>

    <asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
        <script type="text/javascript">
            // Store scroll position before postback
            var scrollPosition;
            function saveScrollPosition() {
                scrollPosition = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop;
                document.getElementById('<%= hdnScrollPosition.ClientID %>').value = scrollPosition;
            }

            // Restore scroll position after postback
            function restoreScrollPosition() {
                var savedPosition = document.getElementById('<%= hdnScrollPosition.ClientID %>').value;
                if (savedPosition && savedPosition !== '') {
                    window.scrollTo(0, parseInt(savedPosition));
                }
            }

            // Add event listeners
            if (window.addEventListener) {
                window.addEventListener('load', restoreScrollPosition, false);
                window.addEventListener('scroll', saveScrollPosition, false);
            } else if (window.attachEvent) {
                window.attachEvent('onload', restoreScrollPosition);
                window.attachEvent('onscroll', saveScrollPosition);
            }

            // Register our functions with the ScriptManager for AJAX support
            function pageLoad() {
                // This will be called both on initial page load and after AJAX postbacks
                restoreImagePreview();
                restoreScrollPosition();

                // Add event handler for AsyncPostBackError if needed
                var pageManager = Sys.WebForms.PageRequestManager.getInstance();
                if (pageManager) {
                    pageManager.add_endRequest(function () {
                        restoreImagePreview();
                        restoreScrollPosition();
                    });
                }
            }
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

    <asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
        <asp:HiddenField ID="hdnScrollPosition" runat="server" Value="0" />
        <asp:HiddenField ID="hdnImageData" runat="server" />
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
                <!-- Notification Panel -->
                <asp:Panel ID="pnlNotification" runat="server" Visible="false" CssClass="mb-6">
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

                            <asp:UpdatePanel ID="UpdatePanelDescription" runat="server">
                                <ContentTemplate>
                                    <div class="mb-4">
                                        <label for="<%= txtDescription.ClientID %>"
                                            class="block text-sm font-medium text-gray-700 mb-2">Description</label>
                                        <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="4"
                                            CssClass="focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md py-2 px-3">
                                        </asp:TextBox>
                                        <div class="mt-2 flex justify-end">
                                            <asp:Button ID="btnGenerateDescription" runat="server"
                                                Text="AI: Generate Description" OnClick="btnGenerateDescription_Click"
                                                CssClass="inline-flex items-center px-3 py-1 border border-transparent text-xs font-medium rounded text-white bg-indigo-600 hover:bg-indigo-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                                                UseSubmitBehavior="false" />
                                        </div>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                            <asp:UpdatePanel ID="UpdatePanelCategory" runat="server">
                                <ContentTemplate>
                                    <div class="mb-4">
                                        <label for="<%= ddlCategory.ClientID %>"
                                            class="block text-sm font-medium text-gray-700 mb-2">Category *</label>
                                        <div class="flex space-x-2">
                                            <asp:DropDownList ID="ddlCategory" runat="server"
                                                CssClass="mt-1 block w-full py-2 px-3 border border-gray-300 bg-white rounded-md shadow-sm focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm">
                                            </asp:DropDownList>
                                            <asp:Button ID="btnSuggestCategory" runat="server" Text="AI: Suggest"
                                                OnClick="btnSuggestCategory_Click"
                                                CssClass="inline-flex items-center px-3 py-2 border border-transparent text-xs font-medium rounded text-white bg-indigo-600 hover:bg-indigo-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                                                UseSubmitBehavior="false" />
                                        </div>
                                        <asp:RequiredFieldValidator ID="rfvCategory" runat="server"
                                            ControlToValidate="ddlCategory" InitialValue="0"
                                            ErrorMessage="Please select a category."
                                            CssClass="text-red-500 text-xs mt-1" Display="Dynamic"
                                            ValidationGroup="ItemForm">
                                        </asp:RequiredFieldValidator>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

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

                            <asp:UpdatePanel ID="UpdatePanelTags" runat="server">
                                <ContentTemplate>
                                    <div class="mb-4">
                                        <label for="<%= txtTags.ClientID %>"
                                            class="block text-sm font-medium text-gray-700 mb-2">Tags</label>
                                        <div class="flex space-x-2">
                                            <asp:TextBox ID="txtTags" runat="server"
                                                CssClass="focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md py-2 px-3"
                                                placeholder="Enter tags separated by commas">
                                            </asp:TextBox>
                                            <asp:Button ID="btnGenerateTags" runat="server" Text="AI: Generate Tags"
                                                OnClick="btnGenerateTags_Click"
                                                CssClass="inline-flex items-center px-3 py-2 border border-transparent text-xs font-medium rounded text-white bg-indigo-600 hover:bg-indigo-500 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                                                UseSubmitBehavior="false" />
                                        </div>
                                        <small class="text-gray-500">Separate tags with commas. Tags help with searching
                                            and
                                            categorizing items.</small>
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>

                            <div class="mb-4">
                                <label for="<%= txtStockNotes.ClientID %>"
                                    class="block text-sm font-medium text-gray-700 mb-2">Initial Stock Notes</label>
                                <asp:TextBox ID="txtStockNotes" runat="server" TextMode="MultiLine" Rows="2"
                                    CssClass="focus:ring-indigo-500 focus:border-indigo-500 block w-full shadow-sm sm:text-sm border-gray-300 rounded-md py-2 px-3"
                                    placeholder="Any notes about the initial stock">
                                </asp:TextBox>
                            </div>
                        </div>

                        <div class="flex space-x-4">
                            <asp:Button ID="btnSave" runat="server" Text="Save Item" OnClick="btnSave_Click"
                                CssClass="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                                ValidationGroup="ItemForm" />
                            <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click"
                                CssClass="inline-flex justify-center py-2 px-4 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                                UseSubmitBehavior="false" />
                        </div>
                    </div>

                    <!-- Image Upload Section -->
                    <div>
                        <div class="bg-white shadow overflow-hidden sm:rounded-lg p-6">
                            <h4 class="text-lg font-medium text-gray-900 mb-4">Item Image</h4>

                            <div class="mb-4">
                                <div
                                    class="mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-gray-300 border-dashed rounded-md">
                                    <div class="space-y-1 text-center">
                                        <asp:Image ID="imgPreview" runat="server"
                                            CssClass="mx-auto h-40 w-auto object-contain" style="display:none;" />
                                        <svg id="uploadIcon" runat="server" class="mx-auto h-12 w-12 text-gray-400"
                                            stroke="currentColor" fill="none" viewBox="0 0 48 48">
                                            <path
                                                d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02"
                                                stroke-width="2" stroke-linecap="round" />
                                        </svg>
                                        <div class="flex text-sm text-gray-600">
                                            <label for="<%= fileUpload.ClientID %>"
                                                class="relative cursor-pointer rounded-md font-medium text-indigo-600 hover:text-indigo-500 focus-within:outline-none">
                                                <span>Upload a file</span>
                                                <asp:FileUpload ID="fileUpload" runat="server" CssClass="sr-only"
                                                    onchange="previewImage(this);" />
                                            </label>
                                            <p class="pl-1">or drag and drop</p>
                                        </div>
                                        <p class="text-xs text-gray-500">
                                            PNG, JPG, JPEG up to 5MB
                                        </p>
                                    </div>
                                </div>
                            </div>

                            <asp:UpdatePanel ID="UpdatePanelAIResults" runat="server">
                                <ContentTemplate>
                                    <div class="mt-4 bg-gray-50 p-4 rounded-md" id="divAIResults" runat="server"
                                        visible="false">
                                        <h5 class="text-sm font-medium text-gray-900 mb-2">AI Analysis Results</h5>

                                        <!-- AI Category Suggestions -->
                                        <div class="mb-3" id="divCategorySuggestions" runat="server" visible="false">
                                            <h6 class="text-xs font-medium text-gray-700">Suggested Categories:</h6>
                                            <asp:Repeater ID="rptrCategorySuggestions" runat="server">
                                                <ItemTemplate>
                                                    <div class="flex items-center mt-1">
                                                        <asp:LinkButton ID="lnkSelectCategory" runat="server"
                                                            Text='<%# Eval("Category") %>'
                                                            CommandArgument='<%# Eval("CategoryId") %>'
                                                            OnCommand="lnkSelectCategory_Command"
                                                            CssClass="text-xs inline-flex items-center px-2 py-0.5 rounded-full text-indigo-800 bg-indigo-100 hover:bg-indigo-200 mr-2">
                                                        </asp:LinkButton>
                                                        <span class="text-xs text-gray-500">Confidence: <%#
                                                                ((double)Eval("Confidence")).ToString("P0") %></span>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>

                                        <!-- AI Tag Suggestions -->
                                        <div class="mb-3" id="divTagSuggestions" runat="server" visible="false">
                                            <h6 class="text-xs font-medium text-gray-700">Suggested Tags:</h6>
                                            <div class="flex flex-wrap gap-1 mt-1">
                                                <asp:Repeater ID="rptrTagSuggestions" runat="server">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkSelectTag" runat="server"
                                                            Text='<%# Container.DataItem %>'
                                                            OnCommand="lnkSelectTag_Command"
                                                            CommandArgument='<%# Container.DataItem %>'
                                                            CssClass="text-xs inline-flex items-center px-2 py-0.5 rounded-full text-green-800 bg-green-100 hover:bg-green-200">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>
                                        </div>

                                        <!-- AI Processing Status -->
                                        <div class="text-xs text-gray-500 mt-2">
                                            <asp:Literal ID="litAIStatus" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnGenerateDescription" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="btnSuggestCategory" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="btnGenerateTags" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <script type="text/javascript">
            function previewImage(input) {
                if (input.files && input.files[0]) {
                    var reader = new FileReader();

                    reader.onload = function (e) {
                        try {
                            var imgPreview = document.getElementById('<%= imgPreview.ClientID %>');
                            var uploadIcon = document.getElementById('<%= uploadIcon.ClientID %>');
                            var hdnImageData = document.getElementById('<%= hdnImageData.ClientID %>');

                            if (imgPreview) {
                                imgPreview.src = e.target.result;
                                imgPreview.style.display = 'block';
                            }

                            if (uploadIcon) {
                                uploadIcon.style.display = 'none';
                            }

                            // Store image data in hidden field
                            if (hdnImageData) {
                                hdnImageData.value = e.target.result;
                            }
                        } catch (error) {
                            console.error("Error displaying preview: " + error.message);
                        }
                    }

                    reader.readAsDataURL(input.files[0]);
                }
            }

            // Restore image from hidden field if it exists
            function restoreImagePreview() {
                var hdnImageData = document.getElementById('<%= hdnImageData.ClientID %>');
                var imgPreview = document.getElementById('<%= imgPreview.ClientID %>');
                var uploadIcon = document.getElementById('<%= uploadIcon.ClientID %>');

                if (hdnImageData && hdnImageData.value && imgPreview) {
                    imgPreview.src = hdnImageData.value;
                    imgPreview.style.display = 'block';

                    if (uploadIcon) {
                        uploadIcon.style.display = 'none';
                    }
                }
            }
        </script>
    </asp:Content>