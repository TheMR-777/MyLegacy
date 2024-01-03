using System.Linq;
using System;

namespace SLA_Remake.Models;

public class LogEntry
{
	// The arrangement of the following properties is crucial
	// as it used to maintain the compatibility with the API.

	public string UserID { get; private set; } = "1";
	public string UserName { get; private set; } = CrossUtility.CurrentUser();
	public string UserIP { get; private set; } = MacroUtility.IP.ToString();
	public string TimeStamp => _timeStamp.ToOADate().ToString(System.Globalization.CultureInfo.InvariantCulture);
	public string LogInTime => _timeStamp.ToString(EntryTimeFormat);
	public string LogOutTime => LogInTime;
	public string LogFlag_01 { get; private set; } = "1";
	public string Reason { get; private set; } = string.Empty;
	public string ReasonType { get; private set; } = string.Empty;
	public string ReasonID { get; private set; } = "0";
	public string UserMachineName { get; private set; } = Environment.MachineName;
	public string UserDisplayName { get; private set; } = "USER";
	public string LogFlag_02 { get; private set; } = "o";

	public static LogEntry CreateLogout() => new();

	public static LogEntry CreateLogin(Reason reason, string reasonDetail) => new()
	{
		LogFlag_01 = "0",
		Reason = RemoveSpecialCharacters(reason.RequiresMoreDetail
						? reasonDetail.Trim()
						: BackwardCompatibility.GetCompatibleReasonText(reason)),
		ReasonType = BackwardCompatibility.GetCompatibleReasonText(reason),
		ReasonID = reason.DatabaseID.ToString(System.Globalization.CultureInfo.InvariantCulture),
		LogFlag_02 = "i"
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
			ApprovedSymbols.Contains(c))
		.ToArray());

	private const string ApprovedSymbols = ".-_";
	private const string EntryTimeFormat = "HH:mm";
	private readonly DateTime _timeStamp = DateTime.Now;
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