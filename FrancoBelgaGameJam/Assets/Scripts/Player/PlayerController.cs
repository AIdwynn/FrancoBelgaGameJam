using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controller")]
    public bool IsGrounded;
    public bool CanMove;
    public GameObject Camera;

    public float WalkSpeed;
    [SerializeField] private float gravity = -9.81f;

    [Header("References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private CameraController cameraController;

    private float XInput, ZInput;
    private Vector3 Forward, Right, velocity;
    private Vector3 constrainPos;
    private float constrainMaxDist;

    private Animator animator;
    private CharacterController controller;

    [HideInInspector] public bool Moving, Constrained;

    public void Init()
    {
        cameraController = GetComponentInChildren<CameraController>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        cameraController.Init();
    }

    public void TurnStart()
    {

    }

    public void UpdatePlayer(bool constrain)
    {
        ReadInputs();

        MovementManagement(constrain);

        Gravity();

        cameraController.UpdateCamera();
    }

    private void ReadInputs()
    {
        InitializeMoveDir();

        XInput = Input.GetAxis("Horizontal") * Time.deltaTime;
        ZInput = Input.GetAxis("Vertical") * Time.deltaTime;
    }

    private void MovementManagement(bool constrain)
    {
        if (!CanMove)
            return;

        Vector3 move = Vector3.zero;

        Vector3 forward = Forward * ZInput;
        Vector3 right = Right * XInput;

        float moveSpeed = WalkSpeed;


        move = Vector3.Normalize(forward + right) * moveSpeed * Time.deltaTime;

        if (constrain)
        {
            var dist = Vector3.Distance(constrainPos, transform.position + move);
            if (dist < constrainMaxDist)
            {
                if (move.magnitude > 0)
                    controller.Move(move);

                Moving = move.magnitude > 0;
            }
        }
        else
        {
            if (move.magnitude > 0)
                controller.Move(move);

            Moving = move.magnitude > 0;
        }

        animator.SetBool("Moving", move.magnitude > 0);
        animator.SetFloat("MoveDir", Mathf.Sign(ZInput));
    }

    private void Gravity()
    {
        //Grounded
        IsGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (IsGrounded && velocity.y < 0)
        {
            velocity.y = -10f;
        }

        //Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void ResetAnimator()
    {
        animator.SetBool("Moving", false);
        animator.SetFloat("MoveDir", 1);
    }

    public void UpdateConstraint(Vector3 consPos, float consDist)
    {
        constrainPos = consPos;
        constrainMaxDist = consDist;
    }


    private void InitializeMoveDir()
    {
        Forward = Camera.transform.forward;
        Forward.y = 0;
        Forward = Vector3.Normalize(Forward);
        Right = Quaternion.Euler(new Vector3(0, 90, 0)) * Forward;
    }
}
