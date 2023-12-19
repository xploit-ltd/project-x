using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float forwardSpeed = 5.0f;
    public float sprintSpeed = 8.0f;
    public float sideSpeed = 2.0f;
    public float backwardSpeed = 2.0f;
    
    private Animator m_Animator;
    private Transform cameraTransform;

    const string VERTICAL_AXIS = "Vertical";
    const string HORIZONTAL_AXIS = "Horizontal";
    const string ANIMATION_FLOAT_SPEED = "Speed";
    const string ANIMATION_TRIGGER_SPRINT = "isSprint";
    const string ANIMATION_TRIGGER_FORWARD = "toForward";
    const string ANIMATION_TRIGGER_BACKWARD = "toBackward";
    const string ANIMATION_TRIGGER_LEFT = "toLeft";
    const string ANIMATION_TRIGGER_RIGHT = "toRight";

    private enum MoveState
    {
        moveForward,
        moveBackward,
        moveLeft,
        moveRight,
        moveForwardLeft,
        moveForwardRight,
        moveBackwardLeft,
        moveBackwardRight,
        runForward,
        idle
    };

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        ResetAnimatorTriggers();

        float axisVertical = Input.GetAxis(VERTICAL_AXIS);
        float axisHorizontal = Input.GetAxis(HORIZONTAL_AXIS);
        bool sprintKeyPressed = Input.GetKey(KeyCode.LeftShift);
        float speed = 0f;
        MoveState moveState = MoveState.idle;

        if (axisVertical > 0 && axisHorizontal == 0) moveState = MoveState.moveForward;
        if (axisVertical < 0 && axisHorizontal == 0) moveState = MoveState.moveBackward;
        if (axisVertical == 0 && axisHorizontal > 0) moveState = MoveState.moveRight;
        if (axisVertical == 0 && axisHorizontal < 0) moveState = MoveState.moveLeft;
        if (axisVertical > 0 && axisHorizontal > 0) moveState = MoveState.moveForwardRight;
        if (axisVertical > 0 && axisHorizontal < 0) moveState = MoveState.moveForwardLeft;
        if (axisVertical < 0 && axisHorizontal > 0) moveState = MoveState.moveBackwardRight;
        if (axisVertical < 0 && axisHorizontal < 0) moveState = MoveState.moveBackwardLeft;
        if (sprintKeyPressed && axisVertical > 0 && axisHorizontal == 0) moveState = MoveState.runForward;

        switch (moveState)
        {
            case MoveState.moveForward:
                m_Animator.SetBool(ANIMATION_TRIGGER_FORWARD, true);
                speed = forwardSpeed;
                break;
            case MoveState.moveBackward:
                m_Animator.SetBool(ANIMATION_TRIGGER_BACKWARD, true);
                speed = backwardSpeed;
                break;
            case MoveState.moveRight:
                m_Animator.SetBool(ANIMATION_TRIGGER_RIGHT, true);
                speed = sideSpeed;
                break;
            case MoveState.moveLeft:
                m_Animator.SetBool(ANIMATION_TRIGGER_LEFT, true);
                speed = sideSpeed;
                break;
            case MoveState.moveForwardLeft:
                m_Animator.SetBool(ANIMATION_TRIGGER_FORWARD, true);
                m_Animator.SetBool(ANIMATION_TRIGGER_LEFT, true);
                speed = forwardSpeed;
                break;
            case MoveState.moveForwardRight:
                m_Animator.SetBool(ANIMATION_TRIGGER_FORWARD, true);
                m_Animator.SetBool(ANIMATION_TRIGGER_RIGHT, true);
                speed = forwardSpeed;
                break;
            case MoveState.moveBackwardLeft:
                m_Animator.SetBool(ANIMATION_TRIGGER_BACKWARD, true);
                m_Animator.SetBool(ANIMATION_TRIGGER_LEFT, true);
                speed = backwardSpeed;
                break;
            case MoveState.runForward:
                m_Animator.SetBool(ANIMATION_TRIGGER_FORWARD, true);
                m_Animator.SetBool(ANIMATION_TRIGGER_SPRINT, true);
                speed = sprintSpeed;
                break;
            case MoveState.moveBackwardRight:
                m_Animator.SetBool(ANIMATION_TRIGGER_BACKWARD, true);
                m_Animator.SetBool(ANIMATION_TRIGGER_RIGHT, true);
                speed = backwardSpeed;
                break;
        }

        m_Animator.SetFloat(ANIMATION_FLOAT_SPEED, speed);

        MoveCharacter(axisVertical, axisHorizontal, speed);
        RotateToCameraView();
    }

    // Поворот в направлении камеры
    private void RotateToCameraView()
    {
        if (cameraTransform == null) return;

        Vector3 lookDirection = new(cameraTransform.forward.x, 0, cameraTransform.forward.z);

        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    // Перемещение персонажа в пространстве
    private void MoveCharacter(float vertical, float horizontal, float speed)
    {
        float translateVertical = vertical * speed * Time.deltaTime;
        float translateHorizontal = horizontal * speed * Time.deltaTime;
        transform.Translate(translateHorizontal, 0, translateVertical);
    }

    // Обнуление триггеров анимаций
    private void ResetAnimatorTriggers()
    {
        m_Animator.SetBool(ANIMATION_TRIGGER_FORWARD, false);
        m_Animator.SetBool(ANIMATION_TRIGGER_BACKWARD, false);
        m_Animator.SetBool(ANIMATION_TRIGGER_RIGHT, false);
        m_Animator.SetBool(ANIMATION_TRIGGER_LEFT, false);
        m_Animator.SetBool(ANIMATION_TRIGGER_SPRINT, false);
    }
}
