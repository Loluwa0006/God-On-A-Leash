using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class HealthComponent : MonoBehaviour
{
    [SerializeField] int maxHealth;
    public int MaxHealth { get => maxHealth; protected set => maxHealth = value;}

    [SerializeField] Collider hurtbox;

    public Collider Hurtbox { get => hurtbox; }

    int health;

    public int Health {  get => health; protected set => health = Mathf.Clamp(value, 0, maxHealth); }

    public UnityEvent entityKilled = new();
    public UnityEvent<HitboxContactInfo> entityDamaged = new();
    public UnityEvent<int> entityHealed = new();

    protected Dictionary<StatusEffectID, StatusEffect> statusEffects = new();

    public void Start()
    {
        health = MaxHealth;
    }

    public virtual void Damage(HitboxContactInfo info)
    {
        foreach (var status in statusEffects)
        {
            info = status.Value.ProcessDamage(info);
        }
        var previousHealth = health;    
        Health -= info.DamageInfo.damage;
        if (Health == 0)
        {
            Kill();
            return;
        }
         entityDamaged.Invoke(info);
    }

    public virtual void Heal(int amount)
    {
        foreach (var status in statusEffects)
        {
            amount = status.Value.Heal(amount);
        }
        Health += amount;
        entityHealed.Invoke(amount);

    }
    public virtual void Kill()
    {
        entityKilled.Invoke();
    }

    public void AddStatusEffect(StatusEffect effect)
    {
        statusEffects[effect.ID] = effect;
        effect.removeStatus += RemoveStatusEffect;
    }

    public void RemoveStatusEffect(StatusEffectID ID)
    {
        if (statusEffects.ContainsKey(ID))
        {
            statusEffects[ID].removeStatus -= RemoveStatusEffect;
            statusEffects.Remove(ID);
        }
    }

    public StatusEffect GetStatusEffect(StatusEffectID ID)
    {
        if (statusEffects.ContainsKey(ID))
        {
            return statusEffects[ID];
        }
        return null;
    }
}

public abstract class StatusEffect
{
    public StatusEffectID ID { set; get; }
    public StatusEffect(StatusEffectID ID)
    {
        this.ID = ID;
    }
    public event Action<StatusEffectID> removeStatus;
    public abstract void PhysicsProcess();
    public abstract HitboxContactInfo ProcessDamage(HitboxContactInfo info);

    public abstract int Heal(int amount);

    protected void RequestDelete() => removeStatus.Invoke(ID);
}

public class InvulnerabilityEffect : StatusEffect
{
    DamageSource invulnerabilityType;

    int duration;

    //use a const, because a bool and a value is obtuse, and a nullable value has to be translated
    public const int INFINITE_DURATION_VALUE = -69420;

    public InvulnerabilityEffect(StatusEffectID ID, DamageSource source, int duration) : base(ID)
    {
        invulnerabilityType = source;
        this.duration = duration;
    }
    public override void PhysicsProcess()
    {
        if (duration != INFINITE_DURATION_VALUE)
        {
            duration--;
            if (duration <= 0) RequestDelete();
        }
    }

    public override HitboxContactInfo ProcessDamage(HitboxContactInfo info)
    {
        if (info.DamageInfo.damageSource == invulnerabilityType || invulnerabilityType == DamageSource.AnySource)
        {
            // i think... i miss c++.
            DamageInfo damageInfo = info.DamageInfo;
            damageInfo.damage = 0;
            info.DamageInfo = damageInfo;   
        }
        return info;
    }

    public override int Heal(int amount)
    {
        return amount;
    }

}

public enum StatusEffectID
{
    PlayerGethitInvulnerability,
    ParryProjectileInvulnerability,
}  