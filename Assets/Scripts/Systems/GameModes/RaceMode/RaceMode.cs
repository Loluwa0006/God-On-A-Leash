using UnityEngine;
public class RaceMode : BaseGameMode
{
    [SerializeField] int timeToReachNextCheckpoint = 60 * 20;
    [SerializeField] RaceUI raceUI;
    float timerTracker = 0;

    /// <summary>
    /// Gets time remaining in seconds.
    /// </summary>
    public float TimeRemaining { get => timerTracker / 60.0f; set { timerTracker = value * 60; } }
    RaceCheckpoint[] checkpoints;

    public int CheckpointsRemaining { get; private set; }
    public override void InitializeMode()
    {
        base.InitializeMode();
        checkpoints = GetComponentsInChildren<RaceCheckpoint>(true);
        CheckpointsRemaining = checkpoints.Length;
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].checkpointReached += OnCheckpointReached;
        }
        timerTracker = timeToReachNextCheckpoint;
        raceUI.InitializeUI(checkpoints, this);
    }

    void OnCheckpointReached(RaceCheckpoint checkpoint)
    {
        CheckpointsRemaining--;
        timerTracker = timeToReachNextCheckpoint;
        if (CheckpointsRemaining == 0 && !gameOver)
        {
            EndGame(won: true);
        }
        raceUI.UpdateCheckpointsRemainingDisplay(CheckpointsRemaining);
    }

    private void FixedUpdate()
    {
        if (!gameOver)
        {
            timerTracker--;
            if (timerTracker == 0)
            {
                EndGame(won: false);
            }
        }
    }
    public override void EndGame(bool won)
    {
        base.EndGame(won);
        for (int i = 0; i < checkpoints.Length;i++)
        {
            checkpoints[i].CheckpointDisabled = true;
        }
    }

}
