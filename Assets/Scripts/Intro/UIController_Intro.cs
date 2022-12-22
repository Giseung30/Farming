using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController_Intro : MonoBehaviour
{
    public RectTransform Screen; //Screen RectTransform
    public bool IsNext; //다음 화면으로 넘어가는 지 확인하는 변수
    public AudioSource ClickSound; //클릭 사운드

    public void Update()
    {
        MoveNext();
    }

    public void ToNext() //다음 화면으로 이동하는 함수
    {
        ClickSound.PlayOneShot(ClickSound.clip); //클릭 사운드 실행
        IsNext = true; //다음 화면으로 이동
    }

    public void MoveNext()
    {
        if (IsNext) //다음 화면으로 넘어가야하면
        {
            if (Screen.anchoredPosition.x > -1680f) //X축 범위를 벗어나지 않으면
            {
                Screen.anchoredPosition += new Vector2(-Time.deltaTime * 1000f, 0f); //왼쪽으로 이동
            }
            else //X축 범위를 벗어나면
            {
                Screen.anchoredPosition = new Vector2(-1680, 0f); //위치 고정
                IsNext = false; //다음 화면 이동 종료
            }
        }
    }

    public void OnClickQuitButton() //나가는 버튼을 클릭하면 실행되는 함수
    {
        Application.Quit(); //게임 종료
    }

    public void OnClickRankingButton() //Ranking 버튼을 클릭하면 실행되는 함수
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(4); //Ranking 씬으르 이동
    }
}