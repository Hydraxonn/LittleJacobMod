﻿using System;
using System.Collections.Generic;
using GTA;

namespace LittleJacobMod.Utils.Weapons
{
    class CompactEMP : Weapon
    {
        public override bool SaveFileWeapon => true;

        public override uint WeaponHash => 3676729658;

        public override int Price => 400000;

        public override string Name => "Compact EMP Launcher";

        public override bool HasMuzzleOrSupp => false;

        public override bool HasClip => false;

        public override bool HasBarrel => false;

        public override bool HasGrip => false;

        public override bool HasScope => false;

        public override bool HasCamo => false;

        public override bool HasFlaslight => false;

        public override Dictionary<string, uint> MuzzlesAndSupps => throw new NotImplementedException();

        public override Dictionary<string, uint> Clips => throw new NotImplementedException();

        public override Dictionary<string, uint> Barrels => throw new NotImplementedException();

        public override Dictionary<string, uint> Grips => throw new NotImplementedException();

        public override Dictionary<string, uint> Scopes => throw new NotImplementedException();

        public override Dictionary<string, uint> Camos => throw new NotImplementedException();

        public override Dictionary<string, uint> FlashLight => throw new NotImplementedException();
    }
}