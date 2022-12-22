using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowCase : MonoBehaviour
{

    public float speed;
    public int childc;
    public GameObject grandChild;

    void Start()
    {
        childc = transform.childCount;
        StartCoroutine(nextScene());
    }

    IEnumerator nextScene()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("StartScene");
    }

    void Update()
    {
        //Debug.Log(Mathf.Floor(Time.time));
        int index = (int)Mathf.Floor(Time.time * speed);
        //Debug.Log(this.gameObject.transform.GetChild(index).name);
        for (int i = 0; i < childc; i++)
        {
            if (i == index)
            {


                transform.GetChild(i).gameObject.SetActive(true);
            }
            else transform.GetChild(i).gameObject.SetActive(false);


        }
    }
}
