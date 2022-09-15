<%@ Page Title="Temperaturer" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="TempHistory.aspx.cs" Inherits="TempHistory" %>

<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Historik - Temperaturer
        <asp:DropDownList ID="PeriodDropDownList" runat="server" AutoPostBack="True" 
            onselectedindexchanged="PeriodDropDownListSelectedIndexChanged">
        </asp:DropDownList>
    </h2>
    <p>
        <asp:Chart ID="TempChart" runat="server" DataSourceID="LogEntityDataSource"
            Width="900px" Height="500px">
        </asp:Chart>
        <asp:Panel ID="ChartPanel" runat="server" Wrap="True" HorizontalAlign="Center">
            <asp:Label ID="LastUpdatedLabel" runat="server"></asp:Label>
            <asp:Label ID="MinValueLabel" runat="server"></asp:Label>&#176;C
            <asp:Label ID="MaxValueLabel" runat="server"></asp:Label>&#176;C
        </asp:Panel>       
        <asp:EntityDataSource ID="LogEntityDataSource" runat="server" 
            ConnectionString="name=MeterLogEntities" DefaultContainerName="MeterLogEntities" 
            EnableDelete="True" EnableInsert="True" 
            EnableUpdate="True" EntitySetName="Temperature">
        </asp:EntityDataSource>
    </p>
</asp:Content>
