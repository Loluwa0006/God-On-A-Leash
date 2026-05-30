using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    [SerializeField] Transform lookTarget;
    public Transform LookTarget { get => lookTarget; }
    [SerializeField] PlayerController player;
    [SerializeField] InputManager inputManager;

    [Header("Cameras")]
    [SerializeField] CinemachineMixingCamera mixingCamera;
    [SerializeField] CinemachineCamera defaultCamera;
    public CinemachineCamera DefaultCamera { get => defaultCamera; }
    [SerializeField] CinemachineCamera closeFollowCamera;


    public CinemachineCamera CloseFollowCamera { get => closeFollowCamera; }


    [SerializeField] CinemachineCamera wideFollowCamera;

    public CinemachineCamera WideFollowCamera { get => wideFollowCamera; }


    CinemachineCamera activeCamera;
    float cameraTransitionTime = 0.0f;
    float elaspedTransitionTime = 0.0f;

    public bool ControlPlayerRotation { get; set; } = true;
    Vector2 lookDirection;

    CinemachineCamera cameraToTransitionTo;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        for (int i = 0; i < mixingCamera.ChildCameras.Count; i++)
        {
            mixingCamera.SetWeight(i, 0.0f);
        }
        mixingCamera.SetWeight(defaultCamera, 1.0f);
        activeCamera = defaultCamera;
    }
    private void Update()
    {
        var lookInput = inputManager.GetLookDirection();

        lookDirection += new Vector2(lookInput.x * inputManager.Sensitivity.x, lookInput.y * inputManager.Sensitivity.y);

        lookDirection.y = Mathf.Clamp(lookDirection.y, -90, 90);
        HandleCameraTransition();
    }

    void HandleCameraTransition()
    {
        if (activeCamera == cameraToTransitionTo || cameraToTransitionTo == null) return;

        elaspedTransitionTime += Time.deltaTime;

        var transitionProgressNormalized = elaspedTransitionTime / cameraTransitionTime;

        mixingCamera.SetWeight(activeCamera, 1 - transitionProgressNormalized);
        mixingCamera.SetWeight(cameraToTransitionTo, transitionProgressNormalized);

        if (transitionProgressNormalized > 0.99f)
        {
            activeCamera = cameraToTransitionTo;
            foreach (var camera in mixingCamera.ChildCameras)
            {
                if (camera == cameraToTransitionTo) continue;
                mixingCamera.SetWeight(camera, 0.0f);
            }
            mixingCamera.SetWeight(activeCamera, 1.0f);
        }
    }

    private void FixedUpdate()
    {
        if (ControlPlayerRotation)
        {
            player.RigidBody.MoveRotation(Quaternion.Euler(0, lookDirection.x, 0));
        }
        lookTarget.localRotation = Quaternion.Euler(-lookDirection.y, lookDirection.x, 0);
    }


    public void TransitionToCamera(CinemachineCamera camera, float duration)
    {
        cameraTransitionTime = duration;
        elaspedTransitionTime = 0.0f;
        cameraToTransitionTo = camera;
    }


}