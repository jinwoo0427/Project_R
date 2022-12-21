using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class cutscene : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;
    public Animator enemyController;
    public GameObject bloodEffect;
    public AudioSource swordClip;
    public AudioSource bloodClip;

    public GameObject GamePlayer;

    public Camera cutSceneCamera;
    public GameObject EndPanel;

    bool isEnd;
    void Update()
    {
        if(isEnd)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene("StartScene");
            }
        }

    }
    //playerController -> cameraroot -> cameracontrols

    void OnEnable()
    {
        //StartCoroutine(cor());
    }

    public void CutSceneStart()
    {
        StartCoroutine(cor());
    }

    IEnumerator cor()
    {
        Destroy(GamePlayer);
        cutSceneCamera.gameObject.SetActive(true);
        Timer.Instance.isEnd = true;

        cutSceneCamera.GetUniversalAdditionalCameraData().renderType 
            = CameraRenderType.Base;

        yield return new WaitForSeconds(0.03f);
        Player.SetActive(true);
        Enemy.SetActive(true);
        swordClip.PlayOneShot(swordClip.clip);

        yield return new WaitForSeconds(0.62f);
        bloodClip.PlayOneShot(bloodClip.clip);
        bloodEffect.SetActive(true);
        enemyController.SetTrigger("death");
        yield return new WaitForSeconds(0.9f);
        EndPanel.SetActive(true);
        isEnd = true;
    }


}
