using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESC_Quit : MonoBehaviour
{
    public QuitGame quit;
    void Update()
    {
        if(Input.GetKeyDown("escape")){
            quit.Quit();
        }
    }
}
