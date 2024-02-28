using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance;

    [Header("Difficulty")]
    public bool EasyMode;

    [Header("Turn Settings")]
    [SerializeField] float maxPlayerDistance;
    [SerializeField] float maxEnemyDistance;
    [SerializeField] int startIndex;
    [SerializeField] List<Turn> turns = new List<Turn>();
    int currentTurnIndex;
    Turn currentTurn;

    [Header("References")]
    [SerializeField] Transform playerStart;
    #endregion

    #region Accessors
    public Turn CurrentTurn { get { return currentTurn; } }
    public float MaxPlayerDistance { get { return maxPlayerDistance; } }
    public float MaxEnemyDistance { get { return maxEnemyDistance; } }

    [HideInInspector] public PlayerController Player;
    #endregion

    #region MainLoop
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        GameObject player = FindObjectOfType<PlayerController>().gameObject;

        if (player == null)
            player = Instantiate((GameObject)Resources.Load("Essentials/PlayerController"));

        player.transform.position = playerStart.position;
        player.transform.forward = playerStart.forward;

        Player = player.GetComponent<PlayerController>();

        foreach (var item in turns)
        {
            item.GameManager = this;
        }

        Destroy(playerStart.gameObject);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Player.Init();

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

        if (currentTurnIndex >= turns.Count)
            currentTurnIndex = 0;

        currentTurn = turns[currentTurnIndex];

        currentTurn.StartTurn();
    }

    public void StartTurn(bool dontIncrement)
    {
        if (!dontIncrement)
            currentTurnIndex++;

        if (currentTurnIndex >= turns.Count)
            currentTurnIndex = 0;

        currentTurn = turns[currentTurnIndex];

        currentTurn.StartTurn();
    }

    public void EndTurn()
    {
        if (currentTurnIndex >= turns.Count)
        {
            currentTurnIndex = 0;

            //Add delay and feedback

            StartTurn(true);
        }
        else
        {
            //Add delay and feedback

            StartTurn();
        }
    }
    #endregion
}
