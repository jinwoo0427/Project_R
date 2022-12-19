using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jinwoo.FirstPersonController;

public class GrapplingHook : MonoBehaviour
{
    [Header("Reference")]
    private PlayerController pm;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    private bool grappling;


    private void Start()
    {
        pm = GetComponent<PlayerController>();
    }


    private void Update()
    {
        if (pm._input.IsHookButtonDown()) StartGrapple();

        if (grapplingCdTimer > 0)
            grapplingCdTimer -= Time.deltaTime;
    }

    //private void LateUpdate()
    //{
    //    //if (grappling)
    //    //{
    //    //    lr.SetPosition(0, gunTip.position);
    //    //}
    //}

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;


        grappling = true;

        //pm.freeze = true;

        RaycastHit hit;
        //그래풀링이 되는 조건
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            StartCoroutine(ExecuteGrapple());
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            StartCoroutine(StopGrapple(grappleDelayTime));
        }

        lr.enabled = true;

        //lr.SetPosition(1, grapplePoint);
    }

    private IEnumerator ExecuteGrapple()
    {
        yield return new WaitForSeconds(grappleDelayTime);
        //pm.freeze = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        //pm.JumpToPosition(grapplePoint, highestPointOnArc);

        StartCoroutine(StopGrapple(1f));
    }

    public IEnumerator StopGrapple(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        //pm.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lr.enabled = false;
    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
