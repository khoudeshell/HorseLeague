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

        <p>Email: <%=this.Model.SecurityUser.Email %></p>

        <p>Id: <%=this.Model.Id %></p>

        <p>Locked Status: <%=this.Model.SecurityUser.IsLockedOut.ToString() %> </p>

        <p>Last Activity Date: 
            <% 
                var est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                var targetTime = TimeZoneInfo.ConvertTime(this.Model.SecurityUser.LastActivityDate, est);
             %>
             <%=targetTime.ToString() %>

        </p>
        
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
    
    <%= Html.ValidationSummary() %>
    <% 
        using (Html.BeginForm()) {
    %>
    <fieldset>
        <legend>Leagues</legend>
        <table>
            <tr>
                <th>League Id</th>
                <th>UL Id</th>
                <th>Has Paid</th>
                <th>Payment Type</th>
                <th>PayPal Payer</th>
                <th>PayPal Id</th>
                <th>PayPal Token</th>                
            </tr>
            <%foreach(var userLeague in this.Model.UserLeagues)
             {
            %>
                <tr>
                    <td><%=userLeague.League.Id %><%=Html.Hidden("txtLeagueId", userLeague.League.Id) %></td>
                    <td><%=userLeague.Id %><%=Html.Hidden("txtUserLeagueId", userLeague.Id) %></td>
                    <td><%=Html.CheckBox("chkHasPaid", userLeague.HasPaid.GetValueOrDefault(false)) %></td>
                    <td><%=Html.DropDownList("cmbPaymentType", 
                        UIFunctions.GetSelectListFromEnum<UserLeague.PaymentTypes>(userLeague.PaymentType.GetValueOrDefault(UserLeague.PaymentTypes.NotPaid))) %>
                    </td>
                    <td><%=Html.TextBox("txtPayPalPayer", userLeague.PayPalPayerId) %></td> 
                    <td><%=Html.TextBox("txtPayPalId", userLeague.PayPalPaymentId) %></td> 
                    <td><%=Html.TextBox("txtPayPalToken", userLeague.PayPalPaymentToken) %></td> 
                </tr>
            <%
            }
            %>   
        </table>
        <p>
           <input type="submit" value="Save" />
        </p>
    <% } %>
    </fieldset>
    

</asp:Content>
