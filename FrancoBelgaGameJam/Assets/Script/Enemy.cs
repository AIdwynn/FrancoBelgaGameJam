using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EnemyAttackState
{
    Not, Charging
}

public class Enemy : Lifeform
{
    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private float _detectionRange;
    [SerializeField] private float _maxTravelDistance;
    [SerializeField] private float _attackRange;

    [Header("References")]
    [SerializeField] AudioSource scream, attack;
    [SerializeField] GameObject distanceVisualizer, blood;
    [SerializeField] Transform head;
    [SerializeField] private bool _dieOnAttack;
    [SerializeField] private bool _chargeAttack;
    [SerializeField] GameObject ragdoll, fakeRagdoll, original;

    [Header ("Editor")]
    [SerializeField] private bool _drawGizmos;

    private PlayerController _player;
    private Animator _animator;
    private float _distanceTraveled;
    private float _previousRemainingDistance;
    private EnemyAttackState _attackState;

    public Action OnStop;

    public Action OnChargingAttack;
    public Action OnAttack;

    bool reviveNextTurn;


    public bool IsStunned { get; set; }
    public ParticleSelfDestruct StunnedParticle;
    public bool IsMoving { get { return !_agent.isStopped; } }
    public bool ReviveNextTurn { get { return reviveNextTurn; } }

    #region UnityMethods

    private void Start()
    {
        _player = GameManager.Instance.Player;
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _agent.isStopped = true;
        _attackState = EnemyAttackState.Not;

        OnAttack += () => { 
            _player.Hurt();
            if (_dieOnAttack)
            {
                OnDeath?.Invoke();
            }
            _attackState = EnemyAttackState.Not;
            _animator.SetTrigger("Attack");
            attack.Play();
        };

        OnChargingAttack += () => { _attackState = EnemyAttackState.Charging; };

        OnDeath.AddListener(KillEnemy);

    }

    private void OnDrawGizmos()
    {
        if (!_drawGizmos) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _maxTravelDistance);
    }

    #endregion


    public override void Hurt()
    {
        ManagerDeParticle.PlayParticleByName(ParticleNames.Hit, this.transform.position);
        base.Hurt();
        GameManager.Instance.Cut();
        if (HP > 0 && head != null && !reviveNextTurn)
        {
            head.transform.localScale = Vector3.zero;
            IsStunned = true;
            fakeRagdoll.SetActive(true);
            original.SetActive(false);
            reviveNextTurn = true;
        }
    }

    public override void Die()
    {
        base.Die();
        ManagerDeParticle.PlayParticleByName(ParticleNames.Death, this.transform.position);
        GameManager.Instance.AddAmmo();
    }

    private void KillEnemy()
    {
        GameObject go = Instantiate(ragdoll);
        go.transform.position = transform.position;
        go.transform.rotation = transform.rotation;
        Destroy(gameObject);
    }

    public void TurnStart()
    {
        distanceVisualizer.SetActive(false);
        _animator.SetBool("Moving", true);
    }

    public void UpdateVisualizer()
    {
        if (!reviveNextTurn)
            distanceVisualizer.SetActive(true);

        var dist = _maxTravelDistance * GameManager.Instance.VisualizerScaleOffset + _attackRange + GameManager.Instance.EnemyRangeOffset;
        distanceVisualizer.transform.position = transform.position + Vector3.up * -1;
        distanceVisualizer.transform.localScale = new Vector3(dist, 1, dist);
    }

    public void Activate()
    {
        if (IsStunned) // if enemy is stunned then don't activate enemy
        {
            IsStunned = false;

            if (StunnedParticle != null)
                Destroy(StunnedParticle.gameObject);

            OnStop?.Invoke();
            _animator.SetBool("Moving", false);
            return;
        } 

        if (reviveNextTurn)
        {
            if (fakeRagdoll.activeSelf)
                fakeRagdoll.SetActive(false);

            if (original != null)
                original.SetActive(true);

            blood.SetActive(true);

            reviveNextTurn = false;
        }

        scream.Play();

        float distanceEnemyPlayer = Vector3.Distance(transform.position, _player.transform.position);

        if (distanceEnemyPlayer > _detectionRange)
        {
            OnStop?.Invoke();
            _animator.SetBool("Moving", false);
            return;
        }

        PrepareNextMove();

        if (distanceEnemyPlayer < _attackRange)
        {
            CheckAttackState();
        }

        else
        {
            // move normally
            _agent.isStopped = false;

            _attackState = EnemyAttackState.Not;

            StartCoroutine(CheckMovement());

        }
    }

    private IEnumerator CheckMovement()
    {
        while (_distanceTraveled < _maxTravelDistance)
        {
            _distanceTraveled += (_previousRemainingDistance - _agent.remainingDistance);

            _previousRemainingDistance = _agent.remainingDistance;
            yield return new WaitForEndOfFrame();

            if(_agent.remainingDistance <= _agent.stoppingDistance) break;
            
        }

        _agent.velocity = Vector3.zero;
        _agent.isStopped = true;

        CheckAttackState();
    }

    private void CheckAttackState()
    {
        // returns if the distance is more than attack range
        if (Vector3.Distance(transform.position, _player.transform.position) < _attackRange)
        {
            switch (_attackState)
            {
                case EnemyAttackState.Charging:

                    OnAttack?.Invoke();
                    break;

                case EnemyAttackState.Not:
                    if (_chargeAttack)
                    {
                        OnChargingAttack?.Invoke();
                    }
                    else
                    {
                        OnAttack?.Invoke();
                    }

                    break;

                default:
                    break;
            }
        }

        scream.Stop();
        OnStop?.Invoke();
        _animator.SetBool("Moving", false);

    }


    #region NavigationLogic
    private void PrepareNextMove()
    {
        _distanceTraveled = 0;
        _agent.isStopped = true;

        NavMeshPath path = MakePath(transform.position, _player.transform.position);
        _agent.SetPath(path);

        _previousRemainingDistance = _agent.remainingDistance;
    }

    private NavMeshPath MakePath(Vector3 startPos, Vector3 endPos)
    {
        NavMeshPath path = new NavMeshPath();

        if (NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, path))
        {
            return path;
        }
        return path;
    }

    private float GetPathLength(NavMeshPath path)
    {
        float length = 0;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length >= 1))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                length += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }
        return length;
    }
    #endregion

}
