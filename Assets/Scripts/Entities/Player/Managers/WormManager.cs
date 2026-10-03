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
    public ObjectPool<WormEntity> WormPool { get; private set; } = new();

    public ObjectPool<WormRailEntity> WormRailPool { get; private set; } = new();

    public event Action<WormEntity> WormRequested;

    public List<WormEntity> ActiveWorms { set; get; } = new();
    public void InitializeManager()
    {
        wormsRemaining = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMaxWorms);
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
            WormPool.AddToPool(newWorm);
        }

        for (int i = 0; i < WormsRemaining; i++)
        {
            WormRailEntity newRail = Instantiate(wormRailPrefab);
            newRail.Initialize();
            EntityManager.Instance.RegisterEntity(newRail);
            //the game hasn't started, so there's no worm to disable, and the function doesn't need the parameter to work
            newRail.DisableLine(disabledWorm: null);
            newRail.name = "WormRail" + ( i + 1);
            WormRailPool.AddToPool(newRail);
        }
    }

    public WormEntity GetNewWorm()
    {
        var newWorm = WormPool.GetNewObject();
        WormRequested?.Invoke(newWorm);
        if (!ActiveWorms.Contains(newWorm)) ActiveWorms.Add(newWorm);
        newWorm.wormDisabled += OnWormDeactivated;
        return newWorm;
    }

    public WormRailEntity GetNewWormRail()
    {
        return WormRailPool.GetNewObject();
    }

    void OnWormDeactivated(WormEntity worm)
    {
       if (ActiveWorms.Contains(worm)) ActiveWorms.Remove(worm);
    }

}
