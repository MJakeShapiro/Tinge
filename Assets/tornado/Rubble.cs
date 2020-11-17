using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubble : MonoBehaviour
{
    private Vector2 MoveDiriection;
    private float moveSpeed;
    private CircleCollider2D circleCollider;

    private void OnEnable()
    {
        StartCoroutine(trig());
        Invoke("Destroy", 1.5f);
    }

    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
        moveSpeed = 5f;
        circleCollider.isTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(MoveDiriection * moveSpeed * Time.deltaTime);
    }

    IEnumerator trig()
    {
        yield return new WaitForSeconds(1f);
        circleCollider.isTrigger = true;
    }

    public void SetMoveDirection(Vector2 dir)
    {
        MoveDiriection = dir;
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
        circleCollider.isTrigger = false;
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}