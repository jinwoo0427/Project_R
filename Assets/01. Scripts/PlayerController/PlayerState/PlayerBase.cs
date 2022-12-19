using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Jinwoo.FirstPersonController;

public class PlayerBase : MonoBehaviour
{
    public PlayerState state;

    #region Fields

    [Header("참조 클래스들"), Space(5)]

    public CharacterInput characterInput;

    public CameraInput cameraInput;

    public CameraController cameraController;

    public CharacterController characterController;

    public Transform cameraTransform;

    //스테이트 값
    protected bool isGrounded;
    protected bool isSliding;
    protected float horizontal;
    protected float vertical;
    protected bool isJumpButtonBeingPressed;
    protected bool isJumpButtonReleased;
    protected bool isJumpButtonDown;
    protected bool isRunning;
    protected bool previousIsSliding;
    protected bool previousIsGrounded;
    protected bool wishToSlide;
    protected bool isTryingToJump;


    [Space(5), Header("Misc"), Space(5)]

    [Tooltip("Layer mask for detecting obstacle above the character")]

    public LayerMask ceilingDetectionLayerMask;

    public bool enableCollisionPush = true;

    public float collisionPushPower = 40;

    public float defaultColliderMorphSpeed = 10;


    [Tooltip("물리 설정의 기본 중력에 이 값을 곱한다.")]
    public float gravityModifier = 2;

    [Space(5), Header("Settings"), Space(5)]


    //public ControllerState currentControllerState { private set; get; }
    protected Transform bodyTransform;
    protected float edgeFallFactor = 23;
    protected Vector3 movement;
    protected Vector3 previousMovement;
    protected Vector3 momentum;
    protected Vector3 releasedMomentum;
    protected bool jumpLocked;
    protected float currentJumpTimer;
    protected bool isMorphingCollider;
    protected float defaultColliderHeight;
    protected Vector3 currentGroundNormal;
    protected Vector3 velocity;
    protected float lastTimeGrounded;
    protected Vector3 currentGroundNormalController;
    protected float colliderMaxRadius;
    protected bool colliderLandingMorph = true;

    protected bool isClimbingAnimation;
    protected float climbDuration;
    protected float climbTimer;
    protected Vector3 climbStartPoint;
    protected float climbStartDistanceSqr;
    protected Vector3 climbEndPoint;
    protected Vector3 climbEndPointRelativeToTarget;
    protected Transform climbTarget;

    protected Transform tr;
    protected float defaultGravityModifier;
    protected bool callbacksEnabled;
    protected int currentJumpsCount;

    protected float currentTacticalSprintTimer;

    protected float lastTimeGrappling;
    protected float currentGroundSlope;
    protected Vector3 edgeFallDirection;
    protected bool raycastIsGrounded;

    protected float cameraHorizontal;
    protected float cameraVertical;

    protected Vector3 currentWallRunDirection;
    protected Vector3 currentWallRunNormal;

    protected float lastTimeBeginWallRun;

    protected Transform standingPlatform;

    protected Vector3 currentPositionInStandingPlaform;
    protected Vector3 currentLocalPositionInStandingPlaform;

    protected Quaternion currentRotationInStandingPlatform;
    protected Quaternion currentLocalRotationInStandingPlatform;

    protected Vector3 relativeMovementOnStandingPlatform;

    protected float inAirToStandingMorphSpeed = 5;

    protected float landPositionY;
    protected Vector3 previousVelocity;
    protected float lastTimeJump;
    protected float jumpEventCooldown = 0.5f;

    #endregion
}
