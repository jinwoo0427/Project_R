using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingHookRope : MonoBehaviour
{

    [Header("References")]
    public GrapplingHook grappling;

    [Header("Settings")]
    public int quality = 200; // ������ ���ҵ� ���׸�Ʈ ��
    public float damper = 14; // �̷��� �ϸ� �ùķ��̼� �ӵ��� �������Ƿ� ��ü ������ ������ ������ ���� �ʽ��ϴ�.
    public float strength = 800; //�ùķ��̼��� ��ǥ ������ �����ϱ� ���� �󸶳� ������ �õ��ϴ���
    public float velocity = 15; // �ִϸ��̼��� �ӵ�
    public float waveCount = 3; // �ĵ��� ���� (�Ϸ��Ϸ��ϴ°�)
    public float waveHeight = 1;
    public AnimationCurve affectCurve;

    private Spring spring; // �ִϸ��̼ǿ� �ʿ��� ���� ��ȯ�ϴ� ����� ���� ��ũ��Ʈ
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
        // �׷��ø��� ���� �ʴ´ٸ� ������ ����� �ʴ´�
        if (!grappling.IsGrappling())
        {
            currentGrapplePosition = grappling.gunTip.position;

            // �ùķ��̼� ����
            spring.Reset();

            // lineRenderer�� positionCount �缳��
            if (lr.positionCount > 0)
                lr.positionCount = 0;

            return;
        }

        if (lr.positionCount == 0)
        {
            // �ùķ��̼��� ���� �ӵ� ����
            spring.SetVelocity(velocity);

            //������ ǰ���� ���� lineRenderer�� positionCount�� �����մϴ�.
            lr.positionCount = quality + 1;
        }

        // ������ �ùķ��̼� ����
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.SpringUpdate(Time.deltaTime);

        Vector3 grapplePoint = grappling.GetGrapplePoint();
        Vector3 gunTipPosition = grappling.gunTip.position;

        // ������ ������� ���� ���� ã��
        Vector3 up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        // grapplePoint�� ���� currentGrapplePositin�� lerp��.
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        // ������ ��� ���׸�Ʈ�� �ݺ��ϰ� �ִϸ��̼�
        for (int i = 0; i < quality + 1; i++)
        {
            float delta = i / (float)quality;
            // ���� ���� ���׸�Ʈ�� ������ ���
            Vector3 offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value * affectCurve.Evaluate(delta);

            // lineRenderer ��ġ�� currentGrapplePosition + ��� ����� ���������� �̵�
            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }
    }
}
