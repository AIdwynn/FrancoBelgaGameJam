using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public enum EnemyAttackState
{
    Not, Charging
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private float _detectionRange;
    [SerializeField] private float _maxTravelDistance;
    [SerializeField] private float _attackRange;

    [Header ("Editor")]
    [SerializeField] private bool _drawGizmos;

    private Transform _player;
    private float _distanceTraveled;
    private float _previousRemainingDistance;
    private EnemyAttackState _attackState;

    private Lifeform _health;

    public Action OnStop;

    public Action OnChargingAttack;
    public Action OnAttack;

    public bool IsStunned { get; set; }
    public bool IsMoving { get { return !_agent.isStopped; } }

    #region UnityMethods

    private void Start()
    {
        _player = GameManager.Instance.Player.transform;
        _agent = GetComponent<NavMeshAgent>();
        _agent.isStopped = true;
        _attackState = EnemyAttackState.Not;

        OnAttack += () => { 
            //_player.GetComponent<Lifeform>().Hurt();
            _attackState = EnemyAttackState.Not; };

        OnChargingAttack += () => { _attackState = EnemyAttackState.Charging; };

        _health = GetComponent<Lifeform>();

        _health.OnDeath.AddListener(KillEnemy);

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
    private void KillEnemy()
    {
        Destroy(gameObject);
    }

    public void Activate()
    {
        if (IsStunned) // if enemy is stunned then don't activate enemy
        {
            OnStop?.Invoke(); 
            return;
        } 

        float distanceEnemyPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceEnemyPlayer > _detectionRange)
        {
            OnStop?.Invoke(); 
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
        if (Vector3.Distance(transform.position, _player.position) < _attackRange)
        {
            switch (_attackState)
            {
                case EnemyAttackState.Charging:
                    OnAttack?.Invoke();
                    break;

                case EnemyAttackState.Not:
                    OnChargingAttack?.Invoke();
                    break;

                default:
                    break;
            }
        }

        OnStop?.Invoke();

        Debug.Log(_attackState);

    }


    #region NavigationLogic
    private void PrepareNextMove()
    {
        _distanceTraveled = 0;
        _agent.isStopped = true;

        NavMeshPath path = MakePath(transform.position, _player.position);
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