using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance;

    [SerializeField] int startIndex;
    [SerializeField] List<Turn> turns = new List<Turn>();
    int currentTurnIndex;
    Turn currentTurn;
    #endregion

    #region Accessors
    public Turn CurrentTurn { get { return currentTurn; } }
    #endregion

    #region MainLoop
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        StartTurn(true);
    }

    private void Update()
    {
        if (currentTurn != null)
            currentTurn.UpdateTurn();
    }

    private void FixedUpdate()
    {
        if (currentTurn != null)
            currentTurn.FixedUpdateTurn();
    }
    #endregion

    #region Turn Start/End
    public void StartTurn()
    {
        currentTurnIndex++;

        currentTurn = turns[currentTurnIndex];

        currentTurn.StartTurn();
    }

    public void StartTurn(bool dontIncrement)
    {
        if (dontIncrement)
            currentTurnIndex++;

        currentTurn = turns[currentTurnIndex];

        currentTurn.StartTurn();
    }

    public void EndTurn()
    {
        if (currentTurnIndex >= turns.Count)
        {
            currentTurnIndex = 0;
        }

        StartTurn(true);
    }
    #endregion
}
