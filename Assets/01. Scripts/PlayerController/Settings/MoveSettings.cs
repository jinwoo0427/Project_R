using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSettings : MonoBehaviour
{
    [Tooltip("Default speed")]
    public float defaultSpeed = 5;

    [Header("Advanced")]

    [Tooltip("ĳ���� ���� �� �ӵ� ���ӷ�. �� 0�� ���� ������ �ǹ�.")]
    public float movementDeceleration = 0.033f;

    public float maxSpeed = 30;

}
