using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jinwoo.FirstPersonController;

public class GrapplingHook : MonoBehaviour
{
    [Header("Reference")]
    private FirstPersonController pm;
    public CameraController cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    private float grapplingCdTimer;

    private bool grappling;

    //public Rigidbody rb;

    private void Start()
    {
        pm = GetComponent<FirstPersonController>();


        maxGrappleDistance = pm.grapplingHookSettings.launchMaxDistance;
        //grappleDelayTime = pm.grapplingHookSettings.detachTimerCondition;
    }


    private void Update()
    {
        //if (pm.characterInput.IsHookButtonDown()) StartGrapple();


        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    //private void LateUpdate()
    //{
    //    //if (grappling)
    //    //{
    //    //    lr.SetPosition(0, gunTip.position);
    //    //}
    //}

    public void StartGrapple(Transform target)
    {
        if (grapplingCdTimer > 0) return;


        grappling = true;

        //pm.freeze = true;

        pm.OnGrappling();

        //RaycastHit hit;
        ////그래풀링이 되는 조건
        //if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        //{
        //    grapplePoint = hit.point;

        //    StartCoroutine(ExecuteGrapple());
        //}
        //else
        //{
        //    grapplePoint = cam.position + cam.forward * maxGrappleDistance;

        //    StartCoroutine(StopGrapple(grappleDelayTime));
        //}

        grapplePoint = target.position;

        StartCoroutine(ExecuteGrapple());

        //lr.enabled = true;

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

        JumpToPosition(grapplePoint, highestPointOnArc);

        StartCoroutine(StopGrapple(1.5f));
    }

    public IEnumerator StopGrapple(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        pm.OnEndGrappling();

        pm.isGrappled = false;
        pm.grapplingDestinationPoint = null;

        //pm.freeze = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

    }

    public bool IsGrappling()
    {
        return grappling;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    private Vector3 velocityToSet;
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
        //SetVelocity();

        Invoke(nameof(ResetRestrictions), 1f);
    }

    private void SetVelocity()
    {
        pm.SetVelocity(velocityToSet);

        cam.SetFOV(110f);
    }

    public void ResetRestrictions()
    {
        cam.SetFOV(85f);
    }
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        //Debug.Log(gravity);
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
