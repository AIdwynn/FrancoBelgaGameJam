using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : Turn
{
    public override void StartTurn()
    {
        GameManager.Player.TurnStart();
    }

    public override void UpdateTurn()
    {
        GameManager.Player.UpdatePlayer();
    }

    public override void FixedUpdateTurn()
    {

    }
}
