﻿using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    internal class MiniSMG : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override WeaponHash WeaponHash => WeaponHash.MiniSMG;

        public override int Price => 7800;

        public override string Name => "Mini SMG";

        public override bool HasMuzzleOrSupp => false;

        public override bool HasClip => true;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => false;

        public override bool HasCamo => false;

        public override bool HasFlaslight => false;

        public override Dictionary<string, WeaponComponentHash> MuzzlesAndSupps => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Clips => new Dictionary<string, WeaponComponentHash>()
        {
            { "Normal - $199", WeaponComponentHash.MiniSMGClip01 },
            { "Extended - $6000", WeaponComponentHash.MiniSMGClip02 },
        };

        public override Dictionary<string, WeaponComponentHash> Barrels => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Grips => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Scopes => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> Camos => throw new NotImplementedException();

        public override Dictionary<string, WeaponComponentHash> FlashLight => throw new NotImplementedException();
    }
}
