using System;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;

public partial class WaterHistory : HistoryPage
{
    private const string Vatten = "Vatten";

    protected void Page_Load(object sender, EventArgs e)
    {
        CreateChartArea(WaterChart, "Tid", "Liter");
        WaterChart.Series.Add(Vatten);
        if (!IsPostBack)
        {
            LoadPeriodDropDownList(PeriodDropDownList);
            LoadData();
        }
    }

    private int DrawWaterLine(string name, DateTime firstDate, int borderWidth)
    {
        int total = 0;
        var series = WaterChart.Series[name];
        series.ChartArea = WaterChart.ChartAreas[0].Name;
        series.ChartType = SeriesChartType.StepLine;
        series.BorderWidth = borderWidth;
        series.IsVisibleInLegend = true;
        series.ToolTip = name;
        series.Points.Clear();
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            data.WaterMeter.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            var q = from r in data.WaterMeter
                    where r.time > firstDate
                    select r;
            foreach (var t in q)
            {
                series.Points.AddXY(t.time, t.value);
                total += t.value;
            }
        }
        return total;
    }

    protected override void LoadData()
    {
        WaterChart.ChartAreas[0].AxisX.LabelStyle.Format = "";
        WaterChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Auto;
        var borderWidth = 1;
        var firstDate = DateTime.Now.AddYears(-100);
        if (PeriodDropDownList.SelectedValue.Equals(LastDay))
        {
            firstDate = DateTime.Now.AddDays(-1.0);
            WaterChart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            WaterChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Hours;
            borderWidth = 2;
        }
        else if (PeriodDropDownList.SelectedValue.Equals(LastWeek))
        {
            firstDate = DateTime.Now.AddDays(-7.0);
        }

        TotalLabel.Text = "Totalt " + DrawWaterLine(Vatten, firstDate, borderWidth);

        using (var data = new MeterLogModel.MeterLogEntities())
        {
            var q = (from r in data.WaterMeter
                     select r.time).Max();
            LastUpdatedLabel.Text = "Uppdaterad " + q;
        }
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            var q = (from r in data.WaterMeter
                     where r.time > firstDate
                     select r.value).Min();
            MinValueLabel.Text = " Min " + String.Format("{0:0.0}", q);
        }
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            var q = (from r in data.WaterMeter
                     where r.time > firstDate
                     select r.value).Max();
            MaxValueLabel.Text = " Max " + String.Format("{0:0.0}", q);
        }
    }
}