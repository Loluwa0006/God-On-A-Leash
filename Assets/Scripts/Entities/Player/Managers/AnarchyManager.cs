using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AnarchyManager : MonoBehaviour
{
    const int MAX_ANARCHY = 99;
    [SerializeField] PlayerController player;
    [SerializeField] TMP_Text anarchyDisplay;
    [SerializeField] Slider anarchyProgressDisplay;
    [SerializeField] Slider anarchyDecayDisplay;

    [SerializeField] int numberOfOptionsToUseToReduceScaling = 2;
    [SerializeField, Range(0, 1)] float scalingGenerationReductionAmount = 0.1f;
    [SerializeField] float generationPerOption = 30.0f;

    [SerializeField] int baseDecayRate = 150;
    [SerializeField] int minDecayRate = 30;
    /// <summary>
    /// Passes the number of charges gained.
    /// </summary>
    public UnityEvent<ScaledGenerationMethod, int> anarchyGainedThroughScaledMethod = new();
    public UnityEvent<UnscaledGenerationMethod, int> anarchyGainedThroughUnscaledMethod = new();


    public int DecayRate { get => Mathf.RoundToInt(Mathf.Lerp(baseDecayRate, minDecayRate, CurrentAnarchy / MAX_ANARCHY)); }

    int decayTracker = 0;
    int currentAnarchy;
     public int CurrentAnarchy { set { currentAnarchy = Mathf.RoundToInt(Mathf.Clamp(value, 0, MAX_ANARCHY)); } get => currentAnarchy; }
    /// <summary>
    /// Progress towards next anarchy charge in percentage
    /// </summary>
    float progressToAnarchy;
    public float ProgressToAnarchy 
    { 
        set
        {
            progressToAnarchy = value;
        }
        get => progressToAnarchy; 
    }

    /// <summary>
    /// Float represents scaling of the base generation value.
    /// </summary>
    
    Dictionary<ScaledGenerationMethod, float> scaledGenerationMethods = new();
    /// <summary>
    /// Int represents flat anarchy progress generated on usage.
    /// </summary>
    Dictionary<UnscaledGenerationMethod, float> unscaledGenerationMethods = new();

    private void Start()
    {
        scaledGenerationMethods[ScaledGenerationMethod.Swing] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.Dash] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.Parry] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.RailParry] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.Shadowstep] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.WormThrow] = 0;

        unscaledGenerationMethods[UnscaledGenerationMethod.JustYawn] = player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.JustYawnAnarchyProgress);
        unscaledGenerationMethods[UnscaledGenerationMethod.Yawn] = player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.YawnAnarchyProgressPerFrame);
        unscaledGenerationMethods[UnscaledGenerationMethod.Slash] = player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.SlashAnarchyProgressAmount);
        unscaledGenerationMethods[UnscaledGenerationMethod.Dragonslash] = player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.DragonslashAnarchyProgressAmount);

        UpdateAnarchyDisplays();
    }

    public void GenerateAnarchy(ScaledGenerationMethod method)
    {
        foreach (var kvp in scaledGenerationMethods.ToList())
        {
            if (kvp.Key == method) continue;
            var scaling = scaledGenerationMethods[kvp.Key];
            scaling = Mathf.MoveTowards(scaling, 0, scalingGenerationReductionAmount / numberOfOptionsToUseToReduceScaling);
            scaledGenerationMethods[kvp.Key] = scaling;
        }
        progressToAnarchy += generationPerOption * (1 - scaledGenerationMethods[method]);
        scaledGenerationMethods[method] = scalingGenerationReductionAmount;

        int chargesGained = ConvertProgressToCharges();
        if (chargesGained > 0) anarchyGainedThroughScaledMethod.Invoke(method, chargesGained);
        decayTracker = DecayRate;
        UpdateAnarchyDisplays();
    }

    public void GenerateAnarchyUnscaled(UnscaledGenerationMethod method)
    {
        progressToAnarchy += unscaledGenerationMethods[method];

        int chargesGained = ConvertProgressToCharges();
        if (chargesGained > 0) anarchyGainedThroughUnscaledMethod.Invoke(method, chargesGained);

        UpdateAnarchyDisplays();
    }


    public int ConvertProgressToCharges()
    {
        var increasesToAnarchy = Mathf.FloorToInt(progressToAnarchy / 100);
        currentAnarchy += increasesToAnarchy;
        player.WormManager.WormsRemaining += increasesToAnarchy;
        ProgressToAnarchy -= increasesToAnarchy * 100;
        return increasesToAnarchy;
    }
    void UpdateAnarchyDisplays()
    {
       if (anarchyDisplay != null) anarchyDisplay.text = "x" + currentAnarchy.ToString();
       if (anarchyProgressDisplay != null) anarchyProgressDisplay.value = progressToAnarchy;
     
    }
    void ResetAnarchy()
    {
        CurrentAnarchy = 0;
        decayTracker = DecayRate;
        foreach (var kvp in scaledGenerationMethods.ToList())
        {
            scaledGenerationMethods[kvp.Key] = 0;
        }
        UpdateAnarchyDisplays();
    }
    private void FixedUpdate()
    {
        if (decayTracker > 0)
        {
            decayTracker--;
            if (decayTracker == 0)
            {
                ResetAnarchy();
            }
        }
        if (anarchyDecayDisplay != null)
        {
            if (currentAnarchy > 0)
            {
                anarchyDecayDisplay.value = (float)decayTracker / (float)DecayRate;
            }
        }

    }
}

public enum ScaledGenerationMethod
{
    Swing,
    Dash,
    Parry,
    RailParry,
    Shadowstep,
    WormThrow,
}

public enum UnscaledGenerationMethod 
{
    Slash,
    Dragonslash,
    JustYawn,
    Yawn,
}