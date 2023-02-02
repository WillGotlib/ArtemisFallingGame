using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    [SerializeField] private Trajectory _trajectory;
    private bool fired = false;

    // Update is called once per frame
    
    void Update()
    {
        ControlBullet();
        Vector3 shootDir = (_target.position - _bulletSpawn.position).normalized;
        _trajectory.SimulateTrajectory(_bullet, _bulletSpawn.position, shootDir * _bulletSpeed);
    }

    [SerializeField] private BulletLogic _bullet;
    [SerializeField] private float _bulletSpeed = 3f;
    [SerializeField] private Transform _bulletSpawn;
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _bulletPivot;
    [SerializeField] private float _rotationSpeed = 0.3f;
    private Vector3 m_EulerAngleVelocity;
    
    private void ControlBullet() {
        if (!fired) {
            transform.Rotate(0, Input.GetAxisRaw("Horizontal") * _rotationSpeed, 0);
            transform.Rotate(Input.GetAxisRaw("Vertical") * _rotationSpeed, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("Pressed space!");
            var spawned = Instantiate(_bullet, _bulletSpawn.position, Quaternion.identity);
            Vector3 shootDir = (_target.position - _bulletSpawn.position).normalized;
            // float n = Mathf.Atan2(shootDir.z, shootDir.x) * Mathf.Rad2Deg;
            // if (n < 0) {
            //     n += 360;
            // }
            // spawned.transform.eulerAngles = new Vector3(n, 0, 0);
            spawned.Fire(shootDir * _bulletSpeed, false, 6);
        }
    }
}
