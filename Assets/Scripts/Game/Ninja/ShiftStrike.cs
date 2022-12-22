using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftStrike : MonoBehaviour
{
    public Ninja PlayerScript; //플레이어 스크립트

    public void OnTriggerEnter(Collider col) //질풍참 도중 충돌하면
    {
        if (col.tag == "Enemy") //적에게 닿으면
        {
            col.GetComponent<Enemy>().Damaged(PlayerScript.QDamage); //데미지 입힘
            PlayerScript.HP += PlayerScript.QDamage * PlayerScript.QRecoveryRatio; //생명력 흡수
        }
    }
}
