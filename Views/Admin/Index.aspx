<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="HorseLeague.Models.DataAccess" %> 

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<title>Index</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <fieldset>
        <legend>Races</legend>
        <table width="100%"> 
            <tr>
                <th>#</th>
                <th>Race</th>
                <th>Date</th>           
                <th>Track</th>           
            </tr>
        <% int i = 1;
            foreach (HorseLeague.Models.Domain.LeagueRace lr in (IEnumerable)ViewData["ScheduledRaces"])
           { %>    
                <tr>
                    <td><%=i%>.</td>
                    <td><%=Html.ActionLink(Html.Encode(lr.Race.Name), "ViewLeagueRace", new {id = lr.Id}, null)   %></td>
                    <td><%=lr.RaceDate.ToShortDateString() %></td>
                    <td align="center"><%=Html.Encode(lr.Race.Track) %></td>
                </tr>
        <% i++;
            } %>
        </table> 
        <p>
            <%=Html.ActionLink("Recalculate Standings", "RecalcStandings") %>
        </p>
    </fieldset>
    <br />
    <fieldset>
    <legend>Past Performances</legend>
        <ul>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/BallenaVistaFarm/BENCHMARK/1991/summary.html" rel="nofollow">Benchmark (sire)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/BallenaVistaFarm/BERTRANDO/1989/summary.html" rel="nofollow">Bertrando (sire)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/BallenaVistaFarm/TRIBAL+RULE/1996/summary.html" rel="nofollow">Tribal Rule (sire)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/Baffert/BAFFERT+BOB/9999/summary.htm" rel="nofollow">Bob Baffert (trainer)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/James_Bond/BOND+HAROLD+JAMES/9999/summary.html" rel="nofollow">H. James Bond (trainer)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/MarkCasse/CASSE+MARK/9999/summary.html" rel="nofollow">Mark Casse (trainer)</a></li>
<li><a href="http://www.tlorehorses.com/trainers/cr/public/brisnet.cfm" rel="nofollow">Gary Contessa (trainer)</a></li>
<li><a href="http://www.tlorehorses.com/trainers/cdp/public/brisnet.cfm" rel="nofollow">Catherine Day Phillips (trainer)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/DavidDonk/DONK+DAVID/9999/summary.htm" rel="nofollow">David Donk (trainer)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/EoinHarty/HARTY+EOIN/9999/summary.html" rel="nofollow">Eoin Harty (trainer)</a></li>
<li><a href="http://www.hennigracing.com/trainers/hr//public/brisnet.cfm" rel="nofollow">Mark Hennig (trainer)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/TimothyKeefe/KEEFE+TIMOTHY+L/9999/summary.html" rel="nofollow">Timothy Keefe (trainer)</a></li>
<li><a href="http://www.kiaranmclaughlinracing.com/trainers/mcl/public/brisnet.cfm" rel="nofollow">Kiaran McLaughlin (trainer)</a></li>
<li><a href="http://jamesmcmullenracing.com/entries.php" rel="nofollow">James McMullen (trainer)</a></li>
<li><a href="http://www.mcpeekracing.com/entries/" rel="nofollow">Ken McPeek (trainer)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/RonMoquett/MOQUETT+RONALD+E/9999/summary.html" rel="nofollow">Ronald Moquett (trainer)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/HGrahamMotion/MOTION+H+GRAHAM/9999/summary.html" rel="nofollow">H. Graham Motion (trainer)</a></li>
<li><a href="http://www.dougoneillracing.com/trainers/oneill/public/brisnet.cfm" rel="nofollow">Doug O'Neill (trainer)</a></li>
<li><a href="http://www.tlorehorses.com/trainers/pletcher/index_new.cfm?page=brisnet" rel="nofollow">Todd Pletcher (trainer)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/RiceLinda/RICE+LINDA/9999/summary.html" rel="nofollow">Linda Rice (trainer)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/DaleRomans/ROMANS+DALE/9999/summary.htm" rel="nofollow">Dale Romans (trainer)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/BarclayTagg/TAGG+BARCLAY/9999/summary.html" rel="nofollow">Barclay Tagg (trainer)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/Stronach/STRONAC/9999/owner_summary.html" rel="nofollow">Adena Springs (breeder)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/Bongo/BONGO/9999/summary.htm" rel="nofollow">Bongo (owner)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/Darley/sire_summary.htm" rel="nofollow">Darley (breeder)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/DogwoodStable/DOGWOOD+STABLE/9999/summary.htm" rel="nofollow">Dogwood Stable (owner)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/Darley/GODOLPHIN/9999/summary.html" rel="nofollow">Godolphin (owner)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/SouthernEquine/SOUTHERN+EQUINE/9999/summary.html" rel="nofollow">Southern Equine (owner)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/SpendThrift/combined%20summary.htm" rel="nofollow">Spendthrift (breeder)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/StarlightStable/combined%20summary.html" rel="nofollow">Starlight (owner)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/Vinery/sire_summary.htm" rel="nofollow">Vinery (breeder)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/Winstar_Farm_LLC/sire_summary.htm" rel="nofollow">Winstar (breeder)</a></li>
<li><a href="http://www.brisnet.com/cgi-bin/briswatch.cgi/public/ZayatStables/ZAYAT+STABLES+LLC/9999/summary.html" rel="nofollow">Zayat (owner)</a></li>
</ul>
    </fieldset>
    <br />
    <fieldset>
        <legend>Users</legend>
        
        <a href="/Users">View All Users</a>
    </fieldset>
</asp:Content>
