using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{

    private static Timer instance = null;

    public static Timer Instance
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
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

       
    }
    public float timer;
    public TextMeshProUGUI timerText;
    public bool isEnd;
    void Update()
    {
        if(timerText == null)
        {
            //GameObject playerParent = GameObject.Find("PlayerController");
            //Transform canvasParent = playerParent.transform.GetChild(2);
            //timerText = canvasParent.GetChild(3).GetComponent<TextMeshProUGUI>();
            timerText = GameObject.Find("TimerText").GetComponent<TextMeshProUGUI>();   
        }
        if(!isEnd)
        {
            timer += Time.deltaTime;
            timer = Mathf.Round(timer * 100) / 100;
            timerText.text = timer.ToString();
        }

        //if(Input.GetKeyDown(KeyCode.R))
        //{
        //    SceneManager.LoadScene("TestScene");
        //}
    }

    public void MainScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
