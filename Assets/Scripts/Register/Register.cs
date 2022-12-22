using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Register : MonoBehaviour
{
    public InputField YearInputField; //학번을 담는 InputField
    public InputField NameInputField; //이름을 담는 InputField
    public InputField PhoneInputField; //전화번호를 담는 InputField
    public Text HeroText; //영웅 텍스트
    public Text ScoreText; //점수 텍스트

    public void Start()
    {
        switch (Static.HeroNumber) //어떤 영웅에 따라 텍스트 출력
        {
            case 0:
                HeroText.text = "<color=#5204A8>Brute</color>";
                break;
            case 1:
                HeroText.text = "<color=#00FF7F>Sorceress</color>";
                break;
            case 2:
                HeroText.text = "<color=#00466A>Ninja</color>";
                break;
        }
        ScoreText.text = "" + Static.Sum; //총 점수 출력
    }

    public void OnClickCancelButton() //취소 버튼을 클릭하면 실행되는 함수
    {
        SceneManager.LoadScene(0); //초기 화면으로 이동
    }

    public void OnClickConfirmButton() //확인 버튼을 클릭하면 실행되는 함수
    {
        try
        {
            StreamWriter Sw = new StreamWriter(@"C:\Farming.txt", true); //추가 => true를 옆에 적으면 파일에 추가한다는 뜻
            Sw.WriteLine(YearInputField.text); //학번 저장
            Sw.WriteLine(NameInputField.text); //이름 저장
            Sw.WriteLine(PhoneInputField.text); //전화번호 저장
            Sw.WriteLine("" + Static.HeroNumber); //영웅 번호 저장
            Sw.WriteLine("" + Static.Sum); //총 점수 저장
            Sw.Close(); //파일 닫음 => 꼭 닫아주어야 저장됨!
            SceneManager.LoadScene(0); //첫 화면으로 이동
        }
        catch //에러 발생하면 첫 화면으로 이동
        {
            SceneManager.LoadScene(0); //첫 화면으로 이동
        }
    }
}