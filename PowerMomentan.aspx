<%@ Page Title="Elförbrukning" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="PowerMomentan.aspx.cs" Inherits="PowerHistory" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Momentan - Elförbrukning
    </h2>
    <p>
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>  

        <asp:UpdatePanel runat="server" id="UpdatePanel1" >
            <ContentTemplate>
                <asp:Timer runat="server" id="PollTimer" Interval="1000" OnTick="PollTimerTick"></asp:Timer>            
                <asp:Chart ID="ElChart" runat="server" 
                    Width="900px" Height="500px" EnableViewState="True">
                </asp:Chart>
                <asp:Panel ID="ChartPanel" runat="server" Wrap="True" HorizontalAlign="Center">
                    <asp:Label ID="LastUpdatedLabel" runat="server"></asp:Label>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
    </p>
</asp:Content>
