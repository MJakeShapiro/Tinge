using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
	[SerializeField] MenuButtonController menuButtonController;
	[SerializeField] Animator animator;
	[SerializeField] AnimatorFunctions animatorFunctions;
	[SerializeField] int thisIndex;

	public MenuStart start;
	public QuitGame quit;


    // Update is called once per frame
    void Update()
    {
		if(menuButtonController.index == thisIndex)
		{
			animator.SetBool ("selected", true);
			if(Input.GetAxis ("Submit") == 1){
				animator.SetBool ("pressed", true);
			}else if (animator.GetBool ("pressed") && thisIndex == 0){
				SoundManager.PlaySound(SoundManager.Sound.MenuPress, 0.75f);
				animator.SetBool ("pressed", false);
				animatorFunctions.disableOnce = true;
				
				start.LoadScene("HubLevel");

			}else if (animator.GetBool ("pressed") && thisIndex == 2){
				SoundManager.PlaySound(SoundManager.Sound.MenuPress, 0.75f);
				animator.SetBool ("pressed", false);
				animatorFunctions.disableOnce = true;

				quit.Quit();

			}else if (animator.GetBool ("pressed")){
				SoundManager.PlaySound(SoundManager.Sound.MenuPress, 0.75f);
				animator.SetBool ("pressed", false);
				animatorFunctions.disableOnce = true;
			}
		}else{
			animator.SetBool ("selected", false);
		}
    }
}
