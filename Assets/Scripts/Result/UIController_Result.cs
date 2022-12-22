using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController_Result : MonoBehaviour
{
    [Header("UI")]
    public Image ResultImage; //결과 이미지
    public Image UseHeroImage; //사용한 영웅 이미지
    public Image EnemyKilledImage; //적 처지 이미지
    public Image TotalScoreImage; //최종 점수 이미지
    public Image[] HeroImages = new Image[3]; //영웅 이미지들
    public Text[] HeroNames = new Text[3]; //영웅 이름들
    public Image[] EnemyImages = new Image[4]; //적 이미지들
    public Text[] EnemyScores = new Text[4]; //적 점수들
    public Text TotalScoreText; //최종 점수 텍스트

    [Header("Object")]
    public GameObject RegisterButton; //등록 버튼
    public GameObject BackButton; //뒤로 가기 버튼

    void Start()
    {
        InitializedInfo();
        StartCoroutine(FadeInSequece());
    }

    public IEnumerator FadeInSequece() //순서대로 Fade In 하는 함수
    {
        while (ResultImage.color.a < 1f) //결과 이미지의 투명도가 남아있으면
        {
            ResultImage.color += new Color(0f, 0f, 0f, Time.deltaTime); //불투명도 서서히 증가
            yield return new WaitForEndOfFrame();
        }
        while (UseHeroImage.color.a < 1f) //사용한 영웅 이미지의 투명도가 남아있으면
        {
            UseHeroImage.color += new Color(0f, 0f, 0f, Time.deltaTime * 1.5f); //불투명도 서서히 증가
            HeroImages[Static.HeroNumber].color += new Color(0f, 0f, 0f, Time.deltaTime * 1.5f); //불투명도 서서히 증가
            HeroNames[Static.HeroNumber].color += new Color(0f, 0f, 0f, Time.deltaTime * 1.5f); //불투명도 서서히 증가
            yield return new WaitForEndOfFrame();
        }
        while (EnemyKilledImage.color.a < 1f) //적 처치 이미지의 투명도가 남아있으면
        {
            EnemyKilledImage.color += new Color(0f, 0f, 0f, Time.deltaTime * 1.5f); //불투명도 서서히 증가
            for (int i = 0; i < 4; i += 1) //적의 개수만큼 반복
            {
                EnemyImages[i].color += new Color(0f, 0f, 0f, Time.deltaTime * 1.5f); //불투명도 서서히 증가
                EnemyScores[i].color += new Color(0f, 0f, 0f, Time.deltaTime * 1.5f); //불투명도 서서히 증가
            }
            yield return new WaitForEndOfFrame();
        }
        while (TotalScoreImage.color.a < 1f) //최종 점수 이미지의 투명도가 남아있으면
        {
            TotalScoreImage.color += new Color(0f, 0f, 0f, Time.deltaTime * 1.5f); //불투명도 서서히 증가
            TotalScoreText.color += new Color(0f, 0f, 0f, Time.deltaTime * 1.5f); //불투명도 서서히 증가
            yield return new WaitForEndOfFrame();
        }
        RegisterButton.SetActive(true); //등록 버튼 활성화
        BackButton.SetActive(true); //뒤로 가기 버튼 활성화
        yield return null;
    }

    public void InitializedInfo() //정보 초기화하는 함수
    {
        ResultImage.color = new Color(ResultImage.color.r, ResultImage.color.g, ResultImage.color.b, 0f); //결과 이미지 투명도 초기화

        UseHeroImage.color = new Color(UseHeroImage.color.r, UseHeroImage.color.g, UseHeroImage.color.b, 0f); //사용한 영웅 이미지 투명도 초기화
        for (int i = 0; i < 3; i += 1) //영웅의 개수만큼 반복
        {
            HeroImages[i].color = new Color(HeroImages[i].color.r, HeroImages[i].color.g, HeroImages[i].color.b, 0f); //영웅 이미지 투명도 초기화
            HeroNames[i].color = new Color(HeroNames[i].color.r, HeroNames[i].color.g, HeroNames[i].color.b, 0f); //영웅 이름 텍스트 투명도 초기화
        }

        EnemyKilledImage.color = new Color(EnemyKilledImage.color.r, EnemyKilledImage.color.g, EnemyKilledImage.color.b, 0f); //적 처지 이미지 투명도 초기화
        for (int i = 0; i < 4; i += 1) //적의 개수만큼 반복
        {
            EnemyImages[i].color = new Color(EnemyImages[i].color.r, EnemyImages[i].color.g, EnemyImages[i].color.b, 0f); //적 이미지 투명도 초기화
            EnemyScores[i].color = new Color(EnemyScores[i].color.r, EnemyScores[i].color.g, EnemyScores[i].color.b, 0f); //적 점수 투명도 초기화
            switch (i) //점수 텍스트 초기화
            {
                case 0:
                    EnemyScores[0].text = "100 x " + Static.Kill[0];
                    break;
                case 1:
                    EnemyScores[1].text = "200 x " + Static.Kill[1];
                    break;
                case 2:
                    EnemyScores[2].text = "120 x " + Static.Kill[2];
                    break;
                case 3:
                    EnemyScores[3].text = "150 x " + Static.Kill[3];
                    break;
            }
        }

        TotalScoreImage.color = new Color(TotalScoreImage.color.r, TotalScoreImage.color.g, TotalScoreImage.color.b, 0f); //최종 점수 이미지 투명도 초기화
        TotalScoreText.color = new Color(TotalScoreText.color.r, TotalScoreText.color.g, TotalScoreText.color.b, 0f); //최종 점수 텍스트 투명도 초기화
        TotalScoreText.text = "" + Static.Sum; //최종 점수 텍스트 초기화

        RegisterButton.SetActive(false); //등록 버튼 비활성화
        BackButton.SetActive(false); //뒤로 가기 버튼 비활성화
    }

    public void OnClickBackButton() //뒤로가기 버튼을 클릭하면 실행되는 함수
    {
        SceneManager.LoadScene(0); //씬 이동
    }

    public void OnClickRegisterButton() //등록 버튼을 클릭하면 실행되는 함수
    {
        SceneManager.LoadScene(3); //씬 이동
    }
}
