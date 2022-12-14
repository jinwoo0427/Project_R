using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private static RespawnManager instance = null;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }

    public static RespawnManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    public Transform respawnTrm;
    public GameObject Player;

    void Update()
    {
        Debug.Log("업데이트");
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }

    }

    public void Respawn()
    {
        Debug.Log("리스폰");
        Debug.Log(respawnTrm.position);
        Vector3 vec = new Vector3(respawnTrm.position.x, respawnTrm.position.y, respawnTrm.position.z);
        Player.transform.position = vec;
    }
}
