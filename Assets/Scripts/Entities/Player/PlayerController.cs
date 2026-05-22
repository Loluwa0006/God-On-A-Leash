using System.Collections.Generic;
using UnityEngine;
public class PlayerController : BaseActor
{
    public enum AnimationParameter
    {
        Trigger_IsAttacking,
        Bool_InSquashbuckler
    }

    [SerializeField] PlayerStatsManager _playerStats;

    [Header("Managers")]
    [SerializeField] InputManager _playerInput;
    [SerializeField] WormManager _wormManager;
    [SerializeField] RodManager _rodManager;
    [SerializeField] AnarchyManager _anarchyManager;
    [SerializeField] SquashbucklerManager _squashbucklerManager;
    [SerializeField] CameraManager _cameraManager;
    [SerializeField] ShipManager _shipManager;

    [Header("Components")]
    [SerializeField] Animator _animator;


    public InputManager PlayerInput { get => _playerInput; }
    public PlayerStatsManager StatsManager { get => _playerStats;}

    public WormManager WormManager { get => _wormManager; }

    public RodManager RodManager { get => _rodManager; }

    public AnarchyManager AnarchyManager { get => _anarchyManager; }

    public SquashbucklerManager SquashbucklerManager { get => _squashbucklerManager; }
    public Animator Animator { get => _animator; }

    public CameraManager CameraManager { get => _cameraManager; }

    public bool PlayerGrounded { get; set; }

    private void Start()
    {
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
        if (info.DamageInfo.damage <= 0) return;
        Dictionary<string, object> getHitStateMessage = new()
        {
            [PlayerGetHitState.PlayerGetHitMessage.ContactInfo.ToString()] = info
        };
        stateMachine.TransitionTo<PlayerGetHitState>(getHitStateMessage);
    }

    public string GetAnimationParameterFormatted(AnimationParameter parameter)
    {
        var parameterString = parameter.ToString();
        parameterString = parameterString.Substring(parameterString.IndexOf("_") + 1);
        return parameterString;
    }
}
