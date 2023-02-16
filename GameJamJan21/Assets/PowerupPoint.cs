using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupPoint : MonoBehaviour
{
    private bool occupied = false;

    void Start()
    {
        setOccupied(false);
    }

    public void setOccupied(bool oc) {
        occupied = oc;
    }

    public bool getOccupied() {
        return occupied;
    }
}
