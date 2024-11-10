using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeWeakSpot : MonoBehaviour, IDamage
{
    [SerializeField] OrangeAI ParentEnemy;
    public void takeDamage(int amount, Vector3 dir, damageType type)
    {
        //Debug.Log("hit weak spot");
        ParentEnemy.Damage(amount);
    }
}
