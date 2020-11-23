﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{

    #region Properties
    [Header("Movement")]
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;

    [Header("Dash")]
    [SerializeField] GameObject dashEffect;
    [SerializeField] float dashSpeed = 50.0f;
    [SerializeField] float TOTAL_DASH_TIME = 0.1f;
    [SerializeField] float MIN_DASH_COOLDOWN = 0.5f;

    [Header("Smash")]
    [SerializeField] float smashSpeed = 20.0f;
    [SerializeField] float TOTAL_SMASH_TIME = 0.05f;
    [SerializeField] float MIN_SMASH_COOLDOWN = 0.5f;
    [SerializeField] Transform leftSmashPos;
    [SerializeField] Transform rightSmashPos;
    [SerializeField] float checkRadius;

    [Header("PlayTesting")]
    [SerializeField] bool diagonalDash;
    [SerializeField] bool variableDashLength;

    Rigidbody2D myRigidBody;
    BoxCollider2D boxCollider;
    float gravityScaleStart;
    Direction direction;
    SmashDirection smashDirection;
    //public Animator animator;
    GameObject p;
    SpriteRenderer player;

    // Private Dash Variables
    bool canDash = true;
    bool hasAirDashed = false;
    bool isDashing = false;
    float dashTime;
    float dashCooldown = 0.0f;

    // Private Smash Variables
    bool canSmash = true;
    bool isSmashing = false;
    bool downSmash;
    float smashTime;
    float smashCooldown = 0.0f;

    #endregion Properties

    #region Initialization
    //Initialize soundTimerDictionary
    private void Awake()
    {
        SoundManager.Initialize();
        player = GetComponent<SpriteRenderer>();
        
    }

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>(); 
        gravityScaleStart = myRigidBody.gravityScale;
        //TOTAL_SMASH_TIME = 0.1f;
    }

    #endregion Initialization

    #region Update Functions
    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.changingScenes)
            return;
        DirectionSet();
        Dash();
        Smash();
        DashCounter();
        SmashCounter();
        if (isDashing || isSmashing)  // Briefly disable player controls if isDashing
            return;
        Run();
        Climb();
        Jump();
    }

    #endregion Update Funcitons

    #region Movement
    private void DirectionSet()
    {
        if (CrossPlatformInputManager.GetAxis("Horizontal") > 0.0f)
        {
            direction = Direction.right;
            player.flipX = false;

            //Play running FX only when player is touching the ground
            if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                //Play Run Sound FX
                SoundManager.PlaySound(SoundManager.Sound.RunFX, Random.Range(0.75f, 1f));
            }
        }
        if (CrossPlatformInputManager.GetAxis("Horizontal") < 0.0f)
        {
            direction = Direction.left;
            player.flipX = true;

            if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                //Play Run Sound FX
                SoundManager.PlaySound(SoundManager.Sound.RunFX, Random.Range(0.75f, 1f));
            }
            
        }
        //if (CrossPlatformInputManager.GetAxis("Vertical") > 0.0f)
        //    direction = Direction.up;
        if (CrossPlatformInputManager.GetAxis("Vertical") < 0.0f)
            direction = Direction.down;
    }



    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(runSpeed * controlThrow, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        //animator.SetFloat("Speed", Mathf.Abs(controlThrow));
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
            // animator.SetTrigger("Jump");

            
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocityToAdd;

            //Play Jump Sound FX
            SoundManager.PlaySound(SoundManager.Sound.JumpFX, Random.Range(0.85f, 1.0f));
            StartCoroutine(jumpTimer(.80f));

        }
        
    }
    #endregion Movement

    #region Abilities

    #region Dash
    /// <summary>
    /// Dashes player according to dashSpeed. DiagonalDash option.
    /// </summary>
    public void Dash()
    {
        if (CrossPlatformInputManager.GetButtonDown("Dash"))
        {
            //animator.SetTrigger("Dash");
            StartCoroutine(dashTimer(.4f));
           
            if (canDash && !isDashing)
            {
                GameObject DashEffectToDestroy;

                if (!boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
                    hasAirDashed = true;

                if (diagonalDash)
                {
                    isDashing = true;
                    dashTime = TOTAL_DASH_TIME;
                    myRigidBody.gravityScale = 0.0f;
                    myRigidBody.velocity = new Vector2(CrossPlatformInputManager.GetAxis("Horizontal") * dashSpeed, CrossPlatformInputManager.GetAxis("Vertical") * dashSpeed);

                    //Play Dash Sound FX
                    SoundManager.PlaySound(SoundManager.Sound.DashFX, Random.Range(0.7f, 1.2f));

                    DashEffectToDestroy = Instantiate(dashEffect, transform.position, Quaternion.identity);
                    Destroy(DashEffectToDestroy, 0.2f);
                }
                else
                {
                    if (direction == Direction.right)
                    {
                        isDashing = true;
                        dashTime = TOTAL_DASH_TIME;
                        myRigidBody.gravityScale = 0.0f;
                        myRigidBody.velocity = Vector2.right * dashSpeed;

                        //Play Dash Sound FX
                        SoundManager.PlaySound(SoundManager.Sound.DashFX, Random.Range(0.7f, 1.2f));

                        DashEffectToDestroy = Instantiate(dashEffect, transform.position, Quaternion.identity);
                        Destroy(DashEffectToDestroy, 0.2f);
                    }
                    else if (direction == Direction.left)
                    {
                        isDashing = true;
                        dashTime = TOTAL_DASH_TIME;
                        myRigidBody.gravityScale = 0.0f;
                        myRigidBody.velocity = Vector2.left * dashSpeed;

                        //Play Dash Sound FX
                        SoundManager.PlaySound(SoundManager.Sound.DashFX, Random.Range(0.7f, 1.2f));

                        DashEffectToDestroy = Instantiate(dashEffect, transform.position, Quaternion.identity);
                        Destroy(DashEffectToDestroy, 0.2f);
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
                myRigidBody.gravityScale = 1.0f;
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
            if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || !hasAirDashed)
            {
                canDash = true;
                hasAirDashed = false;
            }

        }
    }

    #endregion Dash

    #region Smash
    public void Smash()
    {
        if (CrossPlatformInputManager.GetButtonDown("Smash"))
        {
            //animator.SetTrigger("Smash");
            //StartCoroutine(smashTimer(.4f));

            if (canSmash && !isSmashing)
            {
                Debug.Log("SMASH");
                Debug.Log(TOTAL_SMASH_TIME);
                if (direction == Direction.right)
                {
                    isSmashing = true;
                    smashTime = TOTAL_SMASH_TIME;
                    myRigidBody.gravityScale = 0.0f;
                    myRigidBody.velocity = Vector2.right * smashSpeed;
                    smashDirection = SmashDirection.right;
                }
                else if (direction == Direction.left)
                {
                    isSmashing = true;
                    smashTime = TOTAL_SMASH_TIME;
                    myRigidBody.gravityScale = 0.0f;
                    myRigidBody.velocity = Vector2.left * smashSpeed;
                    smashDirection = SmashDirection.left;
                }
                if (direction == Direction.down)
                {
                    isSmashing = true;
                    myRigidBody.gravityScale = 0.0f;
                    myRigidBody.velocity = Vector2.down * smashSpeed;
                    smashDirection = SmashDirection.down;
                }
            }
        }
    }

    public void SmashCounter()
    {
        if (isSmashing)
        {
            // If player collides with the ground while smashing down
            if ((smashDirection == SmashDirection.down) && boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                // Play Collision animation here
                myRigidBody.velocity = Vector2.zero;
                myRigidBody.gravityScale = 1.0f;
                isSmashing = false;
                canSmash = false;
                smashCooldown = MIN_SMASH_COOLDOWN;
            }
            // Made seperate in case player is smashing to the side while on the ground
            // If player collides with a wall while smashing left or right
            else
            {
                if(smashDirection == SmashDirection.left && Physics2D.OverlapCircle(leftSmashPos.position, checkRadius, LayerMask.NameToLayer("Ground")))
                {
                    myRigidBody.velocity = Vector2.zero;
                    myRigidBody.gravityScale = 1.0f;
                    isSmashing = false;
                    canSmash = false;
                    smashCooldown = MIN_SMASH_COOLDOWN;
                }
                // Made seperate in case player smashes with their back to a wall
                else if(smashDirection == SmashDirection.right && Physics2D.OverlapCircle(rightSmashPos.position, checkRadius, LayerMask.NameToLayer("Ground")))
                {
                    myRigidBody.velocity = Vector2.zero;
                    myRigidBody.gravityScale = 1.0f;
                    isSmashing = false;
                    canSmash = false;
                    smashCooldown = MIN_SMASH_COOLDOWN;
                }
            }

            // If player collides with a smashable while smashing down
            if ((smashDirection == SmashDirection.down) && boxCollider.IsTouchingLayers(LayerMask.GetMask("Smashable")))
            {
                    myRigidBody.velocity = Vector2.zero;
                    myRigidBody.gravityScale = 1.0f;
                    isSmashing = false;
                    canSmash = false;
                    smashCooldown = MIN_SMASH_COOLDOWN;
                    downSmash = false;
            }
            // If player collides with a smashable while smashing left or right
            else
            {
                // They continue smashing for another 0.1 seconds in order to "smash through"
                if (smashDirection == SmashDirection.left && Physics2D.OverlapCircle(leftSmashPos.position, checkRadius, LayerMask.NameToLayer("Smashable")))
                {
                    smashTime = 0.1f;
                }
                else if(smashDirection == SmashDirection.right && Physics2D.OverlapCircle(rightSmashPos.position, checkRadius, LayerMask.NameToLayer("Smashable")))
                {
                    smashTime = 0.1f;
                }   
            }

            // If player smashes to the side but collides with nothing before smash ends
            if (smashTime <= 0.0f && (smashDirection != SmashDirection.down))
            {
                myRigidBody.velocity = Vector2.zero;
                myRigidBody.gravityScale = 1.0f;
                isSmashing = false;
                canSmash = false;
                smashCooldown = MIN_SMASH_COOLDOWN;
            }
            else
            {
                smashTime -= Time.deltaTime;
            }
        }
        if (!canSmash)
            smashCooldown -= Time.deltaTime;
        if (smashCooldown <= 0.0f)
        {
            if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                canSmash = true;
                
            }
        }
    }

    #endregion Smash

    #endregion Abilities

    #region Animation
    IEnumerator jumpTimer(float time)
    {
        //animator.SetBool("isJumping", true);
        yield return new WaitForSeconds(time);
        //animator.SetBool("isJumping", false);
    }
    IEnumerator dashTimer(float time)
    {
        //animator.SetBool("isDashing", true);
        yield return new WaitForSeconds(time);
        //animator.SetBool("isDashing", false);
    }

    #endregion Animation

    #region Death
    /// <summary>
    /// Called when player dies. Resets scene
    /// </summary>
    public void Die()
    {
        myRigidBody.velocity = Vector2.zero;
        myRigidBody.gravityScale = 0.0f;
        GameManager.Instance.ReloadScene();
    }
}
    #endregion Death
