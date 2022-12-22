using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Static
{
    public static int HeroNumber; //영웅 번호
    public static Player PlayerScript; //플레이어 스크립트
    public static Transform PlayerTransform; //플레이어 Transform

    public static int[] Kill = new int[4]; //적 킬수
    public static int Sum; //최종 점수
}