using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragmentDestroy : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine(RemoveFragment(2));
    }

    IEnumerator RemoveFragment(float time)
    {
        yield return new WaitForSeconds(time);
            Destroy(gameObject);
    }
}
