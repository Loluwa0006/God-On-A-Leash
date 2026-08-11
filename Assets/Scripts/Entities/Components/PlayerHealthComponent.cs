using UnityEngine;
/// <summary>
/// Special exception to health components, players use worms, not hitpoints
/// </summary>
public class PlayerHealthComponent : HealthComponent
{
    [SerializeField] WormManager wormManager;
    public override void Damage(HitboxContactInfo info)
    {
        foreach (var status in statusEffects)
        {
            Debug.Log("Player has status " + status.Key + ": " + status.Value);
            info = status.Value.ProcessDamage(info);
        }
        Debug.Log("Attack is dealing " + info.DamageInfo.damage + "Damage");
        if (info.DamageInfo.damage > 0)
        {
            if (wormManager.WormsRemaining <= 0)
            {
                Kill();
                return;
            }
            Debug.Log("Lost worm");
            wormManager.WormsRemaining--;
        }
        //invoke even if we didn't take damage, because states like parry need to know they got hit
        entityDamaged.Invoke(info);
    }
}
