namespace ModManager;

public class ModIdentifier
{
    public ModIdentifier()
    {
    }

    public ModIdentifier(string id, string name, string steamWorkshopId)
    {
        Id = id;
        Name = name;
        SteamWorkshopId = steamWorkshopId;
    }

    public string Id { get; }
    public string Name { get; }
    public string SteamWorkshopId { get; }
}