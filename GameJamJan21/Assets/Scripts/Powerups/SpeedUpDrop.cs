using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpDrop : PowerupDrop
{
    public override Effect GiveEffect() {
        Effect speedBoost = new Effect();
        speedBoost.SetupEffect(fireRateBonus, speedBonus, dashBonus, maxDuration);
        return speedBoost;
    }
}
