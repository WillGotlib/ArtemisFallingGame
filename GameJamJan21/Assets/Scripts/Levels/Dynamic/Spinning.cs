using System.Collections;
using UnityEngine;

public class Spinning : DynamicComponent
{
    [SerializeField] Rigidbody rb;
    // Start is called before the first frame update
    [SerializeField] float[] rotationSpeeds = new float[3];
    
    [Header("Elements represent x, y and z respectively.")]
    [SerializeField] float[] reverseIntervals = new float[3];

    bool[] reversing = new bool[3]; // Will default to all false (which is what we want)
    float[] rotations = new float[3]; // Will default to all false (which is what we want)
    private float[] oldRotations = new float[3]; // For comparisons.

    private Coroutine[] intervalRoutines = new Coroutine[3];
    private Coroutine[] speedRamps = new Coroutine[3];

    private float reversalDuration = 1.5f;

    void Start()
    {
        // rotations = rotationSpeeds; 
        for (int i = 0; i < rotations.Length; i++) {
            if (reverseIntervals[i] > 0) {
                intervalRoutines[i] = StartCoroutine(reversalInterval(i));
            }
            oldRotations[i] = Mathf.NegativeInfinity;
        }
    }

    public override void DynamicAction()
    {
        for (int i = 0; i < rotations.Length; i++) {
            if (Mathf.Abs(rotationSpeeds[i] - oldRotations[i]) > 0.1 && speedRamps[i] == null) { // At least some difference. 
                speedRamps[i] = StartCoroutine(ChangeSpeed(rotations[i], rotationSpeeds[i], reversalDuration, i));
            }
        }
        
        // print("(" + rotations[0] + ", " + rotations[1] + ", " + rotations[2] + ")");
        oldRotations = new float[] {rotations[0], rotations[1], rotations[2]};
        Quaternion new_rot = Quaternion.Euler(new Vector3(rotations[2], rotations[0], rotations[1]));
        rb.MoveRotation(rb.rotation * new_rot);
        // transform.Rotate(rotations[0], rotations[1], rotations[2], Space.World);
    }

    private IEnumerator reversalInterval(int i)
    {
        // speedRamps[i] = ChangeSpeed(rotations[i], -rotations[i], reversalDuration, i);
        while (true) {
            yield return new WaitForSeconds(reverseIntervals[i]);
            reversing[i] = true;
            print("REVERSING START!");
            rotationSpeeds[i] = -rotations[i];
        }
    }
    
    private IEnumerator ChangeSpeed( float v_start, float v_end, float duration, int i)
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            rotations[i] = Mathf.Lerp( v_start, v_end, elapsed / duration );
            elapsed += Time.deltaTime;
            yield return null;
        }
        rotations[i] = v_end;
        speedRamps[i] = null;
    }
    
    public void OnTriggerStay(Collider col) {
        var ctrl = col.gameObject.GetComponent<CharacterController>();
        if (ctrl) {
            ctrl.SimpleMove(Vector3.back);
        }
    }
}
