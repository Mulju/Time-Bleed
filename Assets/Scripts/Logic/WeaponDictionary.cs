using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;
public class WeaponDictionary : MonoBehaviour
{
    public Dictionary<string, Data.Weapon> weapons = new Dictionary<string, Data.Weapon>();


    private void Start()
    {
        Data.Weapon rifle = new Data.Weapon(30, 20, 600, 1, 1000, 30, 1.5f, 1, 0.9f, 0.7f, 1.7f, true);
        Data.Weapon sniper = new Data.Weapon(10, 80, 40, 1, 1000, 10, 2f, 5, 0.85f, 0.45f, 2f, false);
        Data.Weapon shotgun = new Data.Weapon(8, 15, 70, 7, 1000, 8, 4, 8, 0.92f, 0.75f, 1.5f, false);


        weapons.Add("rifle", rifle);
        weapons.Add("sniper", sniper);
        weapons.Add("shotgun", shotgun);
    }
}
