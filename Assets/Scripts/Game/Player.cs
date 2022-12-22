using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("HP")]
    public float MaxHP; //최대 체력
    public float HP; //현재 체력
    public bool IsDeath; //죽었는지 판단하는 변수

    [Header("Move")]
    public bool IsMoved; //움직여야 하는 지 확인하는 변수
    public Vector3 MovedPosition; //움직여야 하는 위치
    public float MoveSpeed; //움직이는 속도
    public float RotateSpeed; //회전 속도

    [Header("RayCast")]
    public Camera MainCamera; //메인 카메라
    public Ray RayLine; //판정선
    public RaycastHit[] RayHits; //레이캐스트 맞는 판정에 대한 결과들

    [Header("Public Skill Variable")]
    public float QCoolTimer; //Q 스킬 쿨타임 계산하는 변수
    public float WCoolTimer; //W 스킬 쿨타임 계산하는 변수

    public float UltimateMaxGauge; //궁극기 최대 게이지
    public float UltimateGauge; //현재 궁극기 게이지
    public float UltimateTimer; //궁극기 계산하는 변수

    public void HPManager() //체력 관리하는 함수
    {
        HP = Mathf.Clamp(HP, 0f, MaxHP); //체력 범위를 벗어나지 않도록 관리
        if (HP <= 0f) //체력이 0이 되면
        {
            gameObject.SetActive(false); //비활성화
            IsDeath = true; //사망
        }
    }
    public virtual void Kill(int EnemyNumber) {} //적을 킬 했을 때 실행되는 함수
    public virtual void GiveDamage(Enemy EnemyScript, float Value) { } //데미지를 줄 때 실행되는 함수
    public virtual void Damaged(Enemy EnemyScript, float Value) { } //데미지를 입는 함수
}