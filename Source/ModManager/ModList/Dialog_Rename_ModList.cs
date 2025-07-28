using System.IO;
using Verse;

namespace ModManager;

public class Dialog_Rename_ModList : Dialog_Rename<ModList>
{
    private readonly ModList list;

    public Dialog_Rename_ModList(ModList list) : base(list)
    {
        this.list = list;
        curName = list.Name;
        absorbInputAroundWindow = true;
    }

    protected override AcceptanceReport NameIsValid(string name)
    {
        // any name given?
        if (name.Length < 1)
        {
            return I18n.NameTooShort;
        }

        // check invalid characters
        var invalidChars = Path.GetInvalidFileNameChars();
        foreach (var invalidChar in invalidChars)
        {
            if (name.Contains(invalidChar))
            {
                return I18n.InvalidName(name, new string(invalidChars));
            }
        }

        return true;
    }
}