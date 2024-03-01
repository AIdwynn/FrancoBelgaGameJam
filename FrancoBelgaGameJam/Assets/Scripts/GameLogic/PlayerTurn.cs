using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : Turn
{
    [Header("Visualizer")]
    float visualizerScaleOffset;
    [SerializeField] GameObject distanceVisualizer;
    [HideInInspector] public float DistanceTravelled;

    public override void StartTurn()
    {
        GameManager.Player.TurnStart();

        visualizerScaleOffset = GameManager.VisualizerScaleOffset;

        distanceVisualizer.SetActive(true);
        distanceVisualizer.transform.position = GameManager.Player.transform.position;
        DistanceTravelled = 0f;

        foreach (var item in GameManager.EnemyTurn.Enemies)
        {
            item.UpdateVisualizer();
        }

        GameManager.UIManager.UpdateHP(GameManager.Player.HP);
    }

    public override void UpdateTurn()
    {
        var player = GameManager.Player;

        if (player == null)
            return;

        player.UpdatePlayer(GameManager.EasyMode);

        if (GameManager.EasyMode)
        {
            var dist = GameManager.MaxPlayerDistance * visualizerScaleOffset;
            distanceVisualizer.transform.position = new Vector3(distanceVisualizer.transform.position.x,
                GameManager.Player.transform.position.y, distanceVisualizer.transform.position.z);

            distanceVisualizer.transform.localScale = new Vector3(dist, 1, dist);

            player.UpdateConstraint(distanceVisualizer.transform.position, dist/2);
        }
        else
        {
            CheckDistanceTravelled(player);
            UpdateVisualizer(player.transform.position);
            GameManager.UIManager.UpdateMovementBar(DistanceTravelled);

        }
    }

    private void CheckDistanceTravelled(PlayerController player)
    {
        if (player.Moving)
            DistanceTravelled += player.WalkSpeed * Time.deltaTime;

        if (DistanceTravelled > GameManager.MaxPlayerDistance && player.CanMove)
        {
            distanceVisualizer.SetActive(false);
            player.Freeze();
            player.CanQuickSkip();
            GameManager.UIManager.ShowQuickSkip(true);
        }
    }

    public override void FixedUpdateTurn()
    {

    }

    private void UpdateVisualizer(Vector3 pos)
    {
        distanceVisualizer.transform.position = pos;

        var distanceLeft = Mathf.Abs(GameManager.MaxPlayerDistance - DistanceTravelled) * visualizerScaleOffset;


        distanceVisualizer.transform.localScale = new Vector3(distanceLeft, 1, distanceLeft);
    }
}
