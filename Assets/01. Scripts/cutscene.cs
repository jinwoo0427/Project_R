using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class cutscene : MonoBehaviour
{
    public GameObject Player;
    public GameObject Enemy;
    public Animator enemyController;
    public GameObject bloodEffect;
    public AudioSource swordClip;
    public AudioSource bloodClip;

    public Camera cutSceneCamera;
    //playerController -> cameraroot -> cameracontrols

    void OnEnable()
    {
        StartCoroutine(cor());
    }

    void CutSceneStart()
    {
        StartCoroutine(cor());
    }

    IEnumerator cor()
    {
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
    }


}
