using UnityEngine;

public class ObjectAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    public bool testPlay = false;

    public void PlaySound()
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayLoop()
    {
        audioSource.clip = audioClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopSound()
    {
        audioSource.Stop();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (testPlay)
        {
            testPlay = false;
            PlaySound();
        }
    }


}
