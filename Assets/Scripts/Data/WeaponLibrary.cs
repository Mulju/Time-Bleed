using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Data
{
    public class WeaponLibrary
    {

        public readonly Dictionary<int, Weapon> weapons = new Dictionary<int, Weapon>();
        // Ei ehkä voi olla readonly..? Vai voiko muokata saman scriptin sisällä?
        public readonly Weapon rifle, sniper, pistol;
    
        public WeaponLibrary()
        {
            rifle = new Weapon();
            rifle.id = 0;
            rifle.damage = 0;
            rifle.clipSize = 0;
            rifle.reloadTime = 0;

            sniper = new Weapon();
            sniper.id = 0;
            sniper.damage = 0;
            sniper.clipSize = 0;
            sniper.reloadTime = 0;

            pistol = new Weapon();
            pistol.id = 0;
            pistol.damage = 0;
            pistol.clipSize = 0;
            pistol.reloadTime = 0;

            weapons.Add(rifle.id, rifle);
            weapons.Add(sniper.id, sniper);
            weapons.Add(pistol.id, pistol);
        }
    }
}
