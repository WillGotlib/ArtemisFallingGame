using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeZone : PowerupDrop
{
    private Dictionary<Collider, Effect> targetEffects = new Dictionary<Collider, Effect>();

    public override Effect GiveEffect() {
        Effect speedDown = new Effect();
        speedDown.SetupEffect(fireRateBonus, speedBonus, dashBonus, maxDuration);
        return speedDown;
    }

    void Update() {
        foreach(KeyValuePair<Collider, Effect> e in targetEffects) {
            Controller playerEntered = e.Key.gameObject.GetComponent<Controller>();
            if (!playerEntered.hasEffect(e.Value)) {
                playerEntered.AddEffect(e.Value);
            } else {
                e.Value.ResetTimer();
            }
        }
    }

    void OnTriggerEnter(Collider collider) {
        // print("splash zone trigger was set off");
        if (collider.gameObject.tag == "Player") {
            Controller playerEntered = collider.gameObject.GetComponent<Controller>();
            // targetEffects.TryAdd(collider, GiveEffect());
            targetEffects[collider] = GiveEffect();
            playerEntered.AddEffect(targetEffects[collider]);

            // _analytics.DamageEvent(collider.gameObject,gameObject);
        }
    }

    void OnTriggerExit(Collider collider) {
        if (targetEffects.ContainsKey(collider)) {
            targetEffects[collider].EndTimer(); // Purge the effect.
        }
        targetEffects.Remove(collider); // If this returns false, no big deal.
    }
}
