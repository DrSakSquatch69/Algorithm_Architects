using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [SerializeField] int explodeDur;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(duration());
    }

    IEnumerator duration()
    {
        yield return new WaitForSeconds(explodeDur);
        Destroy(gameObject);
    }
}
