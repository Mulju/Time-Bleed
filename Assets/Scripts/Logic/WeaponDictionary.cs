using System.Collections.Generic;
using System.Collections;
using Data;
using System.Linq;

public class WeaponDictionary
{
    public Dictionary<string, Data.Weapon> weapons = new Dictionary<string, Data.Weapon>();

    public WeaponDictionary()
    {
        Data.Weapon rifle = new Data.Weapon(20, 20, 600, 1, 1000, 1.5f, 2f, 1f, 0.9f, 0.4f, 1.2f, 2f, true);
        Data.Weapon sniper = new Data.Weapon(10, 80, 40, 1, 1000, 2f, 10f, 2f, 0.75f, 0.3f, 1.5f, 5f, false);
        Data.Weapon shotgun = new Data.Weapon(8, 20, 70, 8, 1000, 2f, 8f, 4f, 0.93f, 0.5f, 1f, 1f, false);

        weapons.Add("rifle", rifle);
        weapons.Add("sniper", sniper);
        weapons.Add("shotgun", shotgun);
    }

    public void Respawn()
    {
        foreach (KeyValuePair<string, Data.Weapon> weapon in weapons)
        {
            weapon.Value.ammoLeft = weapon.Value.magSize;
        }
    }

        
}
