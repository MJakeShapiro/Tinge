using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Smashable : MonoBehaviour
{
    [SerializeField] GameObject fracturedVersion;

    // Update is called once per frame
    void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            
        }
    }

    public void SmashObject()
    {

        Instantiate(fracturedVersion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
