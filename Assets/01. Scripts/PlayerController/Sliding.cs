using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Tooltip("�����̵��� ���۵� �� ����Ǵ� �ʱ� ��")]
    public float initialForce = 20;

    public float groundFriction = 15;

    public float colliderHeight = 0.9f;

    [Header("Advanced")]

    [Tooltip("�̲����� �� �߷°� ���� ���� ������")]
    public float slideGravity = 400;

    [Tooltip("ĳ���Ͱ� �̲������� ���� ������ �ϴ� �ּ� ��(����)")]
    public float minimumStopVelocity = 5;

    [Range(0, 1)]
    public float horizontalControl = 0.5f;

    [Tooltip("ī�޶� ĳ���Ͱ� �����̴� �ٸ� ������ �ٶ� �� ����Ǵ� ����")]
    public float cameraRotationFrictionFactor = 0.1f;

    public float colliderMorphSpeed = 10;



}
