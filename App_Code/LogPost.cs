using System;
using System.Linq;

/// <summary>
/// Summary description for LogPost
/// </summary>
public class LogPost
{
	public LogPost()
	{
		//
		// TODO: Add constructor logic here
		//
	}

    public DateTime LastUpdated()
    {
        using (var data = new MeterLogModel.MeterLogEntities())
        {
            return (from r in data.Temperature
                   select r.time).Max();
        }
    }
}