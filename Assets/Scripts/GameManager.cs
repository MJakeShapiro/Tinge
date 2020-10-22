using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public SceneChanger sceneChanger;

    bool changingScenes = false;

    //setup of singleton entity
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance ?? (instance = new GameObject("GameManager").AddComponent<GameManager>()); }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this);
    }

    /// <summary>
    /// Loads scene with fade animation
    /// </summary>
    /// <param name="sceneToLoad">
    /// The Scene to be loaded
    /// </param>
    public void ChangeScene(string sceneToLoad)
    {
        changingScenes = true;
        instance.sceneChanger.FadeToScene(sceneToLoad);
    }

    public bool ChangingScenes { get { return changingScenes; } set { changingScenes = value; } }
}