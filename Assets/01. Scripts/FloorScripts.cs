using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jinwoo.FirstPersonController;

public class FloorScripts : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("플레이어 부딪침");
            RespawnManager.Instance.Respawn();
        }
    }

    
}
