<%@ Page Title="Upload Images" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" Async="true"
    CodeBehind="Upload.aspx.cs" Inherits="Manajemen_Inventaris.Pages.Dashboard.Upload.Upload" %>

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

            // Initialize drag and drop functionality
            function initDragAndDrop() {
                const dropZone = document.getElementById('dropZone');
                const fileInput = document.getElementById('<%= fileUpload.ClientID %>');

                // Prevent default drag behaviors
                ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
                    dropZone.addEventListener(eventName, preventDefaults, false);
                });

                function preventDefaults(e) {
                    e.preventDefault();
                    e.stopPropagation();
                }

                // Highlight drop zone when dragging over it
                ['dragenter', 'dragover'].forEach(eventName => {
                    dropZone.addEventListener(eventName, highlight, false);
                });

                ['dragleave', 'drop'].forEach(eventName => {
                    dropZone.addEventListener(eventName, unhighlight, false);
                });

                function highlight() {
                    dropZone.classList.add('border-indigo-500');
                    dropZone.classList.remove('border-gray-300');
                }

                function unhighlight() {
                    dropZone.classList.remove('border-indigo-500');
                    dropZone.classList.add('border-gray-300');
                }

                // Handle dropped files
                dropZone.addEventListener('drop', handleDrop, false);

                function handleDrop(e) {
                    const dt = e.dataTransfer;
                    const files = dt.files;

                    if (files.length > 0) {
                        fileInput.files = files;
                        handleFiles(files);
                    }
                }

                // Handle selected files from file input
                fileInput.addEventListener('change', function () {
                    if (this.files.length > 0) {
                        handleFiles(this.files);
                    }
                });

                function handleFiles(files) {
                    displayPreview(files);
                    document.getElementById('<%= btnUpload.ClientID %>').click();
                }

                // Display image preview
                function displayPreview(files) {
                    const previewContainer = document.getElementById('previewContainer');
                    previewContainer.innerHTML = '';

                    for (let i = 0; i < files.length; i++) {
                        if (files[i].type.match('image.*')) {
                            const reader = new FileReader();

                            reader.onload = function (e) {
                                const img = document.createElement('img');
                                img.src = e.target.result;
                                img.className = 'h-20 w-20 object-cover rounded-md mx-1 my-1';
                                previewContainer.appendChild(img);

                                // Store the base64 image data
                                document.getElementById('<%= hdnImageData.ClientID %>').value = e.target.result;
                            };

                            reader.readAsDataURL(files[i]);
                        }
                    }

                    // Show the preview container
                    previewContainer.style.display = 'flex';
                    document.getElementById('uploadIcon').style.display = 'none';
                    document.getElementById('uploadText').style.display = 'none';
                }
            }

            // Add event listeners
            if (window.addEventListener) {
                window.addEventListener('load', function () {
                    restoreScrollPosition();
                    initDragAndDrop();
                }, false);
                window.addEventListener('scroll', saveScrollPosition, false);
            } else if (window.attachEvent) {
                window.attachEvent('onload', function () {
                    restoreScrollPosition();
                    initDragAndDrop();
                });
                window.attachEvent('onscroll', saveScrollPosition);
            }

            // Register functions with the ScriptManager for AJAX support
            function pageLoad() {
                restoreScrollPosition();
                initDragAndDrop();

                // Add event handler for AsyncPostBackError if needed
                var pageManager = Sys.WebForms.PageRequestManager.getInstance();
                if (pageManager) {
                    pageManager.add_endRequest(function () {
                        restoreScrollPosition();
                        initDragAndDrop();
                    });
                }
            }
        </script>
    </asp:Content>

    <asp:Content ID="NavContent" ContentPlaceHolderID="NavContent" runat="server">
        <a href="~/Pages/Dashboard/Dashboard.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">Dashboard</a>
        <a href="~/Pages/Dashboard/Inventory/Inventory.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-transparent text-sm font-medium text-gray-500 hover:text-gray-700 hover:border-gray-300">Inventory</a>
        <a href="~/Pages/Dashboard/Upload/Upload.aspx" runat="server"
            class="inline-flex items-center px-1 pt-1 border-b-2 border-indigo-500 text-sm font-medium text-gray-900">Upload</a>
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
        <asp:HiddenField ID="hdnScrollPosition" runat="server" Value="0" />
        <asp:HiddenField ID="hdnImageData" runat="server" />

        <div class="bg-white shadow overflow-hidden sm:rounded-lg mb-8">
            <div class="px-6 py-6 sm:px-8 flex justify-between items-center border-b border-gray-200">
                <div>
                    <h3 class="text-xl leading-6 font-medium text-gray-900">Upload Images</h3>
                    <p class="mt-2 max-w-2xl text-sm text-gray-500">Upload images of inventory items for AI recognition
                        and automatic categorization</p>
                </div>
                <div>
                    <asp:LinkButton ID="lnkBackToDashboard" runat="server" OnClick="lnkBackToDashboard_Click"
                        CssClass="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md shadow-sm text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500">
                        <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24"
                            stroke="currentColor">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                        </svg>
                        Back to Dashboard
                    </asp:LinkButton>
                </div>
            </div>

            <!-- Notification Panel -->
            <asp:Panel ID="pnlNotification" runat="server" Visible="false" CssClass="px-6 py-4 sm:px-8">
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

            <div class="px-6 py-5 sm:px-8">
                <div class="grid grid-cols-1 md:grid-cols-2 gap-8">
                    <!-- Upload Section -->
                    <div>
                        <div class="bg-white shadow sm:rounded-lg p-6">
                            <h4 class="text-lg font-medium text-gray-900 mb-4">Upload Images</h4>

                            <div class="mb-6">
                                <div id="dropZone"
                                    class="mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-gray-300 border-dashed rounded-md cursor-pointer hover:border-indigo-400 transition-colors">
                                    <div class="space-y-1 text-center">
                                        <svg id="uploadIcon" class="mx-auto h-12 w-12 text-gray-400"
                                            stroke="currentColor" fill="none" viewBox="0 0 48 48">
                                            <path
                                                d="M28 8H12a4 4 0 00-4 4v20m32-12v8m0 0v8a4 4 0 01-4 4H12a4 4 0 01-4-4v-4m32-4l-3.172-3.172a4 4 0 00-5.656 0L28 28M8 32l9.172-9.172a4 4 0 015.656 0L28 28m0 0l4 4m4-24h8m-4-4v8m-12 4h.02"
                                                stroke-width="2" stroke-linecap="round" />
                                        </svg>
                                        <div id="previewContainer" class="flex flex-wrap justify-center"
                                            style="display: none;"></div>
                                        <div id="uploadText" class="flex text-sm text-gray-600">
                                            <label for="<%= fileUpload.ClientID %>"
                                                class="relative cursor-pointer rounded-md font-medium text-indigo-600 hover:text-indigo-500 focus-within:outline-none">
                                                <span>Upload a file</span>
                                                <asp:FileUpload ID="fileUpload" runat="server" CssClass="sr-only"
                                                    accept="image/*" AllowMultiple="true" />
                                            </label>
                                            <p class="pl-1">or drag and drop</p>
                                        </div>
                                        <p class="text-xs text-gray-500">PNG, JPG, GIF up to 10MB</p>
                                    </div>
                                </div>
                            </div>

                            <div class="flex space-x-4">
                                <asp:Button ID="btnUpload" runat="server" Text="Process Images"
                                    OnClick="btnUpload_Click"
                                    CssClass="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" />
                                <asp:Button ID="btnClear" runat="server" Text="Clear" OnClick="btnClear_Click"
                                    CssClass="inline-flex justify-center py-2 px-4 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" />
                            </div>
                        </div>

                        <div class="mt-6 bg-white shadow sm:rounded-lg p-6">
                            <h4 class="text-lg font-medium text-gray-900 mb-4">Batch Upload</h4>
                            <p class="text-sm text-gray-500 mb-4">Bulk upload multiple images for batch processing. The
                                system will automatically process each image and prepare them for inventory.</p>

                            <div class="flex space-x-4">
                                <asp:Button ID="btnProcessBatch" runat="server" Text="Start Batch Processing"
                                    OnClick="btnProcessBatch_Click"
                                    CssClass="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500" />
                                <asp:Button ID="btnCancelBatch" runat="server" Text="Cancel"
                                    OnClick="btnCancelBatch_Click"
                                    CssClass="inline-flex justify-center py-2 px-4 border border-gray-300 shadow-sm text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500" />
                            </div>
                        </div>
                    </div>

                    <!-- Recognition Results Section -->
                    <div>
                        <asp:UpdatePanel ID="UpdatePanelResults" runat="server">
                            <ContentTemplate>
                                <div class="bg-white shadow sm:rounded-lg p-6">
                                    <h4 class="text-lg font-medium text-gray-900 mb-4">AI Recognition Results</h4>

                                    <asp:Panel ID="pnlProcessing" runat="server" Visible="false">
                                        <div class="flex items-center justify-center py-6">
                                            <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-indigo-600"
                                                xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor"
                                                    stroke-width="4"></circle>
                                                <path class="opacity-75" fill="currentColor"
                                                    d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z">
                                                </path>
                                            </svg>
                                            <span class="text-gray-700">Processing image, please wait...</span>
                                        </div>
                                    </asp:Panel>

                                    <asp:Panel ID="pnlNoResults" runat="server" Visible="true">
                                        <div class="text-center py-6">
                                            <svg class="mx-auto h-12 w-12 text-gray-300" fill="none"
                                                stroke="currentColor" viewBox="0 0 24 24"
                                                xmlns="http://www.w3.org/2000/svg">
                                                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                                                    d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z">
                                                </path>
                                            </svg>
                                            <h3 class="mt-2 text-sm font-medium text-gray-900">No results yet</h3>
                                            <p class="mt-1 text-sm text-gray-500">Upload an image to see AI recognition
                                                results.</p>
                                        </div>
                                    </asp:Panel>

                                    <asp:Panel ID="pnlResults" runat="server" Visible="false">
                                        <div class="space-y-6">
                                            <!-- Image Preview -->
                                            <div class="mt-2">
                                                <h5 class="text-sm font-medium text-gray-700 mb-2">Uploaded Image</h5>
                                                <asp:Image ID="imgRecognitionPreview" runat="server"
                                                    CssClass="w-full h-auto max-h-64 object-contain rounded-md border border-gray-200" />
                                            </div>

                                            <!-- Category Suggestions -->
                                            <div>
                                                <h5 class="text-sm font-medium text-gray-700 mb-2">Suggested Categories
                                                </h5>
                                                <div class="bg-gray-50 p-3 rounded-md">
                                                    <asp:Repeater ID="rptrCategorySuggestions" runat="server">
                                                        <ItemTemplate>
                                                            <div
                                                                class="flex justify-between items-center py-1 border-b border-gray-200 last:border-0">
                                                                <div class="flex items-center">
                                                                    <span class="text-sm text-gray-900">
                                                                        <%# Eval("Category") %>
                                                                    </span>
                                                                    <span
                                                                        class="ml-2 text-xs bg-indigo-100 text-indigo-800 py-0.5 px-2 rounded-full">
                                                                        <%# String.Format("{0:P1}", Eval("Confidence"))
                                                                            %>
                                                                    </span>
                                                                </div>
                                                                <asp:LinkButton ID="lnkSelectCategory" runat="server"
                                                                    CommandName="SelectCategory"
                                                                    CommandArgument='<%# Eval("CategoryId") %>'
                                                                    OnCommand="lnkSelectCategory_Command"
                                                                    CssClass="text-xs text-indigo-600 hover:text-indigo-900 font-medium">
                                                                    Use this category
                                                                </asp:LinkButton>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </div>

                                            <!-- Generated Tags -->
                                            <div>
                                                <h5 class="text-sm font-medium text-gray-700 mb-2">Generated Tags</h5>
                                                <div class="flex flex-wrap gap-2">
                                                    <asp:Repeater ID="rptrTagSuggestions" runat="server">
                                                        <ItemTemplate>
                                                            <span
                                                                class="bg-blue-100 text-blue-800 text-xs font-medium px-2.5 py-0.5 rounded">
                                                                <%# Container.DataItem %>
                                                            </span>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </div>

                                            <!-- Add to Inventory Button -->
                                            <div class="pt-4">
                                                <asp:Button ID="btnAddToInventory" runat="server"
                                                    Text="Add to Inventory" OnClick="btnAddToInventory_Click"
                                                    CssClass="inline-flex justify-center py-2 px-4 border border-transparent shadow-sm text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500" />
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>

                                <!-- Batch Processing Status -->
                                <asp:Panel ID="pnlBatchStatus" runat="server" Visible="false"
                                    CssClass="mt-6 bg-white shadow sm:rounded-lg p-6">
                                    <h4 class="text-lg font-medium text-gray-900 mb-4">Batch Processing Status</h4>

                                    <div class="space-y-4">
                                        <div class="flex justify-between items-center">
                                            <span class="text-sm font-medium text-gray-700">Progress:</span>
                                            <span class="text-sm text-gray-900">
                                                <asp:Literal ID="litProgress" runat="server"></asp:Literal>
                                            </span>
                                        </div>

                                        <div class="w-full bg-gray-200 rounded-full h-2.5">
                                            <asp:Panel ID="pnlProgressBar" runat="server"
                                                CssClass="bg-indigo-600 h-2.5 rounded-full" Style="width: 0%">
                                            </asp:Panel>
                                        </div>

                                        <div class="overflow-hidden bg-white shadow sm:rounded-lg">
                                            <ul role="list" class="divide-y divide-gray-200">
                                                <asp:Repeater ID="rptrBatchResults" runat="server">
                                                    <ItemTemplate>
                                                        <li class="px-4 py-4 sm:px-6">
                                                            <div class="flex items-center justify-between">
                                                                <p class="text-sm font-medium text-indigo-600 truncate">
                                                                    <%# Eval("FileName") %>
                                                                </p>
                                                                <div class="ml-2 flex-shrink-0">
                                                                    <span
                                                                        class="px-2 inline-flex text-xs leading-5 font-semibold rounded-full <%# GetStatusClass(Eval("
                                                                        Status").ToString()) %>">
                                                                        <%# Eval("Status") %>
                                                                    </span>
                                                                </div>
                                                            </div>
                                                            <div class="mt-2 sm:flex sm:justify-between">
                                                                <div class="sm:flex">
                                                                    <p class="flex items-center text-sm text-gray-500">
                                                                        <svg class="flex-shrink-0 mr-1.5 h-5 w-5 text-gray-400"
                                                                            xmlns="http://www.w3.org/2000/svg"
                                                                            viewBox="0 0 20 20" fill="currentColor">
                                                                            <path fill-rule="evenodd"
                                                                                d="M17.707 9.293a1 1 0 010 1.414l-7 7a1 1 0 01-1.414 0l-7-7A.997.997 0 012 10V5a3 3 0 013-3h5c.256 0 .512.098.707.293l7 7zM5 6a1 1 0 100-2 1 1 0 000 2z"
                                                                                clip-rule="evenodd" />
                                                                        </svg>
                                                                        <%# Eval("Category") %>
                                                                    </p>
                                                                </div>
                                                                <div
                                                                    class="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                                                                    <svg class="flex-shrink-0 mr-1.5 h-5 w-5 text-gray-400"
                                                                        xmlns="http://www.w3.org/2000/svg"
                                                                        viewBox="0 0 20 20" fill="currentColor">
                                                                        <path fill-rule="evenodd"
                                                                            d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z"
                                                                            clip-rule="evenodd" />
                                                                    </svg>
                                                                    <p>
                                                                        <%# Eval("ProcessedTime", "{0:g}" ) %>
                                                                    </p>
                                                                </div>
                                                            </div>
                                                        </li>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </ul>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="btnUpload" />
                                <asp:PostBackTrigger ControlID="btnProcessBatch" />
                                <asp:AsyncPostBackTrigger ControlID="btnAddToInventory" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>
    </asp:Content>