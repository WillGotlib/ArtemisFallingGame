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
        if (target != null) {
            Vector3 relativePos = new Vector3(0, 0, target.transform.position.z) - target.transform.position;

            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            if (rotation != canvas.transform.rotation) {
                canvas.transform.rotation = rotation;
            }
        }
    }
}
