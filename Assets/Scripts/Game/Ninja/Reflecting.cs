using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reflecting : MonoBehaviour
{
    public Ninja PlayerScript; //플레이어 스크립트

    public void OnTriggerStay(Collider Col)
    {
        if (Col.tag == "Enemy") //적에게 닿으면
        {
            Col.gameObject.GetComponent<Enemy>().Damaged(Time.deltaTime * PlayerScript.WDamage); //반사 데미지
        }
    }
}
