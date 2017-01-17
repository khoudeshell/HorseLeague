<%@Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="forgotPasswor" ContentPlaceHolderID="head" runat="server">
    <title>Forgot Password</title>
</asp:Content>

<asp:Content ID="forgotPasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Forgot Password</h2>
    <p>
        Please enter your user name.
    </p>
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) { %>
        <div>
                <p>
                    <label for="email">User Name:</label>
                    <%= Html.TextBox("userName", "", new { type = "userName" })%>
                </p>
                <p>
                    <input type="submit" value="Reset Password" />
                </p>
        </div>
    <% } %>
    <div>
        <%=Html.ActionLink("Back to Login", "Logon") %>
    </div>
</asp:Content>
