using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Material CheckTrueMat;
    public Material CheckFalseMat;
    public MeshRenderer meshRenderer;
    public bool isCheck;

    
    void OnCollisionEnter(Collision collision)
    {
        if(isCheck == false && collision.gameObject.CompareTag("Player"))
        {
            //체크포인트 위치 이 오브젝트의 포지션으로 변경
            RespawnManager.Instance.respawnTrm = this.transform;
            isCheck = true;
            meshRenderer.material = CheckTrueMat;
        }
    }
}
