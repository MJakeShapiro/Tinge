using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard_Delayed : MonoBehaviour
{
    public float waitTime;

    private IEnumerator OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
	        yield return new WaitForSeconds(waitTime);
            collision.GetComponent<Player>().Die();
        }
    }
}
