using Verse;

namespace ModManager;

public interface IUserData : IExposable
{
    public string FilePath { get; }

    public void Write();
}