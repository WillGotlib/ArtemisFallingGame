using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is just inheriting from the PowerupDrop system to play nice. It's not super similar to a powerup.
public class WeaponDrop : PowerupDrop
{
    public GameObject secondaryType;

    public override Effect GiveEffect() {
        Effect weaponNeutral = new Effect();
        weaponNeutral.SetupEffect(fireRateBonus, speedBonus, dashBonus, maxDuration);
        return weaponNeutral;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
