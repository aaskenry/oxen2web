using System.Drawing;
using System.Web.UI.DataVisualization.Charting;

/// <summary>
/// Summary description for ChartPage
/// </summary>
public abstract class ChartPage : System.Web.UI.Page
{

    protected ChartPage()
    {
    }

    protected void CreateChartArea(Chart chart, string xAxisLabel, string yAxisLabel)
    {
        chart.ChartAreas.Add("ChartArea");
        chart.ChartAreas[0].AxisX.Interval = 1;
        chart.ChartAreas[0].AxisX.Title = xAxisLabel;
        chart.ChartAreas[0].AxisY.Title = yAxisLabel;
        chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.DarkGray;
        chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.DarkGray;
        chart.ChartAreas[0].AxisX2.MajorGrid.LineColor = Color.DarkGray;
        chart.ChartAreas[0].AxisY2.MajorGrid.LineColor = Color.DarkGray;
        chart.Legends.Add(new Legend { Docking = Docking.Bottom });
    }
}