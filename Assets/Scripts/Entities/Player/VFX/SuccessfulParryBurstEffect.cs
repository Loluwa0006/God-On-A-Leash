using UnityEngine;
using UnityEngine.VFX;

public class SuccessfulParryBurstEffect : MonoBehaviour
{
    [SerializeField] PlayerParryState parryState;
    [SerializeField] VisualEffect parryBurstEffect;
    private void Start()
    {
     //   parryState.parryPerformed += OnParryPerformed;
    }

    private void OnParryPerformed()
    {
        // Handle parry performed event
       // parryBurstEffect.play
    }
}
