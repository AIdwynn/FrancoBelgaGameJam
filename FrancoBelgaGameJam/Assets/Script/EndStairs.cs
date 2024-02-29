using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndStairs : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneTransition.FadeToBlack(ManagerDeScene.LoadNextScene);
        }
    }
}
