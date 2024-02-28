using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lifeform : MonoBehaviour
{
    public int HP;
    public UnityEvent OnHurt, OnDeath;

    public virtual void Hurt()
    {
        HP--;

        OnHurt.Invoke();    

        if (HP <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        OnDeath.Invoke();
    }
}
