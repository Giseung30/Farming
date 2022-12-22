using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SelectHero : MonoBehaviour
{
    public int HeroNumber; //영웅 번호
    public int SceneNumber; //씬 번호

    public void SceneChange() //씬을 변경하는 함수
    {
        Static.HeroNumber = HeroNumber; //영웅 번호 지정
        for(int i = 0; i < Static.Kill.Length; i += 1) Static.Kill[i] = 0; //킬 수 초기화
        Static.Sum = 0; //합 초기화
        SceneManager.LoadScene(SceneNumber); //씬 변경
    }
}
