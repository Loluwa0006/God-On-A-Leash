using Unity.VisualScripting;
using UnityEngine;

public class PlayerAirState : PlayerBaseState
{
    protected virtual void ApplyGravity(float gravity)
    {
        if (Player.RigidBody.linearVelocity.y < -Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMaxFallSpeed))
        {
            var speedNormalized = Player.RigidBody.linearVelocity.normalized;
            var extraSpeed = Vector3.Dot(Vector3.down * gravity, speedNormalized);
            if (extraSpeed > 0)
            {
                gravity = 0.0f;
            }
        }
        Player.RigidBody.AddForce(gravity * Vector3.down, ForceMode.Acceleration);
    }

    protected void AirborneMovement(Vector2 movementDirection, float acceleration)
    {
        Vector3 moveDirection = movementDirection.x * viewCamera.transform.right + movementDirection.y * viewCamera.transform.forward;

        if (moveDirection.magnitude < MOVEMENT_DEADZONE) return;

        Vector2 lateralSpeed = new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z);
        Vector2 lateralAddition = new(moveDirection.x * acceleration, moveDirection.z * acceleration);

        float currentTurnAngle = Vector2.Angle(lateralSpeed, lateralSpeed + lateralAddition);

        if (currentTurnAngle > Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerAngleToBeConsideredTurning)) //too sharp, decelerating
        {
            //normalize the turn angle so we can sample it later
            float value01 = Mathf.InverseLerp(Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerAngleToBeConsideredTurning) + 0.001f, 180, currentTurnAngle);
            //map the loss to a curve for more control
            float scaler = Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerTurnAngleSpeedLostCurve, 0, value01);
            lateralAddition *= scaler;
		}

		if (lateralSpeed.magnitude >= Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMoveSpeed))
        {
            var speedNormalized = lateralSpeed.normalized;
            var extraSpeed = Vector2.Dot(lateralAddition, speedNormalized);
            if (extraSpeed > 0)
            {
                lateralAddition -= extraSpeed * speedNormalized;
            }
        }
        Player.RigidBody.AddForce(new Vector3(lateralAddition.x, 0, lateralAddition.y), ForceMode.VelocityChange);
        lateralSpeed = new Vector2(Player.RigidBody.linearVelocity.x, Player.RigidBody.linearVelocity.z);
        Player.Animator.SetBool(Player.GetAnimationParameterFormatted(PlayerController.AnimationParameter.Bool_AtHighSpeed), lateralSpeed.magnitude >= Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerSpeedToBeConsideredFast));
    }
}
public static class GrappleUtilities
{

    static RaycastHit raycastResult;

    public static RaycastHit RaycastResult { get => raycastResult; private set => raycastResult = value; }

        
    public static bool AimingAtGrappable(PlayerController Player, LayerMask swingMask)
    {
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Player.StatsManager.GetValueFromStat(StatDatabase.Instance.PlayerStats.PlayerMaxRodRange), swingMask, QueryTriggerInteraction.Collide))
        {
            
            raycastResult = hitInfo;
            if (hitInfo.collider.gameObject.CompareTag("LockOn"))
            {
                raycastResult.point = hitInfo.collider.bounds.center;
            }
            return true;
        }
        return false;
    }
}

