using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : Lifeform
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
    private AbilitiesManager abilitiesManager;

    private float XInput, ZInput;
    private Vector3 Forward, Right, velocity;
    private Vector3 constrainPos;
    private float constrainMaxDist;

    private Animator animator;
    private CharacterController controller;

    [HideInInspector] public bool Moving, Constrained, EndTurnAction;
    private float endActionAnticipation;
    private float endActionRecovery;
    bool canHit, stuns;
    Lifeform endActionTarget;


    public void Init()
    {
        cameraController = GetComponentInChildren<CameraController>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        abilitiesManager = GetComponentInChildren<AbilitiesManager>();

        cameraController.Init();
        abilitiesManager.Init(this);
    }

    public void TurnStart()
    {
        CanMove = true;
        animator.SetBool("CanMove", true);
        abilitiesManager.TurnStart();
    }


    public void UpdatePlayer(bool constrain)
    {
        if (EndTurnAction)
        {
            if (endActionAnticipation > 0)
            {
                endActionAnticipation -= Time.deltaTime;
            }
            else
            {
                if (canHit)
                {
                    canHit = false;

                    if (stuns)
                    {
                        Enemy e = endActionTarget.GetComponent<Enemy>();

                        if (e != null)
                            e.IsStunned = true;
                        else
                            endActionTarget.Hurt();

                        cameraController.Rumble();
                    }
                    else
                    {
                        cameraController.Recoil();
                        endActionTarget.Hurt();
                    }
                }

                if (endActionRecovery > 0)
                {
                    endActionRecovery -= Time.deltaTime;
                }
                else
                {
                    EndTurnAction = false;
                    GameManager.Instance.EndTurn();
                }
            }

            return;
        }

        ReadInputs();

        MovementManagement(constrain);

        Gravity();

        cameraController.UpdateCamera();
        abilitiesManager.UpdateManager();
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
        {
            animator.SetBool("Moving", false);
            return;
        }

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

    public void EndTurnAttack(string animation, float anticipation, float recovery)
    {
        EndTurnAction = true;
        canHit = true;

        endActionAnticipation = anticipation;
        endActionRecovery = recovery;

        animator.SetTrigger(animation);

        canHit = false;
    }

    public void EndTurnAttack(string animation, float anticipation, float recovery, Lifeform target, bool stuns)
    {
        EndTurnAction = true;
        canHit = true;

        endActionAnticipation = anticipation;
        endActionRecovery = recovery;

        endActionTarget = target;

        animator.SetTrigger(animation);

        canHit = true;

        this.stuns = stuns;
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

    public void AddAmmo()
    {
        abilitiesManager.GainZap();
    }

    public void Freeze()
    {
        CanMove = false;
        animator.SetBool("CanMove", true);
    }

    public void SwapWeapon()
    {
        animator.SetTrigger("ChangeWeapon");
    }

    public override void Hurt()
    {
        base.Hurt();

        cameraController.HurtShake();
        animator.SetTrigger("Hurt");

        GameManager.Instance.UIManager.UpdateHP(HP);
    }

    public override void Die()
    {
        base.Die();

        animator.SetTrigger("Die");

        StartCoroutine(C_DeathDelay());
    }

    IEnumerator C_DeathDelay()
    {
        yield return new WaitForSecondsRealtime(4);

        GameManager.Instance.Restart();
    }

    private void InitializeMoveDir()
    {
        Forward = Camera.transform.forward;
        Forward.y = 0;
        Forward = Vector3.Normalize(Forward);
        Right = Quaternion.Euler(new Vector3(0, 90, 0)) * Forward;
    }
}
