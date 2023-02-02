using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Trajectory : MonoBehaviour
{   
    private Scene _simulatorScene;
    private PhysicsScene _physicsScene;
    [SerializeField] private Transform _objects;
    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _maxIterations = 100;


    void Start() {
        CreatePhysicsScene();
    }


    // Create a scene that will help simulation

    void CreatePhysicsScene()
    {
        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        _simulatorScene = SceneManager.CreateScene("Trajectory", parameters);
        _physicsScene = _simulatorScene.GetPhysicsScene();
        foreach (Transform obj in _objects) {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, _simulatorScene);
        }
    }


    public void SimulateTrajectory(BulletLogic bullet, Vector3 pos, Vector3 vel) {
        var ghostBullet = Instantiate(bullet, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(ghostBullet.gameObject, _simulatorScene);
        ghostBullet.Fire(vel, true, 3);

        _line.positionCount = _maxIterations;

        for (int i = 0; i < _maxIterations; i++) {
             _physicsScene.Simulate(Time.fixedDeltaTime);
             _line.SetPosition(i, ghostBullet.transform.position);
        }
        Destroy(ghostBullet.gameObject);
    }
}