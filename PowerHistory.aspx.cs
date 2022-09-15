using System;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;

public partial class PowerHistory : HistoryPage
{
    private const string El = "El";

    protected void Page_Load(object sender, EventArgs e)
    {
        CreateChartArea(ElChart, "Tid", "Watt");
        ElChart.Series.Add(El);

        if (!IsPostBack)
        {
            LoadPeriodDropDownList(PeriodDropDownList);
            LoadData();
        }
    }

    private double DrawElLine(DateTime firstDate, int borderWidth)
    {
        double total = 0;
        var series = ElChart.Series[El];
        series.ChartArea = ElChart.ChartAreas[0].Name;
        series.ChartType = SeriesChartType.Line;
        series.BorderWidth = borderWidth;
        series.IsVisibleInLegend = true;
        series.ToolTip = "El";
        series.Points.Clear();
        ElChart.ChartAreas[0].AxisY.Title = "kWh";
        double lastValue = 0.0;
        var lastTime = new DateTime();
//        double lastValue = 42790.64;
//        var newData = new DateTime(2012, 9, 9, 19, 0, 0);
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            data.ElectricMeter.MergeOption = System.Data.Objects.MergeOption.NoTracking;

            var q = from r in data.ElectricMeter
                    where r.time > firstDate
                    select r;
            foreach (var t in q)
            {
                if (lastValue != 0)
                {
                    double oneHour = new TimeSpan(1, 0, 0).Ticks;
                    double timeSpan = (t.time - lastTime).Ticks;
                    double span = oneHour/timeSpan;
                    var diff = (t.value - lastValue) * span;
                    series.Points.AddXY(t.time, diff);
                    total += diff;
                }
                lastValue = t.value;
                lastTime = t.time;
            }

            /*var q = from r in data.ElectricMeter
                    where r.time > firstDate && r.time < newData
                    select r;
            foreach (var t in q)
            {
                series.Points.AddXY(t.time, t.value);
                total += t.value;
            }
            var q = from r in data.ElectricMeter
                    where r.time > firstDate && r.time > newData
                    select r;
            foreach (var t in q)
            {
                if (lastValue != 0)
                {
                    var diff = (t.value - lastValue)*12.0;
                    series.Points.AddXY(t.time, diff);
                    total += diff;
                }
                lastValue = t.value;
            }
             * */
        }
        return total;
    }

    private double DrawElDailyLine(DateTime firstDate, int borderWidth)
    {
        double total = 0;
        var series = ElChart.Series[El];
        series.ChartArea = ElChart.ChartAreas[0].Name;
        series.ChartType = SeriesChartType.Line;
        series.BorderWidth = borderWidth;
        series.IsVisibleInLegend = true;
        series.ToolTip = "El";
        series.Points.Clear();
        ElChart.ChartAreas[0].AxisY.Title = "kWh";
        double lastValue = 0;
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            data.ElectricMeter.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            var q = from r in data.ElectricMeterDaily
                    where r.time > firstDate
                    orderby r.time ascending
                    select r;
            foreach (var t in q)
            {
                if (lastValue != 0)
                {
                    var diff = (t.value - lastValue);
                    series.Points.AddXY(t.time, diff);
                    total += diff;
                }
                lastValue = t.value;
            }
        }
        return total;
    }

    private double DrawElMonthlyLine(DateTime firstDate, int borderWidth)
    {
        double total = 0;
        var series = ElChart.Series[El];
        series.ChartArea = ElChart.ChartAreas[0].Name;
        series.ChartType = SeriesChartType.Spline;
        series.BorderWidth = borderWidth;
        series.IsVisibleInLegend = true;
        series.ToolTip = "El";
        series.Points.Clear();
        ElChart.ChartAreas[0].AxisY.Title = "kWh";
        double lastValue = 0;
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            data.ElectricMeter.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            var q = from r in data.ElectricMeterMonthly
                    where r.time > firstDate
                    orderby r.time ascending
                    select r;
            foreach (var t in q)
            {
                if (lastValue != 0)
                {
                    var diff = (t.value - lastValue);
                    series.Points.AddXY(t.time, diff);
                    total += diff;
                }
                lastValue = t.value;
            }
        }
        return total;
    }

    protected override void LoadData()
    {
        ElChart.ChartAreas[0].AxisX.LabelStyle.Format = "";
        ElChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Auto;
        var borderWidth = 1;
        var firstDate = DateTime.Now.AddYears(-100);
        var period = "daily";
        if (PeriodDropDownList.SelectedValue.Equals(LastDay))
        {
            firstDate = DateTime.Now.AddDays(-1.0);
            ElChart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            ElChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Hours;
            borderWidth = 1;
            period = "minutes";
            TotalLabel.Text = "Totalt " + DrawElLine(firstDate, borderWidth);
        }
        else if (PeriodDropDownList.SelectedValue.Equals(LastWeek))
        {
            firstDate = DateTime.Now.AddDays(-7.0);
            TotalLabel.Text = "Totalt " + DrawElDailyLine(firstDate, borderWidth);
        }
        else
        {
            ElChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Months; 
            TotalLabel.Text = "Totalt " + DrawElMonthlyLine(firstDate, borderWidth);
        }

        using (var data = new MeterLogModel.MeterLogEntities())
        {
            var q = (from r in data.ElectricMeter
                     select r.time).Max();
            LastUpdatedLabel.Text = "Uppdaterad " + q;
        }
        if (ElChart.Series[El].Points.Count > 0)
        {
            using (var data = new MeterLogModel.MeterLogEntities())
            {
                var q = (from r in data.ElectricMeter
                         where r.time > firstDate
                         select r.value).Min();
                MinValueLabel.Text = " Min " + String.Format("{0:0.0}", q);
            }
            using (var data = new MeterLogModel.MeterLogEntities())
            {
                var q = (from r in data.ElectricMeter
                         where r.time > firstDate
                         select r.value).Max();
                MaxValueLabel.Text = " Max " + String.Format("{0:0.0}", q);
            }
        }
    }
}