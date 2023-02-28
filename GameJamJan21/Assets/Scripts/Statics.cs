using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statics : MonoBehaviour
{
    public static int selectedLevel = 0;

    public static void setLevel(int levelNumber) {
        selectedLevel = levelNumber;
    }
}
