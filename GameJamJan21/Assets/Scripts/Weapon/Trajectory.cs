using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Trajectory : MonoBehaviour
{   
    public int reflections;
    private Ray ray;
    private RaycastHit hit;
    private Vector3 direction;
    private Controller player;
    private GunController gunController;

    [SerializeField] private LineRenderer _line;
    [SerializeField] private int _lineLength;
    [SerializeField] private int _traceLength;

    private void Start() {
        gunController = gameObject.GetComponent<GunController>();
        player = gunController.owner;
    }


    private void Update() {
        ray = new Ray(transform.position, transform.forward);

        _line.positionCount = 1;
        _line.SetPosition(0, transform.position);
        float remainingLineLength = _lineLength;
        float remainingTraceLength = _traceLength;
        int layerNumber = LayerMask.NameToLayer("Ignore Raycast");
        int layerMask = 1 << layerNumber;

        for (int i = 0; i < reflections; i++)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, remainingLineLength, ~layerMask)) {
                _line.positionCount += 1;
                _line.SetPosition(_line.positionCount -1, hit.point);
                remainingLineLength -= Vector3.Distance(ray.origin, hit.point);
                if (hit.collider.tag == "Reflector") {
                    ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
                }
        
                if (hit.collider.tag == "Player") {
                    break;
                }
            }

            else {
                if (_line.positionCount < remainingLineLength) {
                    _line.positionCount += 1;
                    _line.SetPosition(_line.positionCount - 1, ray.origin + ray.direction * remainingLineLength);
                }
            }
        }

        for (int i = 0; i < reflections; i++)
        {
            if (Physics.Raycast(ray.origin, ray.direction, out hit, remainingTraceLength, ~layerMask)) {
                remainingTraceLength -= Vector3.Distance(ray.origin, hit.point);
                if (hit.collider.tag == "Reflector") {
                    ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));
                }
        
                if (hit.collider.tag == "Player") {
                    if (hit.collider.gameObject.GetComponent<Controller>() != player) {
                        _line.startColor = Color.red;
                        _line.endColor = Color.red;
                    }
                    break;
                }
                else {
                    _line.startColor = Color.white;
                    _line.endColor = Color.white;
                }
            }

        }
    }
}