public static string FormatSeconds(string seconds) => FormatSeconds(uint.TryParse(seconds, out var sec) ? sec : 0);

public static string FormatSeconds(uint seconds) => seconds switch
{
  < (uint)Duration.Minute => $"{seconds}s",
	< (uint)Duration.Hour => $"{seconds / (uint)Duration.Minute}m",
	< (uint)Duration.Day => $"{seconds / (uint)Duration.Hour}h",
	< (uint)Duration.Month => $"{seconds / (uint)Duration.Day} day{(seconds / (uint)Duration.Day == 1 ? string.Empty : "s")}",		
	< (uint)Duration.Year => $"{seconds / (uint)Duration.Month} month{(seconds / (uint)Duration.Month == 1 ? string.Empty : "s")}",	
	_ => $"{seconds / (uint)Duration.Year} year{(seconds / (uint)Duration.Year == 1 ? string.Empty : "s")}"
};

private enum Duration : uint
{
	Minute = 60,
	Hour = 3600,
	Day = 86400,
	Month = 2678400,
	Year = 31536000
}
