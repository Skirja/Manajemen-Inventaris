<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true"
    CodeBehind="Register.aspx.cs" Inherits="Manajemen_Inventaris.Register" %>

    <asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    </asp:Content>

    <asp:Content ID="NavContent" ContentPlaceHolderID="NavContent" runat="server">
        <!-- No navigation items for register page -->
    </asp:Content>

    <asp:Content ID="LoginStatusContent" ContentPlaceHolderID="LoginStatusContent" runat="server">
        <!-- No login status for register page -->
    </asp:Content>

    <asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="flex min-h-full flex-col justify-center py-12 sm:px-6 lg:px-8">
            <div class="sm:mx-auto sm:w-full sm:max-w-2xl">
                <h2 class="mt-6 text-center text-4xl font-bold tracking-tight text-gray-900">Daftar Akun Baru</h2>
                <p class="mt-2 text-center text-lg text-gray-600">
                    Buat akun untuk mengakses Manajemen Inventaris
                </p>
            </div>

            <div class="mt-10 sm:mx-auto sm:w-full sm:max-w-2xl">
                <div class="bg-white py-10 px-6 shadow-lg sm:rounded-lg sm:px-12">
                    <div class="space-y-8">
                        <!-- Two column layout for form fields -->
                        <div class="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <!-- Left column -->
                            <div class="space-y-6">
                                <div>
                                    <label for="txtUsername"
                                        class="block text-base font-medium text-gray-700">Username</label>
                                    <div class="mt-2">
                                        <asp:TextBox ID="txtUsername" runat="server"
                                            CssClass="block w-full appearance-none rounded-md border border-gray-300 px-4 py-3 placeholder-gray-400 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 text-base"
                                            placeholder="Masukkan username" required></asp:TextBox>
                                        <span id="usernameError" class="text-red-500 text-sm mt-1 hidden">Username harus
                                            diisi</span>
                                    </div>
                                </div>

                                <div>
                                    <label for="txtEmail"
                                        class="block text-base font-medium text-gray-700">Email</label>
                                    <div class="mt-2">
                                        <asp:TextBox ID="txtEmail" runat="server" TextMode="Email"
                                            CssClass="block w-full appearance-none rounded-md border border-gray-300 px-4 py-3 placeholder-gray-400 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 text-base"
                                            placeholder="Masukkan email" required></asp:TextBox>
                                        <span id="emailError" class="text-red-500 text-sm mt-1 hidden">Email harus diisi
                                            dengan format yang benar</span>
                                    </div>
                                </div>

                                <div>
                                    <label for="txtCompany" class="block text-base font-medium text-gray-700">Nama
                                        Perusahaan</label>
                                    <div class="mt-2">
                                        <asp:TextBox ID="txtCompany" runat="server"
                                            CssClass="block w-full appearance-none rounded-md border border-gray-300 px-4 py-3 placeholder-gray-400 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 text-base"
                                            placeholder="Masukkan nama perusahaan" required></asp:TextBox>
                                        <span id="companyError" class="text-red-500 text-sm mt-1 hidden">Nama perusahaan
                                            harus diisi</span>
                                    </div>
                                </div>
                            </div>

                            <!-- Right column -->
                            <div class="space-y-6">
                                <div>
                                    <label for="txtPassword"
                                        class="block text-base font-medium text-gray-700">Password</label>
                                    <div class="mt-2">
                                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"
                                            CssClass="block w-full appearance-none rounded-md border border-gray-300 px-4 py-3 placeholder-gray-400 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 text-base"
                                            placeholder="Masukkan password" required></asp:TextBox>
                                        <span id="passwordError" class="text-red-500 text-sm mt-1 hidden">Password harus
                                            diisi</span>
                                    </div>
                                </div>

                                <div>
                                    <label for="txtConfirmPassword"
                                        class="block text-base font-medium text-gray-700">Konfirmasi Password</label>
                                    <div class="mt-2">
                                        <asp:TextBox ID="txtConfirmPassword" runat="server" TextMode="Password"
                                            CssClass="block w-full appearance-none rounded-md border border-gray-300 px-4 py-3 placeholder-gray-400 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 text-base"
                                            placeholder="Konfirmasi password" required></asp:TextBox>
                                        <span id="confirmPasswordError"
                                            class="text-red-500 text-sm mt-1 hidden">Konfirmasi password harus sama
                                            dengan password</span>
                                    </div>
                                </div>

                                <div class="flex items-center mt-2">
                                    <asp:CheckBox ID="chkAgree" runat="server"
                                        CssClass="h-5 w-5 text-indigo-600 focus:ring-indigo-500 border-gray-300 rounded" />
                                    <label for="chkAgree" class="ml-2 block text-sm text-gray-700">
                                        Saya setuju dengan <a href="#"
                                            class="text-indigo-600 hover:text-indigo-500">Syarat dan Ketentuan</a>
                                    </label>
                                </div>
                            </div>
                        </div>

                        <div>
                            <asp:Button ID="btnRegister" runat="server" Text="Daftar" OnClick="btnRegister_Click"
                                OnClientClick="return validateForm();"
                                CssClass="flex w-full justify-center rounded-md border border-transparent bg-indigo-600 py-3 px-4 text-base font-medium text-white shadow-sm hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2" />
                        </div>

                        <asp:Label ID="lblMessage" runat="server" CssClass="text-red-500 text-base text-center block"
                            Visible="false"></asp:Label>

                        <div class="text-base text-center">
                            <p>Sudah punya akun?
                                <asp:HyperLink ID="lnkLogin" runat="server" NavigateUrl="~/Login.aspx"
                                    CssClass="font-medium text-indigo-600 hover:text-indigo-500">
                                    Masuk disini
                                </asp:HyperLink>
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <script type="text/javascript">
            function validateForm() {
                var isValid = true;
                var username = document.getElementById('<%= txtUsername.ClientID %>').value;
                var email = document.getElementById('<%= txtEmail.ClientID %>').value;
                var company = document.getElementById('<%= txtCompany.ClientID %>').value;
                var password = document.getElementById('<%= txtPassword.ClientID %>').value;
                var confirmPassword = document.getElementById('<%= txtConfirmPassword.ClientID %>').value;
                var agreeChecked = document.getElementById('<%= chkAgree.ClientID %>').checked;

                // Validate username
                if (username.trim() === '') {
                    document.getElementById('usernameError').classList.remove('hidden');
                    isValid = false;
                } else {
                    document.getElementById('usernameError').classList.add('hidden');
                }

                // Validate email
                var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (email.trim() === '' || !emailRegex.test(email)) {
                    document.getElementById('emailError').classList.remove('hidden');
                    isValid = false;
                } else {
                    document.getElementById('emailError').classList.add('hidden');
                }

                // Validate company
                if (company.trim() === '') {
                    document.getElementById('companyError').classList.remove('hidden');
                    isValid = false;
                } else {
                    document.getElementById('companyError').classList.add('hidden');
                }

                // Validate password
                if (password.trim() === '') {
                    document.getElementById('passwordError').classList.remove('hidden');
                    isValid = false;
                } else {
                    document.getElementById('passwordError').classList.add('hidden');
                }

                // Validate confirm password
                if (confirmPassword.trim() === '' || confirmPassword !== password) {
                    document.getElementById('confirmPasswordError').classList.remove('hidden');
                    isValid = false;
                } else {
                    document.getElementById('confirmPasswordError').classList.add('hidden');
                }

                // Validate agreement checkbox
                if (!agreeChecked) {
                    alert('Anda harus menyetujui Syarat dan Ketentuan untuk melanjutkan.');
                    isValid = false;
                }

                return isValid;
            }
        </script>
    </asp:Content>