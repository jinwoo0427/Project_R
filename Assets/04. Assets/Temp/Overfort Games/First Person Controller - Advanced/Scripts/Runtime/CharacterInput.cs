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
		private string hookInputKey = "Hook";

		[SerializeField]
		private string dashInputKey = "Dash";

		private float horizontalInput;
		private float verticalInput;

		private bool isJumpButtonBeingPressed;
		private bool isJumpButtonReleased;
		private bool isJumpButtonDown;
		private bool isSlideButtonBeingPressed;
		private bool isHookButtonDown;

		private bool isDashButtonBeingPressed;
		private bool isDashButtonDown;
		private bool isDashButtonReleased;

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


			isJumpButtonBeingPressed = Input.GetButton(jumpInputKey);
			isJumpButtonReleased = Input.GetButtonUp(jumpInputKey);
			isJumpButtonDown = Input.GetButtonDown(jumpInputKey);

			isDashButtonBeingPressed = Input.GetButton(dashInputKey);
			isDashButtonDown = Input.GetButtonDown(dashInputKey);
			isDashButtonReleased = Input.GetButtonUp(dashInputKey);

			isSlideButtonBeingPressed = Input.GetButton(slideInputKey);

			isHookButtonDown = Input.GetButtonDown(hookInputKey);

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

		public bool IsSlideButtonBeingPressed()
		{
			return isSlideButtonBeingPressed;
		}

		public bool IsHookButtonDown()
		{
			return isHookButtonDown;
		}
		public bool IsDashButtonBeingPressed()
		{
			return isDashButtonBeingPressed;
		}
		public bool IsDashButtonDown()
        {
			return isDashButtonDown;
        }

		public bool IsDashButtonReleased()
        {
			return isDashButtonReleased;
        }
		#endregion
	}
}
