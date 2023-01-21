using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public float speed = 6f;
    private bool aiming_status = false;
    public GameObject projectile;
    private GameObject bullet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (aiming_status == false) {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

            if (direction.magnitude >= 0.1f) {
                controller.Move(direction * speed * Time.deltaTime);
            }

            if (Input.GetMouseButtonDown(1)) {
                startAiming();
            }
                
        } else {
            if (Input.GetMouseButtonDown(1))
                stopAiming();
        }
    }

    void startAiming() {
        aiming_status = true;
        GameObject bullet = Object.Instantiate(projectile, gameObject.transform, false);
        Vector3 cur_pos = gameObject.transform.position;
        bullet.transform.position = new Vector3(cur_pos[0] + 0.5f, cur_pos[1] + 0.2f, cur_pos[2] + 0.5f);
    }

    void stopAiming() {
        aiming_status = false;
        Object[] allBullets = Object.FindObjectsOfType(typeof(GameObject));
        foreach(GameObject obj in allBullets) {
            if(obj.transform.name == "Bullet Parent(Clone)"){
                Destroy(obj);
            }
        }
        Destroy(bullet);
    }
}
