using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndStairs : MonoBehaviour
{
    private bool _isLastLevel;

    private void Start()
    {
        _isLastLevel = ManagerDeScene.CurrentSceneIndex == SceneManager.sceneCountInBuildSettings - 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_isLastLevel)
            {
                SceneTransition.FadeToBlack(ManagerDeScene.LoadMainMenu);
            }
            else
            {
                SceneTransition.FadeToBlack(ManagerDeScene.LoadNextScene);
            }

        }
    }
}
