using System.Globalization;
using System.Linq;
using System.Net;
using System;
using System.Collections.Generic;

namespace SLA_Remake.Models;

public class LogEntry
{
	public string UserId { get; set; }
	public string UserName { get; set; }
	public string UserIp { get; set; }
	public string LogDate { get; set; }
	public string LogInTime { get; set; }
	public string LogOutTime { get; set; }
	public string LogFlag { get; set; }
	public string Reason { get; set; }
	public string ReasonType { get; set; }
	public string ReasonId { get; set; }
	public string UserPCName { get; set; }
	public string UserDisplayName { get; set; }
	public string LogSide { get; set; }

	public static LogEntry Create(bool login = true, string reasonDetail = null, Reason reason = null) => new()
	{
		// Constants
		// ---------

		UserId = "1",
		UserName = CrossUtility.GetCurrentUser(),
		UserIp = Utility.IP.ToString(),
		LogDate = DateTime.Now.Date.ToOADate().ToString(CultureInfo.InvariantCulture),
		UserPCName = Dns.GetHostName(),
		UserDisplayName = "USER",

		// Variables
		// ---------

		LogInTime = login
			? DateTime.Now.ToString("HH:mm")
			: null,
		LogOutTime = login
			? null
			: DateTime.Now.ToString("HH:mm"),
		LogFlag = login
			? "0"
			: "1",
		Reason = login
			? RemoveSpecialCharacters(reason.RequiresMoreDetail
				? reasonDetail?.Trim()
				: BackwardCompatibility.GetCompatibleReasonText(reason))
			: null,
		ReasonType = login
			? BackwardCompatibility.GetCompatibleReasonText(reason)
			: null,
		ReasonId = login
			? reason.DatabaseID.ToString(CultureInfo.InvariantCulture)
			: "0",
		LogSide = login
			? "i"
			: "o"
	};

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

	private static readonly Dictionary<uint, string> OldReasons = new()
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