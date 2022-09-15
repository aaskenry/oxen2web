using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    private static string _estateServiceUrl = "";
    private delegate void SetTextInUiDeleg(string text);
    private delegate void UpdateTextCallback(string text);
    private static readonly object Locker = new object();
    private static Task _task1;
    private static Task _task2;
    private static Task _task3;
    private static string _lastUpdated;
    private static string _utomhusTempValue = "Connecting...";
    private static string _utomhusTempError;
    private static string _radiatorTurValue = "Connecting...";
    private static string _radiatorTurError;
    private static string _radiatorReturValue = "Connecting...";
    private static string _radiatorReturError;
    private static string _waterMeterValue = "Connecting...";
    private static string _waterMeterError;
    private static string _electricMeterValue = "Connecting...";
    private static string _electricMeterPowerP1Value = "Connecting...";
    private static string _electricMeterPowerP2Value = "Connecting...";
    private static string _electricMeterPowerP3Value = "Connecting...";
    private static string _electricMeterCurrentP1Value = "Connecting...";
    private static string _electricMeterCurrentP2Value = "Connecting...";
    private static string _electricMeterCurrentP3Value = "Connecting...";
    private static string _electricMeterVoltageP1Value = "Connecting...";
    private static string _electricMeterVoltageP2Value = "Connecting...";
    private static string _electricMeterVoltageP3Value = "Connecting...";
    private static string _electricMeterError;
    private static string _heaterMeterEnergy = "Connecting...";
    private static string _heaterMeterPower = "Connecting...";
    private static string _heaterMeterTempIn = "Connecting...";
    private static string _heaterMeterTempOut = "Connecting...";
    private static string _heaterMeterTempDiff = "Connecting...";
    private static string _heaterMeterError;
    private static int _tickCount;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            _estateServiceUrl = ConfigurationManager.AppSettings["EstateServiceUrl"];
            LoadTempSensorList(TempSensorList);
            UtomhusLabel.Text = "Connecting...";
            RadiatorTurLabel.Text = "Connecting...";
            RadiatorReturLabel.Text = "Connecting...";
            FjarrTotaltLabel.Text = "Connecting...";
            FjarrAktuellLabel.Text = "Connecting...";
            FjarrTempInLabel.Text = "Connecting...";
            FjarrTempUtLabel.Text = "Connecting...";
            FjarrDeltaLabel.Text = "Connecting...";
            ElTotalLabel.Text = "Connecting...";
            ElFas1Label.Text = "Connecting...";
            ElFas2Label.Text = "Connecting...";
            ElFas3Label.Text = "Connecting...";
            VattenLabel.Text = "Connecting...";

            _tickCount = 300;
        }
    }

    protected void PollTimerTick(object sender, EventArgs e)
    {
        if (_tickCount < 0)
        {
            LastUpdatedLabel.Text = "Uppdatering avslutad";
        }
        else
        {
            if (_task1 == null || (_task1 != null && _task1.IsCompleted))
            {
                _task1 = Task.Factory.StartNew(Update1);
            }
            if (_task2 == null || (_task2 != null && _task2.IsCompleted))
            {
                _task2 = Task.Factory.StartNew(Update2);
            }
            if (_task3 == null || (_task3 != null && _task3.IsCompleted))
            {
                _task3 = Task.Factory.StartNew(Update3);
            }
            UpdateLabels();
            _tickCount--;
        }
    }

    private void UpdateLabels()
    {
        UtomhusLabel.Text = _utomhusTempValue;
        UtomhusErrorLabel.Text = _utomhusTempError;
        RadiatorTurLabel.Text = _radiatorTurValue;
        RadiatorTurErrorLabel.Text = _radiatorTurError;
        RadiatorReturLabel.Text = _radiatorReturValue;
        RadiatorReturErrorLabel.Text = _radiatorReturError;
        VattenLabel.Text = _waterMeterValue;
        VattenErrorLabel.Text = _waterMeterError;
        ElTotalLabel.Text = _electricMeterValue;
        ElTotalErrorLabel.Text = _electricMeterError;
        ElFas1Label.Text = _electricMeterPowerP1Value + " kW (" + _electricMeterCurrentP1Value + " A, " + _electricMeterVoltageP1Value + " V)";
        ElFas2Label.Text = _electricMeterPowerP2Value + " kW (" + _electricMeterCurrentP2Value + " A, " + _electricMeterVoltageP2Value + " V)";
        ElFas3Label.Text = _electricMeterPowerP3Value + " kW (" + _electricMeterCurrentP3Value + " A, " + _electricMeterVoltageP3Value + " V)";
        ElFas1ErrorLabel.Text = _electricMeterError;
        ElFas2ErrorLabel.Text = _electricMeterError;
        ElFas3ErrorLabel.Text = _electricMeterError;
        FjarrTotaltLabel.Text = _heaterMeterEnergy;
        FjarrAktuellLabel.Text = _heaterMeterPower;
        FjarrTempInLabel.Text = _heaterMeterTempIn;
        FjarrTempUtLabel.Text = _heaterMeterTempOut;
        FjarrDeltaLabel.Text = _heaterMeterTempDiff;
        FjarrTotaltErrorLabel.Text = _heaterMeterError;

        LastUpdatedLabel.Text = _lastUpdated;
    }

    private void LoadTempSensorList(DropDownList list)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(_estateServiceUrl + "/Meters/TemperatureSensor/");
            request.Method = "GET";
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseBody = reader.ReadToEnd();

                    var jss = new JavaScriptSerializer();
                    var sensorList = jss.Deserialize<dynamic>(responseBody);
                    foreach (Dictionary<string, object> sensor in sensorList)
                    {
                        var type = (string)sensor["Type"];
                        if (type == "Temperature")
                        {
                            var name = (string) sensor["Name"];
                            var id = (int) sensor["Id"];

                            list.Items.Add(new ListItem(name, id.ToString()));
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
        }
    }

    private void Update1()
    {
        lock (Locker)
        {
            UpdateTemperatures();
            UpdateWaterMeterData();
            _lastUpdated = "Uppdaterad: " + DateTime.Now.ToLongTimeString();
        }
    }
 
    private void Update2()
    {
        lock (Locker)
        {
            UpdatePowerMeterData();
            _lastUpdated = "Uppdaterad: " + DateTime.Now.ToLongTimeString();
        }
    }

    private void Update3()
    {
        lock (Locker)
        {
            UpdateHeaterMeterData();
            _lastUpdated = "Uppdaterad: " + DateTime.Now.ToLongTimeString();
        }
    }

    private void UpdateTemperatures()
    {
        var jss = new JavaScriptSerializer();
        jss.RegisterConverters(new JavaScriptConverter[] { new DynamicJson.DynamicJsonConverter() });

        try
        {
            var request = (HttpWebRequest)WebRequest.Create(_estateServiceUrl + "/Meters/TemperatureSensor/763668/");
            request.Method = "GET";
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseBody = reader.ReadToEnd();

                    var data = jss.Deserialize(responseBody, typeof(object)) as dynamic;
                    _utomhusTempValue = Convert.ToString((double)data.Value);
                    _utomhusTempError = "";
                }
            }
        }
        catch (Exception ex)
        {
            _utomhusTempError = ex.Message;
        }
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(_estateServiceUrl + "/Meters/TemperatureSensor/2054676/");
            request.Method = "GET";
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseBody = reader.ReadToEnd();

                    var data = jss.Deserialize(responseBody, typeof(object)) as dynamic;
                    _radiatorTurValue = Convert.ToString((double)data.Value);
                    _radiatorTurError = "";
                }
            }
        }
        catch (Exception ex)
        {
            _radiatorTurError = ex.Message;
        }
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(_estateServiceUrl + "/Meters/TemperatureSensor/2054932/");
            request.Method = "GET";
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseBody = reader.ReadToEnd();

                    var data = jss.Deserialize(responseBody, typeof(object)) as dynamic;
                    _radiatorReturValue = Convert.ToString((double)data.Value);
                    _radiatorReturError = "";
                }
            }
        }
        catch (Exception ex)
        {
            _radiatorReturError = ex.Message;
        }
    }

    void UpdateHeaterMeterData()
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(_estateServiceUrl + "/Meters/HeaterMeter/GeneralData/");
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
                    _heaterMeterEnergy = Convert.ToString((int)data.Energy);
                    _heaterMeterPower = Convert.ToString((double)data.Power);
                    _heaterMeterTempIn = Convert.ToString((double)data.TempIn);
                    _heaterMeterTempOut = Convert.ToString((double)data.TempOut);
                    _heaterMeterTempDiff = Convert.ToString((double)data.TempDiff);
                    _heaterMeterError = "";
                }
            }
        }
        catch (Exception ex)
        {
            _heaterMeterError = ex.Message;
        }
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
                    _electricMeterValue = String.Format("{0:0.0000}", (double)data.Energy);
                    _electricMeterPowerP1Value = String.Format("{0:0.0000}", (double)data.Power.P1);
                    _electricMeterPowerP2Value = String.Format("{0:0.0000}", (double)data.Power.P2);
                    _electricMeterPowerP3Value = String.Format("{0:0.0000}", (double)data.Power.P3);
                    _electricMeterCurrentP1Value = String.Format("{0:0.00}", (double)data.Current.P1);
                    _electricMeterCurrentP2Value = String.Format("{0:0.00}", (double)data.Current.P2);
                    _electricMeterCurrentP3Value = String.Format("{0:0.00}", (double)data.Current.P3);
                    _electricMeterVoltageP1Value = String.Format("{0:0}", (double)data.Voltage.P1);
                    _electricMeterVoltageP2Value = String.Format("{0:0}", (double)data.Voltage.P2);
                    _electricMeterVoltageP3Value = String.Format("{0:0}", (double)data.Voltage.P3);
                    _electricMeterError = "";
                }
            }
        }
        catch (Exception ex)
        {
            _electricMeterError = ex.Message;
        }
    }

    void UpdateWaterMeterData()
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(_estateServiceUrl + "/Meters/WaterMeter/GeneralData/");
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
                    _waterMeterValue = Convert.ToString((int)data.Volume);
                    _waterMeterError = "";
                }
            }
        }
        catch (Exception ex)
        {
            _waterMeterError = ex.Message;
        }
    }

    void TestJSON()
    {
        var reponseAsString = "";
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(_estateServiceUrl + "/Meters/HeaterMeter/GeneralData/");
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
                    reponseAsString = Convert.ToString((int)data.Energy);
                }
            }
        }
        catch (Exception ex)
        {
            reponseAsString = ex.Message;
        }

        ElTotalLabel.Text = reponseAsString;
    }
    
    void SetBody(HttpWebRequest request, string requestBody)
    {
        if (requestBody.Length > 0)
        {
            using (Stream requestStream = request.GetRequestStream())
            using (StreamWriter writer = new StreamWriter(requestStream))
            {
                writer.Write(requestBody);
            }
        }
    }
}
