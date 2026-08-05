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
    int currentAnarchy = 0;
    float progressToNextAnarchyCharge = 0.0f;

    public int CurrentAnarchy { set { currentAnarchy = Mathf.RoundToInt(Mathf.Clamp(value, 0, MAX_ANARCHY)); } get => currentAnarchy; }
    public float ProgressToNextAnarchyCharge 
    { 
        set
        {
            progressToNextAnarchyCharge = value;
        }
        get => progressToNextAnarchyCharge; 
    }

    /// <summary>
    /// Float represents scaling of the base generation value.
    /// </summary>
    
    Dictionary<ScaledGenerationMethod, float> scaledGenerationMethods = new();
    Dictionary<UnscaledGenerationMethod, StatObject> unscaledGenerationMethods = new();

    private void Start()
    {
        scaledGenerationMethods[ScaledGenerationMethod.Swing] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.Dash] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.Parry] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.RailParry] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.Shadowstep] = 0;
        scaledGenerationMethods[ScaledGenerationMethod.WormThrow] = 0;

        unscaledGenerationMethods[UnscaledGenerationMethod.JustYawn] = StatDatabase.Instance.PlayerStats.PlayerJustYawnAnarchyProgress;
        unscaledGenerationMethods[UnscaledGenerationMethod.Yawn] = StatDatabase.Instance.PlayerStats.PlayerYawnAnarchyProgress;
        unscaledGenerationMethods[UnscaledGenerationMethod.Slash] = StatDatabase.Instance.PlayerStats.PlayerSlashAnarchyProgressAmount;
        unscaledGenerationMethods[UnscaledGenerationMethod.Dragonslash] = StatDatabase.Instance.PlayerStats.PlayerDragonslashAnarchyRequirement;

        UpdateAnarchyDisplays();
    }

    public void GenerateAnarchy(ScaledGenerationMethod method)
    {
        float scalingReduction = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerAnarchyScalingGenerationReductionAmount);
        float optionUseNumberToResetScaling = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerUniqueAnarchyOptionCountToClearScaling);
        float generationPerOption = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerGenerationPerAnarchyOption);

        ScaleGenerationOptions(method, scalingReduction, optionUseNumberToResetScaling);
        var progress = generationPerOption * (1 - scaledGenerationMethods[method]);
        ProgressToNextAnarchyCharge += progress;
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
        var progress = player.StatsManager.GetValueFromStat(unscaledGenerationMethods[method]);

        ProgressToNextAnarchyCharge += progress;

        int chargesGained = ConvertProgressToCharges();
        if (chargesGained > 0) anarchyGainedThroughUnscaledMethod.Invoke(method, chargesGained);

        UpdateAnarchyDisplays();
    }

    public int ConvertProgressToCharges()
    {
        var increasesToAnarchy = Mathf.FloorToInt(progressToNextAnarchyCharge / 100);
        currentAnarchy += increasesToAnarchy;
        player.WormManager.WormsRemaining += increasesToAnarchy;
        ProgressToNextAnarchyCharge -= increasesToAnarchy * 100;
        return increasesToAnarchy;
    }
    void UpdateAnarchyDisplays()
    {
       if (anarchyDisplay != null) anarchyDisplay.text = "x" + currentAnarchy.ToString();
       if (anarchyProgressDisplay != null) anarchyProgressDisplay.value = progressToNextAnarchyCharge;
    }
    int GetDecayRate()
    {
        float baseDecayRate = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerBaseAnarchyDecayRate);
        float minDecayRate = player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMinAnarchyDecayRate);
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