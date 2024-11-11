using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrangeWeakSpot : MonoBehaviour, IDamage
{
    [SerializeField] OrangeAI ParentEnemy;
    private List<Renderer> ParentBody;
    void Start()
    {
        ParentBody = ParentEnemy.Body;    
    }
    public void takeDamage(int amount, Vector3 dir, damageType type)
    {
        //Debug.Log("hit weak spot");
        DamageParent(amount, dir, type);
    }
    void DamageParent(int amount, Vector3 dir, damageType type)
    {
        StartCoroutine(TakeDamage(amount, dir, type, ParentBody, ParentBody[0].material.color));
    }
    IEnumerator TakeDamage(int amount, Vector3 dir, damageType type, List<Renderer> models, Color origColor)
    {
        ParentEnemy.Damage(amount);
        for(int i = 0; i < models.Count - 1; i++)
        {
            StartCoroutine(flashColor(models[i], origColor));
        }
        yield return new WaitForSeconds(0);
    }
    IEnumerator flashColor(Renderer model, Color origColor)
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = origColor;
    }
}
