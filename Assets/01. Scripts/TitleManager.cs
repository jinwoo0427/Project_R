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

    public AudioSource source;
    public AudioClip clip;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = clip;
        titmeImage.gameObject.SetActive(false);
    }
    private void Start()
    {
        Invoke("SoundStart", 2f);
    }
    void Update()
    {
        pTime -= Time.deltaTime;
        if(pTime <= 0)
        {
            SceneManager.LoadScene("StartScene");
        }
    }

    public void SoundStart()
    {
        source.PlayOneShot(clip, 0.5f);
    }
}
