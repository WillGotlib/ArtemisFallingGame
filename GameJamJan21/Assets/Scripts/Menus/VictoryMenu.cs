using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryMenu : MatchSetupMenu
{
    // Here the winner will be index 0 in robots (and in scores).
    // The loser(s) will be at index >= 1.
    [SerializeField] GameObject seriesStatsRoot;
    [SerializeField] TMP_Text[] scores; 
    [SerializeField] TMP_Text currentWinnerText;

    [SerializeField] GameObject[] loserRobots;

    // Start is called before the first frame update
    void Start()
    {
        currentWinnerText.text = $"PLAYER {mds.lastWinner + 1} WINS!";
        // Correctly colouring the robots
        int j = 0;
        for (int i = 0; i < mds.numPlayers; i++) {
            if (mds.lastWinner == i) {
                ColourUpdate(i, robots[0]);
            } else {
                j++;
                ColourUpdate(i, robots[j]);
            }
            loserRobots[j].SetActive(true);
        }
        int sum = 0;
        foreach (int num in mds.playerWins) sum += num;
        print("SUM: " + sum);
        if (sum > 1) {
            for (int i = 0; i < mds.numPlayers; i++) {
                scores[i].text = "" + mds.playerWins[i];
            }
        } else {
            seriesStatsRoot.SetActive(false);
        }
    }

    public override void PlayGame() {
        // Go to the regular match menu.
        mds.skipMainMenu = true;
        ReturnMainMenu();
    }

    public void ReturnMainMenu() {
        ResetData();
        // Go back to the main menu
        SceneManager.LoadScene("Menu2");
    }

    private void ResetData() {
        for (int i = 0; i < mds.numPlayers; i++) mds.playerWins[i] = 0;
        mds.levelIdx = 0;
    }

    public override void ChooseLevel(int levelNumber) {
        // Not applicable here.
    }
}
