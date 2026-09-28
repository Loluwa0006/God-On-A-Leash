using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ShipManager : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] List<SerializedAbility> selectedAbilties = new();

    List<AbilityEntry> shipAbilities = new();
    public bool AbilitiesAvailable { set; get; } = true;

    [System.Serializable]
    public struct AbilityEntry
    {
        public BaseShipAbility ability;
        public InputManager.BufferableInputs abilityInput;
    }
    [System.Serializable]
    public struct SerializedAbility
    {
        public ShipAbilityID ID;

        public InputManager.BufferableInputs abilityInput;
    }
    public void InitializeShipManager()
    {
        _ = InitializeShipAbilities();
    }

    async Task InitializeShipAbilities()
    {
       for (int x = 0; x < selectedAbilties.Count; x++)
        {
            var addressablesLoad = Addressables.LoadAssetAsync<GameObject>(selectedAbilties[x].ID.ToString());
            var addressablesResult = await addressablesLoad.Task;
            if (addressablesLoad.Status == AsyncOperationStatus.Succeeded)
            {
                if (!addressablesResult.TryGetComponent<BaseShipAbility>(out var abilityComponent)) continue;
                abilityComponent.InitializeShipAbility(player.AnarchyManager, player);
                var newEntry = new AbilityEntry
                {
                    ability = abilityComponent,
                    abilityInput = selectedAbilties[x].abilityInput
                };
                shipAbilities.Add(newEntry);
            }
        }
    }
    private void Update()
    {
        for (int i = 0; i < shipAbilities.Count; i++)
        {
            var entry = shipAbilities[i];
            if (IsAbilityAvailable(entry.ability, entry.abilityInput))
             {
                entry.ability.ActivateAbility();
                player.PlayerInput.BufferRegistry[entry.abilityInput].Consume();
            }
        }
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < shipAbilities.Count; i++)
        {
            var ability = shipAbilities[i].ability;
            if (ability.AbilityActive) ability.UpdateAbility();
        }
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
