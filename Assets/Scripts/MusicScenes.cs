using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicScenes : MonoBehaviour
{
    private AudioSource[] audioSources;
    private void Start()
    {

    }

    void Awake()
    {
        //Get Scene Name
        Scene m_Scene;
        string sceneName;
        m_Scene = SceneManager.GetActiveScene();
        sceneName = m_Scene.name;

        //Get AudioSource from all game object with the Tag 'Music'
        GameObject[] A = GameObject.FindGameObjectsWithTag("music");
        audioSources = new AudioSource[A.Length];
        for (int i = 0; i < audioSources.Length; i++)
        {
            audioSources[i] = A[i].GetComponent<AudioSource>();
        }

        //Play Background Music Based on the level
        if (sceneName.Contains("HubLevel"))
        {
            if (audioSources[0].clip.name != "Upbeat City LOOP")
            {
                AudioClip clip = Resources.Load("Upbeat City LOOP") as AudioClip;
                audioSources[0].clip = clip;
                audioSources[0].Play();
            }
        } else if (sceneName.Contains("Desert"))
        {
            if (audioSources[0].clip.name != "Western OUTSIDE LOOP")
            {
                AudioClip clip = Resources.Load("Western OUTSIDE LOOP") as AudioClip;
                audioSources[0].clip = clip;
                audioSources[0].Play();
            }
        } else if (sceneName.Contains("City"))
        {
            if (audioSources[0].clip.name != "Space Station SLOW LOOP")
            {
                AudioClip clip = Resources.Load("Space Station SLOW LOOP") as AudioClip;
                audioSources[0].clip = clip;
                audioSources[0].Play();
            }
        } else
        {
            audioSources[0].clip = Resources.Load("") as AudioClip;
            audioSources[0].Play();
        }

        //Seamless Music Between Scenes
        if (A.Length > 1)
            Destroy(this.gameObject);
        DontDestroyOnLoad(this.gameObject);
    }
}
