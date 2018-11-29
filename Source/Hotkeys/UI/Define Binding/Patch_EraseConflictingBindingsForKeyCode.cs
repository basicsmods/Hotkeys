﻿using Harmony;
using Verse;
using UnityEngine;
using System;

namespace Hotkeys
{
    // Detour to change how conflicting keybindings are handled.
    [HarmonyPatch(typeof(KeyPrefsData), nameof(KeyPrefsData.EraseConflictingBindingsForKeyCode))]
    public class Patch_EraseConflictingBindingsForKeyCode
    {
        static bool Prefix(KeyPrefsData __instance, KeyBindingDef keyDef, KeyCode keyCode, Action<KeyBindingDef> callBackOnErase = null)
        {
            if (!Hotkeys.settings.useMultiKeys) { return true; }

            foreach (KeyBindingDef keyBindingDef in BindingConflicts.ConflictingBindings(keyDef, keyCode, __instance))
            {
                KeyBindingData keyBindingData = __instance.keyPrefs[keyBindingDef];
                if (keyBindingData.keyBindingA == keyCode)
                {
                    keyBindingData.keyBindingA = KeyCode.None;
                    HotkeysPatch_KeyBindDrawing.ResetModifierList(KeyPrefs.BindingSlot.A, keyBindingDef);
                }
                if (keyBindingData.keyBindingB == keyCode)
                {
                    keyBindingData.keyBindingB = KeyCode.None;
                    HotkeysPatch_KeyBindDrawing.ResetModifierList(KeyPrefs.BindingSlot.B, keyBindingDef);
                }
                if (callBackOnErase != null)
                {
                    callBackOnErase(keyBindingDef);
                }
            }

            return false;
        }
    }
}