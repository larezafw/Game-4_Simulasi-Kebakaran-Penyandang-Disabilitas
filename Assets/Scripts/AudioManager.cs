using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public AudioSource menuAudioSource;
    public AudioSource fireAudioSource;
    public AudioSource buttonAudioSource;
    public AudioSource loseAudioSource;
    public AudioSource hydranAudiorSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != null && Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PlayMenuSound()
    {
        menuAudioSource.Play();
        fireAudioSource.Stop();
    }

    public void PlayIngameSound()
    {
        menuAudioSource.Stop();
        fireAudioSource.Play();
    }

    public void PlayButtonSound()
    {
        buttonAudioSource.Play();
    }

    public void PlayEndingSound()
    {
        fireAudioSource.Stop();
    }

    public void PlayLoseSound()
    {
        loseAudioSource.Play();
    }

    public void PlayHydranSound(bool isUsing)
    {
        if (isUsing)
        {
            if (!hydranAudiorSource.isPlaying)
            {
                hydranAudiorSource.Play();
                FireSoundAdjustment(false);
            }
        }
        else
        {
            if (hydranAudiorSource.isPlaying)
            {
                hydranAudiorSource.Stop();
                FireSoundAdjustment(true);
            }
        }
        
    }

    public void FireSoundAdjustment(bool isMaxVolume)
    {
        float maxVolume = 0.05f;
        float minVolume = 0.0175f;

        if (isMaxVolume) fireAudioSource.volume = maxVolume;
        else fireAudioSource.volume = minVolume;
    }
}
