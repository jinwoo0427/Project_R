using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Debugger : MonoBehaviour
{
    public GameObject Player;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ResetPosition();
        }
    }

    private void ResetPosition()
    {
        SceneManager.LoadScene("TestScene");
    }
}
