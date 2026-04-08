using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private InputReader inputReader;

    Rigidbody rigidbodyComponent;
    Vector3 movementVelocity;
    public float movementSpeed;
    Vector3 cameraForward;
    Vector3 cameraRight;

    Quaternion objectiveRotation;
    [Range(0, 1)] public float rotationSlerpSpeed;

    Animator animatorComponent;

    bool isSprinting;
    bool isBackwards;
    bool isBlocking;
    bool canMove = true;
    bool canRotate = true;
    bool canAttack = true;

    Vector2 moveInput;

    void Awake()
    {
        rigidbodyComponent = GetComponent<Rigidbody>();
        animatorComponent = GetComponent<Animator>();
        objectiveRotation = transform.rotation;
    }

    void OnEnable()
    {
        inputReader.OnMoveEvent += HandleMove;
        inputReader.OnSprintStarted += HandleSprintStart;
        inputReader.OnSprintCanceled += HandleSprintCancel;
        inputReader.OnAttackEvent += HandleAttack;
        inputReader.OnBlockStarted += HandleBlockStart;
        inputReader.OnBlockCanceled += HandleBlockCancel;
    }

    void OnDisable()
    {
        inputReader.OnMoveEvent -= HandleMove;
        inputReader.OnSprintStarted -= HandleSprintStart;
        inputReader.OnSprintCanceled -= HandleSprintCancel;
        inputReader.OnAttackEvent -= HandleAttack;
        inputReader.OnBlockStarted -= HandleBlockStart;
        inputReader.OnBlockCanceled -= HandleBlockCancel;
    }

    void HandleMove(Vector2 input) => moveInput = input;

    void HandleSprintStart()
    {
        isSprinting = true;

        if (isBackwards)
        {
            isBackwards = false;
            animatorComponent.SetBool("isBackwards", false);
        }
    }

    void HandleSprintCancel() => isSprinting = false;

    void HandleAttack()
    {
        if (!canAttack) return;
        animatorComponent.SetTrigger("attack");
    }

    void HandleBlockStart()
    {
        isBlocking = true;
        canAttack = false;
        animatorComponent.SetBool("isBlocking", true);
    }

    void HandleBlockCancel()
    {
        isBlocking = false;
        canAttack = true;
        animatorComponent.SetBool("isBlocking", false);
    }

    void Update()
    {
        if (canMove) Move();
        if (canRotate) Rotate();
        Block();
    }

    void FixedUpdate()
    {
        rigidbodyComponent.linearVelocity = movementVelocity;
    }

    void Move()
    {
        movementVelocity.x = moveInput.x;
        movementVelocity.z = moveInput.y;

        cameraForward = Camera.main.transform.forward;
        cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();

        movementVelocity = cameraForward * movementVelocity.z + cameraRight * movementVelocity.x;

        if (movementVelocity.sqrMagnitude > 1f)
            movementVelocity.Normalize();

        float speed = (isSprinting && !isBackwards) ? movementSpeed * 2 : movementSpeed;
        movementVelocity *= speed;

        animatorComponent.SetFloat("movementSpeed", movementVelocity.magnitude);
    }


    void Rotate()
    {
        if (movementVelocity == Vector3.zero) return;

        float dotForward = Vector3.Dot(movementVelocity.normalized, cameraForward);

        if (!isSprinting)
        {
            if (!isBackwards && dotForward < -0.7f)
            {
                isBackwards = true;
                animatorComponent.SetBool("isBackwards", true);
            }
            else if (isBackwards && dotForward > 0f)
            {
                isBackwards = false;
                animatorComponent.SetBool("isBackwards", false);
            }
        }

        objectiveRotation = Quaternion.LookRotation(movementVelocity);

        if (isBackwards)
        {
            objectiveRotation = Quaternion.Euler(
                objectiveRotation.eulerAngles.x,
                objectiveRotation.eulerAngles.y + 180f,
                objectiveRotation.eulerAngles.z);
        }

        transform.rotation = Quaternion.Slerp(
            transform.rotation, objectiveRotation, rotationSlerpSpeed);
    }

    void Block()
    {
        if (!isBlocking) return;

        if (movementVelocity.sqrMagnitude > 0.01f)
        {
            movementVelocity *= 0.9f;
            animatorComponent.SetFloat("movementSpeed", movementVelocity.magnitude);
        }
        else if (movementVelocity != Vector3.zero)
        {
            movementVelocity = Vector3.zero;
            animatorComponent.SetFloat("movementSpeed", movementVelocity.magnitude);
        }
    }
}