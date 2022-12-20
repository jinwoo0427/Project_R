using UnityEngine;

namespace Jinwoo.FirstPersonController
{
    //ID 및 정적 메서드를 사용하여 클립을 간편하게 재생하는 방법
    public class AudioLibrary : MonoBehaviour
    {
        #region Fields

        private const int MAX_POOL_SIZE = 15;

        //2D 사용을 위한 오디오 소스 풀
        private AudioSource[] sources2D;
        private Transform poolTransform;


        private static float[] volumes;

        private static AudioLibrary _instance;

        public static AudioLibrary Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("Audio Library").AddComponent<AudioLibrary>();
                    _instance.Instantiate2DSources();
                }

                return _instance;
            }
        }

        #endregion

        #region Methods

        private void Instantiate2DSources()
        {
            if (poolTransform == null)
            {
                poolTransform = new GameObject("Pool").transform;
                poolTransform.SetParent(_instance.transform);
            }

            sources2D = new AudioSource[MAX_POOL_SIZE];

            for (int i = 0; i < MAX_POOL_SIZE; i++)
            {
                var go = new GameObject("2D Audio Source " + i);
                go.transform.SetParent(poolTransform);
                sources2D[i] = go.AddComponent<AudioSource>();
            }
        }

        public static AudioSource Play2D(AudioClip clip, float volume = 1)
        {
            if (clip == null)
                return null;

            //아무 것도 재생하지 않는 것을 찾을 때까지 2D 소스를 순환함
            foreach (var source2D in Instance.sources2D)
            {
                if (source2D.isPlaying == false)
                {
                    source2D.PlayOneShot(clip, volume);
                    return source2D;
                }
            }

            //Debug.LogError("Not enough free 2D sources to play this clip");
            return null;
        }

        public static void MuteAll2D()
        {
            if (volumes == null)
                volumes = new float[Instance.sources2D.Length];

            for (int i = 0; i < Instance.sources2D.Length; i++)
            {
                var source2D = Instance.sources2D[i];

                volumes[i] = source2D.volume;
                source2D.volume = 0;
            }
        }

        public static void UnmuteAll2D()
        {
            if (volumes == null)
                return;

            for (int i = 0; i < Instance.sources2D.Length; i++)
            {
                var source2D = Instance.sources2D[i];

                source2D.volume = volumes[i];
            }
        }

        //월드 공간 위치 '포인트'에서 클립 재생
        public static AudioSource Play3D(AudioClip clip, Vector3 point)
        {

            //이 플레이만을 위한 임시 gameObject를 인스턴스화함
            GameObject tempGO = new GameObject("TempAudio");

            //Set it to the desired position
            tempGO.transform.position = point;

            //오디오를 재생하기위해 'AudioSource' 구성요소를 추가
            AudioSource aSource = tempGO.AddComponent<AudioSource>();

            //클립을 놓고 재생
            aSource.clip = clip;
            aSource.Play();

            //클립 지속 시간 후 gameObject를 파괴함 ---> 나중에 풀링 해야됨
            Destroy(tempGO, clip.length);

            return aSource;
        }

        #endregion
    }

}