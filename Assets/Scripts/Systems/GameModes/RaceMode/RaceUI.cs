using TMPro;
using UnityEngine;

public class RaceUI : MonoBehaviour
{
    [SerializeField] TMP_Text timerDisplay;

    public TMP_Text TimerDisplay { get => timerDisplay; private set => timerDisplay = value; }

    [SerializeField] TMP_Text checkpointsRemainingDisplay;

    public TMP_Text CheckpointsRemainingDisplay { get => checkpointsRemainingDisplay; private set => checkpointsRemainingDisplay = value; }

    RaceMode racemodeManager;

    bool gameRunning = false;
    public void InitializeUI(RaceCheckpoint[] checkpoints, RaceMode modeManager)
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].checkpointReached += OnCheckpointReached;
        }
        this.racemodeManager = modeManager;
        racemodeManager.GameEnding += OnGameOver;
        checkpointsRemainingDisplay.text = checkpoints.Length.ToString();
        gameRunning = true;
    }

    void OnCheckpointReached(RaceCheckpoint checkpoint)
    {
        if (!gameRunning) return;
        checkpointsRemainingDisplay.text = racemodeManager.CheckpointsRemaining.ToString();
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
        gameRunning = false;
    }
}
