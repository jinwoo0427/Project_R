using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    public bool enableWallRun = true;

    public float speedWhileRunning = 12f;

    public float verticalWallJumpForce = 2f;

    public float horizontalWallJumpForce = 12.5f;

    [Header("Advanced")]
    public LayerMask walkableObjectLayerMask;

    [Tooltip("ĳ���ʹ� �� �ð��� ������ �ڵ����� ������ �и���.")]
    public float autoDetachTimerCondition = 1;

    [Tooltip("�ڵ� �и��� �� ĳ���ʹ� �� ����� �ݴ� �������� �� ���� �ް� ��")]
    public float autoDetachForce = 2;

    [Tooltip("������ �и��Ǹ� ĳ���ʹ� �� �ð�(��) ���� �ٽ� ������ �� ����")]
    public float cooldown = 1.5f;

    [Tooltip("���� �پ��ִ� ������ �߷�")]
    public float wallRunGravity = 0.5f;

    [Tooltip("���� �����Ǿ� �̵��ϴ� ������ �߷�")]
    public float wallRunGravityWhileMoving = 0.35f;

    [Tooltip("���� ������ �� �޴� �ּ� ���� �ν�Ʈ ��. �� �������� ĳ���Ͱ� ���� �ε����� ���� ���� �̵��� �翡 ���� �޶���")]
    public float attachVerticalBoostMin = 2.5f;

    [Tooltip("���� ������ �� �޴� �ִ� ���� �ν�Ʈ ��. �� �������� ĳ���Ͱ� ���� �ε����� ���� ���� �̵��� �翡 ���� �޶���")]
    public float attachVerticalBoostMax = 5;

    [Tooltip("�� ���� Ʈ�����ϱ� ���� ������ �ּ� �Ÿ�")]
    public float attachMinDistanceCondition = 0.35f;

    [Tooltip("ĳ���ʹ� �� ������ �������� ������ ���� ����")]
    public float attachSideAngleCondition = 20;

    [Tooltip("ĳ���Ͱ� ���� ���ϴ� ���⿡ ���� ������ ���� ī�޶� ƿƮ lerp �� ����� ����Ǵ��� ����. " +
        "�� ������ üũ���� ������ ī�޶� ���� �� ����� ������. ĳ���Ͱ� 20�� �̳����� ���� �ٶ󺸰� �ִ� ��� ȸ�� ���� 0, " +
        "cameraTiltAngle ĳ���Ͱ� ������ �־����� ���")]
    public bool dynamicCameraTilt = true;

    public float cameraTiltAngle = 15;

    public float cameraTiltLerpSpeed = 5;

    public float cameraTiltResetLerpSpeed = 10;




    private void HandleWallRun(float dt)
    {
        //������ Ȱ��ȭ���� ����
        if (enableWallRun == false)
            return;

        //���� �� ���� ���ð� �缳��
        //if (isGrounded)
        //{
        //    lastTimeBeginWallRun = 0;
        //}

        //if (currentControllerState == ControllerState.WallRun && Vector3.Dot(currentWallRunNormal, bodyTransform.forward) < 0.5f)
        //{
        //    float angle = Vector3.SignedAngle(bodyTransform.forward, currentWallRunDirection, bodyTransform.up);

        //    //ī�޶� ���� �� ����� ������ ���� ����
        //    if (wallRunSettings.dynamicCameraTilt)
        //    {
        //        cameraController.SetCameraRootTiltLerped(angle / 90 * wallRunSettings.cameraTiltAngle, wallRunSettings.cameraTiltLerpSpeed, dt);
        //    }
        //    else
        //    {
        //        float unsignedAngle = Mathf.Abs(angle);
        //        if (unsignedAngle > 20)
        //        {
        //            cameraController.SetCameraRootTiltLerped(Mathf.Sign(angle) * wallRunSettings.cameraTiltAngle, wallRunSettings.cameraTiltLerpSpeed, dt);
        //        }
        //        else
        //        {
        //            cameraController.SetCameraRootTiltLerped(0, wallRunSettings.cameraTiltResetLerpSpeed, dt);
        //        }
        //    }
        //}
        //else
        //{
        //    cameraController.SetCameraRootTiltLerped(0, wallRunSettings.cameraTiltResetLerpSpeed, dt);
        //}
    }
}
