using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChildFriendlyFilter : MonoBehaviour
{
    [SerializeField] GameObject blood, confetti;

    private void Start()
    {
        blood.SetActive(!ParticleLoader.ConfettiMode);
        confetti.SetActive(ParticleLoader.ConfettiMode);
    }
}
