using UnityEngine;

namespace OverfortGames.FirstPersonController
{
    public class CameraInput : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private string mouseHorizontalInput = "Mouse X";

        [SerializeField]
        private string mouseVerticalInput = "Mouse Y";

        //The inputs will be multiplied by this value
        [SerializeField]
        private float sensitivity = 0.01f;

        [SerializeField]
        private bool invertInputHorizontal;

        [SerializeField]
        private bool invertInputVertical;

        #endregion

        #region Methods

        public float GetHorizontal()
        {
            float input = Input.GetAxisRaw(mouseHorizontalInput);

            //Adjust input on timeScale
            if (Time.timeScale > 0f)
            {
                if (Time.deltaTime > 0)
                    input /= Time.deltaTime;

                input *= Time.timeScale;
            }
            else
            {
                input = 0;
            }

            //Apply sensitivity
            input *= sensitivity;

            if (invertInputHorizontal)
                input *= -1f;

            return input;
        }

        public float GetVertical()
        {
            //Get inverted input - this will result example in: move mouse up - look up ; move mouse down - look down
            float input = -Input.GetAxisRaw(mouseVerticalInput);

            //Adjust input on timeScale
            if (Time.timeScale > 0)
            {
                if (Time.deltaTime > 0)
                    input /= Time.deltaTime;

                input *= Time.timeScale;
            }
            else
            {
                return input;
            }

            //Apply sensitivity
            input *= sensitivity;

            if (invertInputVertical)
                input *= -1f;

            return input;
        }

        #endregion
    }
}
