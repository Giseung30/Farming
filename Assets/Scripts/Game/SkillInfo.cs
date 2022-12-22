using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillInfo : MonoBehaviour
{
    public GameObject SkillPanel; //스킬 Panel
    public AudioSource TapSound; //Tap 사운드

    public void ActivateSkillPanel()
    {
        SkillPanel.SetActive(true); //패널 활성화
        try
        {
            TapSound.PlayOneShot(TapSound.clip); //소리 한번 실행
        }
        catch { }
    }

    public void DeactivateSkillPanel()
    {
        SkillPanel.SetActive(false); //패널 비활성화
    }
}
