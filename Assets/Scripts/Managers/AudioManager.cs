using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AudioManager : CustomMonoBehaviour
{
    public static AudioManager Instance => instance;

    private static AudioManager instance;
    private AudioSource source;

    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    
        source = GetComponent<AudioSource>();
    }

    public void PlayOneShot(AudioClip clip, float volume = 0.75f)
    {
        source.volume = volume;

        if (source.isPlaying)
        {
            source.volume = volume/2;
        }

        source.PlayOneShot(clip);

    }
}
