using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    private void Awake()
    {
        new ManagerDeScene().Awake();
        ManagerDeScene.Instance.Start();
    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void PressedPlay()
    {
        SceneTransition.FadeToBlack(ManagerDeScene.LoadNextScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}