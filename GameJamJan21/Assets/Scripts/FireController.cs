using UnityEngine;

public class FireController : MonoBehaviour
{
    public GunController gun; 
    public BulletFire bullet;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && bullet.fireStatus == FiringState.NotFired && gun.bouncingCount > 0)
        {
            print("Pressed space!");
            // play gun animation and fire bullet
            gun.SecondaryFire();
            bullet.Fire();
        }
    }
}
