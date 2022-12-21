using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public Material CheckTrueMat;
    public Material CheckFalseMat;
    public MeshRenderer meshRenderer;
    public Collider myCol;
    public bool isCheck;
    private void Start()
    {
        myCol = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCheck == false && other.CompareTag("Player"))
        {
            Debug.Log("플레이어 부딪침");
            //체크포인트 위치 이 오브젝트의 포지션으로 변경
            RespawnManager.Instance.respawnTrm = this.transform;
            isCheck = true;
            meshRenderer.material = CheckTrueMat;
            myCol.enabled = false;
        }
    }
}
