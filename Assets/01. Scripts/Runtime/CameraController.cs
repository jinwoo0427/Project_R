using UnityEngine;

namespace Jinwoo.FirstPersonController
{
	public class CameraController : MonoBehaviour
	{
		#region Fields

		// 카메라 구성 요소에 대한 참조
		[Header("References")]
		[SerializeField]
		private new Camera camera; // new를 사용해서 재정의

		[SerializeField]
		private Transform cameraRoot; //카메라 위치

		[SerializeField]
		private Transform bodyRoot;   //바디 위치 (플레이어위치)

		[SerializeField]
		private FirstPersonController controller;

		// 카메라 입력 관련 
		[SerializeField]
		private CameraInput cameraInput;

		[Space(5), Header("Settings")]

		[SerializeField]
		private Vector3 cameraRootOffsetFromColliderCeil = new Vector3(0, -0.35f, -0.1f); //카메라오프셋

		//수직 제한
		[SerializeField, Range(0f, 90f)]
		private float upperLimit = 90;

		//좌우 제한
		[SerializeField, Range(0f, 90f)]
		private float lowerLimit = 90f;

		//회전 속도
		[SerializeField]
		private float cameraSpeed = 250f;

		[Space(15)]
		//헤드밥 관련
		public bool enableHeadbob = true;

		public HeadbobSetting headbobSettings;

		public Transform target;

		private Vector3 headbobTargetCurrentLocalPosition; 

		private float headbobCurrentFootstepDistance; //현재 헤드밥 거리


		//현재 회전 각도
		private float currentAngleX = 0f;
		private float currentAngleY = 180f;

		private Vector3 cameraRootInitialLocalPosition = Vector3.zero;


		//Transform 캐싱변수
		private Transform tr;

		private float defaultFov;

		private float headbobCurrentMovementSpeed;

		private Vector3 headbobCurrentOffset = Vector3.zero;
		
		private float headbobSpeedAcceleration = 5;

		#endregion

		#region Methods

		private void Awake()
		{
			if (cameraRoot == null)
				cameraRoot = transform.parent;

			//사용하기 편하게 캐싱한거
			tr = transform;

			bodyRoot = transform.GetComponentInParent<FirstPersonController>().transform;
			target = cameraRoot;

			controller = bodyRoot.GetComponent<FirstPersonController>();
			cameraInput = transform.GetComponent<CameraInput>();

			headbobSettings.target = target;

			//사전 설정 각도 가져오기
			currentAngleX = tr.localRotation.eulerAngles.x;
			currentAngleY = tr.localRotation.eulerAngles.y;

			//초기 카메라 위치
			cameraRootInitialLocalPosition = cameraRoot.localPosition;

			defaultFov = camera.fieldOfView;

			//플레이어콜라이더 높이와 오프셋들 더함
			cameraRoot.localPosition = controller.GetColliderCeilPositionLocal() + cameraRootOffsetFromColliderCeil;

			controller.OnColliderResized += OnColliderResized;
		}

		private void LateUpdate()
		{
			//카메라 기본 포지션
			Vector3 anchorPoint = controller.GetColliderCeilPositionLocal() + cameraRootOffsetFromColliderCeil;

			if (headbobSettings.target != cameraRoot || enableHeadbob == false)
			{
				cameraRoot.localPosition = anchorPoint;
			}

			headbobTargetCurrentLocalPosition = anchorPoint;

			HandleHeadbob();
		}

		private void OnColliderResized()
		{
			Vector3 anchorPoint = controller.GetColliderCeilPositionLocal() + cameraRootOffsetFromColliderCeil;

			if (headbobSettings.target != cameraRoot)
			{
				cameraRoot.localPosition = anchorPoint;
			}

			headbobTargetCurrentLocalPosition = anchorPoint;

		}
		/// <summary>
		/// 헤드밥 검사
		/// </summary>
		private void HandleHeadbob()
		{
			if (enableHeadbob == false)
				return;

			if ((controller.currentControllerState != FirstPersonController.ControllerState.Standing &&
				controller.currentControllerState != FirstPersonController.ControllerState.Crouched &&
				controller.currentControllerState != FirstPersonController.ControllerState.TacticalSprint) || 
				controller.IsTryingToJumpThisFrame())
			{
				ResetHeadbob();
				return;
			}

			Vector3 vel = controller.GetVelocity();

			//앞뒤 좌우 속도만 가져오기
			Vector3 horizontalVelocity = new Vector3(vel.x, 0, vel.z);

			//정확한 수치로 변환
			float magnitude = horizontalVelocity.magnitude;

			if (magnitude > 0.01f)
			{
				DoHeadbob(magnitude);
			}
			else
			{
				ResetHeadbob();
			}

		}
		/// <summary>
		/// 헤드밥 리셋
		/// </summary>
		private void ResetHeadbob()
		{
			headbobCurrentOffset = Vector3.MoveTowards(headbobCurrentOffset, Vector3.zero, headbobSettings.resetLerpSpeed * Time.deltaTime);
			headbobSettings.target.localPosition = headbobTargetCurrentLocalPosition + headbobCurrentOffset;

			headbobCurrentFootstepDistance = 0;
		}

		/// <summary>
		/// 헤드밥 실행
		/// </summary>
		/// <param name="currentMovementSpeed">현재 플레이어 속도값</param>
		private void DoHeadbob(float currentMovementSpeed)
		{
			headbobCurrentMovementSpeed = Mathf.Lerp(headbobCurrentMovementSpeed, currentMovementSpeed, headbobSpeedAcceleration * Time.deltaTime);

			headbobCurrentFootstepDistance += Time.deltaTime * headbobCurrentMovementSpeed;

			//헤드밥 스템 거리가 세팅값보다 커지면 다시 0
			if (headbobCurrentFootstepDistance > headbobSettings.cycleDistance)
				headbobCurrentFootstepDistance = 0;

			float speedPercentage = 0f;
			float halfheadbobDistance = headbobSettings.cycleDistance / 2; //헤드밥 사이클 거리의 절반 거리

			if (headbobCurrentFootstepDistance < halfheadbobDistance) //현재 헤드밥 스탭이 세팅의 절반 값보다 작을때 (초반 절반)
			{
				speedPercentage = headbobCurrentFootstepDistance / halfheadbobDistance; //스피드 퍼센트 증가 (0~1)
			}
			else //현재 헤드밥 스탭이 세팅의 절반 값보다 클때
			{ 
				//현재 스탭거리에서 사이클 거리만큼뺸 비율이니깐 가면 갈수록 값이 줄어듦 예시 ) 절댓값(-땡땡나오겟지>?)
				speedPercentage = Mathf.Abs(headbobCurrentFootstepDistance - headbobSettings.cycleDistance ) / halfheadbobDistance; //스피드 퍼센트 감소
			}
			
			float currentIntensityHorizontal = 0;

			//키보드 입력 direction 절댓값
			Vector2 absInputDirection = new Vector2(Mathf.Abs(controller.GetInputDirection().x), Mathf.Abs(controller.GetInputDirection().y));
			
			//좌우 입력 또는 가만히 있을때는 수평인텐시티만 0
			if (absInputDirection.x > 0 || (absInputDirection.x == 0 && absInputDirection.y == 0))
			{
				currentIntensityHorizontal = 0;
			}
			else
			{
				//애니메이션 커브값에 따른(0~1) 수평인텐시티값
				currentIntensityHorizontal = headbobSettings.maxHorizontalIntensity * 
					headbobSettings.horizontalIntensityCurve.Evaluate(speedPercentage); 
			}

			//애니메이션 커브값에 따른(0~1) 수직인텐시티값
			float currentIntensityVertical = headbobSettings.maxVerticalIntensity * 
				headbobSettings.verticalIntensityCurve.Evaluate(speedPercentage);


			Vector3 targetOffset = new Vector3(
				//수평 인텐시티에 기본 속도비율에 인텐시티스피드멀티플라이어를 제곱
				currentIntensityHorizontal * (Mathf.Pow(
				currentMovementSpeed / controller.horizontalSpeedSettings.defaultSpeed, headbobSettings.intensitySpeedMultiplier))
				, currentIntensityVertical * (Mathf.Pow(
				currentMovementSpeed / controller.horizontalSpeedSettings.defaultSpeed, headbobSettings.intensitySpeedMultiplier))
				, 0);

			//현재 헤드밥오프셋을 lerp로 부드럽게 함 (현재값에서, targetOffset값까지)럴프
			headbobCurrentOffset = Vector3.Lerp(headbobCurrentOffset, targetOffset, headbobSettings.lerpSpeed * Time.deltaTime);
			//현재포지션에서 오프셋값 더한 걸로 바꿈
			headbobSettings.target.localPosition = headbobTargetCurrentLocalPosition + headbobCurrentOffset;
		}

		/// <summary>
		/// 카메라 입력받기와 회전
		/// </summary>
		public void InputHandleRotation()
		{
			//input
			float horizontal = cameraInput.GetHorizontal();
			float vertical = cameraInput.GetVertical();

			RotateCamera(horizontal, vertical, Time.deltaTime);
		}

		/// <summary>
		/// dt를 업데이트 빈도로 사용하여 카메라 회전
		/// </summary>
		public void RotateCamera(float horizontalInput, float verticalInput, float dt)
		{
			currentAngleX += verticalInput * cameraSpeed * dt;
			currentAngleY += horizontalInput * cameraSpeed * dt;

			//회전각도 한계 설정
			currentAngleX = Mathf.Clamp(currentAngleX, -upperLimit, lowerLimit);

			UpdateRotation();
		}

		/// <summary>
		/// currentAngleX 및 currentAngleY를 기반으로 카메라 회전 업데이트
		/// </summary>
		private void UpdateRotation() //여기서 실질적으로 회전함
		{
			bodyRoot.localRotation = Quaternion.Euler(new Vector3(0, currentAngleY, 0)); 
			tr.localRotation = Quaternion.Euler(new Vector3(currentAngleX, 0, 0)); 
		}

		public void SetFOV(float fov)
		{
			camera.fieldOfView = fov;
		}

		public void SetDefaultFOV(float fov)
		{
			defaultFov = fov;
			SetFOV(defaultFov);
		}

		public void SetFOVLerped(float fov, float lerpSpeed, float dt)
		{
			camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, fov, lerpSpeed * dt);
		}

		public void ResetFOVLerped(float lerpSpeed, float dt)
		{
			SetFOVLerped(defaultFov, lerpSpeed, dt);
		}

		/// <summary>
		/// 회전 각도를 직접 설정
		/// </summary>
		public void SetRotationAngles(float angleX, float angleY)
		{
			currentAngleX = angleX;
			currentAngleY = angleY;

			UpdateRotation();
		}

		public float GetDefaultFOV() => defaultFov;

		public float GetCurrentAngleX()
		{
			return currentAngleX;
		}

		public float GetCurrentAngleY()
		{
			return currentAngleY;
		}

		public Camera GetCamera()
		{
			return camera;
		}

		public Transform GetRoot()
		{
			return cameraRoot;
		}

		public float GetCameraSpeed() => cameraSpeed;
		public void SetCameraSpeed(float value) { cameraSpeed = value; }

		/// <summary>
		/// 카메라 바로 기울이기
		/// </summary>
		public void SetCameraRootTilt(float angle)
		{
			cameraRoot.transform.localRotation = Quaternion.Euler(cameraRoot.transform.localRotation.eulerAngles.x, cameraRoot.transform.localRotation.eulerAngles.y, angle);
		}
		/// <summary>
		/// 카메라 부드럽게 기울이기
		/// </summary>
		/// <param name="angle">기울일 각도</param>
		/// <param name="speed">스피드</param>
		/// <param name="dt">업데이트 속도</param>
		public void SetCameraRootTiltLerped(float angle, float speed, float dt)
		{
			cameraRoot.transform.localRotation = Quaternion.Lerp(cameraRoot.transform.localRotation, Quaternion.Euler(cameraRoot.transform.localRotation.eulerAngles.x, cameraRoot.transform.localRotation.eulerAngles.y, angle), speed * dt);
		}
		/// <summary>
		/// 카메라 피치 (카메라 흔들림 정도?라 보면될듯)
		/// </summary>
		public void AddCameraPitch(float amount)
		{
			tr.localRotation *= Quaternion.Euler(Vector3.right * amount);
		}



		#endregion
	}
}
