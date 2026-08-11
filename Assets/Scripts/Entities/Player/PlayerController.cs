using System.Collections.Generic;
using UnityEngine;
public class PlayerController : BaseActor
{
    public enum AnimationParameter
    {
        Trigger_StartedSlash,
        Trigger_StartedDragonslash,
        Trigger_StartedThrowingWorm,
        Trigger_StartedSwing,
        Trigger_StartedShadowstep,
        Trigger_StartedFalling,
        Trigger_StartedDashing,
        Trigger_RailParryPerformed,
        Trigger_ParryPerformed,
        Trigger_EnteredHitstun,
        Trigger_JumpPerformed,

        Bool_IsParrying,
        Bool_IsYawning,


        Int_HitstunReactionLevel,

        Bool_AtHighSpeed,
    }


    [Header("Managers")]
    [SerializeField] InputManager _playerInput;
    [SerializeField] WormManager _wormManager;
    [SerializeField] RodManager _rodManager;
    [SerializeField] AnarchyManager _anarchyManager;
    [SerializeField] SquashbucklerManager _squashbucklerManager;
    [SerializeField] CameraManager _cameraManager;
    [SerializeField] ShipManager _shipManager;
    [SerializeField] HealthComponent _healthComponent;

    [Header("Misc")]
    [SerializeField] Collider _collider;
    [SerializeField] GameObject _model;


    public InputManager PlayerInput { get => _playerInput; }
    public WormManager WormManager { get => _wormManager; }

    public RodManager RodManager { get => _rodManager; }

    public AnarchyManager AnarchyManager { get => _anarchyManager; }

    public SquashbucklerManager SquashbucklerManager { get => _squashbucklerManager; }

    public CameraManager CameraManager { get => _cameraManager; }
    public HealthComponent HealthComponent { get => _healthComponent; }
    public Collider Collider { get => _collider; }

    public GameObject Model { get => _model; }
    public bool PlayerGrounded { get; set; }

    public override void Initialize()
    {
        base.Initialize();
        _shipManager.InitializeShipManager();
        EntityManager.Instance.PlayerID = IDComponent.ID;
    }
    public override void Process()
    {
        stateMachine.Process();
    }

    public override void PhysicsProcess()
    {
        stateMachine.PhysicsProcess();
    }

    public void OnPlayerDamaged(HitboxContactInfo info)
    {
        Dictionary<string, object> getHitStateMessage = new()
        {
            [PlayerGetHitState.PlayerGetHitMessage.ContactInfo.ToString()] = info
        };
        var currentState = (PlayerBaseState)stateMachine.GetCurrentState();
        currentState.OnPlayerStruck(info);
    }

    public string GetAnimationParameterFormatted(AnimationParameter parameter)
    {
        var parameterString = parameter.ToString();
        parameterString = parameterString.Substring(parameterString.IndexOf("_") + 1);
        return parameterString;
    }
}
