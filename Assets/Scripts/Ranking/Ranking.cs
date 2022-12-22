using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class Ranking : MonoBehaviour
{
    public List<Info> Informations = new List<Info>(); //정보들을 담는 리스트
    public Text[] Year = new Text[10]; //학번
    public Text[] Name = new Text[10]; //이름
    public Text[] Hero = new Text[10]; //영웅
    public Text[] Score = new Text[10]; //점수
    public string[] HeroString; //영웅 문자

    public struct Info //사용자 정보
    {
        public string Year; //학번
        public string Name; //이름
        public string Phone; //전화번호
        public int Hero; //영웅
        public int Score; //점수
        public int Seq; //순서
        public Info(string Year, string Name, string Phone, int Hero, int Score, int Seq) //생성자
        {
            this.Year = Year;
            this.Name = Name;
            this.Phone = Phone;
            this.Hero = Hero;
            this.Score = Score;
            this.Seq = Seq;
        }
    }

    public void Start()
    {
        HeroString = new string[3] { "<color=#7A00FF>Brute</color>", "<color=#00FF7F>Sorceress</color>", "<color=#0091DB>Ninja</color>" };
        FileToList();
        ListToText();
    }

    public void FileToList() //파일에서 리스트로 정보를 복사하는 함수
    {
        try
        {
            StreamReader Sr = new StreamReader(@"C:\Farming.txt");
            int Seq = 0; //순서를 계산하는 변수
            while (!Sr.EndOfStream) //파일 입출력이 끝나지 않으면
            {
                string Year = Sr.ReadLine(); //학번 입력
                string Name = Sr.ReadLine(); //이름 입력
                string Phone = Sr.ReadLine(); //전화번호 입력
                int Hero = int.Parse(Sr.ReadLine()); //영웅 입력
                int Score = int.Parse(Sr.ReadLine()); //점수 입력
                Informations.Add(new Info(Year, Name, Phone, Hero, Score, Seq++)); //리스트에 삽입
            }
            Sr.Close(); //파일 닫음
            Informations.Sort(delegate (Info A, Info B) //정렬
            {
                if (A.Score < B.Score) //오름차순이면
                    return 1; //변경
                else if (A.Score > B.Score) //내림차순이면
                    return -1; //그대로 유지
                else //점수가 같으면
                    return A.Seq < B.Seq ? -1 : 1; //순서를 따져서 변경
            }
            );
        }
        catch { };
    }

    public void ListToText() //리스트의 값을 텍스트로 모두 출력하는 함수
    {
        for (int i = 0; i < 10 && i < Informations.Count; i += 1) //리스트의 크기만큼 반복
        {
            Year[i].text = Informations[i].Year; //학번 삽입
            Year[i].color = Color.white; //하얀색
            Name[i].text = Informations[i].Name; //이름 삽입
            Name[i].color = Color.white; //하얀색
            Hero[i].text = HeroString[Informations[i].Hero]; //영웅 문자 삽입
            Score[i].text = Informations[i].Score.ToString(); //점수 삽입
            Score[i].color = Color.white; //하얀색
        }
    }

    public void OnClickBackButton() //뒤로가기 버튼 클릭하면 실행되는 함수
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); //초기 씬으로 이동
    }
}