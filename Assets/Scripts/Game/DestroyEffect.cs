using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    [System.Obsolete]
    void Start()
    {
        Destroy(gameObject, GetComponent<ParticleSystem>().duration); //효과 재생 후 제거
    }
}
