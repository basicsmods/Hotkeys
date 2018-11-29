﻿using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Hotkeys
{
    public class Hotkeys : Mod
    {
        public override string SettingsCategory() => "Hotkeys";
        public static Hotkeys_Settings settings;
        private static Hotkeys_SettingsSave saved;
        private Vector2 scrollPosition;

        public Hotkeys(ModContentPack content) : base(content)
        {
            // SETTINGS
            settings = GetSettings<Hotkeys_Settings>();
            saved = LoadedModManager.GetMod<Hotkeys_Save>().GetSettings<Hotkeys_SettingsSave>();

            // THIS
            scrollPosition = new Vector2(0f, 0f);
        }

        public override void DoSettingsWindowContents(Rect canvas)
        {
            float contentHeight = 30f * saved.desCategoryLabelCaps.Count + 1000f;

            Rect source = new Rect(0f, 0f, canvas.width - 24f, contentHeight);
            Widgets.BeginScrollView(canvas, ref scrollPosition, source, true);

            var lMain = new Listing_Standard();
            lMain.ColumnWidth = source.width;
            lMain.Begin(source);

            lMain.GapLine();
            lMain.CheckboxLabeled("Multi Keybindings", ref settings.useMultiKeys, "Check to enable binding multiple keystrokes to each keybinding");
            lMain.CheckboxLabeled("Architect Hotkeys", ref settings.useArchitectHotkeys, "Check to enable the use of hotkeys to select subtabs in the Architect Tab.");
            lMain.GapLine();
            lMain.Label("The game MUST be restarted to add or remove keybinding options.  Set keybinds in the standard menu.");


            lMain.Gap();
            lMain.Gap();
            lMain.CheckboxLabeled("Direct Hotkeys", ref settings.useDirectHotkeys, "Check to enable direct hotkeys to any designator");
            lMain.GapLine();

            if (settings.useDirectHotkeys)
            {
                lMain.Gap();
                lMain.End();

                var grid = new GridLayout(new Rect(source.xMin, source.yMin + lMain.CurHeight, source.width, source.height - lMain.CurHeight), 5, 1, 0, 0);

                CategoryFloatMenus(grid);
                DesignatorFloatMenus(grid);
                RemoveButtons(grid);
            }
            else
            {
                lMain.End();
            }

            Widgets.EndScrollView();
            settings.Write();
        }


        private void RemoveButtons(GridLayout grid)
        {
            var rect = grid.GetCellRect(4, 0, 1);
            var listing = new Listing_Standard();
            var rect2 = new Rect(rect.xMax - 30f, rect.yMin, 30f, rect.height);
            listing.Begin(rect2);

            for (int i = 0; i < saved.desLabelCaps.Count; i++)
            {
                if (listing.ButtonText("-"))
                {
                    saved.desCategoryLabelCaps.RemoveAt(i);
                    saved.desLabelCaps.RemoveAt(i);
                }
            }

            listing.End();
        }

        private void DesignatorFloatMenus(GridLayout grid)
        {
            var rect = grid.GetCellRect(2, 0, 1);
            var listing = new Listing_Standard();
            listing.Begin(rect);

            for (int index = 0; index < saved.desLabelCaps.Count; index++)
            {
                if (listing.ButtonText(saved.desLabelCaps[index]))
                {
                    var options = GetDesFloatMenuOptions(index);

                    FloatMenu window = new FloatMenu(options, "Select Category", false);
                    Find.WindowStack.Add(window);
                }
            }

            listing.Gap();

            if (listing.ButtonText("Add Hotkey", "Add additional direct hotkeys"))
            {
                saved.desCategoryLabelCaps.Add("None");
                saved.desLabelCaps.Add("None");
            }

            listing.End();
        }

        private void CategoryFloatMenus(GridLayout grid)
        {
            var rect = grid.GetCellRect(0, 0, 2);
            var listing = new Listing_Standard();
            listing.Begin(rect);

            for (int index = 0; index < saved.desCategoryLabelCaps.Count; index++)
            {
                if (listing.ButtonTextLabeled("Direct Hotkey " + index.ToString(), saved.desCategoryLabelCaps[index]))
                {
                    var options = GetCatFloatMenuOptions(index);

                    FloatMenu window = new FloatMenu(options, "Select Category", false);
                    Find.WindowStack.Add(window);
                }
            }

            listing.End();
        }

        private List<FloatMenuOption> GetDesFloatMenuOptions(int index)
        {
            int buttonNum = index;
            var options = new List<FloatMenuOption>();

            if (saved.CheckDesCategory(index))
            {
                var designators = saved.GetDesCategory(index).AllResolvedDesignators;
                foreach (var designator in designators)
                {
                    options.Add(new FloatMenuOption(designator.LabelCap, delegate ()
                    {
                        saved.desLabelCaps[buttonNum] = designator.LabelCap;
                    }));
                }
            }
            else
            {
                options.Add(new FloatMenuOption("None", delegate ()
                {
                    saved.desLabelCaps[buttonNum] = "None";
                }));
            }

            return options;
        }

        private List<FloatMenuOption> GetCatFloatMenuOptions(int index)
        {
            int buttonNum = index;
            var options = new List<FloatMenuOption>();

            var desCatDefs = DefDatabase<DesignationCategoryDef>.AllDefsListForReading;
            foreach (var desCat in desCatDefs)
            {
                options.Add(new FloatMenuOption(desCat.LabelCap, delegate ()
                {
                    saved.desCategoryLabelCaps[buttonNum] = desCat.LabelCap;
                }));
            }

            return options;
        }
    }
}



