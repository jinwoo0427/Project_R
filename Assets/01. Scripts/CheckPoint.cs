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
            Debug.Log("�÷��̾� �ε�ħ");
            //üũ����Ʈ ��ġ �� ������Ʈ�� ���������� ����
            RespawnManager.Instance.respawnTrm = this.transform;
            isCheck = true;
            meshRenderer.material = CheckTrueMat;
            myCol.enabled = false;
        }
    }
}
