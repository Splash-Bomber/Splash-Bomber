using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioClip backgroundMusic; // ��� ���� Ŭ��
    private AudioSource audioSource;

    private void Awake()
    {
        // �ٸ� MusicController �ν��Ͻ��� �����ϸ� �ı�
        if (FindObjectsOfType<MusicController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        // �� �ν��Ͻ��� �ı����� �ʰ� ����
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ��� ���� ���� �� ���
        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
}
