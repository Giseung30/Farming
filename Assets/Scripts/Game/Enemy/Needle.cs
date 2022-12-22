using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needle : MonoBehaviour
{
    [Header("Script")]
    public Chili ChiliScript; //Chili 스크립트

    [Header("Component")]
    public Transform NeedleTransform; //바늘 Transform

    [Header("Variable")]
    public float MoveSpeed; //바늘 이동 속도

    public void Start()
    {
        Destroy(gameObject, 10f); //잠시 뒤 파괴
        NeedleTransform = GetComponent<Transform>(); //현재 Transform을 담음
    }

    public void OnTriggerEnter(Collider Col)
    {
        if (Col.tag == "Player") //플레이어와 닿으면
        {
            Col.gameObject.GetComponent<Player>().Damaged(ChiliScript, ChiliScript.AttackDamage * ChiliScript.ProgressScript.GrowthDamage); //데미지 가함
            Destroy(gameObject); //표창 파괴
        }
    }

    public void Update()
    {
        NeedleTransform.Translate(Vector3.forward * MoveSpeed * Time.deltaTime); //앞으로 이동
    }
}
