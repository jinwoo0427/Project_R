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

    [Tooltip("캐릭터는 이 시간이 지나면 자동으로 벽에서 분리됨.")]
    public float autoDetachTimerCondition = 1;

    [Tooltip("자동 분리된 후 캐릭터는 벽 방향과 반대 방향으로 이 힘을 받게 됨")]
    public float autoDetachForce = 2;

    [Tooltip("벽에서 분리되면 캐릭터는 이 시간(초) 동안 다시 부착할 수 없슴")]
    public float cooldown = 1.5f;

    [Tooltip("벽에 붙어있는 동안의 중력")]
    public float wallRunGravity = 0.5f;

    [Tooltip("벽에 부착되어 이동하는 동안의 중력")]
    public float wallRunGravityWhileMoving = 0.35f;

    [Tooltip("벽에 부착할 때 받는 최소 수직 부스트 힘. 이 수직력은 캐릭터가 벽에 부딪히는 동안 수직 이동의 양에 따라 달라짐")]
    public float attachVerticalBoostMin = 2.5f;

    [Tooltip("벽에 부착할 때 받는 최대 수직 부스트 힘. 이 수직력은 캐릭터가 벽에 부딪히는 동안 수직 이동의 양에 따라 달라짐")]
    public float attachVerticalBoostMax = 5;

    [Tooltip("월 런을 트리거하기 위한 벽과의 최소 거리")]
    public float attachMinDistanceCondition = 0.35f;

    [Tooltip("캐릭터는 이 각도를 기준으로 측면의 벽을 감지")]
    public float attachSideAngleCondition = 20;

    [Tooltip("캐릭터가 벽을 향하는 방향에 대한 각도에 따라 카메라 틸트 lerp 값 대상이 변경되는지 여부. " +
        "이 변수를 체크하지 않으면 카메라 기울기 값 대상이 고정됨. 캐릭터가 20도 이내에서 벽을 바라보고 있는 경우 회전 값은 0, " +
        "cameraTiltAngle 캐릭터가 벽에서 멀어지는 경우")]
    public bool dynamicCameraTilt = true;

    public float cameraTiltAngle = 15;

    public float cameraTiltLerpSpeed = 5;

    public float cameraTiltResetLerpSpeed = 10;




    private void HandleWallRun(float dt)
    {
        //월런이 활성화되지 않음
        if (enableWallRun == false)
            return;

        //접지 시 재사용 대기시간 재설정
        //if (isGrounded)
        //{
        //    lastTimeBeginWallRun = 0;
        //}

        //if (currentControllerState == ControllerState.WallRun && Vector3.Dot(currentWallRunNormal, bodyTransform.forward) < 0.5f)
        //{
        //    float angle = Vector3.SignedAngle(bodyTransform.forward, currentWallRunDirection, bodyTransform.up);

        //    //카메라 기울기 값 대상은 각도에 따라 변경
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
