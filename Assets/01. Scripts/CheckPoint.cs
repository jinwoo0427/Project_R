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
            //üũ����Ʈ ��ġ �� ������Ʈ�� ���������� ����
            RespawnManager.Instance.respawnTrm = this.transform;
            isCheck = true;
            meshRenderer.material = CheckTrueMat;
        }
    }
}
