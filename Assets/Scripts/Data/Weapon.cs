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

        // Rounds Per Minute.
        public int fireRate { get; private set; }
        public int bulletsPerShot { get; private set; }
        public int maxAmmoInReserve { get; private set; } // ei käytetä
        public int ammoLeft { get; set; }

        public float reloadTime { get; private set; }
        public float recoilMultiplier { get; private set; }
        public float recoilMultiplierScoped { get; private set; }
        public float movementSpeedMultiplier { get; private set; }
        public float movementSpeedMultiplierScoped { get; private set; }
        public float deployTime { get; private set; }

        public bool holdToShoot { get; private set; }

        public Weapon(int magSize, int damage, int fireRate, int bulletsPerShot, int maxAmmoInReserve, int ammoLeft, 
            float reloadTime, float recoilMultiplier, float recoilMultiplierScoped, float movementSpeedMultiplier, float movementSpeedMultiplierScoped, float deployTime, 
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
            this.recoilMultiplierScoped = recoilMultiplierScoped;
            this.movementSpeedMultiplier = movementSpeedMultiplier;
            this.movementSpeedMultiplierScoped = movementSpeedMultiplierScoped;
            this.deployTime = deployTime;
            this.holdToShoot = holdToShoot;
        }

        
    }
}
