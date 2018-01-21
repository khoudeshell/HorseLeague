<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<HorseLeague.Models.Domain.User>" %>
<%@ Import Namespace="HorseLeague.Models" %>
<%@ Import Namespace="HorseLeague.Models.Domain" %>
<%@ Import Namespace="HorseLeague.Models.DataAccess" %> 
<%@ Import Namespace="HorseLeague.Views.Shared" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>UpdateUser</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Users</h2>
    <p><%=Html.ActionLink("Index", "") %> | <%=Html.ActionLink("Activity", "Activity") %> | <%=Html.ActionLink("Unpaid Users", "Unpaid") %> | <%=Html.ActionLink("Get All", "GetAll") %></p>
    <fieldset>
        <legend>Users</legend>
        <table width="100%"> 
            <tr>
                <th>#</th>
                <th>User</th>
                <th>Email</th>
                <th>Paid</th>
                <th>Last Activity</th>
                <th>Action</th>
            </tr>
        <% var i = 1;
           foreach (HorseLeague.Models.Domain.UserReport user in (IEnumerable)ViewData["Users"])
           { %>    
                <tr>
                    <td><%=i%>.</td>
                    <td><%=Html.ActionLink(Html.Encode(user.UserName), "User", "Users", new { userName = user.UserName }, null)  %></td>
                    <td><%=user.Email %></td>
                    <td><%=user.HasPaid %></td>
                    <td><%=user.LastActivityDate %></td>
                    <td><%=Html.ActionLink("Logs", "LogsByUser", "Logs",  new { userName = user.UserName }, null) %></td>
                </tr>
        <% i++;
            } %>
        </table> 
    </fieldset>

</asp:Content>
