﻿using System;
using System.Collections.Generic;
using GTA;
using LittleJacobMod.Saving.Utils;
using System.IO;
using GTA.Native;

namespace LittleJacobMod.Saving
{
    internal static class LoadoutSaving
    {
        static readonly List<StoredWeapon> _storedWeapons = new List<StoredWeapon>();
        public static bool Busy { get; private set; } = false;
        public static event EventHandler WeaponsLoaded;

        public static void UpdateWeaponMap()
        {
            Mapper.Process(_storedWeapons);
        }

        public static int Count()
        {
            return _storedWeapons.Count;
        }

        public static bool IsWeaponInStore(uint weapon)
        {
            for (int i = 0; i < _storedWeapons.Count; i++)
            {
                if (_storedWeapons[i].WeaponHash == weapon)
                {
                    return true;
                }
            }
            return false;
        }

        public static void AddWeapon (Weapon weapon)
        {
            for(int i = 0; i < _storedWeapons.Count; i++)
            {
                if (_storedWeapons[i].WeaponHash == (uint)weapon.Hash)
                {
                    return;
                }
            }

            var storedWeapon = new StoredWeapon((uint)weapon.Hash)
            {
                Ammo = weapon.Ammo
            };
            _storedWeapons.Add(storedWeapon);
        }

        public static void SetAmmo(uint hash, int ammo)
        {
            for (var i = 0; i < _storedWeapons.Count; i++)
            {
                if (_storedWeapons[i].WeaponHash == hash)
                {
                    _storedWeapons[i].Ammo = ammo;
                    return;
                }
            }
        }

        public static void UpdateAmmo(uint hash, int ammo)
        {
            var playerHandle = Game.Player.Character.Handle;
            var currentWeaponAmmoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, playerHandle, hash);
            foreach (StoredWeapon weapon in _storedWeapons)
            {
                if (weapon.WeaponHash == hash)
                {
                    weapon.Ammo = ammo;
                } else
                {
                    var storedWeaponAmmoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, playerHandle, weapon.WeaponHash);
                    if (storedWeaponAmmoType == currentWeaponAmmoType)
                    {
                        weapon.Ammo = ammo;
                    }
                }
            }
        }

        public static void SetClip(uint hash, uint componentHash)
        {
            StoredWeapon weapon = _storedWeapons.Find((ti) => ti.WeaponHash == hash);
            if (weapon != null)
                weapon.Clip = componentHash;
        }

        public static void SetVarmod(uint hash, uint varmod)
        {
            StoredWeapon weapon = _storedWeapons.Find((ti) => ti.WeaponHash == hash);
            if (weapon != null)
                weapon.Varmod = varmod;
        }

        public static void SetGrip(uint hash, uint componentHash)
        {
            StoredWeapon weapon = _storedWeapons.Find((ti) => ti.WeaponHash == hash);
            if (weapon != null)
                weapon.Grip = componentHash;
        }

        public static void SetBarrel(uint hash, uint componentHash)
        {
            StoredWeapon weapon = _storedWeapons.Find((ti) => ti.WeaponHash == hash);
            if (weapon != null)
                weapon.Barrel = componentHash;
        }

        public static void SetCamo(uint hash, uint componentHash)
        {
            StoredWeapon weapon = _storedWeapons.Find((ti) => ti.WeaponHash == hash);
            if (weapon != null)
                weapon.Camo = componentHash;
        }

        public static void SetTint(uint hash, int tint)
        {
            StoredWeapon weapon = _storedWeapons.Find((ti) => ti.WeaponHash == hash);
            if (weapon != null)
                weapon.Tint = tint;
        }

        public static void SetCamoColor(uint hash, int color)
        {
            StoredWeapon weapon = _storedWeapons.Find((ti) => ti.WeaponHash == hash);
            if (weapon != null)
                weapon.CamoColor = color;
        }

        public static void SetFlashlight(uint hash, uint flashlight)
        {
            StoredWeapon weapon = _storedWeapons.Find((ti) => ti.WeaponHash == hash);
            if (weapon != null)
                weapon.Flashlight = flashlight;
        }

        public static void SetMuzzle(uint hash, uint muzzle)
        {
            StoredWeapon weapon = _storedWeapons.Find((ti) => ti.WeaponHash == hash);
            if (weapon != null)
                weapon.Muzzle = muzzle;
        }

        public static void SetScope(uint hash, uint scope)
        {
            StoredWeapon weapon = _storedWeapons.Find((ti) => ti.WeaponHash == hash);
            if (weapon != null)
                weapon.Scope = scope;
        }

        public static StoredWeapon GetStoreReference(uint hash)
        {
            return _storedWeapons.Find((ti) => ti.WeaponHash == hash);
        }

        public static void PerformSave(uint ped)
        {
            GTA.UI.LoadingPrompt.Show("Saving weapon loadout...");
            Busy = true;
            try
            {
                var dir = Directory.GetCurrentDirectory();
                var filePath = $"{dir}\\scripts\\LittleJacobMod\\Loadouts\\{ped}.data";
                
                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Loadouts"))
                {
                    Directory.CreateDirectory($"{dir}\\scripts\\LittleJacobMod\\Loadouts");
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                using (var writer = new BinaryWriter(File.Open(filePath, FileMode.Create, FileAccess.Write)))
                {
                    writer.Write(_storedWeapons.Count);
                    foreach (var weapon in _storedWeapons)
                    {
                        writer.Write(weapon.Ammo);
                        writer.Write(weapon.Barrel);
                        writer.Write(weapon.Camo);
                        writer.Write(weapon.CamoColor);
                        writer.Write(weapon.Clip);
                        writer.Write(weapon.Flashlight);
                        writer.Write(weapon.Grip);
                        writer.Write(weapon.Muzzle);
                        writer.Write(weapon.Scope);
                        writer.Write(weapon.Tint);
                        writer.Write(weapon.WeaponHash);
                    }
                }
            } catch (Exception)
            {
                GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ Error saving weapon loadout!");
            } finally
            {
                Script.Wait(1000);
                GTA.UI.LoadingPrompt.Hide();
                Busy = false;
            }
        }

        public static void PerformLoad(bool constructor = false)
        {
            if (!constructor)
            {
                GTA.UI.LoadingPrompt.Show("Loading weapon loadout...");
            }

            Busy = true;
            _storedWeapons.Clear();
            var weaponsRemoved = false;

            if (!Main.IsMainCharacter())
            {
                RemoveWeapons();
                weaponsRemoved = true;
            }

            try
            {
                var dir = Directory.GetCurrentDirectory();
                var filePath = $"{dir}\\scripts\\LittleJacobMod\\Loadouts\\{MapperMain.CurrentPed}.data";
                if (!Directory.Exists($"{dir}\\scripts\\LittleJacobMod\\Loadouts"))
                {
                    return;
                } else if (!File.Exists(filePath))
                {
                    GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ No weapon loadouts saved for this ped!");
                    return;
                }

                if (!weaponsRemoved && !Main.MissionFlag)
                {
                    RemoveWeapons();
                }

                var loadedAmmoTypes = new List<uint>();

                using (var reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
                {
                    int count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        int ammo = reader.ReadInt32();
                        uint barrel = reader.ReadUInt32();
                        uint camo = reader.ReadUInt32();
                        int camoColor = reader.ReadInt32();
                        uint clip = reader.ReadUInt32();
                        uint flash = reader.ReadUInt32();
                        uint grip = reader.ReadUInt32();
                        uint muzzle = reader.ReadUInt32();
                        uint scope = reader.ReadUInt32();
                        int tint = reader.ReadInt32();
                        uint weaponHash = reader.ReadUInt32();

                        if (Function.Call<bool>(Hash.HAS_PED_GOT_WEAPON, Main.PPID, weaponHash, false))
                        {
                            Function.Call<bool>(Hash.REMOVE_WEAPON_FROM_PED, Main.PPID, weaponHash);
                        }

                        Function.Call<bool>(Hash.GIVE_WEAPON_TO_PED, Main.PPID, weaponHash, 0, false, false);

                        var storedWeapon = new StoredWeapon(weaponHash)
                        {
                            Ammo = ammo
                        };

                        if (barrel != (uint)WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weaponHash, barrel);
                            storedWeapon.Barrel = barrel;
                        }

                        if (camo != (uint)WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weaponHash, camo);

                            uint slide = LittleJacobMod.Utils.TintsAndCamos.ReturnSlide(camo);

                            if (slide != (uint)WeaponComponentHash.Invalid)
                            {
                                Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weaponHash, slide);
                            }

                            storedWeapon.Camo = camo;
                            
                            if (camoColor != -1)
                            {
                                Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, Main.PPID, weaponHash, camo, camoColor);
                                
                                if (slide != (uint)WeaponComponentHash.Invalid)
                                {
                                    Function.Call(Hash._SET_PED_WEAPON_LIVERY_COLOR, Main.PPID, weaponHash, slide, camoColor);
                                }

                                storedWeapon.CamoColor = camoColor;
                            }
                        }

                        if (clip != (uint)WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weaponHash, clip);
                            storedWeapon.Clip = clip;
                        }

                        if (flash != (uint)WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weaponHash, flash);
                            storedWeapon.Flashlight = flash;
                        }

                        if (grip != (uint)WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weaponHash, grip);
                            storedWeapon.Grip = grip;
                        }

                        if (muzzle != (uint)WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weaponHash, muzzle);
                            storedWeapon.Muzzle = muzzle;
                        }

                        if (scope != (uint)WeaponComponentHash.Invalid)
                        {
                            Function.Call(Hash.GIVE_WEAPON_COMPONENT_TO_PED, Main.PPID, weaponHash, scope);
                            storedWeapon.Scope = scope;
                        }

                        if (tint != -1)
                        {
                            Function.Call(Hash.SET_PED_WEAPON_TINT_INDEX, Main.PPID, weaponHash, tint);
                            storedWeapon.Tint = tint;
                        }

                        uint ammoType = Function.Call<uint>(Hash.GET_PED_AMMO_TYPE_FROM_WEAPON, Game.Player.Character.Handle, weaponHash);
                        
                        if (loadedAmmoTypes.Contains(ammoType))
                        {
                            _storedWeapons.Add(storedWeapon);
                            continue;
                        }

                        Function.Call(Hash._ADD_AMMO_TO_PED_BY_TYPE, Main.PPID, ammoType, ammo);
                        loadedAmmoTypes.Add(ammoType);
                        _storedWeapons.Add(storedWeapon);
                    }
                }
            }
            catch (Exception)
            {
                GTA.UI.Notification.Show("~g~LittleJacobMod:~w~ Error loading weapon loadout!");
            }
            finally
            {
                if (!constructor)
                {
                    Script.Wait(1000);
                }
                
                GTA.UI.LoadingPrompt.Hide();
                Busy = false;
            }

            WeaponsLoaded?.Invoke(null, null);
        }

        public static void RemoveWeapons()
        {
            Game.Player.Character.Weapons.RemoveAll();
        }
    }
}
