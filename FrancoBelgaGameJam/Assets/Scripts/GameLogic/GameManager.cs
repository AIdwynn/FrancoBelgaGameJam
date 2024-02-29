using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    #region Variables
    public static GameManager Instance;

    [Header("Difficulty")]
    public bool EasyMode;

    [Header("Turn Settings")]
    [SerializeField] float maxPlayerDistance;
    public float EnemyRangeOffset;
    public float VisualizerScaleOffset;
    [SerializeField] int startIndex;
    [SerializeField] List<Turn> turns = new List<Turn>();
    int currentTurnIndex;
    Turn currentTurn;

    [Header("References")]
    [SerializeField] Transform playerStart;
    [SerializeField] AudioSource music;
    public UIManager UIManager;

    public EnemyTurn EnemyTurn { get { return turns[1].GetComponent<EnemyTurn>(); } }
    #endregion

    #region Accessors
    public Turn CurrentTurn { get { return currentTurn; } }
    public float MaxPlayerDistance { get { return maxPlayerDistance; } }

    [HideInInspector] public PlayerController Player;
    #endregion

    #region MainLoop
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        var player = FindObjectOfType<PlayerController>();

        GameObject playerObject = null;

        if (player == null)
            playerObject = Instantiate((GameObject)Resources.Load("Essentials/PlayerController"));
        else
            playerObject = player.gameObject;

        playerObject.transform.position = playerStart.position;
        playerObject.transform.forward = playerStart.forward;

        Player = playerObject.GetComponent<PlayerController>();

        foreach (var item in turns)
        {
            item.GameManager = this;
        }
        new ManagerDeScene().Awake();
        Destroy(playerStart.gameObject);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Player.Init();

        StartTurn(true);
        ManagerDeScene.Instance.Start();
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

    public void Restart()
    {
        ManagerDeScene.RestartCurrentScene();
    }

    public void Cut()
    {
        music.DOFade(0, 0.1f);

        StartCoroutine(C_FadeInAgain());
    }

    IEnumerator C_FadeInAgain()
    {
        yield return new WaitForSecondsRealtime(4f);

        music.DOFade(0.2f, 4f);
    }

    public void AddAmmo()
    {
        Player.AddAmmo();
        ManagerDeParticle.PlayParticleByName(ParticleNames.EnergyCollect, Player.transform.position);
    }

    #region Turn Start/End
    public void StartTurn()
    {
        currentTurnIndex++;

        if (currentTurnIndex >= turns.Count)
            currentTurnIndex = 0;

        currentTurn = turns[currentTurnIndex];

        currentTurn.StartTurn();
        UIManager.UpdateTurnText(currentTurn.name);
    }

    public void StartTurn(bool dontIncrement)
    {
        if (!dontIncrement)
            currentTurnIndex++;

        if (currentTurnIndex >= turns.Count)
            currentTurnIndex = 0;

        currentTurn = turns[currentTurnIndex];

        currentTurn.StartTurn();
        UIManager.UpdateTurnText(currentTurn.name);
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
