using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using SCPE;
public class VolumManager : MonoBehaviour
{

    private static VolumManager instance = null;

    public BlackBars blackBars;
    public DoubleVision vision;

    [SerializeField, Range(0f, 1f)] public float _intensity = default;
    [SerializeField, Range(0f, 1f)] public float visionIntensity = default;

    public static VolumManager Instance
    {
        get
        {
            var obj = FindObjectOfType<VolumManager>();
            if (instance != null)
            {
                instance = obj;
            }
            //else
            //{
            //    instance = Create();
            //}
            return instance;
        }
    }


    [SerializeField] Volume volume;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }

    }
    private void Start()
    {
        volume.profile.TryGet<BlackBars>(out blackBars);
        volume.profile.TryGet<DoubleVision>(out vision);

        StartSceneValue();


        StartCoroutine(StartCutScene());

    }
    private void Update()
    {
        blackBars.maxSize.value = _intensity;
        vision.intensity.value = visionIntensity;
        
    }
    public void StartSceneValue()
    {
        _intensity = 1f;
    }
    IEnumerator StartCutScene()
    {
        yield return new WaitForSeconds(0.5f);

        while (true)
        {
            if (_intensity <= 0f)
                break;

            _intensity -= 0.02f;
            yield return new WaitForSeconds(0.02f);
        }
    }

    public void StartVisionF()
    {
        StartCoroutine(StartVision());
    }
    public void CutVisionF()
    {
        StartCoroutine(CutVision());
    }
    IEnumerator StartVision()
    {
        while (true)
        {
            if (visionIntensity >= 0.3f)
                break;

            visionIntensity += 0.02f;
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator CutVision()
    {
        while (true)
        {
            if (visionIntensity <= 0.1f)
                break;

            visionIntensity -= 0.02f;
            yield return new WaitForSeconds(0.02f);
        }
    }
}
