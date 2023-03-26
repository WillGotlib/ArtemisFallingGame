using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelectMenu : MatchSetupMenu
{    
    [Header("Starting Level Select Menu")]
    [SerializeField] GameObject buttonSetParent;
    [SerializeField] GameObject[] playerOptionsParents;

    void Start() {
        //Fetch the Dropdown GameObject the script is attached to
        // button_set = GetComponent<TMPro.TMP_Dropdown>();
        
        Button[] levelButtonSet = buttonSetParent.GetComponentsInChildren<Button>();

        // List of level names
        // TODO: This is not extensible right now. Fix it.
        for (int i = 0; i < mds.levels.Length; i++) {
            Level currLevel = mds.levels[i].GetComponent<Level>();
            levelButtonSet[i].GetComponent<Image>().sprite = currLevel.thumbnail;
            levelButtonSet[i].gameObject.GetComponentInChildren<TMP_Text>().text = currLevel.nid;
            levelButtonSet[i].gameObject.GetComponent<MatchMenuSelector>().buttonOptionNumber = i;
        }
        mds.levelIdx = 0;
        levelButtonSet[0].interactable = false;

    }
    
    public override void ChooseLevel(int levelNumber) {
        mds.levelIdx = levelNumber;
        print("Selected " + levelNumber);

        // Find the disabled level and bring it back
        Button[] levelButtonSet = buttonSetParent.GetComponentsInChildren<Button>();
        foreach (Button level in levelButtonSet) {
            if (level.interactable == false) {
                level.interactable = true;
            }
        }
    }

    public override void PlayGame() {

    }
}
