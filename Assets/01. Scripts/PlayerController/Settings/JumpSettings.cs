using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSettings : MonoBehaviour
{
    public int jumpsCount = 1;

    public float jumpForce = 10;

    [Header("Advanced")]

    public float jumpCooldown;

    [Range(0, 1)]
    public float airControl = 0.5f;

    [Tooltip("이 값이 0보다 크면 캐릭터는 공중에 있는 동안 추진력을 잃음")]
    public float airMomentumFriction = 2;
    public float verticalMaxSpeed = 50f;

    public float inAirColliderHeight = 1f;

    bool readyToJump;
}
