using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorceressAttack : MonoBehaviour
{
    public Sorceress PlayerScript; //플레이어 Script
    public void OnTriggerEnter(Collider Col)
    {
        if(Col.tag == "Enemy") //적과 충돌하면
        {
            Col.GetComponent<Enemy>().Damaged(PlayerScript.AttackDamage); //데미지 가함
        }
    }
}
