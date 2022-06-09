﻿using System.Collections.Generic;
using GTA;
using GTA.Native;
using LittleJacobMod.Saving.Utils;
using LittleJacobMod.Utils.Types;

namespace LittleJacobMod.Saving
{
    internal static class Mapper
    {
        public static List<LittleJacobMod.Utils.Types.Weapon> WeaponData = new();

        public static void Process(List<StoredWeapon> weapons, bool updating)
        {
            if (Main.PPID == 0 || !Main.MenuCreated)
                return;

            var changes = false;

            for (var i = 0; i < WeaponData.Count; i++)
            {
                var weapon = WeaponData[i];
                var hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon.Hash, false);
                var isInStore = LoadoutSaving.IsWeaponInStore(weapon.Hash);

                if (!hasWeapon || isInStore) continue;
                changes = true;
                StoredWeapon storedWeapon = new()
                {
                    WeaponHash = weapon.Hash
                };
                storedWeapon.Tint = storedWeapon.GetTintIndex();
                storedWeapon.Ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon.Hash);
                storedWeapon.Attachments = new Dictionary<string, GroupedComponent>();

                foreach (var attachment in weapon.Attachments)
                {
                    if (attachment.Hash == (uint)WeaponComponentHash.Invalid) continue;
                    if (!storedWeapon.HasComponent(attachment.Hash)) continue;

                    if (storedWeapon.Attachments.ContainsKey(attachment.Group)) storedWeapon.Attachments[attachment.Group] = attachment;
                    else storedWeapon.Attachments.Add(attachment.Group, attachment);
                }

                if (weapon.CamoComponents != null)
                {
                    storedWeapon.Camo = new Component();
                    foreach (var camo in weapon.CamoComponents.Components)
                    {
                        if (camo.Hash == (uint)WeaponComponentHash.Invalid) continue;
                        if (!storedWeapon.HasComponent(camo.Hash)) continue;
                        storedWeapon.Camo = camo;
                        storedWeapon.CamoColor = storedWeapon.GetCamoColor();
                    }
                }

                weapons.Add(storedWeapon);
            }

            for (var i = weapons.Count - 1; i > -1; i--)
            {
                var weapon = weapons[i];

                if (weapon.WeaponHash == (uint)WeaponHash.Unarmed) continue;

                var hasWeapon = Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weapon.WeaponHash, false);

                if (!hasWeapon)
                {
                    changes = true;
                    weapons.RemoveAt(i);
                    continue;
                }

                var weaponCatalogOption = WeaponData.Find(ti => ti.Hash == weapon.WeaponHash);
                var ammo = Function.Call<int>(Hash.GET_AMMO_IN_PED_WEAPON, Main.PPID, weapon.WeaponHash);

                if (ammo != weapon.Ammo && !updating)
                {
                    weapon.Ammo = ammo;
                }

                var tintIndex = weapon.GetTintIndex();

                if (tintIndex != weapon.Tint)
                {
                    weapon.Tint = tintIndex;
                    changes = true;
                }
                
                foreach (var attachment in weaponCatalogOption.Attachments)
                {
                    if (attachment.Hash == (uint)WeaponComponentHash.Invalid) continue;
                    if (weapon.Attachments == null) continue;
                    if (!weapon.Attachments.ContainsKey(attachment.Group)) continue;
                    if (attachment.Hash == weapon.Attachments[attachment.Group].Hash) continue;

                    if (weapon.HasComponent(attachment.Hash))
                    {
                        weapon.Attachments[attachment.Group] = attachment;
                        changes = true;
                    }
                }

                if (weaponCatalogOption.CamoComponents?.Components != null)
                {
                    foreach (var camo in weaponCatalogOption.CamoComponents.Components)
                    {
                        if (camo.Hash == (uint)WeaponComponentHash.Invalid) continue;
                        if (camo.Hash == weapon.Camo.Hash) continue;

                        if (weapon.HasComponent(camo.Hash))
                        {
                            weapon.Camo = camo;
                            weapon.CamoColor = weapon.GetCamoColor();
                            changes = true;
                        }
                    }
                }
            }

            if (changes)
            {
                LoadoutSaving.PerformSave(MapperMain.CurrentPed);
            }
        }
    }
}
