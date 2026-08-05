using UnityEngine;
using UnityEngine.UI;

public class SquashbucklerManager : MonoBehaviour
{

    [SerializeField] int maxCharge = 10;
    [SerializeField] AnarchyManager anarchyManager;
    [SerializeField] Slider squashbucklerMeter;
    [SerializeField] Image squashbucklerMeterFill;
    [SerializeField] EntityStatsManager playerStatManager;
    [SerializeField] Color squashbucklerAvailableColor = Color.green;
    [SerializeField] Color squashbucklerUnavailableColor = Color.gray;
    int squashbucklerCharge;
    public int SquashbucklerCharge { get => squashbucklerCharge;
        set
        {

            squashbucklerCharge = Mathf.Clamp(value, 0, maxCharge);
            UpdateSquashbucklerDisplays();
        }
    }

    public int MaxCharge { get => maxCharge; }


    private void Start()
    {
        squashbucklerMeter.maxValue = maxCharge;
        anarchyManager.anarchyGainedThroughScaledMethod.AddListener((method, charges) => OnAnarchyGained(charges));
        anarchyManager.anarchyGainedThroughUnscaledMethod.AddListener((method, charges) => OnAnarchyGained(charges));
        UpdateSquashbucklerDisplays();
    }

    void OnAnarchyGained(int charges)
    {
        SquashbucklerCharge += charges;
        UpdateSquashbucklerDisplays();
    }

    void UpdateSquashbucklerDisplays()
    {
        squashbucklerMeter.value = squashbucklerCharge;
        var squashbucklerRequirement = playerStatManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerChargesToEnterSquashbucklerMode);
        squashbucklerMeterFill.color = squashbucklerCharge >= squashbucklerRequirement ? squashbucklerAvailableColor : squashbucklerUnavailableColor;
    }
}
