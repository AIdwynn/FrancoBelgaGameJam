using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Turn : MonoBehaviour
{
    [HideInInspector] public GameManager GameManager;

    public abstract void StartTurn();
    public abstract void UpdateTurn();
    public abstract void FixedUpdateTurn();
}
