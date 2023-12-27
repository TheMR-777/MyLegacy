namespace SLA_Remake.Models;

public class Reason(uint databaseID, bool requiresMoreDetail, string name)
{
    public uint DatabaseID { get; } = databaseID;
    public bool RequiresMoreDetail { get; } = requiresMoreDetail;
    public string Name { get; } = name;
}
