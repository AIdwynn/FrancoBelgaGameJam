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
    }

    private void GetAllEnemies()
    {
        _enemies = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }

    public override void StartTurn()
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.enabled = true;
            enemy.Activate();
        }
    }

    public override void UpdateTurn()
    {

    }

    public override void FixedUpdateTurn()
    {

    }
}
