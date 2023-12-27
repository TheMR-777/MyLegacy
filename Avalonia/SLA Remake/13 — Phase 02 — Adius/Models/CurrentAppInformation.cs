namespace SLA_Remake.Models;

public class CurrentAppInformation(uint pid, string title, string name)
{
	public uint ProcessSerial { get; set; } = pid;
	public string WindowTitle { get; set; } = title;
	public string ProcessName { get; set; } = name;
}
