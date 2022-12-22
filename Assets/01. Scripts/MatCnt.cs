using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatCnt : MonoBehaviour
{
    public Material mat;
    public Image img;
    public Shader sha;
    public float a = 0;
    public float timer = 0f;
    void Start()
    {
        img = GetComponent<Image>();
        mat = img.material;
        //mat = GetComponent<Material>();
        sha = mat.shader;
        
        a = 0;
        mat.SetFloat("_Animation_Factor", 0f);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if(timer <= 4f && a < 1.5f)
        {
            a += 0.01f;
            mat.SetFloat("_Animation_Factor", a);
        }
    }
}
