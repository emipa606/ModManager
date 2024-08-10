// MainMenuDrawer_DoMainMenuControls.cs
// Copyright Karel Kroeze, 2018-2018

using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace ModManager;

[HarmonyPatch(typeof(MainMenuDrawer), nameof(MainMenuDrawer.DoMainMenuControls))]
public static class MainMenuDrawer_DoMainMenuControls
{
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        /***
         * DoMainMenuControls builds a List of ListableOptions for the main menu buttons.
         * The problem with this is that they are called with a delegate, which is compiled
         * into a compiler generated method.
         *
         * Patching that is tricky, so we'll replace it with our own method.
         *
         * The logic;
         *
         * Search instructions for the string "Mods"
         * replace the target of the first ldftn instruction with our own Action, so that it
         * gets cached instead of the compiler generated delegate.
         */

        var modsFound = false;
        var done = false;

        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldstr && (string)instruction.operand == "Mods")
            {
                modsFound = true;
            }

            if (modsFound && !done && instruction.opcode == OpCodes.Ldftn)
            {
                instruction.operand = AccessTools.Method(typeof(MainMenuDrawer_DoMainMenuControls), "OpenModsConfig");
                done = true;
            }

            yield return instruction;
        }
    }

    public static void OpenModsConfig()
    {
        Find.WindowStack.Add(new Page_BetterModConfig());
    }
}