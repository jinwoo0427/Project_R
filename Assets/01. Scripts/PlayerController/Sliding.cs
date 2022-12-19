using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Tooltip("슬라이딩이 시작될 때 적용되는 초기 힘")]
    public float initialForce = 20;

    public float groundFriction = 15;

    public float colliderHeight = 0.9f;

    [Header("Advanced")]

    [Tooltip("미끄러질 때 중력과 같은 힘이 가해짐")]
    public float slideGravity = 400;

    [Tooltip("캐릭터가 미끄러지기 위해 가져야 하는 최소 힘(제곱)")]
    public float minimumStopVelocity = 5;

    [Range(0, 1)]
    public float horizontalControl = 0.5f;

    [Tooltip("카메라가 캐릭터가 움직이는 다른 방향을 바라볼 때 적용되는 마찰")]
    public float cameraRotationFrictionFactor = 0.1f;

    public float colliderMorphSpeed = 10;



}
