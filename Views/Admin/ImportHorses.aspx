<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<HorseLeague.Models.Domain.LeagueRace>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Import Horses</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <h2>Import Horses</h2>

    <%= Html.ValidationSummary() %>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Import URL</legend>
            <p>
                <label>URL:</label>
                <%= Html.TextBox("URL") %>
                <%= Html.ValidationMessage("URL", "*") %>
            </p>
            <p>
                <input type="submit" value="Load" />
            </p>
        </fieldset>

    <% } %>

    

</asp:Content>



