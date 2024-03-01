using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] GameObject swap, original, particle;

    bool swapped;

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            if (!swapped)
            {
                swapped = true;
                original.SetActive(false);
                swap.SetActive(true);
                particle.SetActive(true);
                StartCoroutine(C_Restart());
            }
        }
    }

    IEnumerator C_Restart()
    {
        yield return new WaitForSecondsRealtime(8f);

        ManagerDeScene.LoadMainMenu();
    }
}
