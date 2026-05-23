using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [Header("Buffers")]
    [SerializeField] BufferHelper jumpBuffer;
    [SerializeField] BufferHelper fireBuffer;
    [SerializeField] BufferHelper fireRailBuffer;
    [SerializeField] BufferHelper slashBuffer;
    [SerializeField] BufferHelper swingBuffer;
    [SerializeField] BufferHelper parryBuffer;
    [SerializeField] BufferHelper dragonslashBuffer;
    [SerializeField] BufferHelper dashBuffer;
    [SerializeField] BufferHelper yawnBuffer;
    [SerializeField] BufferHelper shipAbilityOneBuffer;
    [SerializeField] BufferHelper shipAbilityTwoBuffer;

    [Header("Input Action References")]
    [SerializeField] InputActionReference movementAxis;
    [SerializeField] InputActionReference lookAxis;

    [Header("Other")]
    [SerializeField] Vector2 sensitivity = new Vector2(2.75f, 1);
    public bool MovementInputLastFrame { private set; get; }

    public Vector2 Sensitivity { get => sensitivity; private set => sensitivity = value; }

    public enum BufferableInputs
    {
        Jump,
        FireWorm,
        FireWormRail,
        Slash,
        Swing,
        Parry,
        Squashbuckler,
        Dash,
        Yawn,
        ShipAbilityOne,
        ShipAbilityTwo 
    }

    public Dictionary<BufferableInputs, BufferHelper> BufferRegistry { get; private set; } = new();

    
    public Vector2 GetLookDirection() => lookAxis.action.ReadValue<Vector2>();


    private void Start()
    {
        BufferRegistry[BufferableInputs.Jump] = jumpBuffer;
        BufferRegistry[BufferableInputs.FireWorm] = fireBuffer;
        BufferRegistry[BufferableInputs.Slash] = slashBuffer;
        BufferRegistry[BufferableInputs.Swing] = swingBuffer;
        BufferRegistry[BufferableInputs.Parry] = parryBuffer;
        BufferRegistry[BufferableInputs.Dash] = dashBuffer;
        BufferRegistry[BufferableInputs.Squashbuckler] = dragonslashBuffer;
        BufferRegistry[BufferableInputs.Yawn] = yawnBuffer;
        BufferRegistry[BufferableInputs.ShipAbilityOne] = shipAbilityOneBuffer; 
        BufferRegistry[BufferableInputs.ShipAbilityTwo] = shipAbilityTwoBuffer;
        BufferRegistry[BufferableInputs.FireWormRail] = fireRailBuffer;
    }
    public Vector2 GetMovementDirection()
    {
        var movement = movementAxis.action.ReadValue<Vector2>();
        return movement.normalized;
    }

    private void FixedUpdate()
    {
        MovementInputLastFrame = GetMovementDirection() != Vector2.zero;
    }
}

