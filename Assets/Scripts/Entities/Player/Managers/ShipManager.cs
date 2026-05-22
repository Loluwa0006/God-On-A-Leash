using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] ShipAbilityRegistry abilityOneID;
    [SerializeField] ShipAbilityRegistry abilityTwoID;
    BaseShipAbility abilityOne;
    BaseShipAbility abilityTwo;
    
    [System.Serializable]
    struct ShipAbilityIndex
    {
        public ShipAbilityRegistry ID;
        public BaseShipAbility Prefab;
    }

    [SerializeField] List<ShipAbilityIndex> shipAbilityRegister = new();

    public bool AbilitiesAvailable { set; get; } = true;

    public void InitializeShipManager()
    {
        InitializeShipAbilities();
    }

    void InitializeShipAbilities()
    {
        for (int i = 0; i < shipAbilityRegister.Count; i++)
        {
            var index = shipAbilityRegister[i];
            if (index.ID == abilityOneID)
            {
                abilityOne = Instantiate(index.Prefab);
            }
            if (index.ID == abilityTwoID)
            {
                abilityTwo = Instantiate(index.Prefab);
            }
        }
        if (abilityOne != null) abilityOne.InitializeShipAbility(player.AnarchyManager, player);
        if (abilityTwo != null) abilityTwo.InitializeShipAbility(player.AnarchyManager, player);
    }
    private void Update()
    {
        if (IsAbilityAvailable(abilityOne, InputManager.BufferableInputs.ShipAbilityOne))
        {
            abilityOne.ActivateAbility();
            player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.ShipAbilityOne].Consume();
        }
        if (IsAbilityAvailable(abilityTwo, InputManager.BufferableInputs.ShipAbilityTwo))
        {
            abilityTwo.ActivateAbility();
            player.PlayerInput.BufferRegistry[InputManager.BufferableInputs.ShipAbilityTwo].Consume();
        }
    }

    private void FixedUpdate()
    {
        if (abilityOne != null && abilityOne.AbilityActive) abilityOne.PhysicsProcess();
        if (abilityTwo != null && abilityTwo.AbilityActive) abilityTwo.PhysicsProcess();
    }

    protected bool IsAbilityAvailable(BaseShipAbility ability, InputManager.BufferableInputs input)
    {
        if (!AbilitiesAvailable) return false;
        if (ability == null) return false;
        if (!ability.AbilityAvailable()) return false;
        if (!player.PlayerInput.BufferRegistry[input].Buffered) return false;
        return true;
    }


}
