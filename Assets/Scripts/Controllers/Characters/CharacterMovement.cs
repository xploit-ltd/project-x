using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;

public class CharacterMovement : NetworkBehaviour
{
    [SerializeField]
    private float forwardSpeed = 5.0f;

    [SerializeField]
    private float sprintSpeed = 8.0f;

    [SerializeField]
    private float sideSpeed = 2.0f;

    [SerializeField]
    private float backwardSpeed = 2.0f;

    [SerializeField]
    private Vector2 defaultPositionRange = new Vector2(-10, 10);

    [SerializeField]
    private NetworkVariable<Vector3> networkCharacterPosition = new();
    
    [SerializeField]
    private NetworkVariable<Quaternion> networkCharacterRotation = new();

    [SerializeField]
    private NetworkVariable<CharacterState> networkAnimatorState = new();

    [SerializeField]
    private float speedCoefficient = 10.0f;

    private Animator animator;
    private Transform cameraTransform;
    private CharacterState oldPlayerState = CharacterState.Idle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
    }

    void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(
                Random.Range(defaultPositionRange.x, defaultPositionRange.y),
                0,
                Random.Range(defaultPositionRange.x, defaultPositionRange.y));
        }
    }

    void Update()
    {
        if (IsClient && IsOwner)
        {
            ClientInput();
        }

        ClientMoveAndRotation();
        ClientVisuals();
    }

    private void ClientMoveAndRotation()
    {
        // ??? перевести локальные координаты в глобальные
        //characterConroller.Move(networkCharacterPosition.Value); 
        transform.Translate(networkCharacterPosition.Value);
        transform.rotation = networkCharacterRotation.Value;
    }

    private void ClientVisuals()
    {
        if (oldPlayerState != networkAnimatorState.Value)
        {
            oldPlayerState = networkAnimatorState.Value;
            animator.SetTrigger($"{networkAnimatorState.Value}");
        }
    }

    void ClientInput()
    {
        bool W = Input.GetKey(KeyCode.W);
        bool A = Input.GetKey(KeyCode.A);
        bool S = Input.GetKey(KeyCode.S);
        bool D = Input.GetKey(KeyCode.D);
        bool SHIFT = Input.GetKey(KeyCode.LeftShift);

        Vector2 moving = new(0,0);
        CharacterState moveState = CharacterState.Idle;

        if (SHIFT && W)
        {
            moveState = CharacterState.RunForward;
            moving = new Vector2(sprintSpeed, 0);
        } else if (W && D)
        {
            moveState = CharacterState.MoveForwardRight;
            moving = new Vector2(forwardSpeed, sideSpeed);
        } else if(W && A)
        {
            moveState = CharacterState.MoveForwardLeft;
            moving = new Vector2(forwardSpeed, -sideSpeed);
        }
        else if(S && D)
        {
            moveState = CharacterState.MoveBackwardRight;
            moving = new Vector2(-backwardSpeed, sideSpeed);
        } else if (S && A)
        {
            moveState = CharacterState.MoveBackwardLeft;
            moving = new Vector2(-backwardSpeed, -sideSpeed);
        } else if (W)
        {
            moveState = CharacterState.MoveForward;
            moving = new Vector2(forwardSpeed, 0);
        } else if (S)
        {
            moveState = CharacterState.MoveBackward;
            moving = new Vector2(-backwardSpeed, 0);
        } else if (D)
        {
            moveState = CharacterState.MoveRight;
            moving = new Vector2(0, sideSpeed);
        } else if (A)
        {
            moveState = CharacterState.MoveLeft;
            moving = new Vector2(0, -sideSpeed);
        }

        float translateVertical = moving.x / speedCoefficient;
        float translateHorizontal = moving.y / speedCoefficient;
        Vector3 pDelta = Vector3.zero;

        if (translateVertical != 0 || translateHorizontal != 0)
        {
            pDelta = new Vector3(translateHorizontal, 0, translateVertical);
        }

        Quaternion rDelta = RotateToCameraView();

        UpdateCharacterServerRpc(pDelta, rDelta, moveState);
    }

    [ServerRpc]
    public void UpdateCharacterServerRpc(
        Vector3 posDelta,
        Quaternion rDelta, 
        CharacterState state)
    {
        networkCharacterPosition.Value = posDelta;
        networkCharacterRotation.Value = rDelta;
        networkAnimatorState.Value = state;
    }

    // Поворот в направлении камеры
    private Quaternion RotateToCameraView()
    {
        if (cameraTransform == null) return transform.rotation;

        Vector3 lookDirection = new(cameraTransform.forward.x, 0, cameraTransform.forward.z);
        return Quaternion.LookRotation(lookDirection);
    }
}
