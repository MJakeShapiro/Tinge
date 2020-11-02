using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class SoundManager 
{

    public enum Sound
    {
        SwapButton,
        MenuPress,
        DashFX,
    }

    public static void PlaySound(Sound sound, float pLevel)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.pitch = pLevel;
        audioSource.PlayOneShot(GetAudioClip(sound));
        UnityEngine.Object.Destroy(soundGameObject, 0.8f);
    }

    public static void PlaySound(Sound sound)
    {
        GameObject soundGameObject = new GameObject("Sound");
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.PlayOneShot(GetAudioClip(sound));
    }

    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameAssets.SoundAudioClip soundAudioClip in GameAssets.i.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }
        UnityEngine.Debug.LogError("Sound " + sound + " not found.");
        return null;
    }
}
