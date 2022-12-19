using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSettings : MonoBehaviour
{
    [Tooltip("Default speed")]
    public float defaultSpeed = 5;

    [Header("Advanced")]

    [Tooltip("캐릭터 정지 시 속도 감속량. 값 0은 감속 없음을 의미.")]
    public float movementDeceleration = 0.033f;

    public float maxSpeed = 30;

}
