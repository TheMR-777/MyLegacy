using System.Linq;
using System;

namespace SLA_Remake.Models;

public class LogEntry
{
	public string UserID { get; set; }
	public string UserName { get; set; }
	public string UserIP { get; set; }
	public string TimeStamp { get; set; }
	public string LogInTime { get; set; }
	public string LogOutTime { get; set; }
	public string LogFlag { get; set; }
	public string Reason { get; set; }
	public string ReasonType { get; set; }
	public string ReasonID { get; set; }
	public string UserPCName { get; set; }
	public string UserDisplayName { get; set; }
	public string FlagForAPI { get; set; }

	public static LogEntry Create(bool login = true, string reasonDetail = Configuration.NotAvailableOrFound, Reason reason = null) => new()
	{
		// Constants
		// ---------

		UserID = "1",
		UserName = CrossUtility.CurrentUser(),
		UserIP = MacroUtility.IP.ToString(),
		TimeStamp = DateTime.Now.ToOADate().ToString(System.Globalization.CultureInfo.InvariantCulture),
		UserPCName = Environment.MachineName,
		UserDisplayName = "USER",

		// Variables
		// ---------

		LogInTime = login
			? DateTime.Now.ToString("HH:mm")
			: string.Empty,
		LogOutTime = login
			? string.Empty
			: DateTime.Now.ToString("HH:mm"),
		LogFlag = login
			? "0"
			: "1",
		Reason = login
			? RemoveSpecialCharacters(reason!.RequiresMoreDetail
				? reasonDetail.Trim()
				: BackwardCompatibility.GetCompatibleReasonText(reason))
			: string.Empty,
		ReasonType = login
			? BackwardCompatibility.GetCompatibleReasonText(reason)
			: string.Empty,
		ReasonID = login
			? reason.DatabaseID.ToString(System.Globalization.CultureInfo.InvariantCulture)
			: "0",
		FlagForAPI = login
			? "i"
			: "o"
	};

	public static string Serialize(System.Collections.Generic.IEnumerable<LogEntry> entries)
	{
		// The Serialization is being done
		// as according to the legacy SLA,
		// to maintain the API-consistency

		var formatted = entries.Select(entry =>
		{
			var properties = entry.GetType().GetProperties();
			var values = properties.Select(property => property.GetValue(entry, null));
			return string.Join("|", values);
		});

		var now = DateTime.Now;
		var req = new
		{
			UserName = CrossUtility.CurrentUser() + '~' + Environment.MachineName,
			logDate = now.Date.ToString("dd/MM/yyyy HH:mm:ss") + "~WQoCW/gL8O/+pi0RP2l6xg==",
			LogDateTimeStamp = now.ToString("dd/MM/yyyy HH:mm:ss"),
			version = Configuration.ApplicationsVersion,
			data = formatted
		};

		return System.Text.Json.JsonSerializer.Serialize(req, WebAPI.OptionsJSON);
	}

	private static string RemoveSpecialCharacters(string str) =>
		new(str.Where(c =>
			char.IsLetterOrDigit(c) ||
			char.IsWhiteSpace(c) ||
			allowedCharacters.Contains(c))
		.ToArray());

	private const string allowedCharacters = ".-_";
}

public static class BackwardCompatibility
{
	// This class contains legacy structures.
	// These are mapped to the new structure.

	private static readonly System.Collections.Generic.Dictionary<uint, string> OldReasons = new()
	{
		// The Dictionary is being used, to maintain the order (of ID) of the Reasons.
		// The order is crucial, as it is by the order of IDs located in the Database.

		{ 1, "Day Start" },
		{ 2, "System Restart/Shutdown" },
		{ 3, "Break" },
		{ 4, "Rest Room" },
		{ 5, "Namaz" },
		{ 6, "Lunch" },
		{ 7, "Dinner" },
		{ 8, "Late Leave" },
		{ 9, "Short Leave" },
		{ 10, "Half Leave" },
		{ 11, "On Official Call / On Seat" },
		{ 12, "Filling / Paper Work" },
		{ 13, "Official Outgoing" },
		{ 14, "Company /Departmental Gathering" },
		{ 15, "Seminar/Lecture/Presentation/Training" },
		{ 16, "Departmental Discussion (Purpose)" },
		{ 17, "Formal Meeting" },
		{ 18, "Informal Meeting" },
		{ 19, "Other" }
	};

	public static string GetCompatibleReasonText(Reason reason)
	{
		return OldReasons.TryGetValue(reason.DatabaseID, out var text) ? text : reason.Name;
	}
}