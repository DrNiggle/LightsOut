﻿using Verse;
using HarmonyLib;
using LightsOut.Common;
using LightsOut.ThingComps;
using System;

namespace LightsOut.Patches.Power
{
    /// <summary>
    /// Disables Buildings as they spawn
    /// </summary>
    [HarmonyPatch(typeof(Building))]
    [HarmonyPatch(nameof(Building.SpawnSetup))]
    public class DisableBuildingsOnSpawn
    {
        /// <summary>
        /// Checks if a Building needs to be disabled when it spawns
        /// </summary>
        /// <param name="__instance"></param>
        public static void Postfix(Building __instance)
        {
            if (Tables.IsTable(__instance) || Tables.IsTelevision(__instance))
            {
                Tables.DisableTable(__instance);
            }
            else if (Common.Lights.CanBeLight(__instance))
            {
                Room room = Rooms.GetRoom(__instance);
                if (!(room is null) && Common.Lights.ShouldTurnOffAllLights(room, null))
                    Common.Lights.DisableLight(__instance);
                else
                    Common.Lights.EnableLight(__instance);

                // return so that we don't remove the KeepOnComp from this
                return;
            }

            bool removed = false;
            uint attempts = 0;
            while(!removed)
            {
                try
                {
                    __instance.AllComps.RemoveAll(x => x is KeepOnComp);
                    removed = true;
                }
                catch (InvalidOperationException e)
                {
                    if (e.Message.ToLower().Contains("modified"))
                    {
                        if (++attempts > 100) removed = true;
                    }
                    else
                    {
                        Log.Warning($"[LightsOut](SpawnSetup): InvalidOperationException: {e.Message}");
                        removed = true;
                    }
                }
            }
            if (attempts > 1)
                Log.Warning($"[LightsOut](SpawnSetup): collection was unexpectedly modified {attempts} time(s). If this number is big please report it.");
        }
    }
}