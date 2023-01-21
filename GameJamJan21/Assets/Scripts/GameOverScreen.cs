using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{

    public Text finalText;

    public void Setup(int status) {
        if (status == 0) { // Failure
            finalText.enabled = true;
            finalText.text = "YOU LOST!!!! LOSER!!!!";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
