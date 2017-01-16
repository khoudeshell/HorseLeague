<%@Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="forgotUserNameSuccessHead" ContentPlaceHolderID="head" runat="server">
    <title>Forgot User Name</title>
</asp:Content>

<asp:Content ID="forgotUserNameSuccessContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Forgot User Name</h2>
    <p>
        Your user name(s) were emailed to <%=Html.Encode(ViewData["email"]) %>.
    </p>
    <div>
        <%=Html.ActionLink("Back to Login", "Logon") %>
    </div>
</asp:Content>
