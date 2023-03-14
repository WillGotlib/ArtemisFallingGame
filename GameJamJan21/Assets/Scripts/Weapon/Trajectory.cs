using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trajectory : MonoBehaviour
{   
    private Scene _simulatorScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxIterations = 100;
    public GameObject bulletType;
    private GameObject ghostBullet;
    private bool isBulletGenerated = false;

    public void RegisterScene() {
        _simulatorScene = SceneManager.GetSceneByName("Trajectory");
        _physicsScene = _simulatorScene.GetPhysicsScene();
    }

    private BulletLogic _bulletLogic;
    
    public void SimulateTrajectory(GunController weapon) {
        if (isBulletGenerated == false) {
            ghostBullet = Instantiate(bulletType);
            ghostBullet.name = "Trajectory bullet";
            Vector3 cur_pos = weapon.transform.position + weapon.transform.forward * 0.1f;
            ghostBullet.transform.position = cur_pos;
            ghostBullet.transform.rotation = weapon.transform.rotation;
            ghostBullet.GetComponentInChildren<Renderer>().enabled = false;
            isBulletGenerated = true;
            SceneManager.MoveGameObjectToScene(ghostBullet, _simulatorScene);
            _bulletLogic = ghostBullet.GetComponent<BulletLogic>();
            _bulletLogic.bullet.GetComponent<MeshRenderer>().enabled = false;
        }

        else {
            Vector3 cur_pos = weapon.transform.position + weapon.transform.forward * 0.1f;
            ghostBullet.transform.position = cur_pos;
            ghostBullet.transform.rotation = weapon.transform.rotation;
        }

        _bulletLogic.Fire(weapon.transform.forward, true);
        

        _line.positionCount = _maxIterations;

        for (int i = 0; i < _maxIterations; i++) {
             _physicsScene.Simulate(Time.fixedDeltaTime * 0.7f);
             _line.SetPosition(i, ghostBullet.transform.position);
        }
        // Destroy(ghostBullet.gameObject);
    }
}