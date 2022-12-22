using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Component")]
    public Transform PlayerTransform; //플레이어
    public Transform MainCamera; //메인 카메라

    [Header("Variable")]
    public float MoveSpeed; //카메라 움직이는 속도
    public bool IsReverse; //역 방향인지 확인하는 변수
    public Vector3 Distance; // 기본 거리
    public Vector3 DistanceReverse; //역 방향 거리
    public Vector3 Rotation; //기본 회전
    public Vector3 RotationReverse; //역 방향 회전

    void Start()
    {
        MainCamera = GetComponent<Transform>(); //현재 오브젝트의 Transform 컴포넌트
        PlayerTransform = Static.PlayerTransform; //플레이어 Transform 담음
    }

    void Update()
    {
        if (IsReverse) //역방향이면
        {
            if (PlayerTransform.position.x < 40f) //범위로 돌아오면
                IsReverse = false; //역방향 해제
        }
        else //역방향이 아닐 때
        {
            if (PlayerTransform.position.x > 47f) //범위를 벗어나면
                IsReverse = true; //역방향으로 전환
        }
        Vector3 GoalPosition = IsReverse ? PlayerTransform.position + DistanceReverse : PlayerTransform.position + Distance; //카메라가 이동해야하는 위치
        Vector3 Direction = Vector3.Normalize(GoalPosition - MainCamera.position); //이동해야하는 방향벡터
        MainCamera.position += Direction * Time.deltaTime * Vector3.Distance(GoalPosition, MainCamera.position) * MoveSpeed; //카메라 서서히 이동

        Quaternion GoalRotation = IsReverse ? Quaternion.Euler(RotationReverse) : Quaternion.Euler(Rotation); //카메라가 회전해야하는 방향
        MainCamera.rotation = Quaternion.Slerp(MainCamera.rotation, GoalRotation, 10f * Time.deltaTime);
    }
}