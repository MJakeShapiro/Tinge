using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// - Fade out method
// - OR find a way to include LoadLevel to LoadAsynchronously

public class SceneChanger : MonoBehaviour
{
    public Animator animator;
    public GameObject loadingScreen;
    public Slider slider;

    private string sceneToLoad;

    public void FadeToScene(string sceneName)
    {
        sceneToLoad = sceneName;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToLoad);
        animator.SetTrigger("FadeIn");
        GameManager.Instance.ChangingScenes = false;
    }

    /// <summary>
    /// Loads a scene with a loading bar
    /// </summary>
    /// <param name="sceneName">
    /// The Scene to be loaded
    /// </param>
    public void LoadingScene(string sceneName)
    {
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);

            slider.value = progress;

            yield return null;
        }
    }
}