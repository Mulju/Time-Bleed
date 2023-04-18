using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
using Data;

public class WeaponDictionary
{
    public Dictionary<string, Data.Weapon> weapons = new Dictionary<string, Data.Weapon>();

    public WeaponDictionary()
    {
        Data.Weapon rifle = new Data.Weapon(30, 20, 600, 1, 1000, 30, 1.5f, 2f, 1f, 0.9f, 0.7f, 1.7f, true);
        Data.Weapon sniper = new Data.Weapon(10, 80, 40, 1, 1000, 10, 2f, 5f, 2f, 0.85f, 0.45f, 2f, false);
        Data.Weapon shotgun = new Data.Weapon(8, 15, 70, 7, 1000, 8, 4f, 8f, 4f, 0.92f, 0.75f, 1.5f, false);

        weapons.Add("rifle", rifle);
        weapons.Add("sniper", sniper);
        weapons.Add("shotgun", shotgun);
    }

        
}
