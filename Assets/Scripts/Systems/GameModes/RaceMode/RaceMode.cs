using UnityEngine;
public class RaceMode : BaseGameMode
{

    public const int SHOW_ALL_CHECKPOINTS = -1;

    [SerializeField] int raceDuration = 60 * 20;
    [SerializeField] RaceUI raceUI;
    [SerializeField] TimerStyle timerStyle = TimerStyle.PerCheckpoint;
    [SerializeField] int numberOfCheckpointsToShow = SHOW_ALL_CHECKPOINTS;
    float timerTracker = 0;

    /// <summary>
    /// Gets time remaining in seconds.
    /// </summary>
    public float TimeRemaining { get => timerTracker / 60.0f; set { timerTracker = value * 60; } }
    RaceCheckpoint[] checkpoints;

    enum TimerStyle
    {
        PerCheckpoint, // Time resets after each checkpoint
        TotalTime, 
    }

    
    public int CheckpointsRemaining { get; private set; }
    public int CheckpointsReached { get; private set; } = 0;

    public float TimeElapsed { get; private set;  }

    RaceCheckpoint previousCheckpoint;

    public override void InitializeMode()
    {
        base.InitializeMode();
        checkpoints = GetComponentsInChildren<RaceCheckpoint>(true);
        CheckpointsRemaining = checkpoints.Length;
        if (checkpoints.Length == 0) return;
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].checkpointReached += OnCheckpointReached;
        }
        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (numberOfCheckpointsToShow != SHOW_ALL_CHECKPOINTS && i >= numberOfCheckpointsToShow)
            {
                checkpoints[i].Hide();
            }
        }
        timerTracker = raceDuration;
        raceUI.InitializeUI(checkpoints, this);
    }

    void OnCheckpointReached(RaceCheckpoint checkpoint)
    {
        CheckpointsRemaining--;
        CheckpointsReached++;
        if (previousCheckpoint != null)
        {
            previousCheckpoint.Hide(); // prevents too much clutter
        }
        previousCheckpoint = checkpoint;
            
        if (timerStyle == TimerStyle.PerCheckpoint)
        {
            timerTracker = raceDuration;
        }
        if (CheckpointsRemaining == 0 && !gameOver)
        {
            EndGame(won: true);
        }
        for (int i = CheckpointsReached; i < CheckpointsReached + numberOfCheckpointsToShow && i < checkpoints.Length; i++)
        {
            checkpoints[i].Show();
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

    void Update()
    {
       if (!gameOver) TimeElapsed += Time.deltaTime;
    }
}
