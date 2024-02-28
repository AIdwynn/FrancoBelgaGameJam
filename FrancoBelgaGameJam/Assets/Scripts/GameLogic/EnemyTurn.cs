using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurn : Turn
{
    [SerializeField] GameObject debugCanvas;

    public override void StartTurn()
    {
        debugCanvas.SetActive(true);
        StartCoroutine(C_WaitABit());
    }

    public override void UpdateTurn()
    {

    }

    public override void FixedUpdateTurn()
    {

    }

    //Debug, please notify me if modifs are needed here - Thomas


    IEnumerator C_WaitABit()
    {
        yield return new WaitForSeconds(2f);

        debugCanvas.SetActive(false);
        GameManager.EndTurn();
    }
}
