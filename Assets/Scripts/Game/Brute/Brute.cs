using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brute : Player
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
    public bool IsQ; //돌진 중인지 확인용 변수
    public Vector3 QPosition; //돌진 사용한 위치
    public float QDistance; //돌진 최대 거리
    public float QSpeed; //돌진 속도
    public GameObject QCollision; //돌진 충돌 판정 오브젝트
    public float QDamage; //돌진 데미지
    public float QCoolTime; //돌진 쿨타임

    public bool IsW; //흡수 사용 중인기 확인용 변수
    public float WTime; //흡수 유지 시간
    public float WTimer; //흡수 계산하는 변수
    public float WCoolTime; //흡수 쿨타임
    public float WAbsorbRatio; //흡수 비율

    public bool IsUltimate; //궁극기 사용 중인지 확인용 변수
    public float UltimateIncrease; //궁극기 서서히 증가하는 양
    public float UltimateDamege; //궁극기 데미지

    [Header("Effect")]
    public GameObject AttackEffect; //공격 효과
    public GameObject RushSmokeEffect; //돌진 연기 효과
    public GameObject EnemyUpEffect; //적이 뜨는 효과
    public GameObject AbsorbEffect; //흡수 효과
    public GameObject EarthQuakeEffect; //지진 효과

    [Header("Sound")]
    public AudioSource AttackSound; //공격 사운드
    public AudioSource WalkSound; //이동 사운드
    public AudioSource RushSound; //돌진 사운드
    public AudioSource EarthQuakeSound; //지진 사운드
    public AudioSource AbsorbSound; //흡수 사운드

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
        if (Input.GetMouseButtonDown(1) && !AvoidDuplicateAttacks()) //오른쪽 마우스 버튼을 누르면
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
            PlayerTransform.rotation = Quaternion.Slerp(PlayerTransform.rotation, Quaternion.LookRotation(Direction), RotateSpeed * Time.deltaTime); //바라보는 방향으로 회전
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
                    break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Q) && QCoolTimer < 0 && !AvoidDuplicateAttacks()) //돌진 사용
        {
            RayLine = MainCamera.ScreenPointToRay(Input.mousePosition); //Ray 판정선 만듦
            RayHits = Physics.RaycastAll(RayLine); //RayCast 발동
            for (int i = 0; i < RayHits.Length; i += 1) //닿은 모든 물체들을 탐색
            {
                if (RayHits[i].transform.tag == "Ground") //지면에 닿으면
                {
                    IsQ = true; //돌진 사용
                    QCoolTimer = QCoolTime; //쿨타임
                    IsMoved = false; //움직임 종료
                    QPosition = PlayerTransform.position + Vector3.Normalize(RayHits[i].point - PlayerTransform.position) * QDistance; //돌진 사용한 위치
                    QPosition = new Vector3(Mathf.Clamp(QPosition.x, -8.5f, 51f), QPosition.y, Mathf.Clamp(QPosition.z, -35f, 23.5f)); //범위 제한
                    PlayerAnimator.Play("Q"); //돌진 애니메이션 실행
                    QCollision.SetActive(true); //충돌 판정 실행
                    break;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.W) && WCoolTimer < 0) //흡수 사용하면
        {
            IsW = true; //흡수 사용
            WCoolTimer = WCoolTime; //쿨타임
            WTimer = WTime; //흡수 시간 초기화
            AbsorbEffect.SetActive(true); //흡수 효과 활성화
            AbsorbSound.Play(); //흡수 사운드 실행
        }
        if (Input.GetKeyDown(KeyCode.E) && UltimateGauge >= UltimateMaxGauge && !AvoidDuplicateAttacks()) //궁극기 사용하면
        {
            IsUltimate = true; //궁극기 사용
            UltimateGauge = 0f; //궁극기 게이지 초기화
            IsMoved = false; //움직이지 못함
            PlayerAnimator.Play("Ultimate"); //궁극기 애니메이션 실행
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
                AttackSound.PlayOneShot(AttackSound.clip); //공격 사운드 실행
                IsAttack = false; //공격 종료
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

    public void Q() //돌진 관련 함수
    {
        QCoolTimer -= Time.deltaTime; //쿨 타임 계산
        if (IsQ) //돌진 사용하면
        {
            if (Vector3.Distance(PlayerTransform.position, QPosition) < 5f) //거리가 가까워지면 
            {
                RushEnd();
            }
            Vector3 Direction = Vector3.Normalize(QPosition - PlayerTransform.position); //방향벡터 계산
            PlayerTransform.position += Direction * Time.deltaTime * QSpeed; //이동 속도
            PlayerTransform.rotation = Quaternion.Slerp(PlayerTransform.rotation, Quaternion.LookRotation(Direction), 100f * Time.deltaTime); //바라보는 방향으로 회전
        }
    }

    public void RushEnd() //돌진 종료
    {
        QCollision.SetActive(false); //충돌 판정 제거
        Collider[] QEnemies = Physics.OverlapBox(PlayerTransform.position, new Vector3(15f, 15f, 15f) / 2f); //주위 적들을 담음
        Instantiate(RushSmokeEffect, RushSmokeEffect.GetComponent<Transform>().position, RushSmokeEffect.GetComponent<Transform>().rotation).SetActive(true); //연기 효과 생성
        RushSound.PlayOneShot(RushSound.clip); //돌진 사운드 실행
        for (int i = 0; i < QEnemies.Length; i += 1) //주위 적 개수만큼 반복
        {
            if (QEnemies[i].tag == "Enemy") //적이면
            {
                QEnemies[i].GetComponent<Enemy>().Damaged(QDamage); //데미지 가함
                QEnemies[i].GetComponent<Rigidbody>().AddForce(Vector3.up * 6000f); //위로 뛰움
                Instantiate(EnemyUpEffect, QEnemies[i].GetComponent<Transform>().position, EnemyUpEffect.GetComponent<Transform>().rotation).SetActive(true); //적 뛰우는 효과 생성
            }
        }
        IsQ = false; //스킬 사용 끝
    }

    public void W() //흡수 관련 함수
    {
        WCoolTimer -= Time.deltaTime; //쿨타임 계산
        if (IsW) //흡수 사용 중이면
        {
            if (WTimer < 0f) //지속 시간이 다 되면
            {
                AbsorbEffect.SetActive(false); //흡수 효과 종료
                AbsorbSound.Stop(); //흡수 사운드 종료
                IsW = false; //흡수 사용 끝
            }
            WTimer -= Time.deltaTime; //지속시간 계산
        }
    }

    public void Ultimate() //궁극기 관련 함수
    {
        UltimateGauge += UltimateIncrease * Time.deltaTime; //궁극기 게이지 서서히 증가
    }

    public void EarthQuake(int Quit) //지진을 일으키는 함수
    {
        if (Quit == 1) //궁극기 종료하려면
        {
            IsUltimate = false; //궁극기 종료
        }
        else //지진 효과 생성하려면
        {
            Instantiate(EarthQuakeEffect, EarthQuakeEffect.GetComponent<Transform>().position, EarthQuakeEffect.GetComponent<Transform>().rotation).SetActive(true); //지진 효과 생성
            EarthQuakeSound.PlayOneShot(EarthQuakeSound.clip); //지진 사운드 실행
            Collider[] UltimateEnemies = Physics.OverlapSphere(PlayerTransform.position, 15f); //주위 적들 담음
            for (int i = 0; i < UltimateEnemies.Length; i += 1) //주위 적 개수만큼 반복
            {
                if (UltimateEnemies[i].tag == "Enemy") //적이면
                {
                    UltimateEnemies[i].GetComponent<Enemy>().Damaged(UltimateDamege); //데미지 가함
                    UltimateEnemies[i].GetComponent<Rigidbody>().AddForce(Vector3.up * 9000f); //위로 뛰움
                    Instantiate(EnemyUpEffect, UltimateEnemies[i].GetComponent<Transform>().position, EnemyUpEffect.GetComponent<Transform>().rotation).SetActive(true); //적 뛰우는 효과 생성
                }
            }
        }
    }

    public bool AvoidDuplicateAttacks() //공격 중복이 일어나지 않도록 하는 함수
    {
        return IsAttack || IsQ || IsUltimate; //하나라도 실행되어 있으면 True
    }

    public override void Kill(int EnemyNumber) //적을 킬 했을 때 실행되는 함수
    {
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
            if (IsW) //흡수 중이면
            {
                HP += Value * WAbsorbRatio; //일부 회복
            }
            else //흡수 중이 아니면
            {
                HP -= Value; //체력 감소
                WCoolTimer -= 0.2f; //맞을 때 마다 흡수 쿨타임 감소
            }
        }
    }
}