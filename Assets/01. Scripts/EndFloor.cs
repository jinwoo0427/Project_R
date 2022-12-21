using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndFloor : MonoBehaviour
{
    public cutscene cutscene;
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("asdf");

        if(other.CompareTag("Player"))
        {
            Debug.Log("asdf");
            cutscene.CutSceneStart();
        }
    }



}
