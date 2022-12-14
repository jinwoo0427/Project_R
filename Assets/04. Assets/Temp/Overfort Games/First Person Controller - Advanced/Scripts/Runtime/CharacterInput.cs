using UnityEngine;

namespace Jinwoo.FirstPersonController
{
	public class CharacterInput : MonoBehaviour
	{
		#region Fields

		[SerializeField]
		private bool useRawInput = true;

		[SerializeField]
		private string horizontalInputKey = "Horizontal";

		[SerializeField]
		private string verticalInputKey = "Vertical";

		[SerializeField]
		private string slideInputKey = "Slide";

		[SerializeField]
		private string jumpInputKey = "Jump";

		[SerializeField]
		private string runInputKey = "Run";

		[SerializeField]
		private string proneInputKey = "Prone";

		[SerializeField]
		private string hookInputKey = "Hook";

		[SerializeField]
		private string zoomKey = "Zoom";


		private float horizontalInput;
		private float verticalInput;

		private bool isRunButtonBeingPressed;
		private bool isJumpButtonBeingPressed;
		private bool isJumpButtonReleased;
		private bool isJumpButtonDown;
		private bool isSlideButtonBeingPressed;
		private bool isProneButtonBeingPressed;
		private bool isProneButtonPressedDown;
		private bool isHookButtonDown;
		private bool isRunButtonPressedDown;
		private bool isRunButtonDoublePressedDown;
		private bool isZoomButtonBeingPressed;

		private float lastTimeRunButtonBeingPressed;
		private float minimumTimerForDoublePress = 0.5f;

		#endregion

		#region Methods
		private void Update()
		{
			if (useRawInput)
			{
				horizontalInput = Input.GetAxisRaw(horizontalInputKey);
				verticalInput = Input.GetAxisRaw(verticalInputKey);
			}
			else
			{
				horizontalInput = Input.GetAxisRaw(horizontalInputKey);
				verticalInput = Input.GetAxis(verticalInputKey);
			}

			isRunButtonPressedDown = Input.GetButtonDown(runInputKey);
			isRunButtonBeingPressed = Input.GetButton(runInputKey);

			isJumpButtonBeingPressed = Input.GetButton(jumpInputKey);
			isJumpButtonReleased = Input.GetButtonUp(jumpInputKey);
			isJumpButtonDown = Input.GetButtonDown(jumpInputKey);

			isSlideButtonBeingPressed = Input.GetButton(slideInputKey);

			isProneButtonBeingPressed = Input.GetButton(proneInputKey);
			isProneButtonPressedDown = Input.GetButtonDown(proneInputKey);

			isHookButtonDown = Input.GetButtonDown(hookInputKey);

			isZoomButtonBeingPressed = Input.GetButton(zoomKey);

			if (isRunButtonPressedDown)
			{
				if (lastTimeRunButtonBeingPressed + minimumTimerForDoublePress > Time.time)
				{
					isRunButtonDoublePressedDown = true;
				}

				lastTimeRunButtonBeingPressed = Time.time;
			}
		}

		private void LateUpdate()
		{
			//업다운 인풋 리셋
			isRunButtonDoublePressedDown = false;
		}

		public float GetHorizontalMovementInput()
		{
			return horizontalInput;
		}

		public float GetVerticalMovementInput()
		{
			return verticalInput;
		}

		public bool IsJumpButtonBeingPressed()
		{
			return isJumpButtonBeingPressed;
		}

		public bool IsJumpButtonReleased()
		{
			return isJumpButtonReleased;
		}

		public bool IsJumpButtonDown()
		{
			return isJumpButtonDown;
		}

		public bool IsZoomButtonBeingPressed()
		{
			return isZoomButtonBeingPressed;
		}
		public bool IsRunButtonBeingPressed()
		{
			return isRunButtonBeingPressed;
		}

		public bool IsRunButtonDoublePressedDown()
		{
			return isRunButtonDoublePressedDown;
		}

		public bool IsProneButtonBeingPressed()
		{
			return isProneButtonBeingPressed;
		}
		public bool IsProneButtonPressedDown()
		{
			return isProneButtonPressedDown;
		}

		public bool IsRunButtonPressedDown()
		{
			return isRunButtonPressedDown;
		}

		public bool IsSlideButtonBeingPressed()
		{
			return isSlideButtonBeingPressed;
		}


		public bool IsHookButtonDown()
		{
			return isHookButtonDown;
		}

		#endregion
	}
}
