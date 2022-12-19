using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookRope : MonoBehaviour
{

    [Header("References")]
    public GrapplingHook grappling;

    [Header("Settings")]
    public int quality = 200; // 로프가 분할될 세그먼트 수
    public float damper = 14; // 이렇게 하면 시뮬레이션 속도가 느려지므로 전체 로프가 동일한 영향을 받지 않습니다.
    public float strength = 800; //시뮬레이션이 목표 지점에 도달하기 위해 얼마나 열심히 시도하는지
    public float velocity = 15; // 애니메이션의 속도
    public float waveCount = 3; // 파도의 갯수 (일렁일렁하는거)
    public float waveHeight = 1;
    public AnimationCurve affectCurve;

    private Spring spring; // 애니메이션에 필요한 값을 반환하는 사용자 지정 스크립트
    private LineRenderer lr;
    private Vector3 currentGrapplePosition;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void DrawRope()
    {
        // 그래플링을 하지 않는다면 로프를 당기지 않는다
        if (!grappling.IsGrappling())
        {
            currentGrapplePosition = grappling.gunTip.position;

            // 시뮬레이션 리셋
            spring.Reset();

            // lineRenderer의 positionCount 재설정
            if (lr.positionCount > 0)
                lr.positionCount = 0;

            return;
        }

        if (lr.positionCount == 0)
        {
            // 시뮬레이션의 시작 속도 설정
            spring.SetVelocity(velocity);

            //로프의 품질에 따라 lineRenderer의 positionCount를 설정합니다.
            lr.positionCount = quality + 1;
        }

        // 스프링 시뮬레이션 설정
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.SpringUpdate(Time.deltaTime);

        Vector3 grapplePoint = grappling.GetGrapplePoint();
        Vector3 gunTipPosition = grappling.gunTip.position;

        // 로프에 상대적인 위쪽 방향 찾기
        Vector3 up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        // grapplePoint를 향해 currentGrapplePositin을 lerp함.
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        // 로프의 모든 세그먼트를 반복하고 애니메이션
        for (int i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;
            // 현재 로프 세그먼트의 오프셋 계산
            Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

            // lineRenderer 위치를 currentGrapplePosition + 방금 계산한 오프셋으로 이동
            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }
    }
}
