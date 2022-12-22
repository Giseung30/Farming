using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Public Variable")]
    public float MaxHP; //최대 체력
    public float HP; //현재 체력

    public virtual void Damaged(float Value) { } //데미지 입는 함수
}