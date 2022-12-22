using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    [Header("Component")]
    public Transform ShurikenTransform; //표창 Transform

    [Header("Script")]
    public Ninja PlayerScript; //Player 스크립트

    [Header("Variable")]
    public Vector3 Directon; //날아가야할 방향 벡터
    public float MoveSpeed; //날아가는 속도
    public float RotateSpeed; //회전 속도
    public void OnTriggerEnter(Collider Col)
    {
        if (Col.tag == "Enemy") //적과 닿으면
        {
            Col.GetComponent<Enemy>().Damaged(PlayerScript.AttackDamage); //데미지 가함
            Destroy(gameObject); //표창 파괴
        }
    }

    public void Start()
    {
        ShurikenTransform = GetComponent<Transform>(); //표창 Transform
        Directon = Vector3.Normalize(Directon); //단위 벡터로 변경
        Destroy(gameObject, 10f); //10초 뒤에 파괴
    }

    public void Update()
    {
        ShurikenTransform.position += Directon * Time.deltaTime * MoveSpeed; //방향벡터로 날아감
        ShurikenTransform.Rotate(0f, RotateSpeed * Time.deltaTime, 0f); //표창 회전
    }
}