using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFire : MonoBehaviour
{
    [Header("Component")]
    public Transform RandomFireObjectTransform; //난사 오브젝트 Transform

    [Header("Script")]
    public Sorceress PlayerScript; //플레이어 스크립트

    [Header("Variable")]
    public float MoveSpeed; //이동 속도

    public void Start()
    {
        RandomFireObjectTransform = GetComponent<Transform>(); //난사 오브젝트 Transform 담음
    }
    public void OnTriggerEnter(Collider Col)
    {
        if(Col.tag == "Enemy") //적과 충돌하면
        {
            Col.GetComponent<Enemy>().Damaged(PlayerScript.QDamage); //데미지 가함
        }
    }

    public void Update()
    {
        RandomFireObjectTransform.Translate(Vector3.forward * Time.deltaTime * MoveSpeed); //앞으로 나아감
    }
}
