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

    /// <summary>
    /// Passes the number of charges gained.
    /// </summary>
    public UnityEvent<ScaledGenerationMethod, int> anarchyGainedThroughScaledMethod = new();
    public UnityEvent<UnscaledGenerationMethod, int> anarchyGainedThroughUnscaledMethod = new();

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
    Dictionary<UnscaledGenerationMethod, PlayerStatsManager.StatID> unscaledGenerationMethods = new();

    private void Start()
    {
        scaledGenerationMethods[ScaledGenerationMethod.Swing] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.Dash] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.Parry] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.RailParry] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.Shadowstep] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.WormThrow] = 0;

        unscaledGenerationMethods[UnscaledGenerationMethod.JustYawn] = PlayerStatsManager.StatID.JustYawnAnarchyProgress;
        unscaledGenerationMethods[UnscaledGenerationMethod.Yawn] = PlayerStatsManager.StatID.YawnAnarchyProgressPerFrame;
        unscaledGenerationMethods[UnscaledGenerationMethod.Slash] = PlayerStatsManager.StatID.SlashAnarchyProgressAmount;
        unscaledGenerationMethods[UnscaledGenerationMethod.Dragonslash] = PlayerStatsManager.StatID.DragonslashAnarchyProgressAmount;

        UpdateAnarchyDisplays();
    }

    public void GenerateAnarchy(ScaledGenerationMethod method)
    {
        float scalingReduction = player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.AnarchyScalingGenerationReductionAmount);
        float optionUseNumberToResetScaling = player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.UniqueAnarchyOptionCountToClearScaling);
        float generationPerOption = player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.GenerationPerAnarchyOption);

        ScaleGenerationOptions(method, scalingReduction, optionUseNumberToResetScaling);
        progressToAnarchy += generationPerOption * (1 - scaledGenerationMethods[method]);
        scaledGenerationMethods[method] = scalingReduction;

        int chargesGained = ConvertProgressToCharges();
        if (chargesGained > 0) anarchyGainedThroughScaledMethod.Invoke(method, chargesGained);
        decayTracker = GetDecayRate();
        UpdateAnarchyDisplays();
    }

    void ScaleGenerationOptions(ScaledGenerationMethod method, float scalingReduction, float optionUseNumberToResetScaling)
    {
        foreach (var kvp in scaledGenerationMethods.ToList())
        {
            if (kvp.Key == method) continue;
            var scaling = scaledGenerationMethods[kvp.Key];
            scaling = Mathf.MoveTowards(scaling, 0, scalingReduction / optionUseNumberToResetScaling);
            scaledGenerationMethods[kvp.Key] = scaling;
        }
    }
    public void GenerateAnarchyUnscaled(UnscaledGenerationMethod method)
    {
        progressToAnarchy += player.StatsManager.GetValueFromStat(unscaledGenerationMethods[method]);

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
    int GetDecayRate()
    {
        float baseDecayRate = player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.BaseAnarchyDecayRate);
        float minDecayRate = player.StatsManager.GetValueFromStat(PlayerStatsManager.StatID.MinAnarchyDecayRate);
        return Mathf.RoundToInt(Mathf.Lerp(baseDecayRate, minDecayRate, CurrentAnarchy / MAX_ANARCHY));
    }
    void ResetAnarchy()
    {
        CurrentAnarchy = 0;
        decayTracker = GetDecayRate();
        foreach (var kvp in scaledGenerationMethods.ToList())
        {
            scaledGenerationMethods[kvp.Key] = 0;
        }
        UpdateAnarchyDisplays();
    }
    void DecayLogic()
    {
        if (decayTracker <= 0) return;
        decayTracker--;
        if (decayTracker == 0)
        {
            ResetAnarchy();
        }    
    }
    private void FixedUpdate()
    {
        DecayLogic();
        if (anarchyDecayDisplay != null)
        {
            if (currentAnarchy > 0)
            {
                anarchyDecayDisplay.value = (float)decayTracker / (float)GetDecayRate();
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