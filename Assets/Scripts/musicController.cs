using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip backgroundMusic; // 배경 음악 클립
    private AudioSource audioSource;

    private void Awake()
    {
        // 다른 MusicController 인스턴스가 존재하면 파괴
        if (FindObjectsOfType<MusicController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // 이 인스턴스를 파괴하지 않고 유지
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 배경 음악 설정 및 재생
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
}
