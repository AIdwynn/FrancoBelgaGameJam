using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using OToon;

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
    [SerializeField] AudioSource pick, chicken, steps;
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
    bool canHit, stuns, ded, endTurn;
    Lifeform endActionTarget;

    public event EventHandler RanOuttaMovement ;


    public void Init()
    {
        cameraController = GetComponentInChildren<CameraController>();
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        abilitiesManager = GetComponentInChildren<AbilitiesManager>();

        cameraController.Init();
        abilitiesManager.Init(this);

        var hurtScreen = GetComponentInChildren<CanvasGroup>();
        hurtScreen.alpha = 0;
        OnHurt.AddListener(() => { hurtScreen.alpha = 1;
            StartCoroutine(FadeHurtScreen(hurtScreen));
        });

        OnDeath.AddListener(() => {
            StopAllCoroutines();
            hurtScreen.alpha = 1; });
    }

    private IEnumerator FadeHurtScreen(CanvasGroup hurtScreen)
    {
        yield return new WaitForSecondsRealtime(0.5f);

        var x = 1f;

        while (x >= 0)
        {
            x -= Time.unscaledDeltaTime * 4;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime * 4);
            hurtScreen.alpha = x;
        }

        hurtScreen.alpha = 0;
    }

    public void TurnStart()
    {
        CanMove = true;
        animator.SetBool("CanMove", true);
        abilitiesManager.TurnStart();
    }


    public void UpdatePlayer(bool constrain)
    {
        if (ded)
            return;

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
                        {
                            e.IsStunned = true;
                            e.StunnedParticle = ManagerDeParticle.PlayParticleByName(ParticleNames.Stun, e.transform.position);
                        }
                            
                        else
                            endActionTarget.Hurt();

                        cameraController.Rumble();
                        chicken.Play();

                        GameObject go = GetComponentInChildren<Chicken>().gameObject;
                        go.GetComponent<Animator>().SetTrigger("Activate");

                        var dir = Vector3.Normalize(endActionTarget.transform.position - abilitiesManager.weaponsParent.position);
                        ManagerDeParticle.PlayParticleByName(ParticleNames.Taser, 
                            abilitiesManager.weaponsParent.position + Vector3.up * 0.2f, Quaternion.Euler(0, 90, 0) * dir);

                        endTurn = false;
                    }
                    else
                    {
                        pick.Play();
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
                    if (endTurn)
                    {
                        CanMove = false;

                        if (steps.isPlaying)
                            steps.Stop();

                        animator.SetBool("Moving", false);
                        GameManager.Instance.EndTurn();
                    }
                    else
                    {
                        CanMove = true;
                    }
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
            if (steps.isPlaying)
                steps.Stop();

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

        if (Moving && !steps.isPlaying)
            steps.Play();

        if (!Moving && steps.isPlaying)
            steps.Stop();

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
        CanMove = false;
        EndTurnAction = false;

        if (steps.isPlaying)
            steps.Stop();

        animator.SetBool("Moving", false);


        EndTurnAction = true;
        canHit = true;

        endActionAnticipation = anticipation;
        endActionRecovery = recovery;

        animator.SetTrigger(animation);

        canHit = false;
        endTurn = true;
    }

    public void EndTurnAttack(string animation, float anticipation, float recovery, Lifeform target, bool stuns)
    {
        CanMove = false;
        EndTurnAction = false;

        if (steps.isPlaying)
            steps.Stop();

        animator.SetBool("Moving", false);


        EndTurnAction = true;
        canHit = true;

        endActionAnticipation = anticipation;
        endActionRecovery = recovery;

        endActionTarget = target;

        animator.SetTrigger(animation);

        canHit = true;

        this.stuns = stuns;
        endTurn = true;
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
        SCR_EventHelper.TrySendEvent(RanOuttaMovement, this);
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

        ded = true;
        cameraController.gameObject.transform.SetParent(null, true);
        cameraController.gameObject.AddComponent<Rigidbody>();
        cameraController.gameObject.AddComponent<SphereCollider>();

        StartCoroutine(C_DeathDelay());
    }

    public void ChangeMoveType(int type)
    {
        animator.SetFloat("WeaponState", type);
    }

    IEnumerator C_DeathDelay()
    {
        yield return new WaitForSecondsRealtime(2);

        SceneTransition.FadeToBlack(ManagerDeScene.RestartCurrentScene);
    }

    private void InitializeMoveDir()
    {
        Forward = Camera.transform.forward;
        Forward.y = 0;
        Forward = Vector3.Normalize(Forward);
        Right = Quaternion.Euler(new Vector3(0, 90, 0)) * Forward;
    }
}
