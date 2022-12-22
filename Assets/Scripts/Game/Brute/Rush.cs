using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rush : MonoBehaviour
{
    public Brute PlayerScript; //플레이어 Script
    public void OnTriggerEnter(Collider Col)
    {
        if(Col.tag == "Enemy") //적과 충돌하면
        {
            PlayerScript.RushEnd(); //돌진 종료
        }
    }
}
