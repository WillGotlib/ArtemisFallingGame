using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRateUpDrop : PowerupDrop
{
    public override Effect GiveEffect() {
        Effect fireBoost = new Effect();
        fireBoost.SetupEffect(fireRateBonus, speedBonus, dashBonus, maxDuration);
        return fireBoost;
    }
}
