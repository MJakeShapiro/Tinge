 using System.Collections;
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
    [SerializeField] float slideDistance = 15f;
    float dirX;
    bool touchingTornado = false;
    bool isPushed = false;
    bool isSliding = false;

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
    [SerializeField] Transform downSmashPos;
    [SerializeField] float checkRadius;

    [Header("PlayTesting")]
    [SerializeField] bool diagonalDash;
    [SerializeField] bool variableDashLength;

    Rigidbody2D myRigidBody;
    BoxCollider2D boxCollider;
    CapsuleCollider2D capsuleCollider;
    float gravityScaleStart;
    Direction direction;
    SmashDirection smashDirection;
    public Animator animator;
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
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        gravityScaleStart = myRigidBody.gravityScale;
        //TOTAL_SMASH_TIME = 0.1f;
    }

    #endregion Initialization

    #region Update Functions
    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxis("Horizontal");

        if (GameManager.Instance.changingScenes)
            return;
        DirectionSet();
        Dash();
        Smash();
        DashCounter();
        SmashCounter();
        if (isDashing || isSmashing)  // Briefly disable player controls if isDashing
            return;
        //Run();
        Slide();
        newRun();
        Climb();
        Jump();
        Tornado();
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

    void newRun()
    {

        if (dirX != 0)
        {
            if (!isPushed)
            {
                myRigidBody.velocity = new Vector2(dirX * runSpeed, myRigidBody.velocity.y);
                animator.SetFloat("Speed", Mathf.Abs(dirX));
            }
        }
        else if (dirX == 0)
        {
            animator.SetFloat("Speed", Mathf.Abs(dirX));
        }


    }

    private void Run()
    {

        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        Vector2 playerVelocity = new Vector2(runSpeed * controlThrow, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        animator.SetFloat("Speed", Mathf.Abs(controlThrow));


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
        if (!isGrounded())
        {
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            if (isSliding)
            {
                isSliding = false;
                StartCoroutine(SlideJumpOut(0.18f));
            }

            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocityToAdd;

            //Play Jump Sound FX
            SoundManager.PlaySound(SoundManager.Sound.JumpFX, Random.Range(0.85f, 1.0f));
            StartCoroutine(jumpTimer(.80f));

        }

    }

    private void Slide()
    {
        if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Slope")))
        {
            Debug.Log("SLIDING");
            if(!isSliding)
                StartCoroutine(SlideTimer(0.18f));
            isSliding = true;
            animator.SetBool("isSliding", true);
        }
        else if (isSliding)
        {
            isSliding = false;
            animator.SetBool("isSliding", false);
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


            if (canDash && !isDashing)
            {
                GameObject DashEffectToDestroy;

                if (!isGrounded())
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

                        animator.SetBool("isDashing", true);

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

                        animator.SetBool("isDashing", true);

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
                animator.SetBool("isDashing", false);
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
            if (isGrounded() || !hasAirDashed)
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

            if (canSmash && !isSmashing)
            {
                if (direction == Direction.right)
                {
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isSideSmashing", true);
                    SoundManager.PlaySound(SoundManager.Sound.SmashWhoosh, 1f);
                    isSmashing = true;
                    smashTime = TOTAL_SMASH_TIME;
                    myRigidBody.gravityScale = 0.0f;
                    myRigidBody.velocity = Vector2.right * smashSpeed;
                    smashDirection = SmashDirection.right;
                }
                else if (direction == Direction.left)
                {
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isSideSmashing", true);
                    SoundManager.PlaySound(SoundManager.Sound.SmashWhoosh, 1f);
                    isSmashing = true;
                    smashTime = TOTAL_SMASH_TIME;
                    myRigidBody.gravityScale = 0.0f;
                    myRigidBody.velocity = Vector2.left * smashSpeed;
                    smashDirection = SmashDirection.left;
                }
                if (direction == Direction.down && !isGrounded())
                {
                    StartCoroutine(DownSmashTimer(0.26f));
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

            Collider2D[] smashablesHit = null;
            // If player collides with the ground while smashing down
            if ((smashDirection == SmashDirection.down) && Physics2D.OverlapCircle(downSmashPos.position, checkRadius, GameManager.Instance.ground))
            {
                SoundManager.PlaySound(SoundManager.Sound.SmashFX, 1f);
                myRigidBody.velocity = Vector2.zero;
                myRigidBody.gravityScale = 1.0f;
                isSmashing = false;
                canSmash = false;
                smashCooldown = MIN_SMASH_COOLDOWN;

                animator.SetBool("isDownSmashIn", false);
                animator.SetBool("isDownSmash", false);
                Debug.Log("smashToGround");
                StartCoroutine(DownSmashCollisionTimer(0.4f));
            }


            // If player collides with a wall while smashing left or right
            else
            {
                if (smashDirection == SmashDirection.left && Physics2D.OverlapCircle(leftSmashPos.position, checkRadius, GameManager.Instance.ground))
                {
                    SoundManager.PlaySound(SoundManager.Sound.SmashFX, 1f);
                    Debug.Log("HERE");
                    myRigidBody.velocity = Vector2.zero;
                    myRigidBody.gravityScale = 1.0f;
                    isSmashing = false;
                    canSmash = false;
                    smashCooldown = MIN_SMASH_COOLDOWN;

                    animator.SetBool("isSideSmashing", false);
                }
                // Made seperate in case player smashes with their back to a wall
                else if (smashDirection == SmashDirection.right && Physics2D.OverlapCircle(rightSmashPos.position, checkRadius, GameManager.Instance.ground))
                {
                    SoundManager.PlaySound(SoundManager.Sound.SmashFX, 1f);
                    myRigidBody.velocity = Vector2.zero;
                    myRigidBody.gravityScale = 1.0f;
                    isSmashing = false;
                    canSmash = false;
                    smashCooldown = MIN_SMASH_COOLDOWN;

                    animator.SetBool("isSideSmashing", false);

                }
            }

            // If player collides with a smashable while smashing down
            if ((smashDirection == SmashDirection.down) && Physics2D.OverlapCircle(downSmashPos.position, checkRadius, GameManager.Instance.smashable))
            {
                SoundManager.PlaySound(SoundManager.Sound.SmashFX, 1f);
                smashablesHit = Physics2D.OverlapCircleAll(downSmashPos.position, checkRadius, GameManager.Instance.smashable);
                myRigidBody.velocity = Vector2.zero;
                myRigidBody.gravityScale = 1.0f;
                isSmashing = false;
                canSmash = false;
                smashCooldown = MIN_SMASH_COOLDOWN;

                animator.SetBool("isDownSmashIn", false);
                animator.SetBool("isDownSmash", false);
                StartCoroutine(DownSmashCollisionTimer(0.32f));
            }
            // If player collides with a smashable while smashing left or right
            else
            {
                // They continue smashing for another 0.1 seconds in order to "smash through"
                if (smashDirection == SmashDirection.left && Physics2D.OverlapCircle(leftSmashPos.position, checkRadius, GameManager.Instance.smashable))
                {
                    SoundManager.PlaySound(SoundManager.Sound.SmashFX, 1f);
                    smashablesHit = Physics2D.OverlapCircleAll(leftSmashPos.position, checkRadius, GameManager.Instance.smashable);
                    smashTime = 0.1f;
                }
                else if (smashDirection == SmashDirection.right && Physics2D.OverlapCircle(rightSmashPos.position, checkRadius, GameManager.Instance.smashable))
                {
                    SoundManager.PlaySound(SoundManager.Sound.SmashFX, 1f);
                    smashablesHit = Physics2D.OverlapCircleAll(rightSmashPos.position, checkRadius, GameManager.Instance.smashable);
                    smashTime = 0.1f;
                }
            }
            if (smashablesHit != null)
                for (int i = 0; i < smashablesHit.Length; i++)
                {
                    smashablesHit[i].gameObject.GetComponent<Explodable>().explode();
                }

            // If player smashes to the side but collides with nothing before smash ends
            if (smashTime <= 0.0f && (smashDirection != SmashDirection.down))
            {
                myRigidBody.velocity = Vector2.zero;
                myRigidBody.gravityScale = 1.0f;
                isSmashing = false;
                canSmash = false;
                smashCooldown = MIN_SMASH_COOLDOWN;

                animator.SetBool("isSideSmashing", false);
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
            if (isGrounded())
            {
                canSmash = true;

            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(downSmashPos.position, checkRadius);

        Gizmos.DrawWireSphere(leftSmashPos.position, checkRadius);

        Gizmos.DrawWireSphere(rightSmashPos.position, checkRadius);


    }
    #endregion Smash

    #endregion Abilities

    #region Animation
    IEnumerator jumpTimer(float time)
    {
        animator.SetBool("isJumping", true);
        yield return new WaitForSeconds(time);
        animator.SetBool("isJumping", false);
    }

    IEnumerator dashTimer(float time)
    {
        animator.SetBool("isDashing", true);
        yield return new WaitForSeconds(time);
        animator.SetBool("isDashing", false);
    }

    IEnumerator DownSmashTimer(float time)
    {
        animator.SetBool("isJumping", false);
        animator.SetBool("isDownSmashIn", true);
        yield return new WaitForSeconds(time);
        animator.SetBool("isDownSmashIn", false);
    }

    IEnumerator DownSmashCollisionTimer(float time)
    {
        animator.SetBool("isDownSmashOut", true);
        yield return new WaitForSeconds(time);
        animator.SetBool("isDownSmashOut", false);

    }

    IEnumerator SlideTimer(float time)
    {
        animator.SetBool("isSlideIn", true);
        yield return new WaitForSeconds(time);
        animator.SetBool("isSlideIn", false);
    }

    IEnumerator SlideJumpOut(float time)
    {
        animator.SetBool("SlideJumpOut", true);
        animator.SetBool("isSliding", false);
        yield return new WaitForSeconds(time);
        animator.SetBool("SlideJumpOut", false);
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

    #endregion Death

    #region Other
    void Tornado()
    {
        if (capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Tornado")))
        {
            if (dirX > 0)
                StartCoroutine(TornadoPush(-1f));
            else if (dirX < 0)
            {
                StartCoroutine(TornadoPush(1f));
            }
        }
    }

    IEnumerator TornadoPush(float direction)
    {
        isPushed = true;
        SlideTimer(0.06f);
        Debug.Log("SLIDING");
        myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, 0f);
        myRigidBody.AddForce(new Vector2(slideDistance * direction, 0f), ForceMode2D.Impulse);
        float gravity = myRigidBody.gravityScale;
        myRigidBody.gravityScale = 0;
        yield return new WaitForSeconds(0.9f);
        isPushed = false;
        myRigidBody.gravityScale = gravity;
    }

    bool isGrounded()
    { 
        if(boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) || boxCollider.IsTouchingLayers(LayerMask.GetMask("Smashable")) || boxCollider.IsTouchingLayers(LayerMask.GetMask("Slope")))
            return true;
        return false;
    }
    #endregion Other
}

