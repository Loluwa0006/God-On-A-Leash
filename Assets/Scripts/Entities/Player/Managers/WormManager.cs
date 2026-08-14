using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class WormManager : MonoBehaviour
{
    [SerializeField] WormEntity wormPrefab;
    [SerializeField] WormRailEntity wormRailPrefab;
    [SerializeField] PlayerController player;
    [SerializeField] TMP_Text wormDisplay;

    float wormsRemaining;
    public float WormsRemaining
    {
        get => wormsRemaining ;
        set
        {
            wormsRemaining =
            Mathf.Clamp(value, 0, player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMaxWorms));
            if (wormDisplay != null) wormDisplay.text = wormsRemaining.ToString();
        }
    }
    Queue<WormEntity> wormPool = new();

    Queue<WormRailEntity> wormRailPool = new();

    public event Action<WormEntity> wormRequested;

     public List<WormEntity> ActiveWorms { set; get; } = new();
    public void InitializeManager()
    {
        wormsRemaining = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMaxWorms);
        foreach (var worm in wormPool)
        {
            Destroy(worm.gameObject);
        }
        foreach (var worm in wormRailPool)
        {
            Destroy(worm.gameObject);
        }
        wormPool.Clear();
        wormRailPool.Clear();

        StartCoroutine(ConfigureWormPools());

    }

    IEnumerator ConfigureWormPools()
    {
        yield return new WaitForFixedUpdate();
        for (int i = 0; i < WormsRemaining; i++)
        {
            WormEntity newWorm = Instantiate(wormPrefab);
            newWorm.Initialize(player.StatsManager);
            EntityManager.Instance.RegisterEntity(newWorm);
            newWorm.Deactivate();
            newWorm.name = "Worm" + (i + 1);
            wormPool.Enqueue(newWorm);
        }

        for (int i = 0; i < WormsRemaining; i++)
        {
            WormRailEntity newRail = Instantiate(wormRailPrefab);
            newRail.Initialize();
            EntityManager.Instance.RegisterEntity(newRail);
            //the game hasn't started, so there's no worm to disable, and the function doesn't need the parameter to work
            newRail.DisableLine(disabledWorm: null);
            newRail.name = "WormRail" + ( i + 1);
            wormRailPool.Enqueue(newRail);
        }
    }

    public WormEntity GetNewWorm()
    {
        var newWorm = wormPool.Dequeue();
        wormPool.Enqueue(newWorm);
        wormRequested.Invoke(newWorm);
        if (!ActiveWorms.Contains(newWorm)) ActiveWorms.Add(newWorm);
        newWorm.wormDisabled += OnWormDeactivated;
        return newWorm;
    }

    public WormRailEntity GetNewWormRail()
    {
        var newWorm = wormRailPool.Dequeue();
        wormRailPool.Enqueue(newWorm);
        return newWorm;
    }

    void OnWormDeactivated(WormEntity worm)
    {
       if (ActiveWorms.Contains(worm)) ActiveWorms.Remove(worm);
    }

}
