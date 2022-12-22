using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("Object")]
    public Transform EnemyHPImages; //체력 이미지를 담아두는 공간
    public GameObject EnemyHPSample; //체력을 표시할 이미지 오브젝트
    public Transform UltimateTimerPosition; //궁극기 남은 시간 출력하는 위치
    public Camera MainCamera; //메인 카메라

    [Header("Skill")]
    public GameObject QBack; //Q 스킬 배경
    public Text QCoolTime; //Q 스킬 시간 텍스트

    public GameObject WBack; //W 스킬 배경
    public Text WCoolTime; //W 스킬 시간 텍스트

    public GameObject UltimateBack; //궁극기 배경
    public Text UltimatePercent; //궁극기 퍼센트 텍스트
    public Text UltimateTimerText; //궁극기 남은 시간 텍스트
    public Transform UltimateTimerTextTransform; //궁극기 남은 시간 텍스트 Transform

    [Header("Script")]
    public Player PlayerScript; //Player 스크립트
    public Progress ProgressScript; //Progress 스크립트

    [Header("HP")]
    public Image PlayerHPBar; //플레이어 체력 이미지
    public Image PlayerHPBarRed; //플레이어 빨간 체력 이미지
    public Text PlayerHPText; //플레이어 체력 텍스트

    [Header("Kill")]
    public Text EggyScore; //Eggy 킬 점수
    public Text KiwiScore; //Kiwi 킬 점수
    public Text LangsatScore; //Langsat 킬 점수
    public Text ChiliScore; //Chili 킬 점수
    public Text Sum; //총 점수 합

    [Header("UI")]
    public Text MessageText; //Message 텍스트
    public Text ProgressTimeText; //ProgressTime 텍스트
    public Image FinishImage; //Finish 이미지

    public void Start()
    {
        UltimateTimerPosition = Static.PlayerTransform.Find("RemainSkillTimePosition"); //궁극기 남은 시간 출력 위치 초기화
        PlayerScript = Static.PlayerScript; //플레이어 스크립트 삽입
        StartCoroutine(FadeMessage()); //코루틴 실행
    }

    public IEnumerator FadeMessage() //메시지를 Fade Out 시키는 코루틴
    {
        MessageText.color += new Color(0f, 0f, 0f, 1f); //색상 초기화
        yield return new WaitForSeconds(4f); //대기
        while (MessageText.color.a > 0f) //투명도가 아직 덜 떨어지면
        {
            MessageText.color -= new Color(0f, 0f, 0f, Time.deltaTime); //투명도 천천히 감소
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public IEnumerator FinishFadeIn() //Finish 이미지를 Fade In 시키는 코루틴
    {
        FinishImage.color = new Color(FinishImage.color.r, FinishImage.color.g, FinishImage.color.b, 0f); //초기 투명도 초기화
        while(FinishImage.color.a < 1f) //투명도가 없어질 때까지
        {
            FinishImage.color += new Color(0f, 0f, 0f, Time.deltaTime * 0.2f); //서서히 불투명도 증가
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public void Update()
    {
        SkillCoolTime();
        HPManager();
        UltimateTimerOutput();
        KillManager();
        ProgressTimeManager();
    }
    public void CreateEnemyHPImage(Transform EnemyObject) //적 체력 이미지를 생성하는 함수
    {
        GameObject EHS = Instantiate(EnemyHPSample); //적 체력 오브젝트 생성
        EHS.GetComponent<Transform>().SetParent(EnemyHPImages); //부모 변경
        EHS.name = EnemyObject.name + "HP"; //이름 설정
        EHS.GetComponent<Transform>().localScale = new Vector3(1f, 1f, 1f); //크기 조절
        EHS.GetComponent<EnemyHPImageController>().EnemyTransform = EnemyObject; //적 오브젝트를 내보냄
        EHS.SetActive(true); //활성화
    }

    public void SkillCoolTime() //스킬 쿨타임을 계산하는 함수
    {
        if (PlayerScript.QCoolTimer > 0) //아직 쿨타임이 남아있으면
        {
            QCoolTime.gameObject.SetActive(true); //쿨타임 텍스트 활성화
            QCoolTime.text = string.Format("{0:0.0}", PlayerScript.QCoolTimer); //텍스트 출력
            QBack.SetActive(true); //뒷 배경 활성화
        }
        else //쿨타임이 없으면
        {
            QCoolTime.gameObject.SetActive(false); //쿨타임 텍스트 비활성화
            QBack.SetActive(false); //뒷 배경 비활성화
        }

        if (PlayerScript.WCoolTimer > 0) //아직 쿨타임이 남아있으면
        {
            WCoolTime.gameObject.SetActive(true); //쿨타임 텍스트 활성화
            WCoolTime.text = string.Format("{0:0.0}", PlayerScript.WCoolTimer); //텍스트 출력
            WBack.SetActive(true); //뒷 배경 활성화
        }
        else //쿨타임이 없으면
        {
            WCoolTime.gameObject.SetActive(false); //쿨타임 텍스트 비활성화
            WBack.SetActive(false); //뒷 배경 비활성화
        }

        if (PlayerScript.UltimateGauge < PlayerScript.UltimateMaxGauge) //아직 궁극기가 덜 차면
        {
            UltimatePercent.gameObject.SetActive(true); //퍼센트 텍스트 활성화
            UltimatePercent.text = string.Format("{0:0}", Mathf.Floor((PlayerScript.UltimateGauge / PlayerScript.UltimateMaxGauge) * 100f)); //소수점 버리고 출력
            UltimateBack.SetActive(true); //뒷 배경 활성화
        }
        else //궁극기가 차면
        {
            UltimatePercent.gameObject.SetActive(false); //퍼센트 텍스트 비활성화
            UltimateBack.SetActive(false); //뒷 배경 비활성화
        }
    }

    public void HPManager() //체력 UI 관리하는 함수
    {
        PlayerHPText.text = "<size=60>" + (int)PlayerScript.HP + "</size> <size=30>/ " + (int)PlayerScript.MaxHP + "</size>"; //체력 텍스트 갱신
        PlayerHPBar.fillAmount = PlayerScript.HP / PlayerScript.MaxHP; //체력에 맞추어 이미지 양 조절

        float DecreaseSpeed = PlayerHPBarRed.fillAmount - PlayerHPBar.fillAmount < 0.4f ? 0.4f : PlayerHPBarRed.fillAmount - PlayerHPBar.fillAmount; //감소 속도 지정
        PlayerHPBarRed.fillAmount = PlayerHPBarRed.fillAmount <= PlayerHPBar.fillAmount ? PlayerHPBar.fillAmount : PlayerHPBarRed.fillAmount - Time.deltaTime * DecreaseSpeed; //체력 깎임 이펙트 갱신
    }

    public void UltimateTimerOutput() //궁극기 남은 시간 출력
    {
        UltimateTimerText.text = PlayerScript.UltimateTimer > 0f ? "" + Mathf.Ceil(PlayerScript.UltimateTimer) : ""; //궁극기 남은 시간 텍스트 갱신
        UltimateTimerTextTransform.position = MainCamera.WorldToScreenPoint(UltimateTimerPosition.position); //궁극기 남은 시간 출력 위치 갱신
    }

    public void KillManager() //킬 수 관리하는 함수
    {
        EggyScore.text = Static.Kill[0] + ""; //킬 수 삽입
        KiwiScore.text = Static.Kill[1] + "";
        LangsatScore.text = Static.Kill[2] + "";
        ChiliScore.text = Static.Kill[3] + "";
        Static.Sum = Static.Kill[0] * 100 + Static.Kill[1] * 200 + Static.Kill[2] * 120 + Static.Kill[3] * 150; //최종 점수를 삽입
        Sum.text = "<color=#ffffff>" + Static.Sum + "</color> <size=20>[현재 점수]</size>"; //총 합 출력
    }

    public void ProgressTimeManager() //시간 관리하는 함수
    {
        if (ProgressScript.ProgressTime > 0f) //시간이 남으면
            ProgressTimeText.text = string.Format("{0:00} : {1:00}", (int)ProgressScript.ProgressTime / 60, (int)ProgressScript.ProgressTime % 60); //시간 출력
        else //시간이 남지 않으면
            ProgressTimeText.text = "00 : 00"; //시간 출력
    }
}
