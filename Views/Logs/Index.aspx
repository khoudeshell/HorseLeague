<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<HorseLeague.Models.Domain.LogItem>" %>
<%@ Import Namespace="HorseLeague.Models" %>
<%@ Import Namespace="HorseLeague.Models.Domain" %>
<%@ Import Namespace="HorseLeague.Models.DataAccess" %> 
<%@ Import Namespace="HorseLeague.Views.Shared" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>View Application Log</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Application Log Items</h2>

    <fieldset>
        <legend>Users</legend>
        <table width="100%" border="1"> 
            <tr>
                <th>Id</th>
                <th>Date</th>
                <th>HostName</th>
                <th>Thread</th>
                <th>Level</th>
                <th>User</th>
                <th>Message</th>
                <th>Exception</th>
            </tr>
        <% var i = 1;
           foreach (HorseLeague.Models.Domain.LogItem logItem in (IEnumerable)ViewData["Logs"])
           { %>    
                <tr>
                    <td><%=logItem.Id%></td>
                    <td><%=logItem.Date%></td>
                    <td><%=logItem.HostName%></td>
                    <td><%=logItem.Thread%></td>
                    <td><%=logItem.Level%></td>
                    <td><%=logItem.User%></td>
                    <td><%=logItem.Message%></td>
                    <td><%=logItem.Exception%></td>                    
                </tr>
        <% i++;
            } %>
        </table> 
    </fieldset>

</asp:Content>
