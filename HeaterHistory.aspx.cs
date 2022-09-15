using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

public partial class HeaterHistory : HistoryPage
{
    private const string Energy = "Energy";
    private const string Outside = "Utomhus";

    protected void Page_Load(object sender, EventArgs e)
    {
        CreateChartArea(HeaterChart, "Tid", "kWh");
        HeaterChart.Series.Add(Energy);

        var secSeries = HeaterChart.Series.Add(Outside);
        secSeries.YAxisType = AxisType.Secondary;
        HeaterChart.ChartAreas[0].AxisY2.Title = "Grader";

        if (!IsPostBack)
        {
            LoadPeriodDropDownList(PeriodDropDownList);
            LoadData();
        }
    }

    private double DrawEnergyLine(string name, DateTime firstDate, int borderWidth, Color color)
    {
        double total = 0;
        var series = HeaterChart.Series[name];
        series.ChartArea = HeaterChart.ChartAreas[0].Name;
        series.ChartType = SeriesChartType.Line; // Spline;
        series.BorderWidth = borderWidth;
        series.IsVisibleInLegend = true;
        series.ToolTip = name;
        series.Color = color;
        series.Points.Clear();
        double lastValue = 0;
        var lastTime = new DateTime();
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            data.HeaterMeter.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            var q = from r in data.HeaterMeter
                    where r.time > firstDate
                    select r;
            foreach (var t in q)
            {
                if (lastValue != 0)
                {
                    double oneHour = new TimeSpan(1, 0, 0).Ticks;
                    double timeSpan = (t.time - lastTime).Ticks;
                    double span = oneHour / timeSpan;
                    var diff = (t.energy - lastValue) * span;
                    series.Points.AddXY(t.time, diff);
                    total += diff;
                }
                lastValue = t.energy;
                lastTime = t.time;
            }
        }
        return total;
    }

    private double DrawEnergyLine2(string name, DateTime firstDate, int borderWidth, Color color)
    {
        double total = 0;
        var series = HeaterChart.Series[name];
        series.ChartArea = HeaterChart.ChartAreas[0].Name;
        series.ChartType = SeriesChartType.Spline; // Spline;
        series.BorderWidth = borderWidth;
        series.IsVisibleInLegend = true;
        series.ToolTip = name;
        series.Color = color;
        series.Points.Clear();
        double lastValue = -1;
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            data.HeaterMeter.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            var q = from r in data.HeaterMeter
                    where r.time > firstDate
                    select r;
            /*
            var yList = new List<double>();
            var xList = new List<double>();
            foreach (var t in q)
            {
                if (lastValue != 0)
                {
                    var diff = t.energy - lastValue;
                    yList.Add(diff);
                    xList.Add(t.time.Ticks);
                    total += diff;
                }
                lastValue = t.energy;
            }
            
            var points = interpolate(yList, xList, xList);
            int i = 0;
            foreach (var point in points)
            {
                series.Points.AddXY(new DateTime(Convert.ToInt64(xList[i++])), point);
            }
             * */
            /*
            DateTime? lastTime = null;
            foreach (var t in q)
            {
                if (lastValue != -1)
                {
                    var diff = t.energy - lastValue;
                    var middleTime = lastTime + (t.time - lastTime);
                    series.Points.AddXY(t.time, diff);
                    total += diff;
                }

                lastValue = t.energy;
                lastTime = t.time;
            }
            */
            var energyValues = new Dictionary<DateTime, double>();
            foreach (var t in q)
            {
                if (lastValue != -1)
                {
                    var diff = t.energy - lastValue;
                    energyValues.Add(t.time, diff);
                }

                lastValue = t.energy;
            }

            DateTime? lastTime = null;
            double lastValue1 = -1;
            double lastValue2 = -1;
            double lastValue3 = -1;
            var values = new double[3];
            values[0] = 0;
            values[1] = 0;
            values[2] = 0;
            int index = 0;
            foreach (var t in energyValues)
            {
                values[index++] = t.Value;
                if (index == 3) index = 0;

                if (values[0] > 0 && values[1] > 0 && values[2] > 0)
                {
                    var diff = (values[0] + values[1] + values[2]) / 3;
                    series.Points.AddXY(t.Key, diff);
                    total += diff;
                }
            }

        }
        return total;
    }

    public static List<double> interpolate(IList<double> xItems, IList<double> yItems, IList<double> breaks)
    {
        double[] interpolated = new double[breaks.Count];
        int id = 1;
        int x = 0;
        while (breaks[x] < xItems[0])
        {
            interpolated[x] = yItems[0];
            x++;
        }

        double p, w;
        // left border case - uphold the value 
        for (int i = x; i < breaks.Count; i++)
        {
            while (breaks[i] > xItems[id])
            {
                id++;
                if (id > xItems.Count - 1)
                {
                    id = xItems.Count - 1;
                    break;
                }
            }

            System.Diagnostics.Debug.WriteLine(string.Format("i: {0}, id {1}", i, id));

            if (id <= xItems.Count - 1)
            {
                if (id == xItems.Count - 1 && breaks[i] > xItems[id])
                {

                    interpolated[i] = yItems[yItems.Count - 1];
                }
                else
                {
                    w = xItems[id] - xItems[id - 1];
                    p = (breaks[i] - xItems[id - 1]) / w;
                    interpolated[i] = yItems[id - 1] + p * (yItems[id] - yItems[id - 1]);
                }
            }
            else // right border case - uphold the value 
            {
                interpolated[i] = yItems[yItems.Count - 1];
            }

        }
        return interpolated.ToList();

    } 

    private double DrawEnergyMonthLine(string name, DateTime firstDate, int borderWidth, Color color)
    {
        double total = 0;
        var series = HeaterChart.Series[name];
        series.ChartArea = HeaterChart.ChartAreas[0].Name;
        series.ChartType = SeriesChartType.Spline;
        series.BorderWidth = borderWidth;
        series.IsVisibleInLegend = true;
        series.ToolTip = name;
        series.Color = color;
        series.Points.Clear();
        double lastValue = 0;
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            data.HeaterMeterMonthly.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            var q = from r in data.HeaterMeterMonthly
                    where r.time > firstDate
                    select r;
            foreach (var t in q)
            {
                if (lastValue != 0)
                {
                    var diff = t.energy - lastValue;
                    series.Points.AddXY(t.time, diff);
                    total += diff;
                }
                lastValue = t.energy;
            }
        }
        return total;
    }

    private void DrawTempLine(string name, DateTime firstDate, int borderWidth, Color color)
    {
        var series = HeaterChart.Series[name];
        series.ChartArea = HeaterChart.ChartAreas[0].Name;
        series.ChartType = SeriesChartType.Line;
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
                    orderby r.time ascending
                    select r;
            foreach (var t in q)
            {
                series.Points.AddXY(t.time, t.value);
            }
        }
    }

    protected override void LoadData()
    {
        HeaterChart.ChartAreas[0].AxisX.LabelStyle.Format = "";
        HeaterChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Auto;
        var borderWidth = 1;
        var firstDate = DateTime.Now.AddYears(-100);
        if (PeriodDropDownList.SelectedValue.Equals(LastDay))
        {
            firstDate = DateTime.Now.AddDays(-1.0);
            HeaterChart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm";
            HeaterChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Hours;
            borderWidth = 2;
            TotalLabel.Text = "Totalt " + DrawEnergyLine(Energy, firstDate, borderWidth, Color.Crimson);
        }
        else if (PeriodDropDownList.SelectedValue.Equals(LastWeek))
        {
            firstDate = DateTime.Now.AddDays(-7.0);
            TotalLabel.Text = "Totalt " + DrawEnergyLine(Energy, firstDate, borderWidth, Color.Crimson);
        }
        else if (PeriodDropDownList.SelectedValue.Equals(LastYears))
        {
            firstDate = DateTime.Now.AddYears(-2);
            borderWidth = 2;
            TotalLabel.Text = "Totalt " + DrawEnergyMonthLine(Energy, firstDate, borderWidth, Color.Crimson);
            HeaterChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Months;
        }
        else
        {
            TotalLabel.Text = "Totalt " + DrawEnergyMonthLine(Energy, firstDate, borderWidth, Color.Crimson);
            HeaterChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Years;
        }
        DrawTempLine(Outside, firstDate, borderWidth, Color.DarkGreen);


        using (var data = new MeterLogModel.MeterLogEntities())
        {
            var q = (from r in data.HeaterMeter
                     select r.time).Max();
            LastUpdatedLabel.Text = "Uppdaterad " + q;
        }
    }
}