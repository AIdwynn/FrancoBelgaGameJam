using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurn : Turn
{
    private Enemy[] _enemies = new Enemy[0];

    private void Start()
    {
        GetAllEnemies();
        foreach (Enemy enemy in _enemies)
        {
            enemy.OnStop += () => CheckForAllEnemies();
        }
    }

    private void GetAllEnemies()
    {
        _enemies = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }

    public override void StartTurn()
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.Activate();
        }
    }

    public override void UpdateTurn()
    {

    }

    public override void FixedUpdateTurn()
    {

    }

    public void CheckForAllEnemies()
    {
        bool canEnd = true;

        foreach(Enemy enemy in _enemies)
        {
            if (enemy.IsMoving)
            {
                canEnd = false;
                break;
            }

        }

        if (canEnd)
        {
            GameManager.EndTurn();
        }
    }
}
