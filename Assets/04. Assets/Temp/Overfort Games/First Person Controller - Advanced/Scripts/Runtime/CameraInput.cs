using UnityEngine;

namespace Jinwoo.FirstPersonController
{
    public class CameraInput : MonoBehaviour
    {
        #region Fields

        [SerializeField]
        private string mouseHorizontalInput = "Mouse X";

        [SerializeField]
        private string mouseVerticalInput = "Mouse Y";

        //민감도(마우스 감도)
        [SerializeField]
        private float sensitivity = 0.01f;

        #endregion

        #region Methods


        /// <summary>
        /// 좌우 마우스 움직임 값 반환
        /// </summary>
        /// <returns></returns>
        public float GetHorizontal()
        {
            float input = Input.GetAxisRaw(mouseHorizontalInput);

            //타임 스캐일로 움직임 조정
            if (Time.timeScale > 0f) //타임 스케일이 0 이상일때만
            {
                if (Time.deltaTime > 0)
                    input /= Time.deltaTime; //input / 1프레임당 실행속도(1/144)

                input *= Time.timeScale;
            }
            else
            {
                input = 0;
            }

            //마우스 감도 곱하기
            input *= sensitivity; 

            return input;
        }
        /// <summary>
        /// 수직 마우스 움직임 값 반환
        /// </summary>
        /// <returns></returns>
        public float GetVertical()
        {
            float input = -Input.GetAxisRaw(mouseVerticalInput);

            //타임 스캐일로 움직임 조정
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

            //마우스 감도 곱하기
            input *= sensitivity;

            return input;
        }

        #endregion
    }
}
