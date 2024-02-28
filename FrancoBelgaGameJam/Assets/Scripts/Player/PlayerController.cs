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

    [SerializeField] private float walkSpeed;
    [SerializeField] private float gravity = -9.81f;

    [Header("References")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    private CameraController cameraController;

    private float XInput, ZInput;
    private Vector3 Forward, Right, velocity;


    private Animator animator;
    private CharacterController controller;

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

    public void UpdatePlayer()
    {
        ReadInputs();

        MovementManagement();

        Gravity();

        cameraController.UpdateCamera();
    }

    private void ReadInputs()
    {
        InitializeMoveDir();

        XInput = Input.GetAxis("Horizontal") * Time.deltaTime;
        ZInput = Input.GetAxis("Vertical") * Time.deltaTime;
    }

    private void MovementManagement()
    {
        if (!CanMove)
            return;

        Vector3 move = Vector3.zero;

        Vector3 forward = Forward * ZInput;
        Vector3 right = Right * XInput;

        float moveSpeed = walkSpeed;


        move = Vector3.Normalize(forward + right) * moveSpeed * Time.deltaTime;

        if (move.magnitude > 0)
            controller.Move(move);


        animator.SetBool("Moving", move.magnitude > 0);
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


    private void InitializeMoveDir()
    {
        Forward = Camera.transform.forward;
        Forward.y = 0;
        Forward = Vector3.Normalize(Forward);
        Right = Quaternion.Euler(new Vector3(0, 90, 0)) * Forward;
    }
}
