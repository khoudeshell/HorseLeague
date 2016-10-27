<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Upload Race File</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Upload File to Race</h2>
     <% using (Html.BeginForm("Upload", "File", FormMethod.Post, new { enctype = "multipart/form-data" }))
        {%>
        <fieldset>
            <legend>Upload File</legend>
        
            <p>
                Form: <input type="file" name="fileForm" id="fileForm" />
            </p>
            
            <input type="submit" value="Save" />
        </fieldset>
       <% } %>
</asp:Content>
