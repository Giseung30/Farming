using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eggy : Enemy
{
    [Header("Component")]
    public Transform EnemyTransform; //적 Transform
    public Transform PlayerTransform; //플레이어 Transform
    public Animator EnemyAnimator; //적 Animator
    public Collider EnemyCollider; //적 Collider
    public Rigidbody EnemyRigidbody; //적 Rigidbody

    [Header("Move")]
    public bool IsMove; //움직이는 확인하는 변수
    public float MoveSpeed; //움직이는 속도
    public float RotateSpeed; //회전 속도

    [Header("Script")]
    public UIController UIControllerScript; //UI 관리 스크립트
    public Player PlayerScript; //플레이어 스크립트
    public Progress ProgressScript; //Progress 스크립트

    [Header("Variable")]
    public float DisappeardTime; //죽고나서 사라지는 시간
    public bool IsDeath; //죽었는지 확인하는 변수

    public bool IsAttack; //공격 중인지 확인하는 변수
    public float AttackDamage; //공격 데미지
    public float AttackDistance; //공격 거리

    [Header("Sound")]
    public AudioSource DamagedSound; //피해 입는 사운드

    void Start()
    {
        EnemyTransform = GetComponent<Transform>(); //적 Transform
        PlayerTransform = Static.PlayerTransform; //플레이어 Transform 담음
        EnemyAnimator = GetComponent<Animator>(); //적 Animator
        EnemyCollider = GetComponent<Collider>(); //적 Collider
        EnemyRigidbody = GetComponent<Rigidbody>(); //적 Rigidbody
        HP = MaxHP; //체력 할당
        IsMove = true; //움직일 수 있도록 초기값 설정
        UIControllerScript.CreateEnemyHPImage(GetComponent<Transform>()); //현재 오브젝트의 HP 이미지를 만듦
        PlayerScript = Static.PlayerScript; //플레이어 Script
    }

    void Update()
    {
        FixedRotation();
        AnimatorParameter();
        HPControl();
        MoveToPlayer();
        Attack();
    }

    public void FixedRotation() //회전 각도를 일정하게 유지시키는 함수
    {
        EnemyTransform.rotation = Quaternion.Euler(0f, EnemyTransform.rotation.eulerAngles.y, 0f); //항상 Y축 각도만 움직일 수 있음
    }

    public void AnimatorParameter() //Animator에 파라미터 값 전달하는 함수
    {
        EnemyAnimator.SetBool("IsMove", IsMove);
    }

    public override void Damaged(float Value) //데미지를 입는 함수
    {
        HP -= Value; //수치만큼 체력 감소
        if(!IsAttack) //공격 중이 아니면
            EnemyAnimator.Play("Damaged"); //데미지 입는 애니메이션 실행
        PlayerScript.GiveDamage(this, Value); //데미지를 가했다고 알림
        if (!DamagedSound.isPlaying) DamagedSound.PlayOneShot(DamagedSound.clip); //데미지 입는 사운드 실행
    }

    public void HPControl() //체력 관리하는 함수
    {
        if (HP <= 0 && !IsDeath) //체력이 다 떨어지면
        {
            IsDeath = true; //사망
            EnemyAnimator.Play("Death"); //죽음 애니메이션 실행
            Destroy(EnemyCollider); //충돌 판정 제거
            Destroy(EnemyRigidbody); //리지드바디 제거
            Destroy(gameObject, DisappeardTime); //적 제거
            PlayerScript.Kill(0); //플레이어의 킬 함수 실행
        }
    }

    public void MoveToPlayer() //플레이어를 향해 다가가는 함수
    {
        IsMove = !IsDeath && PlayerTransform.gameObject.activeSelf; //죽거나 플레이어가 없으면 움직일 수 없도록 설정
        if (IsMove) //True 시 움직임
        {
            Vector3 Direction = PlayerTransform.position - EnemyTransform.position; //방향벡터 구함
            EnemyTransform.rotation = Quaternion.Slerp(EnemyTransform.rotation, Quaternion.LookRotation(Direction), RotateSpeed * Time.deltaTime); //해당 방향으로 서서히 회전
            if (!IsAttack) //공격 중이 아니면
            {
                EnemyTransform.Translate(Vector3.forward * Time.deltaTime * MoveSpeed * ProgressScript.GrowthMoveSpeed); //앞을 향해 나아감
            }
        }
    }

    public void Attack() //공격하는 함수
    {
        if (Vector3.Distance(PlayerTransform.position, EnemyTransform.position) < AttackDistance && Static.PlayerTransform.gameObject.activeSelf) //거리가 가까우면서 플레이어가 살아있으면
        {
            IsAttack = true; //공격 실행
        }

        if (IsAttack && !EnemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !IsDeath) //공격을 해야하면서 공격 애니메이션이 실행 중이 아니면서 죽지 않으면
        {
            EnemyAnimator.Play("Attack"); //공격 애니메이션 실행
        }
    }

    public void AttackRangeCheck() //공격 범위 체크하는 함수
    {
        if (Vector3.Distance(PlayerTransform.position, EnemyTransform.position) < AttackDistance) //거리가 가까우면
        {
            PlayerScript.Damaged(this, AttackDamage * ProgressScript.GrowthDamage); //공격 
        }
    }

    public void AttackEnd() //공격 종료하는 함수
    {
        IsAttack = false; //공격 종료
    }
}
