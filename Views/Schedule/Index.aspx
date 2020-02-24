<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="HorseLeague.Models.DataAccess" %> 
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Race Schedule</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

     <fieldset>
            <legend>Race Schedule</legend>
            <table width="100%"> 
                <tr>
                    <th>#</th>
                    <th>Race</th>
                    <th>Date</th>           
                    <th>Track</th>           
                    <th>Weight</th>   
                </tr>
            <% int i = 1;
                foreach (HorseLeague.Models.Domain.LeagueRace lr in ((IList<HorseLeague.Models.Domain.LeagueRace>)ViewData["ScheduledRaces"]).OrderBy(x => x.RaceDate))
               { %>    
                    <tr>
                        <td><%=i%>.</td>
                        <td><%=Html.Encode(lr.Race.Name)  %></td>
                        <td><%=lr.RaceDate.ToShortDateString() %></td>
                        <td align="center"><%=Html.Encode(lr.Race.Track) %></td>
                        <td align="center"><%=lr.Weight %></td>
                    </tr>
            <% i++;
                } %>
            </table> 
        </fieldset>

</asp:Content>
