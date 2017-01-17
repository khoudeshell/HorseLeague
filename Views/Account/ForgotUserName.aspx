<%@Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="forgotUserName" ContentPlaceHolderID="head" runat="server">
    <title>Forgot Username</title>
</asp:Content>

<asp:Content ID="forgotUserNameContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Forgot User Name</h2>
    <p>
        Please enter the email address you registered with to retrieve your user name.
    </p>
    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) { %>
        <div>
                <p>
                    <label for="email">Email:</label>
                    <%= Html.TextBox("email", "", new { type = "email" })%>
                </p>
                <p>
                    <input type="submit" value="Get User Name" />
                </p>
        </div>
    <% } %>
    <div>
        <%=Html.ActionLink("Back to Login", "Logon") %>
    </div>
</asp:Content>
