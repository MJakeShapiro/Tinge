using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TransitionPoint : MonoBehaviour
{

    [SerializeField] SceneAsset nextScene;
    bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isTriggered)
            return;
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            isTriggered = true;
            GameManager.Instance.ChangeScene(nextScene.name);
        }
    }
}
