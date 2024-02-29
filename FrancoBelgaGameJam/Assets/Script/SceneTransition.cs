using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private CanvasGroup _blackScreen;

    private static SceneTransition Instance;

    private void Start()
    {
        // fail safe
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        StartCoroutine(FadeFromBlack());
    }

    public static void FadeToBlack(Action doAfterFade = null)
    {
        Instance.StartCoroutine(Instance.FadeToBlackCoroutine(doAfterFade));
    }

    private IEnumerator FadeToBlackCoroutine(Action doAfterFade = null)
    {
        float x = 0;

        while (x <= 1)
        {
            x += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
            Instance._blackScreen.alpha = x;
        }

        Instance._blackScreen.alpha = 1;

        doAfterFade?.Invoke();

    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeFromBlack()
    {
        float x = 1;

        _blackScreen.alpha = 1;

        while (x >= 0)
        {
            x -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
            _blackScreen.alpha = x;
        }

        _blackScreen.alpha = 0;

    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            ManagerDeScene.RestartCurrentScene();
        }
    }
}
