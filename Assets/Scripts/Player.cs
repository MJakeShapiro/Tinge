 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    
    [Header("Movement")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;

    [Header("Dash")]
    [SerializeField] GameObject dashEffect;
    [SerializeField] float dashSpeed = 50.0f;
    [SerializeField] float TOTAL_DASH_TIME = 0.1f;
    [SerializeField] float MIN_DASH_COOLDOWN = 0.5f;

    [Header("PlayTesting")]
    [SerializeField] bool diagonalDash;
    [SerializeField] bool variableDashLength;

    Rigidbody2D myRigidBody;
    BoxCollider2D boxCollider;
    float gravityScaleStart;
    Direction direction = Direction.right;

    // Private Dash Variables
    bool canDash = true;
    bool hasAirDashed = false;
    bool isDashing = false;
    float dashTime;
    float dashCooldown = 0.0f;



    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        gravityScaleStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        DirectionSet();
        Dash();
        DashCounter();
        if (isDashing)  // Briefly disable player controls if isDashing
            return;
        Run();
        Climb();
        Jump();
    }

    private void DirectionSet()
    {
        if (CrossPlatformInputManager.GetAxis("Horizontal") > 0.0f)
            direction = Direction.right;
        if (CrossPlatformInputManager.GetAxis("Horizontal") < 0.0f)
            direction = Direction.left;
        if (CrossPlatformInputManager.GetAxis("Vertical") > 0.0f)
            direction = Direction.up;
        if (CrossPlatformInputManager.GetAxis("Vertical") < 0.0f)
            direction = Direction.down;
    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(runSpeed * controlThrow, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
    }

    private void Climb()
    {
        if (!myRigidBody.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            myRigidBody.gravityScale = gravityScaleStart;
            return;
        }

        

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;
        
    }

    private void Jump()
    {
        if (!boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocityToAdd;
        }
    }

    /// <summary>
    /// Dashes player according to dashSpeed. DiagonalDash option.
    /// </summary>
    public void Dash()
    {
        if (CrossPlatformInputManager.GetButtonDown("Dash"))
        {
            if (canDash && !isDashing)
            {
                isDashing = true;
                dashTime = TOTAL_DASH_TIME;
                myRigidBody.gravityScale = 0.0f;
                GameObject DashEffectToDestroy = Instantiate(dashEffect, transform.position, Quaternion.identity);
                Destroy(DashEffectToDestroy, 0.2f);
                //animator.SetBool("IsDashing", true);
                //AudioManager.instance.PlaySound("dash");

                if (!boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
                    hasAirDashed = true;

                if (diagonalDash)
                    myRigidBody.velocity = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal") * dashSpeed, CrossPlatformInputManager.GetAxis("Vertical") * dashSpeed);
                else
                {
                    switch (direction)
                    {
                        case Direction.right:
                            myRigidBody.velocity = Vector2.right * dashSpeed;
                            break;
                        case Direction.left:
                            myRigidBody.velocity = Vector2.left * dashSpeed;
                            break;
                        //case Direction.up:
                        //    myRigidBody.velocity = Vector2.up * dashSpeed;
                        //    break;
                        //case Direction.down:
                        //    myRigidBody.velocity = Vector2.down * dashSpeed;
                        //    break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Manages dash distance according to TOTAL_DASH_TIME
    /// </summary>
    private void DashCounter()
    {
        if (isDashing)
        {

            if (dashTime <= 0.0f || (variableDashLength && CrossPlatformInputManager.GetButtonUp("Dash")))
            {
                myRigidBody.velocity = Vector2.zero;
                myRigidBody.gravityScale = 4.0f;
                isDashing = false;
                canDash = false;
                dashCooldown = MIN_DASH_COOLDOWN;
            }
            else
            {
                dashTime -= Time.deltaTime;
            }
        }
        if (!canDash)
            dashCooldown -= Time.deltaTime;
        if (dashCooldown <= 0.0f)
        {
            if (!boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || !hasAirDashed)
            {
                canDash = true;
                hasAirDashed = false;
            }

        }
    }
}
