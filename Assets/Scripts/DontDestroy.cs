using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{

    public void Awake()
    {
        var Obj = FindObjectsOfType<DontDestroy>();
        if (Obj.Length == 1)
        {
            DontDestroyOnLoad(gameObject); //현재 오브젝트 파괴 안함
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
