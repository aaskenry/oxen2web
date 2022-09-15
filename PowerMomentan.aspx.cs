using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Web.UI.DataVisualization.Charting;

public partial class PowerHistory : ChartPage
{
    private const string CurrentP1 = "Fas1";
    private const string CurrentP2 = "Fas2";
    private const string CurrentP3 = "Fas3";
    private static string _estateServiceUrl = "";
    private static double _totalEnergy;
    private static int _tickCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CreateChartArea(ElChart, "Tid", "Ampere");
            ElChart.Series.Add(CurrentP1);
            ElChart.Series.Add(CurrentP2);
            ElChart.Series.Add(CurrentP3);

            ElChart.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            ElChart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Minutes;
            ElChart.ChartAreas[0].AxisX.Interval = 1;

            var minValue = DateTime.Now; 
            var maxValue = minValue.AddMinutes(5);
            ElChart.ChartAreas[0].AxisX.Minimum = minValue.ToOADate();
            ElChart.ChartAreas[0].AxisX.Maximum = maxValue.ToOADate();

            _estateServiceUrl = ConfigurationManager.AppSettings["EstateServiceUrl"];

            var series = ElChart.Series[CurrentP1];
            series.ChartArea = ElChart.ChartAreas[0].Name;
            series.ChartType = SeriesChartType.Line;
            series.XValueType = ChartValueType.Auto; 
            series.BorderWidth = 2;
            series.IsVisibleInLegend = true;
            series.ToolTip = "Fas 1";
            series.Points.Clear();

            series = ElChart.Series[CurrentP2];
            series.ChartArea = ElChart.ChartAreas[0].Name;
            series.ChartType = SeriesChartType.Line;
            series.XValueType = ChartValueType.Auto;
            series.BorderWidth = 2;
            series.IsVisibleInLegend = true;
            series.ToolTip = "Fas 2";
            series.Points.Clear();

            series = ElChart.Series[CurrentP3];
            series.ChartArea = ElChart.ChartAreas[0].Name;
            series.ChartType = SeriesChartType.Line;
            series.XValueType = ChartValueType.Auto;
            series.BorderWidth = 2;
            series.IsVisibleInLegend = true;
            series.ToolTip = "Fas 3";
            series.Points.Clear();

            ElChart.ChartAreas[0].AxisY.Title = "Ampere";

            _tickCount = 300;
        }
    }

    protected void PollTimerTick(object sender, EventArgs e)
    {
        if (_tickCount < 0)
        {
            LastUpdatedLabel.Text = "Uppdatering avslutad";

            PollTimer.Enabled = false;
        }
        else
        {
            UpdateChart();
            _tickCount--;
        }
    }

    private void UpdateChart()
    {
        UpdatePowerMeterData();
        LastUpdatedLabel.Text = "Uppdaterad: " + DateTime.Now.ToLongTimeString() + " (Total effekt = " + String.Format("{0:0.000}", _totalEnergy) + " kW)";
    }

    void UpdatePowerMeterData()
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(_estateServiceUrl + "/Meters/PowerMeter/ForcedGeneralData/");
            request.Method = "GET";
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseBody = reader.ReadToEnd();

                    var jss = new JavaScriptSerializer();
                    jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJson.DynamicJsonConverter() });
                    var data = jss.Deserialize(responseBody, typeof(object)) as dynamic;
                    var series = ElChart.Series[CurrentP1];
                    series.Points.AddXY(DateTime.Now, (double)data.Current.P1);
                    series = ElChart.Series[CurrentP2];
                    series.Points.AddXY(DateTime.Now, (double)data.Current.P2);
                    series = ElChart.Series[CurrentP3];
                    series.Points.AddXY(DateTime.Now, (double)data.Current.P3);

                    _totalEnergy = (double)data.Power.P1 + (double)data.Power.P2 + (double)data.Power.P3;
                }
            }

            if (DateTime.Now > DateTime.FromOADate(ElChart.ChartAreas[0].AxisX.Maximum))
            {
                var minValue = DateTime.FromOADate(ElChart.ChartAreas[0].AxisX.Minimum).AddMinutes(1);
                var maxValue = minValue.AddMinutes(5);
                ElChart.ChartAreas[0].AxisX.Minimum = minValue.ToOADate();
                ElChart.ChartAreas[0].AxisX.Maximum = maxValue.ToOADate();

                // Remove points from the left chart side
                while (ElChart.Series[CurrentP1].Points[0].XValue < minValue.ToOADate())
                {
                    // Remove series points
                    foreach (Series series in ElChart.Series)
                    {
                        series.Points.RemoveAt(0);
                    }

                }
            }
        }
        catch (Exception ex)
        {
            // Ignore
        }
    }

    void UpdatePowerMeterDataOld()
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(_estateServiceUrl + "/Meters/PowerMeter/RegisterData/");
            request.Method = "GET";
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseBody = reader.ReadToEnd();

                    var jss = new JavaScriptSerializer();
                    jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJson.DynamicJsonConverter() });
                    var data = jss.Deserialize(responseBody, typeof(object)) as dynamic;
                    var series = ElChart.Series[CurrentP1];
                    series.Points.AddXY(DateTime.Now, (double)data.Current.P1);
                    series = ElChart.Series[CurrentP2];
                    series.Points.AddXY(DateTime.Now, (double)data.Current.P2);
                    series = ElChart.Series[CurrentP3];
                    series.Points.AddXY(DateTime.Now, (double)data.Current.P3);

                    _totalEnergy = (double)data.Power.P1 + (double)data.Power.P2 + (double)data.Power.P3;
                }
            }

            if (DateTime.Now > DateTime.FromOADate(ElChart.ChartAreas[0].AxisX.Maximum))
            {
                var minValue = DateTime.FromOADate(ElChart.ChartAreas[0].AxisX.Minimum).AddMinutes(1);
                var maxValue = minValue.AddMinutes(5);
                ElChart.ChartAreas[0].AxisX.Minimum = minValue.ToOADate();
                ElChart.ChartAreas[0].AxisX.Maximum = maxValue.ToOADate();

                // Remove points from the left chart side
                while (ElChart.Series[CurrentP1].Points[0].XValue < minValue.ToOADate())
                {
                    // Remove series points
                    foreach (Series series in ElChart.Series)
                    {
                        series.Points.RemoveAt(0);
                    }

                }
            }
        }
        catch (Exception ex)
        {
            // Ignore
        }
    }
}