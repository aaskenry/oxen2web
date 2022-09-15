using System;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for HistoryPage
/// </summary>
public abstract class HistoryPage : ChartPage
{
    protected const string All = "All";
    protected const string LastYears = "LastYears";
    protected const string LastDay = "LastDay";
    protected const string LastWeek = "LastWeek";

    protected HistoryPage()
    {
    }

    protected void PeriodDropDownListSelectedIndexChanged(object sender, EventArgs e)
    {
        LoadData();
    }

    protected abstract void LoadData();

    protected void LoadPeriodDropDownList(DropDownList list)
    {
        list.Items.Add(new ListItem("Senaste 24 timmarna", LastDay));
        list.Items.Add(new ListItem("Senaste 7 dagarna", LastWeek));
        list.Items.Add(new ListItem("Senaste två åren", LastYears));
        list.Items.Add(new ListItem("Allt", All));
    }
}