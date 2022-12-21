using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Jinwoo.FirstPersonController
{
    public class FirstPersonController : MonoBehaviour
    {
        public enum ControllerState
        {
            Standing,
            InAir,
            TacticalSprint,
            Crouched,
            Proned,
            Sliding,
            Climb,
            Grappling,
            WallRun
        }

        #region Fields

        //이벤트
        public event Action<float> OnLand = delegate { };
        public event Action OnJump = delegate { };
        public event Action<int> OnJumpsCountIncrease = delegate { };
        public event Action OnSlide = delegate { }; //각 프레임 호출
        public event Action OnEndSlide = delegate { };
        public event Action OnBeginSlide = delegate { };

        public event Action OnBeginGrapplingLine = delegate { };
        public event Action OnEndGrapplingLine = delegate { };
        public event Action OnEndFailedGrapplingLine = delegate { };
        public event Action OnGrapplingLine = delegate { }; //각 프레임 호출
        public event Action OnBeginGrappling = delegate { };
        public Action OnEndGrappling = delegate { };
        public Action OnGrappling = delegate { }; //각 프레임 호출

        public event Action OnBeginWallRun = delegate { };
        public event Action OnWallRun = delegate { }; //각 프레임 호출
        public event Action OnEndWallRun = delegate { };

        public event Action OnClimbBegin = delegate { };
        public event Action OnClimbEnd = delegate { };

        public event Action OnColliderResized = delegate { };

        [Header("References"), Space(5)]

        public CharacterInput characterInput;

        public CameraInput cameraInput;

        public CameraController cameraController;

        public CharacterController characterController;

        public Transform cameraTransform;


        //스테이트 값
        private bool isGrounded;
        private bool isSliding;
        private float horizontal;
        private float vertical;
        private bool isJumpButtonBeingPressed;
        private bool isJumpButtonReleased;
        private bool isJumpButtonDown;
        private bool isRunButtonDoublePressedDown;
        private bool isRunning;
        private bool previousIsSliding;
        private bool previousIsGrounded;
        private bool wishToSlide;
        private bool isTryingToJump;
        private bool isProneButtonBeingPressed;
        private bool isProneButtonPressedDown;

        [Space(5), Header("Misc"), Space(5)]

        [Tooltip("캐릭터 위의 장애물을 감지하기 위한 레이어 마스크")]

        public LayerMask ceilingDetectionLayerMask;

        public bool enableCollisionPush = true;

        public float collisionPushPower = 40;

        public float defaultColliderMorphSpeed = 10;


        [Tooltip("물리 설정의 기본 중력에 이 값을 곱함")]
        public float gravityModifier = 2;

        [Space(5), Header("Settings"), Space(5)]

        public HorizontalSpeedSettings horizontalSpeedSettings;

        [System.Serializable]
        public class HorizontalSpeedSettings
        {
            [Tooltip("기본 보행 속도")]
            public float defaultSpeed = 5;

            public float backwardsSpeed = 4.5f;

            [Header("Advanced")]

            [Tooltip("캐릭터 정지 시 속도 감속량입니다. 값 0은 감속 없음을 의미")]
            public float movementDeceleration = 0.033f;

            public float horizontalMaxSpeed = 30;

        }

        [Space(15)]

        public JumpSettings jumpSettings;

        [System.Serializable]
        public class JumpSettings
        {
            [Tooltip("점프를 적응형으로 설정. 플레이어가 점프 버튼을 더 많이 누를수록 캐릭터가 더 높이 올라감")]
            public bool adaptiveJump = false;

            public int jumpsCount = 1;

            public float jumpForce = 10;
            
            [Header("Advanced")]
            [Tooltip("어댑티브 점프에 대한 최대 점프 버튼 누름 시간")]
            public float adaptiveJumpDuration = 0;

            [Tooltip("캐릭터가 접지되지 않은 상태에서 점프를 시작하는 데 필요한 시간")]
            public float coyoteTime = 0.15f;

            [Range(0, 1)]
            public float airControl = 0.5f;

            [Tooltip("이 값이 0보다 크면 캐릭터는 공중에 있는 동안 추진력을 잃게 됨")]
            public float airMomentumFriction = 2;

            public float verticalMaxSpeed = 50f;

            public float inAirColliderHeight = 1f;

        }

        [Space(15)]
        public bool enableRun = true;

        public RunSettings runSettings;

        [System.Serializable]
        public class RunSettings
        {
            public float runSpeed = 8;
            public bool canRunWhileStrafing = true;
        }


        [Space(15)]

        public bool enableTacticalSprint = true;

        public TacticalSprintSettings tacticalSprintSettings;

        [System.Serializable]
        public class TacticalSprintSettings
        {
            public float speed = 18;
            public float duration = 1.5f;
        }

        [Space(15)]
        public bool enableProne = true;

        public ProneSettings proneSettings;

        [System.Serializable]
        public class ProneSettings
        {
            public float speed = 1.5f;
            public float colliderHeight = 0.9f;
            public float colliderMorphSpeed = 10;
        }

        [Space(15)]
        public bool enableCrouch = true;

        public CrouchSettings crouchSettings;

        [System.Serializable]
        public class CrouchSettings
        {
            public float speed = 1.5f;
            public float colliderHeight = 1.2f;
            public float colliderMorphSpeed = 10;
        }


        [Space(15)]

        public bool enableClimb = true;
        public ClimbSettings climbSettings;

        [System.Serializable]
        public class ClimbSettings
        {
            [Tooltip("최대 상승 시간(초)")]
            public float maxDuration = 2;

            [Tooltip("상승 시간 지속 시간은 동적. 캐릭터가 등반 종료 위치에서 이 거리만큼 떨어져 있을 때 최대 지속 시간이 발생.")]
            public float durationMaxDistance = 3;

            [Tooltip("물체를 오를 수 있는 높이")]
            public float maxHeight = 2.5f;

            [Header("Advanced")]

            [Tooltip("이 곡선의 값은 등반 애니메이션의 lerp 시간에 곱함. 선형 곡선은 캐릭터가 시작점에서 끝점까지 동일한 속도로 이동")]
            public AnimationCurve animationSpeedCurve;

            public LayerMask climbableObjectLayerMask;

            [Tooltip("오르기 애니메이션을 트리거하기 위해 오르기 가능한 물체로부터의 최대 거리")]
            public float maxDistanceFromClimbableObject = 0.8f;

            public float cameraInclinationIntensity = 800;

            [Tooltip("이 곡선은 등반 애니메이션 중 카메라 피치의 속도를 나타냄. 이 곡선의 값은 cameraInclinationSpeed에 곱해짐.")]
            public AnimationCurve cameraInclinationIntensityCurve;

        }

        [Space(15)]

        public bool enableSlide = true;
        public SlideSettings slideSettings;

        [System.Serializable]
        public class SlideSettings
        {
            [Tooltip("슬라이딩이 시작될 때 적용되는 초기 힘")]
            public float initialForce = 20;

            public float groundFriction = 15;

            public float colliderHeight = 0.9f;

            [Header("Advanced")]

            [Tooltip("미끄러질 때 중력과 같은 힘이 가해짐")]
            public float slideGravity = 400;

            [Tooltip("캐릭터가 미끄러지기 위해 가져야 하는 최소 힘(제곱)")]
            public float minimumStopVelocity = 5;

            [Range(0, 1)]
            public float horizontalControl = 0.5f;

            [Tooltip("카메라가 캐릭터가 움직이는 다른 방향을 바라볼 때 적용되는 마찰")]
            public float cameraRotationFrictionFactor = 0.1f;

            public float colliderMorphSpeed = 10;
        }


        [Space(15)]

        public bool enableGrapplingHook = true;

        public GrapplingHookSettings grapplingHookSettings;

        [System.Serializable]
        public class GrapplingHookSettings
        {
            [Tooltip("재사용 대기시간은 캐릭터가 훅을 발사할 때 시작")]
            public float cooldown = 3f;

            [Tooltip("그래플링 중 캐릭터의 속도")]
            public float speedWhileHooked = 50;

            public float launchMaxDistance = 22;

            public float grapplingLaunchSpeed = 4;

            public Color crosshairColor = Color.white;

            [Header("Advanced")]

            public float horizontalControlStrength = 10f;

            [Tooltip("붙잡힌 상태에서 이 속도를 초과하면 캐릭터가 분리됩니다.")]
            public float detachSpeedLimitCondition = 27;

            [Tooltip("이 각도 조건을 확장하려면 'detachTimerCondition'을 사용하삼")]
            [Range(0,90)]
            public float detachAngleCondition = 90;

            [Tooltip("이 타이머는 연결 위치의 각도 > 'detachAngleCondition'일 때 시작됨. 이는 캐릭터가 그래플하는 동안 물체 주위를 회전하도록 만드는 데 유용할 수 있움.")]
            public float detachTimerCondition = 0.3f;

            [Tooltip("붙잡힌 상태에서 이 거리 내에서 목표 지점에 도달하면 캐릭터가 분리됨")]
            public float detachMinDistanceCondition = 3.25f;

            [Tooltip("그래플링이 대상에 부착될 때 캐릭터에 적용되는 수직력")]
            public float initialVerticalForce = 1;

            public LayerMask hookableObjectLayerMask;

            public GameObject hookPrefab;

            public float hookOffsetFromTarget = 0.5f;

            public Transform lineRendererStartPositionTransform;

            public Material lineRendererMaterial;

            public float lineRendererWidth = 0.05f;

            [Tooltip("그래플링 라인의 품질을 제어")]
            public int lineRendererSegmentCount = 100;

            public float lineRendererWaveStiffness = 800;

            public float lineRendererWaveStrength = 10;

            public float lineRendererWaveCount = 3;

        }

        [Space(15)]

        public bool enableWallRun = true;

        public WallRunSettings wallRunSettings;

        [System.Serializable]
        public class WallRunSettings
        {
            public float speedWhileRunning = 12f;

            public float verticalWallJumpForce = 2f;

            public float horizontalWallJumpForce = 12.5f;

            [Header("Advanced")]
            public LayerMask walkableObjectLayerMask;

            [Tooltip("캐릭터는 이 시간이 지나면 자동으로 벽에서 분리됨")]
            public float autoDetachTimerCondition = 1;

            [Tooltip("자동 분리된 후 캐릭터는 벽 방향과 반대 방향으로 이 힘을 받게 됨")]
            public float autoDetachForce = 2;

            [Tooltip("벽에서 분리되면 캐릭터는 이 시간(초) 동안 다시 부착할 수 없슴")]
            public float cooldown = 1.5f;

            [Tooltip("벽에 붙어있는 동안의 중력")]
            public float wallRunGravity = 0.5f;

            [Tooltip("벽에 부착되어 이동하는 동안의 중력")]
            public float wallRunGravityWhileMoving = 0.35f;

            [Tooltip("벽에 부착할 때 받는 최소 수직 부스트 힘. 이 수직력은 캐릭터가 벽에 부딪히는 동안 수직 이동의 양에 따라 달라짐.")]
            public float attachVerticalBoostMin = 2.5f;

            [Tooltip("벽에 부착할 때 받는 최대 수직 부스트 힘. 이 수직력은 캐릭터가 벽에 부딪히는 동안 수직 이동의 양에 따라 달라짐")]
            public float attachVerticalBoostMax = 5;

            [Tooltip("월 런을 트리거하기 위한 벽과의 최소 거리")]
            public float attachMinDistanceCondition = 0.35f;

            [Tooltip("캐릭터는 이 각도를 기준으로 측면의 벽을 감지")]
            public float attachSideAngleCondition = 20;

            [Tooltip("캐릭터가 벽을 향하는 방향에 대한 각도에 따라 카메라 틸트 lerp 값 대상이 변경되는지 여부임. " +
                "이 변수를 체크하지 않으면 카메라 기울기 값 대상이 고정됨. 캐릭터가 20도 이내에서 벽을 바라보고 있는 경우 회전 값은 0, " +
                "cameraTiltAngle 캐릭터가 벽에서 멀어지는 경우")]
            public bool dynamicCameraTilt = true;

            public float cameraTiltAngle = 15;

            public float cameraTiltLerpSpeed = 5;

            public float cameraTiltResetLerpSpeed = 10;
        }

        
        public ControllerState currentControllerState { private set; get; }
        private Transform bodyTransform;
        private float edgeFallFactor = 23;
        private Vector3 movement;
        private Vector3 previousMovement;
        private Vector3 momentum;
        private Vector3 releasedMomentum;
        private bool jumpLocked;
        private float currentJumpTimer;
        private bool isMorphingCollider;
        private float defaultColliderHeight;
        private Vector3 currentGroundNormal;
        private Vector3 velocity;
        private float lastTimeGrounded;
        private Vector3 currentGroundNormalController;
        private float colliderMaxRadius;
        private bool colliderLandingMorph = true;

        private bool isClimbingAnimation;
        private float climbDuration;
        private float climbTimer;
        private Vector3 climbStartPoint;
        private float climbStartDistanceSqr;
        private Vector3 climbEndPoint;
        private Vector3 climbEndPointRelativeToTarget;
        private Transform climbTarget;

        private Vector3 grapplingCurrentPoint;
        private float grapplingStartDistanceSqr;
        private Vector3 grapplingDirection;
        private Vector3 grapplingDirectionStart;
        private float grapplingCurrentDistance;
        public bool isGrappled;
        private LineRenderer grapplingLine;
        private GameObject grapplingLineHook;
        private float grapplingCurrentTimer;
        private float grapplingLaunchTimer = 0;
        private float grapplingCurrentDetachTimer;
        private Spring grapplingLineSpring;
        private Transform grapplingTarget;
        public Vector3? grapplingDestinationPoint;
        private Vector3 grapplingDestinationPointTargetLocalPosition;
        private Vector3[] grapplingLineSegmentsPositions;
        private float grapplingLineRendererDamper = 14;
        private float grapplingLineRendererWaveHeight = 1;


        private Transform tr;
        private float defaultGravityModifier;
        private bool callbacksEnabled;
        private int currentJumpsCount;

        private float currentTacticalSprintTimer;

        private float lastTimeGrappling;
        private float currentGroundSlope;
        private Vector3 edgeFallDirection;
        private bool raycastIsGrounded;

        private float cameraHorizontal;
        private float cameraVertical;

        private Vector3 currentWallRunDirection;
        private Vector3 currentWallRunNormal;

        private float lastTimeBeginWallRun;

        private Transform standingPlatform;

        private Vector3 currentPositionInStandingPlaform;
        private Vector3 currentLocalPositionInStandingPlaform;

        private Quaternion currentRotationInStandingPlatform;
        private Quaternion currentLocalRotationInStandingPlatform;

        private Vector3 relativeMovementOnStandingPlatform;

        private float inAirToStandingMorphSpeed = 5;

        private float landPositionY;
        private Vector3 previousVelocity;
        private float lastTimeJump;
        private float jumpEventCooldown = 0.5f;

        public bool freeze = false;

        //private bool startFirstScene = true;

        #endregion

        #region Methods

        private void Awake()
        {
            tr = transform;
            bodyTransform = tr;


            defaultGravityModifier = gravityModifier;
            defaultColliderHeight = characterController.height;

            colliderMaxRadius = characterController.radius;
            lastTimeGrappling = -grapplingHookSettings.cooldown;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            grapplingLineSpring = new Spring();
            grapplingLineSpring.SetTarget(0);

            //startFirstScene = true;

            freeze = false;
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {

            currentGroundNormalController = hit.normal;

            if (enableCollisionPush)
            {

                TryPushObject(hit);
            }


            if (hit.moveDirection.y < -0.9 && hit.normal.y > 0.41)
            {
                if (standingPlatform != hit.collider.transform)
                {
                    standingPlatform = hit.collider.transform;
                    UpdateMovingPlatformTransform();
                }
            }
            else
            {
                standingPlatform = null;
            }

        }

        private void Update()
        {
            horizontal = characterInput.GetHorizontalMovementInput();
            vertical = characterInput.GetVerticalMovementInput();
            isJumpButtonBeingPressed = characterInput.IsJumpButtonBeingPressed();
            isJumpButtonReleased = characterInput.IsJumpButtonReleased();
            isJumpButtonDown = characterInput.IsJumpButtonDown();
            isRunButtonDoublePressedDown = characterInput.IsRunButtonDoublePressedDown();
            isProneButtonBeingPressed = characterInput.IsProneButtonBeingPressed();
            isProneButtonPressedDown = characterInput.IsProneButtonPressedDown();
            isRunning = characterInput.IsRunButtonBeingPressed();
            isSliding = characterInput.IsSlideButtonBeingPressed();
            cameraHorizontal = cameraInput.GetHorizontal();
            cameraVertical = cameraInput.GetVertical();

            Simulate(Time.deltaTime, true);
        }

        private void LateUpdate()
        {
            DrawGrapplingLine(Time.deltaTime);

            HandleEdgeFalling();

            HandlePlatforms();
        }

        public void Simulate(float dt, bool callbacksEnabled)
        {
            this.callbacksEnabled = callbacksEnabled;

            Vector3 rayOrigin = GetTransformOrigin();
            Ray ray = new Ray(rayOrigin, Vector3.down);
            if (Physics.Raycast(ray, out var hit, 100))
            {
                currentGroundNormal = hit.normal;
            }

            isGrounded = CheckForGround();

            currentControllerState = DetermineControllerState();

            HandleClimb(dt);

            HandleGrappling(dt);

            HandleWallRun(dt);

            HandleTacticalSprint(dt);

            HandleMovementVelocity();

            HandleJump(dt);

            HandleGravityModifier();

            AddGravity(dt);

            HandleMomentum(dt);

            ApplyMovement(dt);


            previousMovement = movement;
            previousIsSliding = isSliding;
            previousIsGrounded = isGrounded;
            previousVelocity = velocity;

            velocity = characterController.velocity;

            
            if (currentControllerState != ControllerState.Climb)
                cameraController.RotateCamera(cameraHorizontal, cameraVertical, dt);

            //Debug.Log(cameraHorizontal + " : " + cameraVertical);
        }
        //private IEnumerator FirstSet()
        //{
        //    yield return new WaitForSeconds(1.5f);
        //    startFirstScene = false;
        //}
        public bool CheckForGround()
        {

            currentGroundSlope = CalculateSlope(currentGroundNormal);
            float controllerGroundNormalSlope = CalculateSlope(currentGroundNormalController);

            float slope = controllerGroundNormalSlope > currentGroundSlope ? controllerGroundNormalSlope : currentGroundSlope;

            float rayMaxDistance = Mathf.Max(slope / characterController.slopeLimit, characterController.stepOffset + 0.01f);

            raycastIsGrounded = Physics.Raycast(new Ray(GetTransformOrigin(), Vector3.down), rayMaxDistance);
            bool controllerIsGrounded = characterController.isGrounded;

            return raycastIsGrounded && controllerIsGrounded;
        }

        public ControllerState DetermineControllerState()
        {
            switch (currentControllerState)
            {
                case ControllerState.Standing:

                    //Standing ---> Grappling
                    if (isGrappled == true)
                    {
                        AddMomentum(tr.up * grapplingHookSettings.initialVerticalForce);
                        return ControllerState.Grappling;
                    }

                    //Standing ---> InAir
                    if (isGrounded == false)
                    {
                        return ControllerState.InAir;
                    }

                    //Standing ---> Sliding
                    if (enableSlide && IsSlidingButtonPressedDown() && isMorphingCollider == false && vertical > 0)
                    {
                        //충돌체를 슬라이드 높이로 모핑
                        SetColliderHeightAutoLerp(slideSettings.colliderHeight, slideSettings.colliderMorphSpeed);

                        //Momentum을 미리 설정
                        SetMomentum(new Vector3(GetVelocity().x, 0, GetVelocity().z) + new Vector3(GetVelocity().x, 0, GetVelocity().z).normalized * slideSettings.initialForce);

                        if (callbacksEnabled)
                            OnBeginSlide();

                        return ControllerState.Sliding;
                    }

                    //Standing ---> Crouched
                    if (enableCrouch && IsSlidingButtonPressedDown() && isMorphingCollider == false)
                    {
                        SetColliderHeightAutoLerp(crouchSettings.colliderHeight, crouchSettings.colliderMorphSpeed);

                        return ControllerState.Crouched;
                    }

                    //Standing ---> Tactical Sprint
                    if (isRunButtonDoublePressedDown && enableTacticalSprint)
                    {
                        return ControllerState.TacticalSprint;
                    }

                    //Standing ---> Proned
                    if (isProneButtonPressedDown && enableProne && isMorphingCollider == false)
                    {
                        SetColliderHeightAutoLerp(proneSettings.colliderHeight, proneSettings.colliderMorphSpeed);
                        return ControllerState.Proned;
                    }

                    return ControllerState.Standing;

                case ControllerState.InAir:

                    if (movement.y < 0 && isMorphingCollider == false)
                    {
                        if (colliderLandingMorph)
                        {
                            SetColliderHeightAutoLerp(defaultColliderHeight, inAirToStandingMorphSpeed, true);
                        }
                        else
                        {
                            StopColliderHeightAutoLerp();
                            ResizeCollider(defaultColliderHeight);
                        }
                    }

                    //InAir ---> Climb
                    if (enableClimb && CheckClimb())
                    {
                        return ControllerState.Climb;
                    }

                    //InAir ---> Grappling
                    if (isGrappled == true)
                    {
                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);
                        return ControllerState.Grappling;
                    }

                    if (enableSlide && IsSlidingButtonPressedDown())
                        wishToSlide = true;

                    //InAir ---> Sliding 
                    if (enableSlide && isGrounded && wishToSlide && isMorphingCollider == false)
                    {
                        SetMomentum(new Vector3(GetVelocity().x, 0, GetVelocity().z).normalized * runSettings.runSpeed);

                        wishToSlide = false;

                        SetColliderHeightAutoLerp(slideSettings.colliderHeight, slideSettings.colliderMorphSpeed);

                        if (callbacksEnabled)
                            OnBeginSlide();

                        return ControllerState.Sliding;
                    }

                    //InAir ---> Standing
                    if (isGrounded && wishToSlide == false)
                    {
                        //캐릭터가 서 있을 수 있는 충분한 공간이 있음
                        if (IsColliderSpaceFree(defaultColliderHeight))
                        {
                            SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);
                            return ControllerState.Standing;
                        }

                        //캐릭터가 웅크릴 수 있는 충분한 공간이 있음
                        if (IsColliderSpaceFree(crouchSettings.colliderHeight) && enableCrouch)
                        {
                            SetColliderHeightAutoLerp(crouchSettings.colliderHeight, crouchSettings.colliderMorphSpeed);
                            return ControllerState.Crouched;
                        }

                        //최소 공간 제한 지원 상태
                        if (enableProne)
                        {
                            SetColliderHeightAutoLerp(proneSettings.colliderHeight, proneSettings.colliderMorphSpeed);
                            return ControllerState.Proned;
                        }

                    }

                    //InAir ---> WallRun
                    if (enableWallRun)
                    {
                        Ray inAirWallRunRayCheckCenter = new Ray(GetColliderCenterPosition(), InputToMovementDirection());
                        Ray inAirWallRunRayCheckRight = new Ray(GetColliderCenterPosition(), Quaternion.AngleAxis(wallRunSettings.attachSideAngleCondition, bodyTransform.up)
                            * inAirWallRunRayCheckCenter.direction);
                        Ray inAirWallRunRayCheckLeft = new Ray(GetColliderCenterPosition(), Quaternion.AngleAxis(-wallRunSettings.attachSideAngleCondition, bodyTransform.up)
                            * inAirWallRunRayCheckCenter.direction);

                        float radius = 0.25f;

                        //벽을 바라보고 있으며 벽 오르기 쿨다운이 만료됨
                        if ((Physics.SphereCast(inAirWallRunRayCheckCenter, radius, out var inAirWallRunHit, wallRunSettings.attachMinDistanceCondition, wallRunSettings.walkableObjectLayerMask) ||
                            Physics.SphereCast(inAirWallRunRayCheckLeft, radius, out inAirWallRunHit, wallRunSettings.attachMinDistanceCondition, wallRunSettings.walkableObjectLayerMask) ||
                            Physics.SphereCast(inAirWallRunRayCheckRight, radius, out inAirWallRunHit, wallRunSettings.attachMinDistanceCondition, wallRunSettings.walkableObjectLayerMask)
                            )
                            && Time.time > lastTimeBeginWallRun + wallRunSettings.cooldown)
                        {
                            float verticalForce = 0;

                            //벽에 부착했을 때의 움직임에 상대적인 수직력을 부여함
                            if (movement.y > 0)
                            {
                                verticalForce = wallRunSettings.attachVerticalBoostMax * movement.y / (jumpSettings.jumpForce * wallRunSettings.wallRunGravity / defaultGravityModifier / 2);
                            }

                            verticalForce = Mathf.Clamp(verticalForce, wallRunSettings.attachVerticalBoostMin, wallRunSettings.attachVerticalBoostMax);
                            movement.y = verticalForce;

                            lastTimeBeginWallRun = Time.time;

                            if (callbacksEnabled)
                            {
                                OnBeginWallRun();
                            }

                            return ControllerState.WallRun;
                        }
                    }

                    return ControllerState.InAir;

                case ControllerState.TacticalSprint:

                    //TacticalSprint ---> Grappling
                    if (isGrappled == true)
                    {
                        AddMomentum(tr.up * grapplingHookSettings.initialVerticalForce);
                        return ControllerState.Grappling;
                    }

                    //TacticalSprint ---> InAir
                    if (isGrounded == false)
                    {
                        currentTacticalSprintTimer = 0;
                        return ControllerState.InAir;
                    }

                    //TacticalSprint ---> Standing (duration over)
                    if (IsTacticalSprintDurationOver())
                    {
                        currentTacticalSprintTimer = 0;
                        return ControllerState.Standing;
                    }

                    //TacticalSprint ---> Sliding
                    if (enableSlide && IsSlidingButtonPressedDown() && isMorphingCollider == false && vertical > 0)
                    {
                        currentTacticalSprintTimer = 0;

                        SetColliderHeightAutoLerp(slideSettings.colliderHeight, slideSettings.colliderMorphSpeed);

                        SetMomentum(new Vector3(GetVelocity().x, 0, GetVelocity().z) + new Vector3(GetVelocity().x, 0, GetVelocity().z).normalized * slideSettings.initialForce);

                        if (callbacksEnabled)
                            OnBeginSlide();

                        return ControllerState.Sliding;
                    }

                    //TacticalSprint ---> Crouched
                    if (enableCrouch && IsSlidingButtonPressedDown() && isMorphingCollider == false)
                    {
                        currentTacticalSprintTimer = 0;

                        SetColliderHeightAutoLerp(crouchSettings.colliderHeight, crouchSettings.colliderMorphSpeed);

                        return ControllerState.Crouched;
                    }

                    //TacticalSprint ---> Proned
                    if (isProneButtonPressedDown && enableProne && isMorphingCollider == false)
                    {
                        currentTacticalSprintTimer = 0;

                        SetColliderHeightAutoLerp(proneSettings.colliderHeight, proneSettings.colliderMorphSpeed);
                        return ControllerState.Proned;
                    }

                    return ControllerState.TacticalSprint;

                case ControllerState.Crouched:

                    //Crouched ---> Grappling
                    if (isGrappled == true)
                    {
                        AddMomentum(tr.up * grapplingHookSettings.initialVerticalForce);

                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);

                        return ControllerState.Grappling;
                    }

                    //Crouched ---> InAir
                    if (isGrounded == false && isMorphingCollider == false)
                    {
                        return ControllerState.InAir;
                    }

                    //Crouched ---> Standing
                    if (isGrounded == true && IsSlidingButtonPressedDown() && isMorphingCollider == false && IsColliderSpaceFree(defaultColliderHeight))
                    {
                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);

                        return ControllerState.Standing;
                    }

                    //Crouched ---> Standing (running)
                    if (isGrounded == true && enableRun && isRunning && isMorphingCollider == false && IsColliderSpaceFree(defaultColliderHeight))
                    {
                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);

                        return ControllerState.Standing;
                    }

                    //Crouched ---> Proned
                    if (isProneButtonPressedDown && enableProne && isMorphingCollider == false)
                    {
                        SetColliderHeightAutoLerp(proneSettings.colliderHeight, proneSettings.colliderMorphSpeed);

                        return ControllerState.Proned;
                    }

                    return ControllerState.Crouched;

                case ControllerState.Proned:

                    //Proned ---> Grappling
                    if (isGrappled == true)
                    {
                        AddMomentum(tr.up * grapplingHookSettings.initialVerticalForce);

                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);

                        return ControllerState.Grappling;
                    }

                    //Proned ---> InAir
                    if (isGrounded == false)
                    {
                        return ControllerState.InAir;
                    }

                    //Proned ---> Standing (점프 또는 엎드림 버튼 누르기)
                    if ((isProneButtonPressedDown || isJumpButtonDown) && isMorphingCollider == false && IsColliderSpaceFree(defaultColliderHeight))
                    {
                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);
                        return ControllerState.Standing;
                    }

                    //Proned ---> Crouched
                    if (enableCrouch && IsSlidingButtonPressedDown() && isMorphingCollider == false && IsColliderSpaceFree(crouchSettings.colliderHeight))
                    {
                        SetColliderHeightAutoLerp(crouchSettings.colliderHeight, crouchSettings.colliderMorphSpeed);

                        return ControllerState.Crouched;
                    }

                    return ControllerState.Proned;

                case ControllerState.Sliding:

                    //Sliding ---> Grappling
                    if (isGrappled == true)
                    {
                        if (callbacksEnabled)
                            OnEndSlide();

                        AddMomentum(tr.up * grapplingHookSettings.initialVerticalForce);

                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);

                        return ControllerState.Grappling;
                    }

                    //Sliding ---> Standing (최소 정지 속도)
                    if (GetVelocity().sqrMagnitude < slideSettings.minimumStopVelocity * slideSettings.minimumStopVelocity && isMorphingCollider == false)
                    {
                        SetMomentum(Vector3.zero);

                        if (callbacksEnabled)
                            OnEndSlide();

                        //캐릭터가 서 있을 수 있는 충분한 공간이 있음
                        if (IsColliderSpaceFree(defaultColliderHeight))
                        {
                            SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);

                            return ControllerState.Standing;
                        }

                        //캐릭터가 웅크릴 수 있는 충분한 공간이 있음
                        if (IsColliderSpaceFree(crouchSettings.colliderHeight) && enableCrouch)
                        {
                            SetColliderHeightAutoLerp(crouchSettings.colliderHeight, crouchSettings.colliderMorphSpeed);

                            return ControllerState.Crouched;
                        }

                        //최소 공간 제한 지원 상태
                        if (enableProne)
                        {
                            SetColliderHeightAutoLerp(proneSettings.colliderHeight, proneSettings.colliderMorphSpeed);

                            return ControllerState.Proned;
                        }
                    }

                    //Sliding ---> InAir
                    if (IsSlidingButtonPressedDown() && isMorphingCollider == false && isGrounded == false)
                    {
                        if (callbacksEnabled)
                            OnEndSlide();

                        return ControllerState.InAir;
                    }

                    //Sliding ---> Standing (슬라이드 버튼을 다시 눌러 슬라이딩 중지)
                    if (IsSlidingButtonPressedDown() && isMorphingCollider == false && IsColliderSpaceFree(defaultColliderHeight))
                    {
                        SetMomentum(Vector3.zero);

                        if (callbacksEnabled)
                            OnEndSlide();

                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);

                        return ControllerState.Standing;
                    }

                    //업데이트 콜백당 OnSlide
                    if (callbacksEnabled && isGrounded)
                        OnSlide();

                    //Sliding ---> InAir
                    if (isJumpButtonDown && IsColliderSpaceFree(defaultColliderHeight))
                    {
                        if (callbacksEnabled)
                            OnEndSlide();

                        return ControllerState.InAir;
                    }

                    return ControllerState.Sliding;

                case ControllerState.Climb:

                    //Climb ---> Standing 
                    if (isClimbingAnimation == false)
                    {
                        if (colliderLandingMorph)
                        {

                            if (IsColliderSpaceFree(defaultColliderHeight))
                            {
                                SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);
                                return ControllerState.Standing;
                            }

                            if (IsColliderSpaceFree(crouchSettings.colliderHeight) && enableCrouch)
                            {
                                SetColliderHeightAutoLerp(crouchSettings.colliderHeight, crouchSettings.colliderMorphSpeed);
                                return ControllerState.Crouched;
                            }

                            if (enableProne)
                            {
                                SetColliderHeightAutoLerp(proneSettings.colliderHeight, proneSettings.colliderMorphSpeed);
                                return ControllerState.Proned;
                            }

                        }
                        else
                        {
                            ResizeCollider(defaultColliderHeight);
                        }

                        return ControllerState.Standing;
                    }

                    return ControllerState.Climb;

                case ControllerState.Grappling:

                    //그래플링하는 동안 콜라이더 높이를 inAirHeight로 설정
                    if (characterController.height > jumpSettings.inAirColliderHeight)
                    {
                        StopColliderHeightAutoLerp();
                        ResizeCollider(jumpSettings.inAirColliderHeight);
                    }

                    if (isGrappled == false)
                    {
                        //Grappling ---> InAir
                        if (isGrounded == false)
                        {
                            movement.y = 0;
                            SetMomentum(GetVelocity());
                            if (colliderLandingMorph)
                            {
                                SetColliderHeightAutoLerp(defaultColliderHeight, inAirToStandingMorphSpeed, true);
                            }
                            else
                            {
                                StopColliderHeightAutoLerp();
                                ResizeCollider(defaultColliderHeight);
                            }
                            return ControllerState.InAir;
                        }
                        else //Grappling ---> Standing
                        {
                            if (colliderLandingMorph)
                            {
                                SetColliderHeightAutoLerp(defaultColliderHeight, inAirToStandingMorphSpeed, true);
                            }
                            else
                            {
                                StopColliderHeightAutoLerp();
                                ResizeCollider(defaultColliderHeight);
                            }
                            return ControllerState.Standing;
                        }
                    }

                    return ControllerState.Grappling;

                case ControllerState.WallRun:

                    //WallRun ---> Climb
                    if (enableClimb && CheckClimb())
                    {
                        return ControllerState.Climb;
                    }

                    //WallRun ---> InAir
                    if (Time.time > lastTimeBeginWallRun + wallRunSettings.autoDetachTimerCondition)
                    {
                        SetMomentum(-currentWallRunDirection * wallRunSettings.autoDetachForce);
                        currentWallRunDirection = Vector3.zero;

                        if (callbacksEnabled)
                        {
                            OnEndWallRun();
                        }
                        return ControllerState.InAir;
                    }

                    //Wallrun ---> Grappling
                    if (isGrappled)
                    {
                        if (callbacksEnabled)
                        {
                            OnEndWallRun();
                        }
                        return ControllerState.Grappling;
                    }

                    //WallRun ---> Standing
                    if (isGrounded == true)
                    {
                        currentWallRunDirection = Vector3.zero;

                        if (callbacksEnabled)
                        {
                            OnEndWallRun();
                        }

                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);
                        return ControllerState.Standing;
                    }

                    //Wallrun ---> Standing (벽에 부딪히지 않음)
                    Vector3 previousWallRunNormal = currentWallRunNormal;
                    Vector3 previousWallRunDirection = currentWallRunDirection;
                    if (CheckWallRunRaycast(bodyTransform.right, out var wallRunRay, out var hitWall) ||
                        CheckWallRunRaycast(-bodyTransform.right, out wallRunRay, out hitWall) ||
                        CheckWallRunRaycast(bodyTransform.forward, out wallRunRay, out hitWall) ||
                        CheckWallRunRaycast(-bodyTransform.forward, out wallRunRay, out hitWall))
                    {

                        //벽 방향 및 법선 수집
                        currentWallRunDirection = wallRunRay.direction;
                        currentWallRunNormal = hitWall.normal;

                    }
                    else //어떤 벽도 부딪치지 않음
                    {
                        currentWallRunDirection = Vector3.zero;

                        if (callbacksEnabled)
                        {
                            OnEndWallRun();
                        }

                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);
                        return ControllerState.Standing;
                    }


                    //WallRun ---> Standing 
                    float angleFromPreviousFrame = Vector3.Angle(previousWallRunNormal, currentWallRunNormal);
                    if (previousWallRunDirection != Vector3.zero && angleFromPreviousFrame > 20)
                    {
                        currentWallRunDirection = Vector3.zero;

                        if (callbacksEnabled)
                        {
                            OnEndWallRun();
                        }

                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);
                        return ControllerState.Standing;
                    }

                    //WallRun ---> Standing (벽에서 반대 방향을 바라봄)
                    if (previousWallRunDirection != Vector3.zero && Vector3.Dot(currentWallRunDirection, InputToMovementDirection()) < -0.6)
                    {
                        currentWallRunDirection = Vector3.zero;

                        if (callbacksEnabled)
                        {
                            OnEndWallRun();
                        }

                        SetColliderHeightAutoLerp(defaultColliderHeight, defaultColliderMorphSpeed);
                        return ControllerState.Standing;
                    }

                    //Wallrun ---> InAir (월런 점프)
                    if (isJumpButtonDown)
                    {
                        SetMomentum(Vector3.ClampMagnitude(-currentWallRunDirection * wallRunSettings.horizontalWallJumpForce + Vector3.up * wallRunSettings.verticalWallJumpForce + velocity, wallRunSettings.speedWhileRunning));
                        lastTimeBeginWallRun = Time.time - wallRunSettings.cooldown + 0.25f;
                        currentWallRunDirection = Vector3.zero;

                        if (callbacksEnabled)
                        {
                            OnEndWallRun();
                        }
                        return ControllerState.InAir;
                    }

                    //이 콜백은 컨트롤러 상태가 결정될 때마다(각 프레임) 호출됨
                    if (callbacksEnabled)
                    {
                        OnWallRun();
                    }

                    return ControllerState.WallRun;

                default:
                    return ControllerState.Standing;
            }
        }

        private void HandleClimb(float dt)
        {
            //클라이밍 할 수 없음
            if (enableClimb == false || currentControllerState != ControllerState.Climb)
                return;

            //클라이밍 애니메이션 시작
            if (isClimbingAnimation == false)
            {
                
                SetMomentum(Vector3.zero);
                movement = Vector3.zero;

                isClimbingAnimation = true;

                OnClimbBegin();
            }
            else
            {
                //클라이밍 애니메이션 진행 중
                if (climbTimer < 1)
                {
                    //카메라를 피치. 회전량은 cameraInclinationSpeedCurve에 의해 결정
                    //예: 시간 0 값 1 | 시간 1 값 0 , 카메라가 같은 양만큼 위아래로 회전함을 의미
                    float cameraPitchAmount = climbSettings.cameraInclinationIntensity * climbSettings.cameraInclinationIntensityCurve.Evaluate(climbTimer)
                        * (climbStartDistanceSqr / (climbSettings.durationMaxDistance * climbSettings.durationMaxDistance)) * dt;

                    cameraController.AddCameraPitch(cameraPitchAmount);
                    climbTimer += dt / climbDuration;

                    //맞춤형 클라이밍 곡선 값 얻기
                    float lerpValue = climbSettings.animationSpeedCurve.Evaluate(climbTimer);

                    climbEndPoint = climbTarget.TransformPoint(climbEndPointRelativeToTarget);

                    //캐릭터를 시작점에서 끝점으로 이동
                    Teleport(Vector3.Lerp(climbStartPoint, climbEndPoint, Mathf.Min(lerpValue, 1)));
                }
                else //클라이밍 애니메이션이 종료
                {
                    //Reset
                    climbTimer = 0;
                    isClimbingAnimation = false;
                    OnClimbEnd();
                }
            }
        }

        private void HandleGrappling(float dt)
        {
            //그래플링 활성화 안됨
            if (enableGrapplingHook == false)
                return;

            //라인 시작 핸들
            if (IsGrapplingOnCooldown() == false)
            {
                //카메라 방향에서 광선을 얻음
                Ray ray = cameraController.GetCamera().ViewportPointToRay(new Vector3(0.5f, 0.5f));

                //캐릭터가 훅 레이어 오브젝트 범위에 있음
                if (Physics.Raycast(ray, out var hit, grapplingHookSettings.launchMaxDistance, grapplingHookSettings.hookableObjectLayerMask))
                {
                    Crosshair.SetCrosshairColor(grapplingHookSettings.crosshairColor);

                    //플레이어가 훅 버튼을 누름
                    if (characterInput.IsHookButtonDown())
                    {
                        GrapplingLineBegin(hit.point);
                        grapplingTarget = hit.collider.transform;
                        
                        //훅에 맞은 대상과의 상대 위치 설정(대상이 움직이는 발판인 경우)
                        grapplingDestinationPointTargetLocalPosition = grapplingTarget.InverseTransformPoint(hit.point);
                    }
                }
                else //캐릭터가 그래플 오브젝트 범위에 있지 않음. 어쨌든 로프는 발사되지만 캐릭터는 움직이지 않음
                {
                    Crosshair.SetCrosshairToDefaultColor();

                    //플레이어가 훅 버튼을 누름.
                    if (characterInput.IsHookButtonDown())
                    {
                        GrapplingLineBegin(ray.origin + ray.direction * grapplingHookSettings.launchMaxDistance);
                        grapplingTarget = null;
                    }
                }

            }
            else //그래플링 쿨다운 상태
            {
                Crosshair.SetCrosshairToDefaultColor();
            }

            //핸들 라인 이동
            if (grapplingDestinationPoint != null)
            {
                //대상이 있는 경우 라인의 대상 지점을 업데이트함(움직이는 대상의 경우)
                if (grapplingTarget != null)
                {
                    grapplingDestinationPoint = grapplingTarget.TransformPoint(grapplingDestinationPointTargetLocalPosition);
                }

                OnGrapplingLine();

                //라인의 속도는 목적지로부터의 거리에 비례함
                grapplingLaunchTimer += dt * grapplingHookSettings.grapplingLaunchSpeed * (grapplingHookSettings.launchMaxDistance * grapplingHookSettings.launchMaxDistance / grapplingStartDistanceSqr);
                grapplingCurrentPoint = Vector3.Lerp(GetGrapplingLineStartPosition(), grapplingDestinationPoint.Value, grapplingLaunchTimer);

                //줄이 목적지에 도달함
                if ((grapplingCurrentPoint - grapplingDestinationPoint.Value).sqrMagnitude < 0.1f)
                {
                    grapplingDirectionStart = (grapplingCurrentPoint - GetTransformOrigin()).normalized;

                    
                    if (grapplingTarget != null)
                    {
                        OnEndGrapplingLine();
                        OnBeginGrappling();
                        isGrappled = true;
                    }
                    else //목표가 없음
                    {
                        OnEndFailedGrapplingLine();
                    }

                    grapplingDestinationPoint = null;
                    grapplingLaunchTimer = 0;
                }
            }

            //그래플된 캐릭터 이동 처리 **

            if (isGrappled == false) //아직 그래플링 사용하지 않음
            {
                grapplingCurrentDetachTimer = 0;
                grapplingCurrentTimer = 0;
                return;
            }

            //대상을 따라가도록 목적지 업데이트(움직이는 플랫폼의 경우)
            if (grapplingTarget != null)
            {
                grapplingCurrentPoint = grapplingTarget.TransformPoint(grapplingDestinationPointTargetLocalPosition);
                //grapplingHook.StartGrapple(grapplingTarget);
            }

            grapplingDirection = (grapplingCurrentPoint - GetTransformOrigin()).normalized;
            grapplingCurrentDistance = Vector3.Distance(grapplingCurrentPoint, tr.position);
            grapplingCurrentTimer += dt;

            OnGrappling();

            //현재 거리가 임계값 이하이거나 캐릭터의 속도가 제한을 초과하는 경우 캐릭터를 분리함.
            if (grapplingCurrentDistance <= grapplingHookSettings.detachMinDistanceCondition
                || GetCurrentSpeedSqr() > grapplingHookSettings.detachSpeedLimitCondition * grapplingHookSettings.detachSpeedLimitCondition
                || momentum.sqrMagnitude > grapplingHookSettings.detachSpeedLimitCondition * grapplingHookSettings.detachSpeedLimitCondition)
            {
                OnEndGrappling();
                isGrappled = false;
                grapplingDestinationPoint = null;
            }


            if (Vector3.Angle(grapplingDirection, grapplingDirectionStart) > grapplingHookSettings.detachAngleCondition)
            {
                grapplingCurrentDetachTimer += dt;
                if (grapplingCurrentDetachTimer > grapplingHookSettings.detachTimerCondition)
                {
                    OnEndGrappling();
                    isGrappled = false;
                    grapplingDestinationPoint = null;
                }
            }
        }

        private void HandleWallRun(float dt)
        {
            //월런이 활성화되지 않음
            if (enableWallRun == false)
                return;

            //접지 시 재사용 대기시간 재설정
            if (isGrounded)
            {
                lastTimeBeginWallRun = 0;
            }

            if (currentControllerState == ControllerState.WallRun && Vector3.Dot(currentWallRunNormal, bodyTransform.forward) < 0.5f)
            {
                float angle = Vector3.SignedAngle(bodyTransform.forward, currentWallRunDirection, bodyTransform.up);

                //카메라 기울기 값 대상은 각도에 따라 변경됩니다.
                if (wallRunSettings.dynamicCameraTilt)
                {
                    cameraController.SetCameraRootTiltLerped(angle / 90 * wallRunSettings.cameraTiltAngle, wallRunSettings.cameraTiltLerpSpeed, dt);
                }
                else 
                {
                    float unsignedAngle = Mathf.Abs(angle);
                    if (unsignedAngle > 20)
                    {
                        cameraController.SetCameraRootTiltLerped(Mathf.Sign(angle) * wallRunSettings.cameraTiltAngle, wallRunSettings.cameraTiltLerpSpeed, dt);
                    }
                    else
                    {
                        cameraController.SetCameraRootTiltLerped(0, wallRunSettings.cameraTiltResetLerpSpeed, dt);
                    }
                }
            }
            else
            {
                cameraController.SetCameraRootTiltLerped(0, wallRunSettings.cameraTiltResetLerpSpeed, dt);
            }
        }

        private void HandleTacticalSprint(float dt)
        {
            if (currentControllerState != ControllerState.TacticalSprint)
                return;

            //전술 스프린트 종료 여부를 결정하는 데 사용되는 타이머 값을 늘립니다.
            currentTacticalSprintTimer += dt;
        }


        private void HandleMovementVelocity()
        {
            Vector3 direction = InputToMovementDirection();

            float speedMultiplier;

            if (currentControllerState == ControllerState.Proned)
            {
                //'speedMultiplier'를 경향 속도로 설정
                speedMultiplier = proneSettings.speed;

                movement = new Vector3(direction.x * speedMultiplier, this.movement.y, direction.z * speedMultiplier);
            }

            if (currentControllerState == ControllerState.TacticalSprint)
            {
                //'speedMultiplier'를 전술 속도 또는 정상 속도 또는 백페달 속도로 설정
                if (vertical < 0)
                    speedMultiplier = horizontalSpeedSettings.backwardsSpeed;
                else
                {
                    if (horizontal > 0.05f || horizontal < -0.05f)
                    {
                        if (runSettings.canRunWhileStrafing && vertical > 0)
                        {
                            speedMultiplier = tacticalSprintSettings.speed;
                        }
                        else
                        {
                            speedMultiplier = horizontalSpeedSettings.defaultSpeed;
                        }
                    }
                    else
                    {
                        speedMultiplier = tacticalSprintSettings.speed;
                    }
                }

                movement = new Vector3(direction.x * speedMultiplier, this.movement.y, direction.z * speedMultiplier);
            }

            if (currentControllerState == ControllerState.Standing)
            {
                //'speedMultiplier'를 실행 속도 또는 정상 속도 또는 백페달 속도로 설정
                if (vertical < 0)
                    speedMultiplier = horizontalSpeedSettings.backwardsSpeed;
                else
                {
                    if (isRunning && enableRun)
                    {
                        if (horizontal > 0.05f || horizontal < -0.05f)
                        {
                            
                            if (runSettings.canRunWhileStrafing && vertical > 0)
                            {
                                speedMultiplier = runSettings.runSpeed;
                            }
                            else
                            {
                                speedMultiplier = horizontalSpeedSettings.defaultSpeed;
                            }
                        }
                        else //앞으로 입력을 누르고 있음
                        {
                            speedMultiplier = runSettings.runSpeed;
                        }
                    }
                    else
                    {
                        speedMultiplier = horizontalSpeedSettings.defaultSpeed;
                    }
                }

                movement = new Vector3(direction.x * speedMultiplier, this.movement.y, direction.z * speedMultiplier);
            }

            if (currentControllerState == ControllerState.Crouched)
            {
                speedMultiplier = crouchSettings.speed;

                movement = new Vector3(direction.x * speedMultiplier, this.movement.y, direction.z * speedMultiplier);
            }

            if (currentControllerState == ControllerState.InAir)
            {
                speedMultiplier = horizontalSpeedSettings.defaultSpeed;

                //공중 컨트롤 적용
                movement = new Vector3(direction.x * speedMultiplier * jumpSettings.airControl, this.movement.y, direction.z * speedMultiplier * jumpSettings.airControl);
            }

            if (currentControllerState == ControllerState.WallRun)
            {
                Vector3 projection = Vector3.ProjectOnPlane(direction, currentWallRunNormal);

                movement = new Vector3(projection.x * wallRunSettings.speedWhileRunning, this.movement.y, projection.z * wallRunSettings.speedWhileRunning);
            }
        }

        private void HandleJump(float dt)
        {
            float currentJumpSpeed = jumpSettings.jumpForce;

            if (previousVelocity.y >= 0 && velocity.y < 0)
            {
                landPositionY = GetColliderCeilPosition().y;
            }

            //착륙 처리
            if (previousIsGrounded == false && isGrounded == true)
            {
                if (callbacksEnabled && isMorphingCollider == false)
                    Land();
            }

            if (jumpSettings.jumpsCount == 0)
                return;

            //핸들 시작 점프
            if (previousIsGrounded == true && isGrounded == false)
            {
                if (callbacksEnabled && isMorphingCollider == false)
                    BeginJump();
            }

            //천장 이동 구속
            Ray ray = new Ray(GetColliderCeilPosition(), bodyTransform.up);
            if (Physics.Raycast(ray, 0.05f))
            {
                if (movement.y > 0)
                {
                    jumpLocked = true;
                    movement.y = 0;
                }
            }

            if (isGrounded)
            {
                //점프 값 리셋
                currentJumpTimer = 0;
                jumpLocked = false;
                lastTimeGrounded = Time.time;
                currentJumpsCount = 0;
            }

            //다중 점프 핸들
            if (isJumpButtonDown)
            {
                currentJumpsCount++;

                if (currentJumpsCount <= jumpSettings.jumpsCount)
                {
                    OnJumpsCountIncrease(currentJumpsCount);
                }
            }

            if (isJumpButtonReleased)
            {
                if (currentJumpsCount > jumpSettings.jumpsCount)
                {
                    jumpLocked = true;
                }
                else
                {
                    currentJumpTimer = 0;
                }
            }

            if (currentControllerState == ControllerState.WallRun)
            {
                currentJumpSpeed = jumpSettings.jumpForce * wallRunSettings.wallRunGravity / defaultGravityModifier;
            }

            if (jumpSettings.adaptiveJump)
            {
                if ((isJumpButtonBeingPressed && currentJumpTimer < jumpSettings.adaptiveJumpDuration)
                    && (isGrounded || IsJumpEdgeTimer() || (currentJumpsCount <= jumpSettings.jumpsCount && currentJumpsCount > 0))
                    && jumpLocked == false && currentControllerState != ControllerState.Proned && IsColliderSpaceFree(defaultColliderHeight * 1.5f))
                {
                    if (movement.y < 0)
                        movement.y = 0;

                    //점프 타이머 증가
                    currentJumpTimer += dt;

                    //점프 스피드 더하기
                    movement.y += currentJumpSpeed * dt;

                    isTryingToJump = true;

                    StopColliderHeightAutoLerp();
                    ResizeCollider(jumpSettings.inAirColliderHeight);
                }
                else
                {
                    isTryingToJump = false;
                }
            }
            else
            {
                if (isJumpButtonDown && (isGrounded || IsJumpEdgeTimer() || (currentJumpsCount <= jumpSettings.jumpsCount && currentJumpsCount > 0))
                    && currentControllerState != ControllerState.Proned && IsColliderSpaceFree(defaultColliderHeight * 1.5f))
                {
                    movement.y = currentJumpSpeed;
                    isTryingToJump = true;
                    StopColliderHeightAutoLerp();
                    ResizeCollider(jumpSettings.inAirColliderHeight);
                }
                else
                {
                    isTryingToJump = false;
                }
            }

            if (movement.y > jumpSettings.verticalMaxSpeed)
                movement.y = jumpSettings.verticalMaxSpeed;
        }

        private void HandleGravityModifier()
        {
            if (currentControllerState == ControllerState.WallRun)
            {
                if (InputToMovementDirection().sqrMagnitude > 0.01f)
                {
                    gravityModifier = wallRunSettings.wallRunGravityWhileMoving;
                }
                else
                {
                    gravityModifier = wallRunSettings.wallRunGravity;
                }

                return;
            }

            gravityModifier = defaultGravityModifier;
        }

        private void AddGravity(float dt)
        {
            if (currentControllerState == ControllerState.Grappling)
                return;

            if (isGrounded == false)
            {
                movement.y -= -Physics.gravity.y * gravityModifier * dt;
            }
            else
            {
                if (isTryingToJump == false)
                    movement.y = -0.1f;
            }
        }

        private void HandleMomentum(float dt)
        {

            if (currentControllerState == ControllerState.Grappling)
            {
                momentum += grapplingDirection * grapplingHookSettings.speedWhileHooked * dt * grapplingCurrentTimer;
                momentum += Vector3.ProjectOnPlane(cameraTransform.right, tr.up).normalized * horizontal * grapplingHookSettings.horizontalControlStrength * dt;

                releasedMomentum = momentum;
            }

            if (currentControllerState == ControllerState.Sliding)
            {
                Vector3 slideDirection = Vector3.ProjectOnPlane(-tr.up, currentGroundNormal).normalized;

                //지면에 투영된 캐릭터의 전방 방향 계산
                Vector3 groundDirection = Vector3.ProjectOnPlane(cameraTransform.forward, currentGroundNormal).normalized;

                //경사면에서 미끄러짐
                if (slideDirection.x < 0 || slideDirection.z < 0)
                {
                    momentum += slideDirection * dt * dt * slideSettings.slideGravity;
                }
                else
                {
                    //우리는 경사면에 있지 않음 'slideFriction' 값을 사용하여 모멘텀을 늦춤.
                    momentum = IncrementVectorTowardTargetVector(momentum, slideSettings.groundFriction, dt, Vector3.zero);

                    momentum = IncrementVectorTowardTargetVector(momentum, Vector3.Angle(momentum.normalized, groundDirection) * slideSettings.cameraRotationFrictionFactor, dt, Vector3.zero);
                }

                //운동량에 수평 이동 제어 적용
                momentum += Vector3.ProjectOnPlane(cameraTransform.right, tr.up).normalized * horizontal * horizontalSpeedSettings.defaultSpeed * slideSettings.horizontalControl * dt;

                momentum.y = 0;

                //미끄러지는 동안 최종 속도 계산에서 해제된 운동량을 사용함
                releasedMomentum = momentum;
            }

            if (currentControllerState == ControllerState.InAir)
            {
                if (momentum.sqrMagnitude > 0.01f)
                {
                    //'airMomentumFriction' 값을 사용하여 모멘텀을 늦춘다
                    momentum = IncrementVectorTowardTargetVector(momentum, jumpSettings.airMomentumFriction, dt, Vector3.zero);

                    releasedMomentum = momentum;
                }
                else
                {
                    momentum = releasedMomentum = new Vector3(movement.x, 0, movement.z);
                }

            }

            if (currentControllerState == ControllerState.Standing || currentControllerState == ControllerState.Crouched || currentControllerState == ControllerState.TacticalSprint || currentControllerState == ControllerState.Proned)
            {
                if (InputToMovementDirection().sqrMagnitude < 0.01f)
                {
                    momentum = IncrementVectorTowardTargetVector(momentum, 1 / Mathf.Max(0.01f, horizontalSpeedSettings.movementDeceleration), dt, Vector3.zero);
                    releasedMomentum = momentum;
                }
                else
                {
                    //정상적인 이동(걷기, 달리기 등...)을 하는 동안 "저장된" 운동량은 수평 이동됨
                    Vector3 horizontalMovement = previousMovement - ExtractDotVector(previousMovement, tr.up);
                    momentum = horizontalMovement;
                    releasedMomentum = Vector3.zero;
                }
            }

            momentum = Vector3.ClampMagnitude(momentum, horizontalSpeedSettings.horizontalMaxSpeed);
        }

        private void ApplyMovement(float dt)
        {
            Vector3 finalMovement = Vector3.zero;

            if (freeze)
            {
                //finalMovement = Vector3.zero;
                return;
            }
            if (currentControllerState == ControllerState.Sliding)
            {
                //미끄러지는 동안 방출된 운동량을 움직임으로 사용하면 안됨.
                finalMovement = releasedMomentum;

                //공중에서 미끄러질 때 현재 중력을 수직이동으로 사용
                if (isGrounded == false || isTryingToJump)
                    finalMovement.y = movement.y;
            }

            if (currentControllerState == ControllerState.Standing || currentControllerState == ControllerState.Crouched)
            {
                Vector3 momentumMovement = movement + releasedMomentum;

                finalMovement = momentumMovement;
            }

            if (currentControllerState == ControllerState.InAir)
            {
                finalMovement = movement + releasedMomentum;
            }

            if (currentControllerState == ControllerState.TacticalSprint)
            {
                Vector3 momentumMovement = movement + releasedMomentum;

                Vector3 verticalMovement = ExtractDotVector(momentumMovement, Vector3.up);

                //미끄러지지 않고 수평 이동을 실행 속도 또는 정상 속도로 고정
                Vector3 clampedHorizontalMovement = Vector3.ClampMagnitude(momentumMovement - verticalMovement, tacticalSprintSettings.speed);

                finalMovement = verticalMovement + clampedHorizontalMovement;
            }

            if (currentControllerState == ControllerState.Proned)
            {
                Vector3 momentumMovement = movement + releasedMomentum;

                Vector3 verticalMovement = ExtractDotVector(momentumMovement, Vector3.up);

                //미끄러지지 않고 수평 이동을 실행 속도 또는 정상 속도로 고정
                Vector3 clampedHorizontalMovement = Vector3.ClampMagnitude(momentumMovement - verticalMovement, proneSettings.speed);

                finalMovement = verticalMovement + clampedHorizontalMovement;
            }

            if (currentControllerState == ControllerState.Grappling)
            {
                finalMovement = releasedMomentum;
            }

            if (currentControllerState == ControllerState.WallRun)
            {
                finalMovement = movement;
            }

            //경사면을 내려갈 때 충돌을 피하기 위해 점프하지 않는 동안 캐릭터를 땅에 붙임
            if (isGrounded && isTryingToJump == false && isGrappled == false && currentControllerState != ControllerState.Climb)
            {
                finalMovement.y = -2000;
            }

            //수평 이동을 최대 수평 속도로 고정
            Vector3 finalMovementVertical = ExtractDotVector(finalMovement, Vector3.up);
            Vector3 finalMovementHorizontalClamped = Vector3.ClampMagnitude(finalMovement - finalMovementVertical, horizontalSpeedSettings.horizontalMaxSpeed);
            finalMovement = finalMovementVertical + finalMovementHorizontalClamped;

            //수직 이동을 최대 수직 속도로 고정
            if (finalMovement.y > jumpSettings.verticalMaxSpeed)
            {
                finalMovement.y = jumpSettings.verticalMaxSpeed;
            }

            //컨트롤러 이동
            characterController.Move(finalMovement * dt);

        }

        private void HandlePlatforms()
        {
            //우리는 플랫폼에 있음
            if (standingPlatform != null)
            {
                //플레이어 월드 위치
                Vector3 newGlobalPlatformPoint = standingPlatform.TransformPoint(currentLocalPositionInStandingPlaform);

                //이전 프레임 위치에 상대적인 이동
                relativeMovementOnStandingPlatform = newGlobalPlatformPoint - currentPositionInStandingPlaform;

                //움직임이 발생하면 플랫폼에 머물도록 캐릭터를 텔레포트함
                if (relativeMovementOnStandingPlatform.magnitude > 0.001f)
                {
                    Teleport(tr.position + relativeMovementOnStandingPlatform);
                }

                //이동 플랫폼 회전 지원
                Quaternion newGlobalPlatformRotation = standingPlatform.rotation * currentLocalRotationInStandingPlatform;
                Quaternion rotationDiff = newGlobalPlatformRotation * Quaternion.Inverse(currentRotationInStandingPlatform);

                //로컬 상향 벡터의 회전 방지
                rotationDiff = Quaternion.FromToRotation(rotationDiff * Vector3.up, Vector3.up) * rotationDiff;
                transform.rotation = rotationDiff * transform.rotation;
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);

                UpdateMovingPlatformTransform();
            }
            else
            {
                if (relativeMovementOnStandingPlatform.magnitude > 0.01f)
                {
                    relativeMovementOnStandingPlatform = Vector3.Lerp(relativeMovementOnStandingPlatform, Vector3.zero, Time.deltaTime);
                    Teleport(tr.position + relativeMovementOnStandingPlatform);
                }
            }
        }

        private void HandleEdgeFalling()
        {
            if (raycastIsGrounded == false && characterController.isGrounded && isClimbingAnimation == false)
            {
                //세계 공간을 가리키는 광선 만들기
                if (Physics.SphereCast(GetTransformOrigin() + characterController.center, characterController.radius - characterController.skinWidth, Vector3.down, out var hit, characterController.height * 0.7f))
                {
                    edgeFallDirection = (hit.normal + Vector3.down).normalized;
                    AddMomentum(edgeFallDirection * edgeFallFactor * Time.deltaTime);
                }
            }
        }


        //이 함수를 사용하여 임의의 거리만큼 캐릭터를 이동함. transform.position은 'CharacterController'에 의해 재정의되므로 사용하지 마라
        // 문제없이 각 프레임마다 캐릭터를 이동하는 데 사용할 수 있음
        public void Teleport(Vector3 worldPosition)
        {
            characterController.enabled = false;
            tr.position = worldPosition;
            characterController.enabled = true;
        }

        private void TryPushObject(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;

            if (body == null || body.isKinematic)
            {
                return;
            }

            //자신 아래에 있는 물체를 밀지 않음.
            if (hit.moveDirection.y < -0.3)
            {
                return;
            }

            //이동 방향에서 푸시 방향을 계산함
            //위아래가 아닌 측면으로만 개체를 ​​밀어 넣는다
            Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

            //푸시 적용
            body.AddForceAtPosition(pushDir * collisionPushPower, hit.point);

        }

        public void StopColliderHeightAutoLerp()
        {
            StopAllCoroutines();
            isMorphingCollider = false;
        }

        public void SetColliderHeightAutoLerp(float target, float morphSpeed)
        {
            StopColliderHeightAutoLerp();
            StartCoroutine(Co_SetColliderHeight(target, morphSpeed, false));
        }

        public void SetColliderHeightAutoLerp(float target, float speed, bool resize = false)
        {
            StopColliderHeightAutoLerp();
            StartCoroutine(Co_SetColliderHeight(target, speed, resize));
        }

        private IEnumerator Co_SetColliderHeight(float target, float speed, bool resize)
        {
            float threshold = 0.001f;
            isMorphingCollider = true;

            //캐릭터 높이가 타겟과 거의 같아질 때까지 반복
            while (Mathf.Abs(characterController.height - target) > threshold)
            {
                float oldHeight = characterController.height;
                float newHeight = Mathf.MoveTowards(characterController.height, target, speed * Time.deltaTime);

                characterController.radius = Mathf.Min(newHeight / 2f, colliderMaxRadius);
                characterController.height = newHeight;

                //콜라이더 베이스에서 변환 위치를 유지하기 위해 콜라이더를 다시 중앙에 배치함.
                characterController.center = new Vector3(characterController.center.x, characterController.height / 2, characterController.center.z);

                //콜라이더 중심을 조정했기 때문에 동일한 위치를 유지하기 위해 캐릭터를 위 또는 아래로 이동해야 함
                if (resize)
                {
                    if (characterController.height < oldHeight)
                    {
                        Teleport(tr.position + Vector3.up * (oldHeight - characterController.height));
                    }
                    else
                    {
                        Teleport(tr.position - Vector3.up * (characterController.height - oldHeight));
                    }
                }

                yield return null;
            }

            characterController.height = target;

            isMorphingCollider = false;

        }

        //보간 없이 콜라이더 높이 크기 조정
        private void ResizeCollider(float newHeight)
        {
            StopColliderHeightAutoLerp();

            float oldHeight = characterController.height;
            characterController.radius = Mathf.Min(newHeight / 2f, colliderMaxRadius);
            characterController.height = newHeight;
            characterController.center = new Vector3(characterController.center.x, characterController.height / 2, characterController.center.z);

            if (characterController.height < oldHeight)
            {
                Teleport(tr.position + Vector3.up * (oldHeight - characterController.height));
            }
            else
            {
                Teleport(tr.position + Vector3.up * (characterController.height - oldHeight));
            }

            OnColliderResized();
        }

        public void SetMomentum(Vector3 newMomentum)
        {
            momentum = newMomentum;
        }

        public void AddMomentum(Vector3 addMomentum)
        {
            momentum += addMomentum;
        }

        public Vector3 GetPosition()
        {
            return tr.position;
        }

        private float CalculateSlope(Vector3 normal)
        {
            return Vector3.Angle(normal, Vector3.up);
        }

        public Vector3 GetVelocity()
        {
            return velocity;
        }
        public void SetVelocity(Vector3 vel)
        {
            velocity = vel;
        }
        public bool IsGrounded()
        {
            return isGrounded;
        }

        //이 기능을 사용하여 대상 충돌체 높이로 캐릭터가 물체와 충돌하는지 여부를 확인함
        private bool IsColliderSpaceFree(float targetHeight)
        {
            float targetRadius = characterController.radius = Mathf.Min(targetHeight / 2f, colliderMaxRadius);

            Vector3 p1 = GetColliderBasePosition() + (Vector3.up * targetRadius) + (Vector3.up * 0.25f);
            Vector3 p2 = GetColliderBasePosition() + (Vector3.up * targetHeight) - (Vector3.up * targetRadius);

            climbGizmosP1 = p1;
            climbGizmosP2 = p2;

            if (Physics.CheckCapsule(p1, p2, characterController.radius, ceilingDetectionLayerMask))
            {
                return false;
            }

            return true;
        }

        //충돌기 베이스의 세계 위치
        public Vector3 GetColliderBasePosition()
        {
            return tr.TransformPoint(characterController.center - Vector3.up * characterController.height / 2);
        }

        //충돌기 센터의 세계 위치
        public Vector3 GetColliderCenterPosition()
        {
            return tr.TransformPoint(characterController.center);
        }

        public Vector3 GetColliderCeilPosition()
        {
            return transform.TransformPoint(characterController.center + Vector3.up * characterController.height / 2);
        }

        public Vector3 GetColliderCeilPositionLocal()
        {
            return characterController.center + (Vector3.up * characterController.height / 2);
        }

        private Vector3 GetTransformOrigin()
        {
            return GetColliderBasePosition() + Vector3.up * 0.05f;
        }

        private Vector3 GetGrapplingLineStartPosition()
        {
            return grapplingHookSettings.lineRendererStartPositionTransform.position;
        }

        private bool CheckClimb(bool autoClimb = false)
        {
            //등반을 하려면 플레이어가 전방 입력을 누르고 캐릭터가 공중에 있어야 함
            //예를 들어 벽을 향해 이동하는 동안 점프 또는 낙하

            if ((vertical > 0 && (isGrounded == false || isJumpButtonDown)) || autoClimb)
            {
                Vector3 p1 = GetColliderBasePosition() + Vector3.up * (characterController.radius);
                Vector3 p2 = GetColliderBasePosition() + Vector3.up * (characterController.height) - Vector3.up * (characterController.radius);

                //이 캡슐 캐스트는 캐릭터 앞에 오를 수 있는 물체가 있는지 확인함
                if (Physics.CapsuleCast(p1, p2, characterController.radius, bodyTransform.forward, out var climbDetectionRayHit, climbSettings.maxDistanceFromClimbableObject, climbSettings.climbableObjectLayerMask))
                {
                    //벽과 같은 장애물에 직면해야 함
                    if (CalculateSlope(climbDetectionRayHit.normal) > 80)
                    {
                        int wallCheckSegmentsMaxIterations= 5;
                        for (int i = 0; i < wallCheckSegmentsMaxIterations; i++)
                        {
                            Ray wallCheckSegmentRay = new Ray(GetColliderBasePosition() + Vector3.up * (characterController.stepOffset + 0.01f + climbSettings.maxHeight * i/ wallCheckSegmentsMaxIterations), bodyTransform.forward);

                            float wallCheckSegmentRayMaxDistance = climbSettings.maxDistanceFromClimbableObject + 0.01f + characterController.radius;

                            Debug.DrawRay(wallCheckSegmentRay.origin, wallCheckSegmentRay.direction * wallCheckSegmentRayMaxDistance, Color.red);

                            if (!Physics.Raycast(wallCheckSegmentRay, wallCheckSegmentRayMaxDistance, climbSettings.climbableObjectLayerMask))
                            {
                                float climbCharacterRadius = characterController.radius = Mathf.Min(jumpSettings.inAirColliderHeight / 2f, colliderMaxRadius);
                                int ledgeCheckSegmentsMaxIterations = 8;

                                for (int k = 1; k <= ledgeCheckSegmentsMaxIterations; k++)
                                {
                                    Ray ledgeDetectionRay = new Ray(wallCheckSegmentRay.origin + wallCheckSegmentRay.direction * wallCheckSegmentRayMaxDistance * k/ledgeCheckSegmentsMaxIterations, Vector3.down);
                                   // Debug.Break();
                                    Debug.DrawRay(ledgeDetectionRay.origin, ledgeDetectionRay.direction, Color.cyan);

                                    //목적지를 확인하고 클라이밍을 시작.
                                    if (Physics.Raycast(ledgeDetectionRay, out var freeSpaceHit, Mathf.Abs(wallCheckSegmentRay.origin.y - GetColliderBasePosition().y), climbSettings.climbableObjectLayerMask))
                                    {
                                        Vector3 freeSpaceHitPoint = freeSpaceHit.point;
                                        float distanceFromHitPointToSlopeLevel = climbCharacterRadius * Mathf.Sin(Mathf.Deg2Rad * CalculateSlope(freeSpaceHit.normal)) / Mathf.Sin(Mathf.Deg2Rad * (90 - CalculateSlope(freeSpaceHit.normal)));
                                        Vector3 capsuleCheckFreeSpacePoint1 = new Vector3(freeSpaceHitPoint.x, freeSpaceHitPoint.y + 0.05f + (climbCharacterRadius) + distanceFromHitPointToSlopeLevel, freeSpaceHitPoint.z);
                                        Vector3 capsuleCheckFreeSpacePoint2 = new Vector3(freeSpaceHitPoint.x, freeSpaceHitPoint.y + 0.05f + jumpSettings.inAirColliderHeight - (climbCharacterRadius), freeSpaceHitPoint.z);

                                        if (Physics.CheckCapsule(capsuleCheckFreeSpacePoint1, capsuleCheckFreeSpacePoint2, climbCharacterRadius + 0.01f, climbSettings.climbableObjectLayerMask) == false)
                                        {
                                            Vector3 checkFreeSpaceAboveCharacterOrigin = GetColliderCeilPosition();
                                            Vector3 checkFreeSpaceAboveCharacterDestination = checkFreeSpaceAboveCharacterOrigin + Vector3.up * (freeSpaceHitPoint.y - checkFreeSpaceAboveCharacterOrigin.y + defaultColliderHeight / 2);

                                            if (Physics.Linecast(checkFreeSpaceAboveCharacterOrigin, checkFreeSpaceAboveCharacterDestination, climbSettings.climbableObjectLayerMask) == false)
                                            {
                                                StopColliderHeightAutoLerp();
                                                ResizeCollider(jumpSettings.inAirColliderHeight);
                                                climbEndPoint = freeSpaceHit.point;
                                                climbStartPoint = tr.position;
                                                climbStartDistanceSqr = (climbStartPoint - climbEndPoint).sqrMagnitude;

                                                //타이머를 늘림. 'dt / ClimbDuration'은 진행률이 속도 기반이 아니라 기간 기반임을 의미
                                                climbDuration = Mathf.Min((climbSettings.maxDuration * (climbStartDistanceSqr / (climbSettings.durationMaxDistance * climbSettings.durationMaxDistance))),
                                                    climbSettings.maxDuration);

                                                climbTarget = freeSpaceHit.collider.transform;
                                                climbEndPointRelativeToTarget = climbTarget.InverseTransformPoint(climbEndPoint);

                                                return true;
                                            }
                                        }
                                    }

                                }

                               
                            }

                        }

                        
                    }
                }
            }
            return false;
        }

        private bool IsJumpEdgeTimer()
        {
            return Time.time < lastTimeGrounded + jumpSettings.coyoteTime;
        }

        public bool IsGrapplingOnCooldown()
        {
            return lastTimeGrappling + grapplingHookSettings.cooldown > Time.time;
        }

        private bool IsTacticalSprintDurationOver()
        {
            return currentTacticalSprintTimer >= tacticalSprintSettings.duration;
        }

        private bool CheckWallRunRaycast(Vector3 direction, out Ray ray, out RaycastHit hit)
        {
            ray = new Ray(GetColliderCenterPosition(), direction);
            return Physics.SphereCast(ray, 0.25f, out hit, wallRunSettings.attachMinDistanceCondition + 0.1f, wallRunSettings.walkableObjectLayerMask);
        }

        private void BeginJump()
        {
            if (currentControllerState == ControllerState.Sliding)
            {
                OnEndSlide();
            }

            if (lastTimeJump + jumpEventCooldown < Time.time)
            {
                lastTimeJump = Time.time;

                OnJump();
            }
        }

        private void Land()
        {
            if (currentControllerState == ControllerState.Sliding)
            {
                OnBeginSlide();
                return;
            }

            OnLand(Mathf.Abs(landPositionY - GetColliderCeilPosition().y));
        }

        private void UpdateMovingPlatformTransform()
        {
            currentPositionInStandingPlaform = transform.position;

            //스탠딩 플랫폼을 기준으로 로컬 위치 계산
            currentLocalPositionInStandingPlaform = standingPlatform.InverseTransformPoint(transform.position);

            currentRotationInStandingPlatform = transform.rotation;

            //스탠딩 플랫폼에 대한 로컬 회전 계산
            currentLocalRotationInStandingPlatform = Quaternion.Inverse(standingPlatform.rotation) * transform.rotation;
        }

        private void DrawGrapplingLine(float dt)
        {
            if (grapplingLine != null)
            {
                //캐릭터가 잡혔거나 대상 지점이 평가된 경우에만 라인 렌더러와 후크를 활성화함
                grapplingLine.enabled = grapplingDestinationPoint.HasValue || isGrappled;
                grapplingLineHook.SetActive(grapplingLine.enabled);

                if (grapplingLine.enabled)
                {

                    //그래플링 라인 렌더러 및 스프링 설정
                    if (grapplingLine.positionCount == 0)
                    {
                        grapplingLineSpring.SetVelocity(grapplingHookSettings.lineRendererWaveStrength);
                        grapplingLine.positionCount = grapplingHookSettings.lineRendererSegmentCount + 1;
                    }

                    grapplingLineSpring.SetDamper(grapplingLineRendererDamper);
                    grapplingLineSpring.SetStrength(grapplingHookSettings.lineRendererWaveStiffness);
                    grapplingLineSpring.Update(dt);

                    //그래플 포인트 찾기
                    var grapplePoint = grapplingCurrentPoint - (grapplingCurrentPoint - GetGrapplingLineStartPosition()).normalized * grapplingHookSettings.hookOffsetFromTarget;
                    var grappleStartPosition = GetGrapplingLineStartPosition();
                    var up = Quaternion.LookRotation((grapplePoint - grappleStartPosition).normalized) * Vector3.up;

                    if (grapplingLineSegmentsPositions == null)
                        grapplingLineSegmentsPositions = new Vector3[grapplingHookSettings.lineRendererSegmentCount + 1];


                    for (var i = 0; i < grapplingHookSettings.lineRendererSegmentCount + 1; i++)
                    {
                        var delta = i / (float)grapplingHookSettings.lineRendererSegmentCount;
                        var offset = up * grapplingLineRendererWaveHeight * Mathf.Sin(delta * grapplingHookSettings.lineRendererWaveCount * Mathf.PI) * grapplingLineSpring.Value;

                        Vector3 segmentPosition = Vector3.Lerp(grappleStartPosition, grapplePoint, delta);
                        grapplingLine.SetPosition(i, segmentPosition + offset);

                        grapplingLineSegmentsPositions[i] = segmentPosition;
                    }

                    //후크 위치를 라인 렌더러의 마지막 세그먼트로 설정
                    grapplingLineHook.transform.position = grapplingLineSegmentsPositions[grapplingLineSegmentsPositions.Length - 1];
                }
            }
        }

        //그래플링 라인이 시작되면 이 메서드가 호출됨
        private void GrapplingLineBegin(Vector3 target)
        {
            lastTimeGrappling = Time.time;
            grapplingCurrentPoint = GetGrapplingLineStartPosition();
            grapplingDestinationPoint = target;
            grapplingStartDistanceSqr = (grapplingCurrentPoint - grapplingDestinationPoint.Value).sqrMagnitude;

            //그래플링 라인이 아직 인스턴스화되지 않은 경우 인스턴스화하고 해당 속성을 설정
            if (grapplingLine == null)
            {
                grapplingLine = new GameObject("Grappling Hook Line").AddComponent<LineRenderer>();
                grapplingLine.material = grapplingHookSettings.lineRendererMaterial;
                grapplingLine.startWidth = grapplingHookSettings.lineRendererWidth;
                grapplingLine.endWidth = grapplingHookSettings.lineRendererWidth;
                grapplingLine.textureMode = LineTextureMode.Tile;
                grapplingLine.alignment = LineAlignment.View;
                grapplingLine.generateLightingData = true;
                grapplingLine.positionCount = grapplingHookSettings.lineRendererSegmentCount + 1;

                grapplingLineHook = Instantiate(grapplingHookSettings.hookPrefab, grapplingLine.transform);
            }

            grapplingLineSpring.Reset();
            if (grapplingLine.positionCount > 0)
                grapplingLine.positionCount = 0;

            grapplingLineHook.transform.rotation = Quaternion.LookRotation((grapplingDestinationPoint.Value - grapplingCurrentPoint).normalized, Vector3.up);

            OnBeginGrapplingLine();
        }

        public float GetLastTimeGrappling()
        {
            return lastTimeGrappling;
        }

        private Vector3 InputToMovementDirection()
        {
            Vector3 direction = Vector3.zero;

            direction += Vector3.ProjectOnPlane(cameraTransform.right, tr.up).normalized * horizontal;
            direction += Vector3.ProjectOnPlane(cameraTransform.forward, tr.up).normalized * vertical;
            direction.y = 0;
            direction.Normalize();

            return direction;
        }

        public Vector2 GetInputDirection()
        {
            return new Vector2(horizontal, vertical);
        }

        private bool IsSlidingButtonPressedDown()
        {
            bool result = isSliding == true && previousIsSliding == false;
            return result;
        }

        public float GetCurrentSpeedSqr()
        {
            return velocity.sqrMagnitude;
        }

        public bool IsTryingToJumpThisFrame()
        {
            return isTryingToJump;
        }    

        //Utility

        public Vector3 IncrementVectorTowardTargetVector(Vector3 _currentVector, float _speed, float _deltaTime, Vector3 _targetVector)
        {
            return Vector3.MoveTowards(_currentVector, _targetVector, _speed * _deltaTime);
        }

        public Vector3 ExtractDotVector(Vector3 _vector, Vector3 _direction)
        {
            if (_direction.sqrMagnitude != 1)
                _direction.Normalize();

            float _amount = Vector3.Dot(_vector, _direction);

            return _direction * _amount;
        }

        public static float SignedAngle360(Vector3 from, Vector3 to, Vector3 normal)
        {
            float angle = Vector3.SignedAngle(from, to, normal);
            if (angle < 0)
            {
                angle = 360 - angle * -1;
            }

            return angle;
        }

      

        Vector3 climbGizmosP1;
        Vector3 climbGizmosP2;

        #endregion

        public class Spring
        {
            private float strength;
            private float damper;
            private float target;
            private float velocity;

            public void Update(float deltaTime)
            {
                var direction = target - Value >= 0 ? 1f : -1f;
                var force = Mathf.Abs(target - Value) * strength;
                velocity += (force * direction - velocity * damper) * deltaTime;
                Value += velocity * deltaTime;
            }

            public void Reset()
            {
                velocity = 0f;
                Value = 0f;
            }

            public void SetValue(float value)
            {
                this.Value = value;
            }

            public void SetTarget(float target)
            {
                this.target = target;
            }

            public void SetDamper(float damper)
            {
                this.damper = damper;
            }

            public void SetStrength(float strength)
            {
                this.strength = strength;
            }

            public void SetVelocity(float velocity)
            {
                this.velocity = velocity;
            }

            public float Value { get; private set; }
        }

    }

}
