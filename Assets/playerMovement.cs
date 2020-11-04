using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{

    Rigidbody2D rigidBody;
    BoxCollider2D boxCollider;
    CapsuleCollider2D capsuleCollider;

    float gravityScaleStart;

    public float runSpeed = 10f;
    public float slideDistance = 15f;
    public float jumpForce = 20f;
    public float climbSpeed = 10f;

    float dirX;
    bool isSliding = false;

    

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        gravityScaleStart = rigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");

        Jump();
        Climb();
        Tornado();
        Slide();


        



    }

    private void FixedUpdate()
    {
        run();
    }

    void run()
    {
        if(dirX != 0)
        {
            if (!isSliding)
            {
                rigidBody.velocity = new Vector2(dirX * runSpeed, rigidBody.velocity.y);
            }
        }
    }

    void Slide()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(Dash(-1f));
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(Dash(1f));
        }
    }

    void Tornado()
    {
        if (capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Tornado")))
        {
            if (dirX > 0)
                StartCoroutine(Dash(-1f));
            else if (dirX < 0)
            {
                StartCoroutine(Dash(1f));
            }
        }
    }

    void Jump()
    {

        if (!boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
        Vector2 movement = new Vector2(rigidBody.velocity.x, jumpForce);
        rigidBody.velocity = movement;
        }
    }

    private void Climb()
    {
        if (!rigidBody.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            rigidBody.gravityScale = gravityScaleStart;
            return;
        }

        float controlTrhow = Input.GetAxisRaw("Vertical");
        Vector2 climbVelocity = new Vector2(rigidBody.velocity.x, controlTrhow * climbSpeed);
        rigidBody.velocity = climbVelocity;
        rigidBody.gravityScale = 0f;

    }

    IEnumerator Dash(float direction)
    {
        isSliding = true;
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);
        rigidBody.AddForce(new Vector2(slideDistance * direction, 0f), ForceMode2D.Impulse);
        float gravity = rigidBody.gravityScale;
        rigidBody.gravityScale = 0;
        yield return new WaitForSeconds(0.4f);
        isSliding = false;
        rigidBody.gravityScale = gravity;
    }
}
