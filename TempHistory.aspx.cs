using System;
using System.Drawing;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;

public partial class TempHistory : HistoryPage
{
    private const string Utomhus = "Utomhus";
    private const string RadiatorTur = "Radiator-Tur";
    private const string RadiatorRetur = "Radiator-Retur";
    private const string Duschrum = "Duschrum";
    private const string Garage = "Garage";
    private const string Kylrum = "Kylrum";

    protected void Page_Load(object sender, EventArgs e)
    {
        CreateChartArea(TempChart, "Tid", "Grader");
        TempChart.Series.Add(Utomhus);
        TempChart.Series.Add(RadiatorTur);
        TempChart.Series.Add(RadiatorRetur);
        TempChart.Series.Add(Duschrum);
        TempChart.Series.Add(Garage);
        TempChart.Series.Add(Kylrum);

        if (!IsPostBack)
        {
            LoadPeriodDropDownList(PeriodDropDownList);
            LoadData();
        }
    }

    private void DrawTempLine(string name, DateTime firstDate, int borderWidth, Color color)
    {
        var series = TempChart.Series[name];
        series.ChartArea = TempChart.ChartAreas[0].Name;
        series.ChartType = SeriesChartType.FastLine;
        series.BorderWidth = borderWidth;
        series.IsVisibleInLegend = true;
        series.ToolTip = name;
        series.Color = color;
        series.Points.Clear();
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            data.Temperature.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            var q = from r in data.Temperature
                    where r.name == name && r.time > firstDate
                    orderby r.time descending 
                    select r;
            foreach (var t in q)
            {
                series.Points.AddXY(t.time, t.value);
            }
        }        
    }

    protected override void LoadData()
    {
        TempChart.ChartAreas[0].AxisX.LabelStyle.Format = "";
        TempChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Auto;
        var borderWidth = 1;
        var firstDate = DateTime.Now.AddYears(-100);
        if (PeriodDropDownList.SelectedValue.Equals(LastDay))
        {
            firstDate = DateTime.Now.AddDays(-1.0);
            TempChart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            TempChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Hours;
            borderWidth = 1;
        }
        else if (PeriodDropDownList.SelectedValue.Equals(LastWeek))
        {
            firstDate = DateTime.Now.AddDays(-7.0);
        }
        else if (PeriodDropDownList.SelectedValue.Equals(LastYears))
        {
            firstDate = DateTime.Now.AddYears(-2);
            TempChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Months;
        }
        else
        {
            TempChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Years;
        }

        DrawTempLine(Utomhus, firstDate, borderWidth, Color.DarkGreen);
        DrawTempLine(RadiatorTur, firstDate, borderWidth, Color.Red);
        DrawTempLine(RadiatorRetur, firstDate, borderWidth, Color.Blue);
        DrawTempLine(Duschrum, firstDate, borderWidth, Color.SpringGreen);
        DrawTempLine(Garage, firstDate, borderWidth, Color.Yellow);
        DrawTempLine(Kylrum, firstDate, borderWidth, Color.Black);

        using (var data = new MeterLogModel.MeterLogEntities())
        {
            var q = (from r in data.Temperature
                     select r.time).Max();
            LastUpdatedLabel.Text = "Uppdaterad " + q.ToString();
        }
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            var q = (from r in data.Temperature
                     where r.name == Utomhus && r.time > firstDate
                     select r.value).Min();
            MinValueLabel.Text = " Min utomhus " + String.Format("{0:0.0}", q);
        }
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            var q = (from r in data.Temperature
                     where r.name == Utomhus && r.time > firstDate
                     select r.value).Max();
            MaxValueLabel.Text = " Max utomhus " + String.Format("{0:0.0}", q);
        }
    }
}