using TMPro;
using UnityEngine;

public class RaceUI : MonoBehaviour
{
    [SerializeField] TMP_Text timerDisplay;

    public TMP_Text TimerDisplay { get => timerDisplay; private set => timerDisplay = value; }

    [SerializeField] TMP_Text checkpointsRemainingDisplay;

    public TMP_Text CheckpointsRemainingDisplay { get => checkpointsRemainingDisplay; private set => checkpointsRemainingDisplay = value; }

    [SerializeField] TMP_Text completionTimeDisplay;
    [SerializeField] Color successColor = Color.green;
    [SerializeField] Color failureColor = Color.red;

    RaceMode racemodeManager;

    bool gameRunning = false;
    public void InitializeUI(RaceCheckpoint[] checkpoints, RaceMode modeManager)
    {
        if (gameRunning) return;
       
        this.racemodeManager = modeManager;
        racemodeManager.GameEnding += OnGameOver;
        checkpointsRemainingDisplay.text = checkpoints.Length.ToString();
        gameRunning = true;
        completionTimeDisplay.gameObject.SetActive(false);
    }

    public void UpdateCheckpointsRemainingDisplay(int checkpointsRemaining) 
    {
        if (!gameRunning) return;
        checkpointsRemainingDisplay.text = checkpointsRemaining.ToString();
    }

    private void FixedUpdate()
    {
        if (!gameRunning) return;
        if (!racemodeManager.GameOver)
        {
            timerDisplay.text = racemodeManager.TimeRemaining.ToString("F2");
        }
    }

    void OnGameOver(bool won)
    {
        if (won)
        {
            timerDisplay.text = "WIN";
        }
        else
        {
            timerDisplay.text = "LOSE";
        }
        completionTimeDisplay.text = racemodeManager.TimeElapsed.ToString("F2");
        completionTimeDisplay.gameObject.SetActive(true);
        completionTimeDisplay.color = won ? successColor : failureColor;
        gameRunning = false;
    }
}
