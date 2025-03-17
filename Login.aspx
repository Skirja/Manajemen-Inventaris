<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs"
    Inherits="Manajemen_Inventaris.Login" %>

    <asp:Content ID="HeadContent" ContentPlaceHolderID="HeadContent" runat="server">
    </asp:Content>

    <asp:Content ID="NavContent" ContentPlaceHolderID="NavContent" runat="server">
        <!-- No navigation items for login page -->
    </asp:Content>

    <asp:Content ID="LoginStatusContent" ContentPlaceHolderID="LoginStatusContent" runat="server">
        <!-- No login status for login page -->
    </asp:Content>

    <asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
        <div class="flex min-h-full flex-col justify-center py-12 sm:px-6 lg:px-8">
            <div class="sm:mx-auto sm:w-full sm:max-w-lg">
                <h2 class="mt-6 text-center text-4xl font-bold tracking-tight text-gray-900">Manajemen Inventaris</h2>
                <p class="mt-2 text-center text-lg text-gray-600">
                    Masuk ke akun Anda
                </p>
            </div>

            <div class="mt-10 sm:mx-auto sm:w-full sm:max-w-lg">
                <div class="bg-white py-10 px-6 shadow-lg sm:rounded-lg sm:px-12">
                    <div class="space-y-8">
                        <div>
                            <label for="txtUsername" class="block text-base font-medium text-gray-700">Username</label>
                            <div class="mt-2">
                                <asp:TextBox ID="txtUsername" runat="server"
                                    CssClass="block w-full appearance-none rounded-md border border-gray-300 px-4 py-3 placeholder-gray-400 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 text-base"
                                    placeholder="Masukkan username" required></asp:TextBox>
                                <span id="usernameError" class="text-red-500 text-sm mt-1 hidden">Username harus
                                    diisi</span>
                            </div>
                        </div>

                        <div>
                            <label for="txtPassword" class="block text-base font-medium text-gray-700">Password</label>
                            <div class="mt-2">
                                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password"
                                    CssClass="block w-full appearance-none rounded-md border border-gray-300 px-4 py-3 placeholder-gray-400 shadow-sm focus:border-indigo-500 focus:outline-none focus:ring-indigo-500 text-base"
                                    placeholder="Masukkan password" required></asp:TextBox>
                                <span id="passwordError" class="text-red-500 text-sm mt-1 hidden">Password harus
                                    diisi</span>
                            </div>
                        </div>

                        <div>
                            <asp:Button ID="btnLogin" runat="server" Text="Masuk" OnClick="btnLogin_Click"
                                OnClientClick="return validateForm();"
                                CssClass="flex w-full justify-center rounded-md border border-transparent bg-indigo-600 py-3 px-4 text-base font-medium text-white shadow-sm hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2" />
                        </div>

                        <asp:Label ID="lblMessage" runat="server" CssClass="text-red-500 text-base text-center block"
                            Visible="false"></asp:Label>

                        <div class="text-base text-center">
                            <p>Belum punya akun?
                                <asp:HyperLink ID="lnkRegister" runat="server" NavigateUrl="~/Register.aspx"
                                    CssClass="font-medium text-indigo-600 hover:text-indigo-500">
                                    Daftar disini
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
                var password = document.getElementById('<%= txtPassword.ClientID %>').value;

                if (username.trim() === '') {
                    document.getElementById('usernameError').classList.remove('hidden');
                    isValid = false;
                } else {
                    document.getElementById('usernameError').classList.add('hidden');
                }

                if (password.trim() === '') {
                    document.getElementById('passwordError').classList.remove('hidden');
                    isValid = false;
                } else {
                    document.getElementById('passwordError').classList.add('hidden');
                }

                return isValid;
            }
        </script>
    </asp:Content>