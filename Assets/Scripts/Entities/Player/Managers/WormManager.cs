using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WormManager : MonoBehaviour
{
    [SerializeField] WormEntity wormPrefab;
    [SerializeField] WormEntity wormRailPrefab;
    [SerializeField] PlayerController player;
    [SerializeField] TMP_Text wormDisplay;

    float wormsRemaining;
    public float WormsRemaining
    {
        get => wormsRemaining ;
        set
        {
            wormsRemaining =
            Mathf.Clamp(value, 0, player.StatsManager.GetValueFromStat(StatID.MaxWorms));
            if (wormDisplay != null) wormDisplay.text = wormsRemaining.ToString();
        }
    }
    Queue<WormEntity> wormPool = new();

    Queue<WormEntity> wormRailPool = new();

    public event Action<WormEntity> wormRequested;

    private void Start()
    {
        wormsRemaining = player.StatsManager.GetValueFromStat(StatID.MaxWorms);

        for (int i = 0; i < WormsRemaining; i++)
        {
            WormEntity newWorm = Instantiate(wormPrefab);
            newWorm.Deactivate();
            wormPool.Enqueue(newWorm);
        }

        for (int i = 0; i < WormsRemaining; i++)
        {
            WormEntity newWorm = Instantiate(wormRailPrefab);
            newWorm.Deactivate();
            wormRailPool.Enqueue(newWorm);
        }
    }

    public WormEntity GetNewWorm()
    {
        var newWorm = wormPool.Dequeue();
        wormPool.Enqueue(newWorm);
        wormRequested.Invoke(newWorm);
        return newWorm;
    }

    public WormEntity GetNewWormRail()
    {
        var newWorm = wormRailPool.Dequeue();
        wormRailPool.Enqueue(newWorm);
        wormRequested.Invoke(newWorm);
        return newWorm;

    }

}
