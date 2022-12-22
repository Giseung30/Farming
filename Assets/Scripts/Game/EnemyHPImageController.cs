using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHPImageController : MonoBehaviour
{
    [Header("Component")]
    public Transform EnemyTransform; //적 Transform
    public Transform EnemyHPBarPositionTransform; //적 체력바 이미지 위치 Transform
    public Transform EnemyHPImageTransform; //적 체력 이미지 Transform

    [Header("Object")]
    public Image HPBar; //체력 바 이미지
    public Camera MainCamera; //메인 카메라

    [Header("Script")]
    public Enemy EnemyScript; //적 스크립트

    void Start()
    {
        EnemyHPBarPositionTransform = EnemyTransform.Find("HPBarPosition"); //체력 바 이미지 위치 Transform
        EnemyHPImageTransform = GetComponent<Transform>(); //적 체력 이미지 Transform 담음

        HPBar = GetComponent<Transform>().GetChild(0).GetComponent<Image>(); //자식으로부터 Bar 이미지 찾음
        EnemyScript = EnemyTransform.GetComponent<Enemy>(); //적 스크립트를 담음
    }

    void Update()
    {
        if (EnemyScript.HP <= 0f) Destroy(gameObject); //체력 다 닳으면 체력 바 이미지 제거
        EnemyHPImageTransform.position = MainCamera.WorldToScreenPoint(EnemyHPBarPositionTransform.position); //실시간 체력 이미지 위치 갱신
        HPBar.fillAmount = EnemyScript.HP / EnemyScript.MaxHP; //체력 바 갱신
    }
}
