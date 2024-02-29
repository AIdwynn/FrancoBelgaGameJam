using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOneRef : MonoBehaviour
{
    CinemachineImpulseSource shake;

    private void Awake()
    {
        shake = GetComponent<CinemachineImpulseSource>();
    }
    public void Shake()
    {
        shake.GenerateImpulse();
    }
}
