<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<HorseLeague.Models.Domain.User>" %>
<%@ Import Namespace="HorseLeague.Models" %>
<%@ Import Namespace="HorseLeague.Models.Domain" %>
<%@ Import Namespace="HorseLeague.Models.DataAccess" %> 
<%@ Import Namespace="HorseLeague.Views.Shared" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>View User</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>View User</h2>
     
    <% var name = this.Model.UserName; %>
    <fieldset>
        <legend>Account Details</legend>

        <p>Name: <%=name %></p>

        <p>Id: <%=this.Model.Id %></p>

        <p>Locked Status: <%=this.Model.SecurityUser.IsLockedOut.ToString() %> </p>

        <p>Last Activity Date: <%= this.Model.SecurityUser.LastActivityDate.ToString() %></p>
        
        <p>Account Actions: 
            <ul>
                <% if(this.Model.SecurityUser.IsLockedOut)
                   {
                 %>
                        <li>
                            <%=Html.ActionLink("Unlock Account", "UnlockUser", new { userName = name }) %>
                        </li>
                 <% 
                    }
                  %>  
                <li><%=Html.ActionLink("Reset Password", "ResetPassword", "Account", new { userName = name }, null)%></li>

            </ul>
        </p>
    </fieldset>

    <fieldset>
        <legend>Leagues</legend>

        <table>
            <tr>
                <th>League Id</th>
                <th>UL Id</th>
                <th>Has Paid</th>
                <th>Action</th>
            </tr>
        <%foreach(var userLeague in this.Model.UserLeagues)
          {
         %>
            <tr>
                <td><%=userLeague.League.Id %></td>
                <td><%=userLeague.Id %></td>
                <td><%=userLeague.HasPaid %></td>
                <td><%=Html.ActionLink("Flip Pay Status", "SetPayStatus", new { userName = name, userLeagueId = userLeague.Id, payStatus = !userLeague.HasPaid }) %></td> </tr>
        <%
          }
          %>

          </table>
    </fieldset>
    

</asp:Content>
