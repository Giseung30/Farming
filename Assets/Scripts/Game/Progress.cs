using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Progress : MonoBehaviour
{
    [Header("Object")]
    public GameObject[] EnemyObject; //적 오브젝트
    public GameObject Brute; //Brute 캐릭터
    public GameObject Sorceress; //Sorceress 캐릭터
    public GameObject Ninja; //Ninja 캐릭터

    [Header("Skill")]
    public Image QImage; //Q 스킬 Image
    public Image WImage; //W 스킬 Image
    public Image UltimateImage; //궁극기 Image
    public Sprite[] BruteSkill = new Sprite[3]; //Brute 스킬 이미지
    public Sprite[] SorceressSkill = new Sprite[3]; //Sorceress 스킬 이미지
    public Sprite[] NinjaSkill = new Sprite[3]; //Ninja 스킬 이미지

    public Text QInfo; //Q 스킬 정보
    public Text WInfo; //W 스킬 정보
    public Text UltimateInfo; //궁극기 정보
    [TextArea]
    public string[] BruteSkillInfo = new string[3]; //Brute 스킬 정보
    [TextArea]
    public string[] SorceressSkillInfo = new string[3]; //Sorceress 스킬 정보
    [TextArea]
    public string[] NinjaSkillInfo = new string[3]; //Ninja 스킬 정보
    public AudioSource[] HeroThemeSound = new AudioSource[3]; //영웅 테마 사운드

    [Header("Variable")]
    public bool IsGameOver; //게임이 종료되었는지 판단하는 변수
    public float ProgressTime; //시간
    public float MaxSpawn; //최대 생성되는 적의 개수
    public float SpawnDelay; //생성 지연 시간
    public float SpawnDelayTimer; //생성 지연 시간
    public float GrowthMoveSpeed; //이동 속도 증가율
    public float GrowthDamage; //데미지 증가율
    public List <Enemy> SpawnEnemies; //현재 생성된 적들을 담아두는 리스트

    [Header("Script")]
    public UIController UIControllerScript; //UIController 스크립트

    public void Awake()
    {
        /* 캐릭터 초기화 과정 */
        switch (Static.HeroNumber)
        {
            case 0:
                Brute.SetActive(true); //활성화
                Static.PlayerScript = Brute.GetComponent<Brute>();
                Static.PlayerTransform = Brute.GetComponent<Transform>();

                QImage.sprite = BruteSkill[0]; //스킬 이미지 삽입
                WImage.sprite = BruteSkill[1];
                UltimateImage.sprite = BruteSkill[2];

                QInfo.text = BruteSkillInfo[0]; //스킬 정보 삽입
                WInfo.text = BruteSkillInfo[1];
                UltimateInfo.text = BruteSkillInfo[2];

                HeroThemeSound[0].Play(); //테마 사운드 재생
                break;

            case 1:
                Sorceress.SetActive(true); //활성화
                Static.PlayerScript = Sorceress.GetComponent<Sorceress>();
                Static.PlayerTransform = Sorceress.GetComponent<Transform>();

                QImage.sprite = SorceressSkill[0]; //스킬 이미지 삽입
                WImage.sprite = SorceressSkill[1];
                UltimateImage.sprite = SorceressSkill[2];

                QInfo.text = SorceressSkillInfo[0]; //스킬 정보 삽입
                WInfo.text = SorceressSkillInfo[1];
                UltimateInfo.text = SorceressSkillInfo[2];

                HeroThemeSound[1].Play(); //테마 사운드 재생
                break;
            case 2:
                Ninja.SetActive(true); //활성화
                Static.PlayerScript = Ninja.GetComponent<Ninja>();
                Static.PlayerTransform = Ninja.GetComponent<Transform>();

                QImage.sprite = NinjaSkill[0]; //스킬 이미지 삽입
                WImage.sprite = NinjaSkill[1];
                UltimateImage.sprite = NinjaSkill[2];

                QInfo.text = NinjaSkillInfo[0]; //스킬 정보 삽입
                WInfo.text = NinjaSkillInfo[1];
                UltimateInfo.text = NinjaSkillInfo[2];

                HeroThemeSound[2].Play(); //테마 사운드 재생
                break;
        }
    }
    
    public void Start()
    {
        StartCoroutine(GrowthManager());
        StartCoroutine(SpawnDelayManager());
        StartCoroutine(Finish());
        try
        {
            Destroy(FindObjectOfType<DontDestroy>().gameObject); //배경음악 삭제
        }
        catch { }
    }

    public IEnumerator GrowthManager() //증가율 관리하는 코루틴
    {
        int CntTime = 150; //증가시킬 시간
        GrowthMoveSpeed = 1f; //이동 속도 증가율 초기화
        GrowthDamage = 1f; //데미지 증가율 초기화
        while (CntTime > 0) //시간이 모두 지나가지 않으면
        {
            GrowthMoveSpeed += 1f / 150f; //이동속도 증가
            GrowthDamage += 0.5f / 150f; //데미지 증가
            CntTime -= 1; //시간 감소
            yield return new WaitForSeconds(1f); //1초 대기
        }
        yield return null;
    }

    public IEnumerator SpawnDelayManager()  //생성 지연 시간 관리하는 코루틴
    {
        int CntTime = 150; //감소시킬 시간
        float InitialSpawnDelay = SpawnDelay; //초기 지연시간 담음
        while (CntTime > 0) //시간이 모두 지나가지 않으면
        {
            SpawnDelay -= InitialSpawnDelay / 150f; //생성 지연 시간 감소
            CntTime -= 1; //시간 감소
            yield return new WaitForSeconds(1f); //1초 대기
        }
        SpawnDelay = 0f; //지연 시간 없음
        yield return null;
    }

    public IEnumerator Finish() //게임 종료를 담당하는 코루틴
    {
        while (ProgressTime > 0f && !Static.PlayerScript.IsDeath) //게임 시간이 남았거나 플레이어가 죽지 않으면
        {
            yield return new WaitForEndOfFrame(); //계속 대기
        }
        IsGameOver = true; //게임 종료
        StartCoroutine(UIControllerScript.FinishFadeIn()); //Finish 이미지 Fade In
        if (ProgressTime <= 0f) //시간이 종료되면
        {
            for (int i = 0; i < SpawnEnemies.Count; i += 1) //생성된 적의 개수만큼 반복
            {
                SpawnEnemies[i].HP = 0f; //체력 모두 제거
            }
        }
        yield return new WaitForSeconds(8f); //좀 기다렸다가
        SceneManager.LoadScene(2); //결과 씬으로 이동
        yield return null;
    }


    public void Update()
    {
        SpawnEnemy();
        if(!IsGameOver) ProgressTime -= Time.deltaTime; //시간 계산
        SpawnEnemies.RemoveAll(Enemy => Enemy == null); //사라진 적을 리스트에서 정리
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene(0); //ESC 버튼을 누르면 첫 화면으로 이동
    }

    public void SpawnEnemy() //적을 생성하는 함수
    {
        SpawnDelayTimer += Time.deltaTime; //적 생성 지연 시간 계산
        if (SpawnDelayTimer > SpawnDelay && SpawnEnemies.Count < MaxSpawn && !IsGameOver) //지연 시간이 지나고 적의 개수가 최대가 아니며 게임 종료가 아니면
        {
            int SpawnIndex = Random.Range(0, EnemyObject.Length); //적 생성 인덱스
            Vector3 SpawnPosition = new Vector3(Random.Range(-10f, 52f), 0f, Random.Range(-37f, 25f)); //적 생성 위치 지정
            GameObject EnemyClone = Instantiate(EnemyObject[SpawnIndex], SpawnPosition, EnemyObject[SpawnIndex].GetComponent<Transform>().rotation); //적 생성
            SpawnEnemies.Add(EnemyClone.GetComponent<Enemy>()); //리스트에 적 추가
            EnemyClone.SetActive(true); //적 활성화
            SpawnDelayTimer = 0f; //지연 시간 초기화
        }
    }
}