using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    // Each of these three numbers are multiplicative, and therefore default 1
    public float fireRateBonus = 1;
    public float speedBonus = 1;
    public float dashBonus = 1;

    public float maxDuration;
    private float remainingDuration;

    public void SetupEffect(float frr = 1, float spd = 1, float dsh = 1, float dur = 5) {
        maxDuration = dur;
        remainingDuration = maxDuration;
        fireRateBonus = frr;
        speedBonus = spd;
        dashBonus = dsh;
    }

    public void TickDown() {
        remainingDuration -= Time.deltaTime;
    }

    public void EndTimer() {
        remainingDuration = 0;
    }

    public void ResetTimer() {
        remainingDuration = maxDuration;
    }

    public bool CheckTimer() {
        // Returns true if Effect has used up its duration
        if (remainingDuration <= 0) 
            return true;
        return false;
    }
}
