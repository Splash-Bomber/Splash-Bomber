using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public void PlayAudio()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySound();
        }
    }
}