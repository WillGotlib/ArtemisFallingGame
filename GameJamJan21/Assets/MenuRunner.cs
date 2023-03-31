using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuRunner : MonoBehaviour
{
    public int startingIndex;

    public GameObject[] menus;
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject menu in menus) menu.SetActive(false);
        menus[startingIndex].SetActive(true);
    }
}
