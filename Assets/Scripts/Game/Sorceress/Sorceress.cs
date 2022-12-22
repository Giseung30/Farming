using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sorceress : Player
{
    [Header("Component")]
    public Transform PlayerTransform;
    public Animator PlayerAnimator;

    [Header("Attack")]
    public bool IsAttack; //공격중인지 확인용 변수
    public float AttackCoolTime; //공격 쿨타임
    public float AttackCoolTimer; //공격 쿨타임 계산하는 변수
    public float AttackDamage; //공격 데미지
    public Vector3 AttackPosition; //공격해야 하는 위치
    public GameObject AttackCollision; //공격 충돌 판정 오브젝트

    [Header("Skill Variable")]
    public bool IsQ; //난사 사용 중인지 확인용 변수
    public float QTime; //난사 유지시간
    public float QTimer; //난사 유지시간 계산하는 변수
    public Quaternion QDirection; //난사를 위해 바라봐야 하는 방향
    public float QDamage; //난사 데미지
    public float QCoolTime; //난사 쿨타임
    public Transform[] QFirePosition; //난사하는 위치
    public float QGapTime; //난사 공백
    public float QGapTimer; //난사 공백 계산하는 변수
    public GameObject RandomFireObject; //난사 오브젝트


    public bool IsW; //다크니스 사용 중인기 확인용 변수
    public float WDamage; //다크니스 데미지
    public float WCoolTime; //다크니스 쿨타임
    public Vector3 WDirection; //다크니스 사용한 방향벡터
    public Transform WFirePosition; //다크니스 발사 위치
    public GameObject DarknessObject; //다크니스 오브젝트

    public bool IsUltimate; //궁극기 사용 중인지 확인용 변수
    public float UltimateTime; //궁극기 유지 시간
    public float HealQuantity; //초당 힐량

    [Header("Skill Effect")]
    public GameObject AttackEffect; //공격 효과
    public GameObject UltimateEffect; //궁극기 효과

    [Header("Sound")]
    public AudioSource AttackSound; //공격 사운드
    public AudioSource RandomFireSound; //난사 사운드
    public AudioSource DarknessSound; //다크니스 사운드
    public AudioSource HealSound; //구원 사운드

    public void Start()
    {
        PlayerTransform = GetComponent<Transform>(); //현재 객체의 Transform 컴포넌트
        PlayerAnimator = GetComponent<Animator>(); //현재 객체의 Animator 컴포넌트
        HP = MaxHP; //체력 초기화
    }

    public void Update()
    {
        HPManager();
        AnimatorParameter();
        FixedRotation();
        RenewalMovedPosition();
        MoveToMovedPosition();
        InputSkill();
        Attack();
        Q();
        W();
        Ultimate();
    }

    public void FixedRotation() //회전 각도를 일정하게 유지시키는 함수
    {
        PlayerTransform.rotation = Quaternion.Euler(0f, PlayerTransform.rotation.eulerAngles.y, 0f); //항상 Y축 각도만 움직일 수 있음
    }

    public void AnimatorParameter() //애니메이터 파라미터 전달하는 함수
    {
        PlayerAnimator.SetBool("IsQ", IsQ);
        PlayerAnimator.SetBool("IsMove", IsMoved);
        PlayerAnimator.SetBool("IsW", IsW);
    }

    public void RenewalMovedPosition() //이동해야 하는 위치를 갱신하는 함수
    {
        if (Input.GetMouseButtonDown(1) && !IsW) //오른쪽 마우스 버튼을 누르면서 다크니스를 사용 중이지 않으면
        {
            RayLine = MainCamera.ScreenPointToRay(Input.mousePosition); //Ray 판정선 만듦
            RayHits = Physics.RaycastAll(RayLine); //RayCast 발동
            for (int i = 0; i < RayHits.Length; i += 1) //닿은 모든 물체들을 탐색
            {
                if (RayHits[i].transform.tag == "Ground") //지면에 닿으면
                {
                    IsMoved = true; //움직여야 함
                    MovedPosition = RayHits[i].point; //닿은 위치로 이동할 것
                    break; //반복문 빠져나옴
                }
            }
        }
    }

    public void MoveToMovedPosition() //이동해야 하는 위치로 이동하는 함수
    {
        if (IsMoved) //움직여야 하는 위치가 존재하면
        {
            if (Vector3.Distance(MovedPosition, PlayerTransform.position) < 1f) IsMoved = false; //목표 지점과 가까워지면 이동 중지
            Vector3 Direction = Vector3.Normalize(MovedPosition - PlayerTransform.position); //방향벡터 계산
            PlayerTransform.position += Direction * Time.deltaTime * MoveSpeed; //이동 속도
            if (!IsQ) //난사 중이지 않으면
            {
                PlayerTransform.rotation = Quaternion.Slerp(PlayerTransform.rotation, Quaternion.LookRotation(Direction), RotateSpeed * Time.deltaTime); //바라보는 방향으로 회전
            }
        }
    }

    public void InputSkill() //스킬을 입력하는 함수
    {
        if (Input.GetMouseButtonDown(0) && AttackCoolTimer < 0 && !AvoidDuplicateAttacks()) //공격을 하면
        {
            RayLine = MainCamera.ScreenPointToRay(Input.mousePosition); //Ray 판정선 만듦
            RayHits = Physics.RaycastAll(RayLine); //RayCast 발동
            for (int i = 0; i < RayHits.Length; i += 1) //닿은 모든 물체들을 탐색
            {
                if (RayHits[i].transform.tag == "Ground") //지면에 닿으면
                {
                    IsAttack = true; //공격
                    AttackCoolTimer = AttackCoolTime; //쿨타임
                    IsMoved = false; //움직임 금지
                    AttackPosition = RayHits[i].point; //닿은 위치를 공격 위치로 지정
                    AttackSound.PlayOneShot(AttackSound.clip); //공격 사운드 실행
                    break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Q) && QCoolTimer < 0 && !AvoidDuplicateAttacks()) //난사 사용
        {
            RayLine = MainCamera.ScreenPointToRay(Input.mousePosition); //Ray 판정선 만듦
            RayHits = Physics.RaycastAll(RayLine); //RayCast 발동
            for (int i = 0; i < RayHits.Length; i += 1) //닿은 모든 물체들을 탐색
            {
                if (RayHits[i].transform.tag == "Ground") //지면에 닿으면
                {
                    IsQ = true; //난사 사용
                    QCoolTimer = QCoolTime; //쿨타임
                    QTimer = QTime; //유지시간 지정
                    PlayerAnimator.Play("Q"); //난사 애니메이션 실행
                    QDirection = Quaternion.LookRotation(RayHits[i].point - PlayerTransform.position); //방향 벡터로 바라봐야하는 방향 계산
                    break;
                }
            }

        }
        if (Input.GetKeyDown(KeyCode.W) && WCoolTimer < 0 && !AvoidDuplicateAttacks()) //다크니스 사용하면
        {
            RayLine = MainCamera.ScreenPointToRay(Input.mousePosition); //Ray 판정선 만듦
            RayHits = Physics.RaycastAll(RayLine); //RayCast 발동
            for (int i = 0; i < RayHits.Length; i += 1) //닿은 모든 물체들을 탐색
            {
                if (RayHits[i].transform.tag == "Ground") //지면에 닿으면
                {
                    IsW = true; //다크니스 사용
                    WCoolTimer = WCoolTime; //쿨타임
                    IsMoved = false; //이동 종료
                    WDirection = RayHits[i].point - PlayerTransform.position; //바라봐야 하는 방향 초기화
                    DarknessSound.PlayOneShot(DarknessSound.clip); //다크니스 사운드 실행
                    break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && UltimateGauge >= UltimateMaxGauge) //궁극기 사용하면
        {
            IsUltimate = true; //궁극기 사용
            UltimateGauge = 0f; //궁극기 게이지 초기화
            UltimateTimer = UltimateTime; //궁극기 시간 초기화
            UltimateEffect.SetActive(true); //궁극기 효과 재생
            HealSound.Play(); //궁극기 사운드 실행
        }
    }

    public void Attack() //기본 공격 관련 함수
    {
        AttackCoolTimer -= Time.deltaTime; //쿨 타임 계산
        if (IsAttack)//공격하면
        {
            Vector3 Direction = AttackPosition - PlayerTransform.position; //방향벡터 계산
            PlayerTransform.rotation = Quaternion.Slerp(PlayerTransform.rotation, Quaternion.LookRotation(Direction), 100f * Time.deltaTime); //바라보는 방향으로 회전
            if (Vector3.Angle(Direction, PlayerTransform.forward) < 0.05f) //각도가 어느정도 줄어들면
            {
                PlayerAnimator.Play("Attack"); //공격 애니메이션 실행
                IsAttack = false; //공격 끝
            }
        }
    }

    public void ActiveAttack(int State) //공격 판정 활성화 / 비활성화 시키는 함수
    {
        if (State == 1) //공격
        {
            Instantiate(AttackEffect, AttackEffect.GetComponent<Transform>().position, AttackEffect.GetComponent<Transform>().rotation).SetActive(true); //공격 효과 생성
        }
        AttackCollision.SetActive(State == 1); //공격 판정 활성화 / 비활성화
    }

    public void Q() //난사 관련 함수
    {
        QCoolTimer -= Time.deltaTime; //쿨 타임 계산
        if (IsQ) //난사 사용하면
        {
            if (QTimer < 0f) //난사 지속 시간이 끝나면
            {
                IsQ = false; //난사 종료
            }
            PlayerTransform.rotation = Quaternion.Slerp(PlayerTransform.rotation, QDirection, 100f * Time.deltaTime); //바라보는 방향으로 서서히 회전
            if (QGapTimer < 0f) //공백 시간이 지나면
            {
                int RandomPositionIndex = Random.Range(0, QFirePosition.Length); //어디로 발사할 지 정함
                Instantiate(RandomFireObject, QFirePosition[RandomPositionIndex].position, QFirePosition[RandomPositionIndex].rotation).SetActive(true); //발사 오브젝트 생성
                RandomFireSound.PlayOneShot(RandomFireSound.clip); //발사 사운드 실행
                QGapTimer = QGapTime; //공백 시간 초기화
            }
            QGapTimer -= Time.deltaTime; //공백 시간 감소
            QTimer -= Time.deltaTime; //유지 시간 감소
        }
    }

    public void W() //다크니스 관련 함수
    {
        WCoolTimer -= Time.deltaTime; //쿨타임 계산
        if (IsW) //다크니스 사용 중이면
        {
            if (Vector3.Angle(PlayerTransform.forward, WDirection) < 0.05f) //각도가 어느 정도 줄어들면
            {
                PlayerAnimator.Play("W"); //다크니스 애니메이션 실행
            }
            PlayerTransform.rotation = Quaternion.Slerp(PlayerTransform.rotation, Quaternion.LookRotation(WDirection), 100f * Time.deltaTime); //바라봐야 하는 방향 초기화
        }
    }

    public void DarknessActive() //다크니스 활성화
    {
        Instantiate(DarknessObject, WFirePosition.position, WFirePosition.rotation).SetActive(true); //다크니스 오브젝트 생성
        IsW = false; //다크니스 종료
    }

    public void Ultimate() //궁극기 관련 함수
    {
        if (IsUltimate) //궁극기 사용 중이면
        {
            UltimateTimer -= Time.deltaTime; //궁극기 지속시간 계산
            HP += HealQuantity * Time.deltaTime; //체력 지속적으로 증가
            if (UltimateTimer < 0f) //지속 시간이 다 되면
            {
                UltimateEffect.SetActive(false); //궁극기 효과 종료
                IsUltimate = false; //궁극기 끝
                HealSound.Stop(); //궁극기 사운드 종료
            }
        }
        else //궁극기 사용중이지 않으면
        {
            UltimateGauge += (MaxHP - HP) * Time.deltaTime; //체력이 적을수록 궁극기 게이지 충전량 증가
        }
    }

    public bool AvoidDuplicateAttacks() //공격 중복이 일어나지 않도록 하는 함수
    {
        return IsAttack || IsQ || IsW; //하나라도 실행되어 있으면 True
    }

    public override void Kill(int EnemyNumber) //적을 킬 했을 때 실행되는 함수
    {
        Static.Kill[EnemyNumber]++; //킬 수 증가
    }

    public override void GiveDamage(Enemy EnemyScript, float Value) //데미지를 줄 때 실행되는 함수
    {
        /* 궁극기 증가는 구현되지 않는 영웅 */
    }

    public override void Damaged(Enemy EnemyScript, float Value) //데미지를 입는 함수
    {
        if (!IsDeath) //죽지 않으면
        {
            HP -= Value; //체력 감소
        }
    }
}
