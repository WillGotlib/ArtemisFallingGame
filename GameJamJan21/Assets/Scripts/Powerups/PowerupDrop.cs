using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerupDrop : MonoBehaviour
{
    private PowerupManager manager;
    private PowerupPoint relatedPoint;
    public float fireRateBonus = 1;
    public float speedBonus = 1;
    public float dashBonus = 1;
    public float maxDuration = 5;

    public bool requiresWeapon;

    public void SetManager(PowerupManager newManager) {
        manager = newManager;
    }

    public void SetRelatedPoint(PowerupPoint rp) {
        relatedPoint = rp;
    }

    public virtual Effect GiveEffect() {
        return new Effect();
    }

    public void removePowerup() {
        relatedPoint.Occupied = false;
        manager.RemovePowerup();
    }
}
