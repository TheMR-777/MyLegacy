using System;
using System.Collections.Generic;

namespace SLA_Remake;

public static class Reasons
{
	public static List<Models.Reason> ReasonsAll { get; } =
	[
		new(1, false, "Start of the Day"),
		new(2, false, "System Reboot"),
		new(3, false, "Break"),
		new(4, false, "Rest Room"),
		new(5, false, "Namaz"),
		new(6, false, "Lunch"),
		new(7, false, "Dinner"),
		new(8, false, "Late Leave"),
		new(9, false, "Short Leave"),
		new(10, false, "Half Leave"),
		new(11, true, "On Official Call / Duty"),
		new(12, true, "Filing / Paper Work"),
		new(13, true, "Official Outgoing"),
		new(14, true, "Official Gathering"),
		new(15, true, "Seminar / Training / Presentation / Lecture"),
		new(16, true, "Departmental Discussion"),
		new(17, true, "Formal Meeting"),
		new(18, true, "Informal Meeting"),
		new(19, true, "Other")
	];

	public static void SortReasons() => 
		ReasonsAll.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.OrdinalIgnoreCase));
}