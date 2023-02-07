using UnityEngine;

public class FireController : MonoBehaviour
{
    public GunController gun; 
    public BulletLogic bullet;

    // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space) && gun.bouncingCount > 0)
    //     {
    //         print("Pressed space!");
    //         // play gun animation and fire bullet
    //         gun.SecondaryFire();
    //         bullet.Fire(transform.forward, false);
    //     }
    // }
}
