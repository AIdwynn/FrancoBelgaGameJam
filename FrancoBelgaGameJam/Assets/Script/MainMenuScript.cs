using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] Toggle toggle;
    private void Awake()
    {
        new ManagerDeScene().Awake();
        ManagerDeScene.Instance.Start();
        ToggleFamilyFriendly();
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

    public void ToggleFamilyFriendly()
    {
        ParticleLoader.ConfettiMode = toggle.isOn;
        Debug.Log(ParticleLoader.ConfettiMode);
    }
}
