using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingSettings : MonoBehaviour
{
    public DetectObs detectVaultObject;
    public DetectObs detectVaultObstruction;
    public DetectObs detectClimbObject; 
    public DetectObs detectClimbObstruction;


    public DetectObs DetectWallL; 
    public DetectObs DetectWallR;

    public Animator cameraAnimator;

    public bool IsParkour;
    private float t_parkour;
    private float chosenParkourMoveTime;

    private bool CanVault;
    public float VaultTime; 
    public Transform VaultEndPoint;

    private bool CanClimb;
    public float ClimbTime; 
    public Transform ClimbEndPoint;
}
