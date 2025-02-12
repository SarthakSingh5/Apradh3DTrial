using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class AimStateManager : MonoBehaviour
{
    public AimBaseState currentState;
    public HipFireState Hip = new HipFireState();
    public AimState Aim = new AimState();




    [SerializeField] private CinemachineFreeLook cinemachineCamera;
    [SerializeField] private Transform camFollowPos;

    [SerializeField] private float mouseSensitivity = 2f;

    private PlayerInput playerInput;
    private InputAction lookAction;


    [HideInInspector] public Animator anim;




    [HideInInspector] public Transform aimPos;
    [SerializeField] float aimSmoothSpeed;
    [SerializeField] LayerMask aimMask;

    float xFollowPos;
    float yFollowPos, ogYPos;
    [SerializeField] float crouchCamHeight = 0.6f;
    [SerializeField] float shoulderSwapSpeed = 10;
    MovementStateManager moving;


    MultiAimConstraint[] multiAims;
    WeightedTransform aimPosWeightedTransform;

    private void Awake()
    {
        aimPos = new GameObject().transform;
        aimPos.name = "AimPosition";
        aimPosWeightedTransform.transform = aimPos;
        aimPosWeightedTransform.weight = 1;

        multiAims = GetComponentsInChildren<MultiAimConstraint>();
        foreach (MultiAimConstraint constraint in multiAims)
        {
            var data = constraint.data.sourceObjects;
            data.Clear();
            data.Add(aimPosWeightedTransform);
            constraint.data.sourceObjects = data;
        }
    }



    private void Start()
    {
        moving = GetComponent<MovementStateManager>();
        xFollowPos = camFollowPos.localPosition.x;
        ogYPos = camFollowPos.localPosition.y;
        yFollowPos = ogYPos;
        // Set up input
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            lookAction = playerInput.actions["Look"];
            lookAction?.Enable();
        }

        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        anim = GetComponent<Animator>();
        SwitchState(Hip);
    }

    private void OnDisable()
    {
        lookAction?.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (lookAction != null && cinemachineCamera != null)
        {
            Vector2 lookDelta = lookAction.ReadValue<Vector2>();

            // Rotate the camera using Cinemachine
            cinemachineCamera.m_XAxis.Value += lookDelta.x * mouseSensitivity;
            cinemachineCamera.m_YAxis.Value -= lookDelta.y * mouseSensitivity;
        }


        Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask))
        {
            if (Vector3.Dot(ray.direction, Vector3.up) > 0.5f) // Allow more upward aiming
            {
                aimPos.position = ray.origin + ray.direction * 50f;
            }
            else
            {
                aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);
            }
        }


        MoveCamera();


        currentState.UpdateState(this);
    }


    public void SwitchState(AimBaseState state)
    {
        currentState = state;
        currentState.EnterState(this);
    }


    void MoveCamera()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            xFollowPos = -xFollowPos;

        if (moving.currentState == moving.Crouch)
            yFollowPos = crouchCamHeight;
        else
            yFollowPos = ogYPos;

        Vector3 newFollowPos = new Vector3(xFollowPos, yFollowPos, camFollowPos.localPosition.z);
        camFollowPos.localPosition = Vector3.Lerp(camFollowPos.localPosition, newFollowPos, shoulderSwapSpeed * Time.deltaTime);
    }


}
