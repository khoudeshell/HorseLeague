<%@Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="forgotPasswordSuccessHead" ContentPlaceHolderID="head" runat="server">
    <title>Forgot User Name</title>
</asp:Content>

<asp:Content ID="forgotPasswordSuccessContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Forgot Password</h2>
    <p>
        A new password was sent to the email on file for <%=Html.Encode(ViewData["userName"]) %>.
    </p>
    <div>
        <%=Html.ActionLink("Back to Login", "Logon") %>
    </div>
</asp:Content>
