using System;
using System.IO;
using UnityEngine;

public class CrashReporter : MonoBehaviour
{
	private static readonly ILogger _log = LogBuilder.Instance.GetLogger(typeof(CrashReporter), LogLevel.Debug);

	private void Start()
	{
		_log.LogDebug("Checking for crash reports.");
		CrashReport[] reports = CrashReport.reports;
		if (reports.Length > 0)
		{
			_log.Log("Found {0} crash reports.", reports.Length);
			CrashReport[] array = reports;
			foreach (CrashReport crashReport in array)
			{
				string text = string.Format("{0}.unityCrash", crashReport.time.ToString("yyyy-MM-dd_hh-mm-ss"));
				string text2 = Application.persistentDataPath + "/" + text;
				_log.Log("Writing report from {0} to file '{1}': {2}", crashReport.time, text2, crashReport.text);
				try
				{
					File.WriteAllText(text2, string.Format("{0}\n--\n{1}", crashReport.time, crashReport.text));
					crashReport.Remove();
				}
				catch (Exception ex)
				{
					_log.LogError("Failed to write crash report to file '{0}':{1}", text2, ex);
				}
			}
		}
		else
		{
			_log.LogDebug("No crash reports found.");
		}
	}
}
