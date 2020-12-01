using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public SceneChanger sceneChanger;

    public bool changingScenes = false;

    public LayerMask ground;
    public LayerMask smashable;

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
        instance.sceneChanger.FadeToScene(sceneToLoad);
    }

    /// <summary>
    /// Reloads current active scene
    /// </summary>
    public void ReloadScene()
    {
        string sceneToLoad = SceneManager.GetActiveScene().name;
        instance.sceneChanger.FadeToScene(sceneToLoad);
    }
}