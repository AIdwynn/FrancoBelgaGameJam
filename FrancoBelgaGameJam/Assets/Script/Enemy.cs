using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;

    [SerializeField] private float _detectionRange;
    [SerializeField] private float _maxTravelDistance;
    [SerializeField] private float _attackRange;

    private Transform _player;   
    private float _distanceTraveled;
    private float _previousRemainingDistance;

    public bool IsStunned {  get; set; }

    public void Activate()
    {
        _distanceTraveled = 0;

        if (IsStunned) return; // if enemy is stunned then don't activate enemy

        float distanceEnemyPlayer = Vector3.Distance(transform.position, _player.position);

        if (distanceEnemyPlayer > _detectionRange) return;

        if (distanceEnemyPlayer < _attackRange)
        {
            CheckAttackState();
        }

        else
        {
            // move normally
            _agent.isStopped = false;

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
        }

        _agent.isStopped = true;
    }

    private void CheckAttackState()
    {
        Debug.Log("Is close enough to attack");
    }

    #region UnityMethods
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

    }

    private void OnEnable()
    {
        _agent = GetComponent<NavMeshAgent>();
        PrepareNextMove();

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _maxTravelDistance);
    }

    #endregion

    private void PrepareNextMove()
    {
        _agent.isStopped = true;

        NavMeshPath path = MakePath(transform.position, _player.position);
        _agent.SetPath(path);

        _previousRemainingDistance = _agent.remainingDistance;
    }

    private NavMeshPath MakePath(Vector3 startPos, Vector3 endPos)
    {
        NavMeshPath path = new NavMeshPath();

        if(NavMesh.CalculatePath(startPos, endPos, NavMesh.AllAreas, path))
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


}
