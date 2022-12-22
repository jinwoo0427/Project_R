using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject gam;
    void Start()
    {
        Invoke("OnBuilding", 1f);
    }
    public void OnBuilding()
    {
        gam.SetActive(true);
    }
    
}
