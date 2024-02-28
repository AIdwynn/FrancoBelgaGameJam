using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Turn : MonoBehaviour
{
    public abstract void StartTurn();
    public abstract void UpdateTurn();
    public abstract void FixedUpdateTurn();
}
