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
         InitializeShipAbilities();
    }

    void InitializeShipAbilities()
    {
        for (int x = 0; x < selectedAbilties.Count; x++)
        {
            AddNewAbility(selectedAbilties[x].ID, selectedAbilties[x].abilityInput);      
        }
    }

    async void AddNewAbility(ShipAbilityID ID, InputManager.BufferableInputs input)
    {
        var addressablesLoad = Addressables.LoadAssetAsync<GameObject>(ID.ToString());
        var addressablesResult = await addressablesLoad.Task;

        if (addressablesResult == null) return;
        if (!addressablesResult.TryGetComponent<BaseShipAbility>(out var abilityComponent))
        {
            Debug.LogWarning("Could not find ship ability in " + addressablesResult.ToString());
        }
        abilityComponent.InitializeShipAbility(player.AnarchyManager, player);
        var newEntry = new AbilityEntry
        {
            ability = abilityComponent,
            abilityInput = input
        };
        shipAbilities.Add(newEntry);
        Debug.Log("Added new ability " + abilityComponent.AbilityID.ToString() + " to ship abilities list");
    }
    public void Update()
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

    public void FixedUpdate()
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
