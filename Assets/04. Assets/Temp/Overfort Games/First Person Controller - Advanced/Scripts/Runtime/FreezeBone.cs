using UnityEngine;

namespace Jinwoo.FirstPersonController.Test
{
    public class FreezeBone : MonoBehaviour
    {
        private void LateUpdate()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}