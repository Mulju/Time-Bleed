using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Data
{
    public class Weapon
    {
        public int magSize { get; private set; }
        public int damage { get; private set; }
        public int fireRate { get; private set; }
        public int bulletsPerShot { get; private set; }
        public int maxAmmoInReserve { get; private set; }
        public int ammoLeft { get; private set; }

        public float reloadTime { get; private set; }
        public float recoilMultiplier { get; private set; }
        public float movementSpeedMultiplier { get; private set; }
        public float movementSpeedScopedMultiplier { get; private set; }
        public float deployTime { get; private set; }

        public bool holdToShoot { get; private set; }

        public Weapon(int magSize, int damage, int fireRate, int bulletsPerShot, int maxAmmoInReserve, int ammoLeft, 
            float reloadTime, float recoilMultiplier, float movementSpeedMultiplier, float movementSpeedScopedMultiplier, float deployTime, 
            bool holdToShoot)
        {
            this.magSize = magSize;
            this.damage = damage;
            this.fireRate = fireRate;
            this.bulletsPerShot = bulletsPerShot;
            this.maxAmmoInReserve = maxAmmoInReserve;
            this.ammoLeft = ammoLeft;
            this.reloadTime = reloadTime;
            this.recoilMultiplier = recoilMultiplier;
            this.movementSpeedMultiplier = movementSpeedMultiplier;
            this.movementSpeedScopedMultiplier = movementSpeedScopedMultiplier;
            this.deployTime = deployTime;
            this.holdToShoot = holdToShoot;
        }

        
    }
}
