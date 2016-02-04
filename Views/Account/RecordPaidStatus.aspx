<%@Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="recordPaidStatusHead" ContentPlaceHolderID="head" runat="server">
    <title>Paypal Payment</title>
</asp:Content>

<asp:Content ID="recrodPaidStatusContent" ContentPlaceHolderID="MainContent" runat="server">
    <h3>Step 1: PayPal Payment</h3>
    <p class="success">Your paypal transaction has been completed successfully!  You will be emailed a record of your transaction.</p>

    <h3>Step 2: TCR record of transaction</h3>
    <%
        if(!ViewData.ModelState.IsValid)
        {
     %>
            
            <p class="error">
                Although the Paypal transaction was successful an error occurred when marking your status as paid in TCR.
            </p>
            <p class="error">    
                Please <a href="mailto:triplecrownroyal@gmail.com">contact TCR</a> about this issue to ensure your status is updated.
            </p>
     <%
        }
        else
        {
     %>
            <p class="success">Your payment status has been recorded successfully in TCR!</p>
     <%
        }
     %>
     <p>Click <a href="/home">here</a> to log in.</p> 
</asp:Content>
