using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyTurn : Turn
{
    private Enemy[] _enemies = new Enemy[0];
    private Enemy[] _doneEnemies = new Enemy[10];

    public Enemy[] Enemies { get { return _enemies; } }

    private int _index = 0; 

    private void Start()
    {
        GetAllEnemies();
        foreach (Enemy enemy in _enemies)
        {
            enemy.OnStop += () => CheckForAllEnemies(enemy);
        }
    }

    private void GetAllEnemies()
    {
        _enemies = GameObject.FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }

    public override void StartTurn()
    {
        _index = 0;
        GetAllEnemies();
        _doneEnemies = new Enemy[_enemies.Length];

        foreach (Enemy enemy in _enemies)
        {
            enemy.TurnStart();
            enemy.Activate();
        }

        if(_enemies.Length == 0)
        {
            GameManager.EndTurn();
        }
    }

    public override void UpdateTurn()
    {

    }

    public override void FixedUpdateTurn()
    {

    }

    public void CheckForAllEnemies(Enemy addToDone)
    {
        if(_doneEnemies.GetValue(_index) == null)
        {
            if (!_doneEnemies.Contains(addToDone))
            {
                _doneEnemies.SetValue(addToDone, _index);

                _index++;
            }
        }
        bool stillHasNull = false;

        foreach (Enemy enemy in _doneEnemies)
        {
            if (enemy == null)
            {
                stillHasNull = true;
                break;
            }
        }

        if (_doneEnemies.Length == 0 || !stillHasNull)
        {
            GameManager.EndTurn();
        }






        //bool canEnd = true;

        //foreach(Enemy enemy in _enemies)
        //{
        //    if (enemy.IsMoving)
        //    {
        //        canEnd = false;
        //        break;
        //    }

        //}

        //if (canEnd)
        //{
        //    GameManager.EndTurn();
        //}
    }
}
