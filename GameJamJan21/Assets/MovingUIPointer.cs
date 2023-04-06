using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MovingUIPointer : MonoBehaviour
{

    public Canvas canvas;
    public AimConstraint AimConstraint;
    public GameObject target;

    // public void AddToConstraint(GameObject target) {
    //     ConstraintSource ConstraintSource = new ConstraintSource();
    //     ConstraintSource.sourceTransform = target.transform;
    //     AimConstraint.AddSource(ConstraintSource);
    // }

    // Update is called once per frame
    void Update()
    {
        Vector3 relativePos = new Vector3(0, 0, target.transform.position.z) - target.transform.position;

        // the second argument, upwards, defaults to Vector3.up
        Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
        if (rotation != canvas.transform.rotation) {
            canvas.transform.rotation = rotation;
        }
        
        // canvas.transform.rotation = (Vector3.left, 10.0f * Time.deltaTime, Space.World);

        // Quaternion newVal = Quaternion.LookRotation(transform.position - target.transform.position);
        // if (canvas.transform.rotation != newVal) {
        //     canvas.transform.rotation = Quaternion.LookRotation(transform.position - target.transform.position);
        // }
    }
}
