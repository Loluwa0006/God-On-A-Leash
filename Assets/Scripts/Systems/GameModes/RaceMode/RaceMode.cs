using UnityEngine;
public class RaceMode : BaseGameMode
{
    RaceCheckpoint[] checkpoints;

    int checkpointsRemaining;
    public override void InitializeMode()
    {
        base.InitializeMode();
        checkpoints = GetComponentsInChildren<RaceCheckpoint>(true);
        checkpointsRemaining = checkpoints.Length;
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].checkpointReached += OnCheckpointReached;
        }
    }

    void OnCheckpointReached(RaceCheckpoint checkpoint)
    {
        checkpointsRemaining--;
        if (checkpointsRemaining == 0)
        {
            Debug.Log("You Win");
        }
    }
}
