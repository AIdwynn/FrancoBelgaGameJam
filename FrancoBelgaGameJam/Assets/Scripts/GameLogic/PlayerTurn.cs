using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : Turn
{
    [Header("Visualizer")]
    [SerializeField] float visualizerScaleOffset;
    [SerializeField] GameObject distanceVisualizer;
    [HideInInspector] public float DistanceTravelled;

    public override void StartTurn()
    {
        distanceVisualizer.SetActive(true);
        distanceVisualizer.transform.position = GameManager.Player.transform.position;
        DistanceTravelled = 0f;
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
            distanceVisualizer.transform.localScale = new Vector3(dist, 1, dist);

            player.UpdateConstraint(distanceVisualizer.transform.position, dist/2);
        }
        else
        {
            CheckDistanceTravelled(player);
            UpdateVisualizer(player.transform.position);
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
