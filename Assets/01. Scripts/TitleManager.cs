using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TitleManager : MonoBehaviour
{
    public float pTime = 5f;
    public Image titmeImage;
    public Material titleChangeMat;

    private void Awake()
    {
        titmeImage.gameObject.SetActive(false);
    }
    void Update()
    {
        pTime -= Time.deltaTime;

        if(pTime <= 0)
        {
            SceneManager.LoadScene("StartScene");
        }
    }
}
