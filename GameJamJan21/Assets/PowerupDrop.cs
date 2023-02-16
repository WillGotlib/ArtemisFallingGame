using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupDrop : MonoBehaviour
{
    public PowerupPoint relatedPoint;

    public void removePowerup() {
        relatedPoint.setOccupied(false);
    }
}
