﻿using Harmony;
using System.Collections.Generic;
using UnityEngine;
using Verse;


namespace Hotkeys
{
    [HarmonyPatch(typeof(Game), nameof(Game.UpdatePlay))]
    public class Patch_DirectHotkeys
    {
        static void Postfix()
        {
            var settings = LoadedModManager.GetMod<Hotkeys>().GetSettings<Hotkeys_Settings>();
            var saved = LoadedModManager.GetMod<Hotkeys_Save>().GetSettings<Hotkeys_SettingsSave>();

            if (Event.current.type != EventType.KeyDown) { return; }
            if (!settings.useDirectHotkeys) { return; }

            List<KeyBindingDef> designatorKeys = DefDatabase<KeyBindingDef>.AllDefsListForReading.FindAll(x => x.category == DefDatabase<KeyBindingCategoryDef>.GetNamed("DirectHotkeys"));

            for (int i = 0; i < designatorKeys.Count; i++)
            {
                if (designatorKeys[i].JustPressed)
                {
                    var designator = saved.GetDesignator(i);
                    if (designator != null)
                    {
                        Find.DesignatorManager.Select(designator);
                    }
                }
            }
        }
    }
}

