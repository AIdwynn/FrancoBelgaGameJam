using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSelfDestruct : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1;
    public void StartCountDown()
    {
        StartCoroutine(CountDown());
    }

    public IEnumerator CountDown()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(this.gameObject);
    }
}
