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

    
    float slideDistance = 10f;
    bool isSliding = false;
    

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
    CapsuleCollider2D capsuleCollider;
    float gravityScaleStart;
    float dirX;
    Direction direction = Direction.right;

    // Private Dash Variables
    bool canDash = true;
    bool hasAirDashed = false;
    bool isDashing = false;
    float dashTime;
    float dashCooldown = 0.0f;

    //Initialize soundTimerDictionary
    private void Awake()
    {
        SoundManager.Initialize();
    }

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        gravityScaleStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal");

        if (GameManager.Instance.changingScenes)
            return;
        DirectionSet();
        Dash();
        DashCounter();

        meetTornado();
        if (isDashing)  // Briefly disable player controls if isDashing
            return;
        
        Climb();
        Jump();
        //Slide();
    }

    private void FixedUpdate()
    {
        if (isDashing)
            return;
        Run();
    }

    void meetTornado()
    {
        if (capsuleCollider.IsTouchingLayers(LayerMask.GetMask("Tornado")))
        {
            if (dirX > 0)
                StartCoroutine(Sliding(-1f));
            else if (dirX < 0)
            {
                StartCoroutine(Sliding(1f));
            }
        }
    }

    private void DirectionSet()
    {
        if (CrossPlatformInputManager.GetAxis("Horizontal") > 0.0f)
        {
            direction = Direction.right;

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

            if (boxCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                //Play Run Sound FX
                SoundManager.PlaySound(SoundManager.Sound.RunFX, Random.Range(0.75f, 1f));
            }
            
        }
        //if (CrossPlatformInputManager.GetAxis("Vertical") > 0.0f)
        //    direction = Direction.up;
        //if (CrossPlatformInputManager.GetAxis("Vertical") < 0.0f)
        //    direction = Direction.down;
    }

    private void Run()
    {
        if (dirX != 0)
        {
                myRigidBody.velocity = new Vector2(dirX * runSpeed, myRigidBody.velocity.y);
            
        }
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

            //Play Jump Sound FX
            SoundManager.PlaySound(SoundManager.Sound.JumpFX, Random.Range(0.85f, 1.0f));
        }
    }

    /// <summary>
    /// Dashes player according to dashSpeed. DiagonalDash option.
    /// </summary>
    public void Dash()
    {
        if (CrossPlatformInputManager.GetButtonDown("Dash"))
        {
            if (canDash)
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

    /// <summary>
    /// Called when player dies. Resets scene
    /// </summary>
    public void Die()
    {
        myRigidBody.velocity = Vector2.zero;
        myRigidBody.gravityScale = 0.0f;
        GameManager.Instance.ReloadScene();
    }

    /*
    void Slide()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(Sliding(-1f));
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            StartCoroutine(Sliding(1f));
        }
    }
    */

    IEnumerator Sliding(float direction)
    {
        isSliding = true;
        myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, 0f);
        myRigidBody.AddForce(new Vector2(slideDistance * direction, 0f), ForceMode2D.Impulse);
        float gravity = myRigidBody.gravityScale;
        myRigidBody.gravityScale = 0;
        yield return new WaitForSeconds(0.4f);
        isSliding = false;
        myRigidBody.gravityScale = gravity;
    }
    
}


