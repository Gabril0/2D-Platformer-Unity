using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class CharacterController : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private float deceleration = 1.5f;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float maxSpeed = 15f;
    private float horizontalInput;

    [Header("Collision")]
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private LayerMask playerLayer;


    [Header("Jump")]
    [SerializeField] private float jumpDeceleration = 15;
    [SerializeField] private float airAcceleration = 0.4f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float jumpForce = 1.5f;
    private bool pressedJump = false;
    private bool isJumping = false;
    private float timeStartedHoldingJump;
    private float timeHoldingJump;

    //Variable Jump Height
    [SerializeField] private float maxJumpTime = 0.8f;
    private float variableJumpHeight;
    private bool releasedJumpButton = false;
    [SerializeField] float minimumJumpHeight = 0.5f;

    //Jump Buffer
    private float timeTouchedGround = 0;
    [SerializeField] private float jumpBuffer = 0.2f;
    private float startedJumpBuffer;
    private bool isJumpBuffered = false;

    //Terminal Velocity
    [SerializeField] private float terminalVelocity = 15;

    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private Vector2 tempVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
    }
    private void Update()
    {
        inputManager();
    }

    private void inputManager()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            pressedJump = isGrounded? true : false;
            timeStartedHoldingJump = Time.time;
            startedJumpBuffer = Time.time;
            releasedJumpButton = false;
        }
        if (Input.GetKeyUp(KeyCode.Space)){
            timeHoldingJump = Time.time - timeStartedHoldingJump;
            variableJumpHeight = Mathf.Clamp01(timeHoldingJump / maxJumpTime);
            variableJumpHeight = variableJumpHeight > minimumJumpHeight ? variableJumpHeight : minimumJumpHeight;
            releasedJumpButton = true;
        }
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        CheckCollision();
        Move();
        ApplyGravity();
        Jump();
        //TODO
        //Coyote
        //Apex Modifiers
        //Edge detection
        //Wall stop
        

        rb.velocity = tempVelocity;
    }
    private void ApplyGravity()
    {
        if (!isGrounded && tempVelocity.y <=0){
            if (tempVelocity.y > -terminalVelocity) { tempVelocity.y -= gravity; }
        }
        if (!isGrounded && tempVelocity.y > 0)
        {
            tempVelocity.y -= jumpForce / jumpDeceleration;
        }
        if (isGrounded && !isJumping){
            if(tempVelocity.y < 0) tempVelocity.y = 0;
        }
        if (tempVelocity.y < -terminalVelocity) { tempVelocity.y = -terminalVelocity; }
    }


    private void Jump() {
        if ((timeTouchedGround - startedJumpBuffer < jumpBuffer) && !isGrounded)
        {
            isJumpBuffered = true;
        }
        else if ((timeTouchedGround - startedJumpBuffer > jumpBuffer)) isJumpBuffered = false;
        if ((pressedJump || isJumpBuffered) && isGrounded) {
            tempVelocity.y = jumpForce;
            pressedJump = false;
            isJumping = true;
            startedJumpBuffer = 0;
        }
        if (isJumping) {
            tempVelocity.y -= releasedJumpButton ? jumpForce / jumpDeceleration * 1 - variableJumpHeight : 0;
        }
        
    }

    private void Move()
    {
         if (Mathf.Abs(horizontalInput) > 0)
        {
            if (Mathf.Abs(tempVelocity.x) <= maxSpeed)
            {
                tempVelocity += horizontalInput * Vector2.right * speed;
            }
            if (Mathf.Sign(horizontalInput) != Mathf.Sign(tempVelocity.x))
            {
                tempVelocity += horizontalInput * Vector2.right * deceleration/4;
            }
            if ((Mathf.Abs(tempVelocity.x) <= maxSpeed) && isJumping) {
                tempVelocity += horizontalInput * Vector2.right * deceleration / 4 * airAcceleration;
            }
        }
        else
        {
            if (Mathf.Abs(tempVelocity.x) > 0)
            {
                tempVelocity -= tempVelocity.normalized * deceleration;

                
            }
            if (Mathf.Abs(tempVelocity.x) <= 4f)
            {
                tempVelocity = new Vector2(0, tempVelocity.y);
            }
        }
    }

    private void CheckCollision()
    {
        Vector2 bottomCenter = new Vector2(col.bounds.center.x, col.bounds.min.y);
        Vector2 directionDown = Vector2.down;

        bool hitGround = Physics2D.Raycast(bottomCenter, directionDown, 0.05f, ~playerLayer);

        Vector2 topCenter = new Vector2(col.bounds.center.x, col.bounds.max.y);
        Vector2 directionUp = Vector2.up;

        bool hitCeiling = Physics2D.Raycast(topCenter, directionUp, 0.05f, ~playerLayer);


        if (hitGround)
        {
            isGrounded = true;
            isJumping = false;
            timeTouchedGround = Time.time;
        }
        else
        {
            isGrounded = false;
        }
        if (!isGrounded && hitCeiling)
        {
            float vectorUp = tempVelocity.y;
            tempVelocity.y = -vectorUp / 2;
        }
    }

}
