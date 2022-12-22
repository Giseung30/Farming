using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ninja : Player
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
    public GameObject Shuriken; //표창 프리팸
    public Transform FirePosition; //발사 위치

    [Header("Skill Variable")]
    public bool IsQ; //질풍참 사용 중인지 확인용 변수
    public Vector3 QPosition; //질풍참 사용한 위치
    public float QDistance; //질풍참 거리
    public GameObject QCollision; //질풍참 충돌 판정 오브젝트
    public float QDamage; //질풍참 데미지
    public float QCoolTime; //질풍참 쿨타임
    public float QRecoveryRatio; //질풍참 회복 비율

    public bool IsW; //튕겨내기 사용 중인기 확인용 변수
    public float WTime; //튕겨내기 유지 시간
    public float WTimer; //튕겨내기 계산하는 변수
    public Quaternion WDirection; //튕겨내기를 위해 바라봐야하는 방향
    public GameObject WCollision; //튕겨내기 충돌 판정 오브젝트
    public float WDamage; //튕겨내기 데미지
    public float WCoolTime; //튕겨내기 쿨타임

    public bool IsUltimate; //궁극기 사용 중인지 확인용 변수
    public float UltimateTime = 10f; //궁극기 유지 시간
    public float UltimateIncrease; //궁극기 서서히 증가하는 양
    public float[] ForUltimateBaseValue; //궁극기 종료 후 원래 수치로 돌아가기 위한 배열

    [Header("Skill Effect")]
    public GameObject NinjaAfterimage; //닌자 잔상 오브젝트
    public ParticleSystem UltimateEffect; //궁극기 효과

    [Header("Sound")]
    public AudioSource AttackSound; //공격 사운드
    public AudioSource ShiftStrikeSound; //질풍참 사운드
    public AudioSource ReflectingSound; //반사 사운드
    public AudioSource DominusSound; //궁극기 사운드
    public AudioSource WalkSound; //걷는 소리
    public void Start()
    {
        PlayerTransform = GetComponent<Transform>(); //현재 객체의 Transform 컴포넌트
        PlayerAnimator = GetComponent<Animator>(); //현재 객체의 Animator 컴포넌트

        ForUltimateBaseValue = new float[5]; //배열 크기 초기화
        ForUltimateBaseValue[0] = AttackDamage;
        ForUltimateBaseValue[1] = QCoolTime;
        ForUltimateBaseValue[2] = QDamage;
        ForUltimateBaseValue[3] = WCoolTime;
        ForUltimateBaseValue[4] = WDamage;

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
        if (Input.GetMouseButtonDown(1)) //오른쪽 마우스 버튼을 누르면
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
            if (!IsW) //튕겨내기가 실행 중이지 않으면
            {
                PlayerTransform.rotation = Quaternion.Slerp(PlayerTransform.rotation, Quaternion.LookRotation(Direction), RotateSpeed * Time.deltaTime); //바라보는 방향으로 회전
            }
        }
    }
    public void PlayWalkSound() //걷는 소리 실행시키는 함수
    {
        WalkSound.PlayOneShot(WalkSound.clip); //걷는 소리 실행
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
        if (Input.GetKeyDown(KeyCode.Q) && QCoolTimer < 0) //질풍참 사용
        {
            RayLine = MainCamera.ScreenPointToRay(Input.mousePosition); //Ray 판정선 만듦
            RayHits = Physics.RaycastAll(RayLine); //RayCast 발동
            for (int i = 0; i < RayHits.Length; i += 1) //닿은 모든 물체들을 탐색
            {
                if (RayHits[i].transform.tag == "Ground") //지면에 닿으면
                {
                    IsQ = true; //질풍참 사용
                    QCoolTimer = QCoolTime; //쿨타임
                    IsMoved = false; //움직임 종료
                    WTimer = 0f; //튕겨내기 종료
                    QPosition = PlayerTransform.position + Vector3.Normalize(RayHits[i].point - PlayerTransform.position) * QDistance; //질풍참 사용한 위치
                    QPosition = new Vector3(Mathf.Clamp(QPosition.x, -8.5f, 51f), QPosition.y, Mathf.Clamp(QPosition.z, -35f, 23.5f)); //범위 제한
                    PlayerAnimator.Play("Q"); //질풍참 애니메이션 실행
                    QCollision.SetActive(true); //충돌 판정 실행
                    ShiftStrikeSound.PlayOneShot(ShiftStrikeSound.clip); //질풍참 사운드 실행
                    break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.W) && WCoolTimer < 0 && !AvoidDuplicateAttacks()) //튕겨내기 사용하면
        {
            RayLine = MainCamera.ScreenPointToRay(Input.mousePosition); //Ray 판정선 만듦
            RayHits = Physics.RaycastAll(RayLine); //RayCast 발동
            for (int i = 0; i < RayHits.Length; i += 1) //닿은 모든 물체들을 탐색
            {
                if (RayHits[i].transform.tag == "Ground") //지면에 닿으면
                {
                    IsW = true; //튕겨내기 사용
                    WCoolTimer = WCoolTime; //쿨타임
                    WTimer = WTime; //튕겨내기 시간 초기화
                    PlayerAnimator.Play("W"); //튕겨내기 애니메이션 실행
                    WDirection = Quaternion.LookRotation(RayHits[i].point - PlayerTransform.position); //바라봐야 하는 방향 초기화
                    WCollision.SetActive(true); //충돌 판정 활성화
                    ReflectingSound.PlayOneShot(ReflectingSound.clip); //반사 사운드 실행
                    break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.E) && UltimateGauge >= UltimateMaxGauge) //궁극기 사용하면
        {
            IsUltimate = true; //궁극기 사용
            UltimateGauge = 0f; //궁극기 게이지 초기화
            UltimateTimer = UltimateTime; //궁극기 시간 초기화
            UltimateAbility(true); //궁극기 능력 적용
            UltimateEffect.Play(); //궁극기 효과 재생
            QCoolTimer = 0f; //질풍참 초기화
            DominusSound.PlayOneShot(DominusSound.clip); //궁극기 사운드 실행
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
            }
        }
    }

    public void ThrowShuriken() //표창 던지는 함수
    {
        GameObject ShurikenClone = Instantiate(Shuriken, FirePosition.position, FirePosition.rotation); //표창 생성
        ShurikenClone.GetComponent<Shuriken>().Directon = FirePosition.forward; //발사 위치로 방향벡터 지정
        ShurikenClone.SetActive(true); //표창 활성화
    }

    public void AttackEnd() //공격 종료하는 함수
    {
        IsAttack = false; //공격 끝
    }

    public void Q() //질풍참 관련 함수
    {
        QCoolTimer -= Time.deltaTime; //쿨 타임 계산
        if (IsQ) //질풍참 사용하면
        {
            if (Vector3.Distance(PlayerTransform.position, QPosition) < 5f) //거리가 가까워지면 
            {
                QCollision.SetActive(false); //충돌 판정 제거
                IsQ = false; //스킬 사용 끝
            }
            Vector3 Direction = Vector3.Normalize(QPosition - PlayerTransform.position); //방향벡터 계산
            PlayerTransform.position += Direction * Time.deltaTime * 90f; //이동 속도
            PlayerTransform.rotation = Quaternion.Slerp(PlayerTransform.rotation, Quaternion.LookRotation(Direction), 100f * Time.deltaTime); //바라보는 방향으로 회전

            Destroy(Instantiate(NinjaAfterimage, PlayerTransform.position, PlayerTransform.rotation), 0.2f); //잔상 남김
        }
    }

    public void W() //튕겨내기 관련 함수
    {
        WCoolTimer -= Time.deltaTime; //쿨타임 계산
        if (IsW) //튕겨내기 사용 중이면
        {
            if (WTimer < 0f) //지속 시간이 다 되면
            {
                WCollision.SetActive(false); //충돌 판정 제거
                IsW = false; //튕겨내기 끝
            }
            WTimer -= Time.deltaTime; //지속시간 계산
            PlayerTransform.rotation = Quaternion.Slerp(PlayerTransform.rotation, WDirection, 100f * Time.deltaTime); //바라봐야 하는 방향으로 회전
        }
    }

    public void Ultimate() //궁극기 관련 함수
    {
        UltimateGauge += UltimateIncrease * Time.deltaTime; //궁극기 게이지 서서히 증가
        if (IsUltimate) //궁극기 사용 중이면
        {
            UltimateTimer -= Time.deltaTime; //궁극기 지속시간 계산
            if (UltimateTimer < 0f) //지속 시간이 다 되면
            {
                UltimateAbility(false); //궁극기 능력 종료
                IsUltimate = false; //궁극기 끝
            }
        }
    }

    public void UltimateAbility(bool State) //궁극기 효과를 적용하는 함수
    {
        AttackDamage = State ? ForUltimateBaseValue[0] * 2f : ForUltimateBaseValue[0]; //평타 데미지 조절

        QCoolTime = State ? ForUltimateBaseValue[1] / 2f : ForUltimateBaseValue[1]; //질풍참 쿨타임 조절
        QDamage = State ? ForUltimateBaseValue[2] * 2f : ForUltimateBaseValue[2]; //질풍참 데미지 조절

        WCoolTime = State ? ForUltimateBaseValue[3] / 2f : ForUltimateBaseValue[3]; //반사 쿨타임 조절
        WDamage = State ? ForUltimateBaseValue[4] * 2f : ForUltimateBaseValue[4]; //반사 데미지 조절
    }

    public bool AvoidDuplicateAttacks() //공격 중복이 일어나지 않도록 하는 함수
    {
        return IsAttack || IsQ || IsW; //하나라도 실행되어 있으면 True
    }

    public override void Kill(int EnemyNumber) //적을 킬 했을 때 실행되는 함수
    {
        QCoolTimer = 0f; //질풍참 쿨타임 초기화
        Static.Kill[EnemyNumber]++; //킬 수 증가
    }

    public override void GiveDamage(Enemy EnemyScript, float Value) //데미지를 줄 때 실행되는 함수
    {
        if (!IsUltimate) //궁극기 실행 중이 아니면
        {
            UltimateGauge += Value; //궁극기 게이지 증가
        }
    }

    public override void Damaged(Enemy EnemyScript, float Value) //데미지를 입는 함수
    {
        if (!IsDeath) //죽지 않으면
        {
            if (IsW) //반사 중이면
            {
                EnemyScript.Damaged(IsUltimate ? Value * 2f : Value); //데미지 돌려줌
            }
            else if (!IsQ) //질풍참 사용 중이 아니면
            {
                HP -= Value; //체력 감소
            }
        }
    }
}