using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDManager : MonoBehaviour
{

    [SerializeField] Image CooldownBar;
    [SerializeField] Image ChargeBar;

    public void UpdateCooldownBar(float amount) {
        CooldownBar.fillAmount = 1f - amount;
    }

    public void UpdateChargeBar(float amount) {
        ChargeBar.fillAmount = 1f - amount;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
