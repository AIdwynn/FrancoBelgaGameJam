using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{

    public void PressedPlay()
    {
        SceneTransition.FadeToBlack(ManagerDeScene.LoadNextScene);
    }
}
