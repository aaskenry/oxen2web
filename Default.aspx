<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
         CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    
    <h2>
        MOMENTANMÄTNINGAR
    </h2>
    <p>
        <asp:ScriptManager EnablePartialRendering="true"  ID="ScriptManager1" runat="server"></asp:ScriptManager>  

        <asp:UpdatePanel runat="server" id="UpdatePanel1">
            <ContentTemplate>
                <asp:Timer runat="server" id="PollTimer" Interval="1000" OnTick="PollTimerTick"></asp:Timer>            
                <table >
                    <tr>
                        <td width="200">
                            Radiatortemperatur, 
                            tur</td>
                        <td width="300">
                            <asp:Label ID="RadiatorTurLabel" runat="server" ReadOnly="True"></asp:Label>
                            &#176;C</td>
                        <td width="400">
                            <asp:Label ID="RadiatorTurErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>            
                    </tr>
                    <tr>
                        <td width="200">
                            Radiatortemperatur, retur</td>
                        <td width="300">
                            <asp:Label ID="RadiatorReturLabel" runat="server" ReadOnly="True"></asp:Label>
                            &#176;C</td>
                        <td width="400">
                            <asp:Label ID="RadiatorReturErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>            
                    </tr>
                    <tr>
                        <td width="200">
                            Utomhustemperatur</td>
                        <td width="300">
                            <asp:Label ID="UtomhusLabel" runat="server"></asp:Label>
                            &#176;C</td>
                        <td width="400">
                            <asp:Label ID="UtomhusErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>            
                    </tr>
                    <tr>
                        <td width="200">
                            Elenergi, totalt</td>
                        <td width="300">
                            <asp:Label ID="ElTotalLabel" runat="server"></asp:Label>
                            kWh</td>
                        <td width="400">
                            <asp:Label ID="ElTotalErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>            
                    </tr>
                    <tr>
                        <td width="200">
                            Elenergi, fas 1</td>
                        <td width="300">
                            <asp:Label ID="ElFas1Label" runat="server"></asp:Label>
                        </td>
                        <td width="400">
                            <asp:Label ID="ElFas1ErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td width="200">
                            Elenergi, fas 2</td>
                        <td width="300">
                            <asp:Label ID="ElFas2Label" runat="server"></asp:Label>
                        </td>
                        <td width="400">
                            <asp:Label ID="ElFas2ErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td width="200">
                            Elenergi, fas 3</td>
                        <td width="300">
                            <asp:Label ID="ElFas3Label" runat="server"></asp:Label>
                        </td>
                        <td width="400">
                            <asp:Label ID="ElFas3ErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td width="200">
                            Fjärrvärmeenergi, totalt</td>
                        <td width="300">
                            <asp:Label ID="FjarrTotaltLabel" runat="server"></asp:Label>
                            kWh</td>
                        <td width="400">
                            <asp:Label ID="FjarrTotaltErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>            
                    </tr>
                    <tr>
                        <td width="200">
                            Fjärrvärmeenergi, aktuell</td>
                        <td width="300">
                            <asp:Label ID="FjarrAktuellLabel" runat="server"></asp:Label>
                            kW</td>
                        <td width="400">
                            <asp:Label ID="FjarrAktuellErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>            
                    </tr>
                    <tr>
                        <td width="200">
                            Fjärrvärmeenergi, temp in</td>
                        <td width="300">
                            <asp:Label ID="FjarrTempInLabel" runat="server"></asp:Label>
                            &#176;C</td>
                        <td width="400">
                            <asp:Label ID="FjarrTempInErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>            
                    </tr>
                    <tr>
                        <td width="200">
                            Fjärrvärmeenergi, temp ut</td>
                        <td width="300">
                            <asp:Label ID="FjarrTempUtLabel" runat="server"></asp:Label>
                            &#176;C</td>
                        <td width="400">
                            <asp:Label ID="FjarrTempUtErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>            
                    </tr>
                    <tr>
                        <td width="200">
                            Fjärrvärmeenergi, delta</td>
                        <td width="300">
                            <asp:Label ID="FjarrDeltaLabel" runat="server"></asp:Label>
                            &#176;C</td>
                        <td width="400">
                            <asp:Label ID="FjarrDeltaErrorLabel" runat="server" ReadOnly="True" 
                                       ForeColor="Red"></asp:Label>
                        </td>            
                    </tr>
                    <tr>
                        <td width="200">
                            Vattenförbrukning, totalt</td>
                        <td width="300">
                            <asp:Label ID="VattenLabel" runat="server"></asp:Label>
                            m&#179;</td>
                        <td width="400">
                            <asp:Label ID="VattenErrorLabel" runat="server" ReadOnly="True" ForeColor="Red"></asp:Label>
                        </td>            
                    </tr>
                    <tr>
                        <td width="200">
                            <asp:DropDownList ID="TempSensorList" runat="server" Visible="False">
                            </asp:DropDownList>
                        </td>
                        <td width="300">                    
                        </td>
                        <td width="400">
                        </td>            
                    </tr>
                </table>
                <div align="center">
                    <asp:Label runat="server" Text="Ej uppdaterad än" id="LastUpdatedLabel"></asp:Label>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </p>
</asp:Content>
