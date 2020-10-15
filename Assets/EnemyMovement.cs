using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    Rigidbody2D myRigidBody;
    BoxCollider2D myBox;
    CircleCollider2D myCircle;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myBox = GetComponent<BoxCollider2D>();
        myCircle = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isFacingRight())
        {
            whereToFace();
            myRigidBody.velocity = new Vector2(moveSpeed, 0f);
        }
        else
        {
            whereToFace();
            myRigidBody.velocity = new Vector2(-moveSpeed, 0f);
        }
    }

    bool isFacingRight()
    {
        return transform.localScale.x > 0;
    }

    void whereToFace()
    {
        if (myCircle.IsTouchingLayers() || !myBox.IsTouchingLayers())
        {
            transform.localScale = new Vector2(-(Mathf.Sign(myRigidBody.velocity.x)), 1f);
        }
    }
        
}
