using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingSettings : MonoBehaviour
{
    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    private bool grappling;

}
