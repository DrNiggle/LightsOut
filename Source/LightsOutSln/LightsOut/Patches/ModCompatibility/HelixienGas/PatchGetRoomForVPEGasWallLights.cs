﻿using LightsOut.Common;
using System;
using System.Collections.Generic;
using Verse;

namespace LightsOut.Patches.ModCompatibility.HelixienGas
{
    /// <summary>
    /// Patch the IsInRoom method for the VPE Wall Lights
    /// </summary>
    public class PatchGetRoomForVPEGasWallLights : ICompatibilityPatchComponent
    {
        public override string ComponentName => "Patch GetRoom for VPE Wall Lights";
        public override string TypeNameToPatch => "Rooms";
        public override bool TargetsMultipleTypes => false;
        public override bool TypeNameIsExact => true;

        public override IEnumerable<PatchInfo> GetPatches(Type type)
        {
            PatchInfo patch = new PatchInfo();
            patch.method = GetMethod(typeof(Rooms), "GetRoom");
            patch.patch = GetMethod<PatchGetRoomForVPEGasWallLights>("GetRoomPatch");
            patch.patchType = PatchType.Postfix;

            return new List<PatchInfo>() { patch };
        }

        /// <summary>
        /// The patch applied to GetRoom
        /// </summary>
        /// <param name="__0">The Building being checked</param>
        /// <param name="__result">The Room the Building actually belongs to</param>
        private static void GetRoomPatch(Building __0, ref Room __result)
        {
            if (__0 is null) return;
            if (__0.def.defName.StartsWith("VPE_GasWall")
                && __0.def.defName.EndsWith("Light")
                && (__0.Map?.regionAndRoomUpdater?.Enabled ?? false))
                __result = RegionAndRoomQuery.RoomAt(__0.Position + __0.Rotation.FacingCell, __0.Map);
        }
    }
}