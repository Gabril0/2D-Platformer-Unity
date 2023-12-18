using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class CharacterController : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private float deceleration = 1.5f;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] public float maxSpeed = 15f; //{get;set;}
    private float horizontalInput;
    private float verticalInput;
    private int lastFrameDirection = 5;

    [Header("Collision")]
    [SerializeField] private bool isGrounded = false;
    [SerializeField] public LayerMask PlayerLayer;
    [SerializeField] public LayerMask PlatformLayer;
    [SerializeField] public LayerMask EnemyLayer;
    [SerializeField] public LayerMask TriggerLayer;
    private Rigidbody2D rb;
    private CapsuleCollider2D col;
    private Vector2 tempVelocity;
    private bool isCollidingWithOneWayPlatforms = false;
    bool hitWallRight = false;
    bool hitWallLeft = false;


[Header("Jump")]
    [SerializeField] private float jumpDeceleration = 15;
    [SerializeField] private float airAcceleration = 0.4f;
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float jumpForce = 1.5f;
    private bool pressedJump = false;
    private bool isJumping = false;
    private float timeStartedHoldingJump;
    private float timeHoldingJump;
    private float speedBeforeJump;


    //Variable Jump Height
    [SerializeField] private float maxJumpTime = 0.8f;
    private float variableJumpHeight;
    private bool releasedJumpButton = false;
    [SerializeField] private float minimumJumpHeight = 0.5f;

    //Jump Buffer
    private float lastTimeTouchedGround = 0;
    [SerializeField] private float jumpBuffer = 0.2f;
    private float startedJumpBuffer;
    private bool isJumpBuffered = false;

    //Terminal Velocity
    [SerializeField] private float terminalVelocity = 15;

    //Coyote
    [SerializeField] float coyoteTime = 0.1f;
    private bool coyoteActive = false;

    [Header("Run")]
    [SerializeField] public float maxSpeedRun = 25;
    public float originalMaxSpeed;
    private bool pressedRun = false;

    [Header("GroundPound")]
    [SerializeField] float groundPoundForce = 30;
    private bool pressedGroundPound = false;
    private bool isGroundPounding = false;
    public bool justGroundPounded = false;
    private float speedXBeforeGP = 0;
    private bool superJump = false;

    //Crouch
    private bool isCrouching = false;
    private bool isHittingHead = false;

    //Boost by changing max speed
    private float timeSinceStartedBoost = 0;

    //Health
    [SerializeField] int health = 3;
    private bool hitStun = false;
    private int reverseDirection = 0;




    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        originalMaxSpeed = maxSpeedRun;
    }
    private void Update()
    {
        InputManager();
        CheckCollision();
        CheckHitStun();
    }

    private void InputManager()
    {
        // Jump
        if (Input.GetKeyDown(KeyCode.Space)){
            pressedJump = isGrounded ? true : false;
            if (!isGrounded) { pressedJump = (Time.time - lastTimeTouchedGround < coyoteTime) ? true : false; 
                coyoteActive = pressedJump? true:false; 
            }

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
        // Running
        pressedRun = Input.GetKey(KeyCode.X)? true : false;

        // GroundPound
        if(Input.GetKeyDown(KeyCode.Z)) pressedGroundPound = true;
        superJump = ((Input.GetKey(KeyCode.Space)) && isGroundPounding) ? true : false;

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

    }


    void FixedUpdate()
    {
        Move();
        ApplyGravity();
        Jump();
        GroundPound();
        Crouch();        

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
        if ((lastTimeTouchedGround - startedJumpBuffer < jumpBuffer) && !isGrounded)
        {
            isJumpBuffered = true;
        }
        else if ((lastTimeTouchedGround - startedJumpBuffer > jumpBuffer)) isJumpBuffered = false;
        if (((pressedJump || isJumpBuffered) && isGrounded) || (coyoteActive && !isJumping)) {
            speedBeforeJump = tempVelocity.x;
            tempVelocity.y = jumpForce;
            pressedJump = false;
            isJumping = true;
            startedJumpBuffer = 0;
            coyoteActive = false;
        }
        if (isJumping) {
            tempVelocity.y -= releasedJumpButton ? jumpForce / jumpDeceleration * 1 - variableJumpHeight : 0;
        }
        
    }

    private void GroundPound(){// For the move where the player change directions while keeping the speed
        if (isGroundPounding && !hitStun) {
            tempVelocity = new Vector2(0, -groundPoundForce);
            if (isGrounded){
                justGroundPounded = true;
                tempVelocity = new Vector2(0, 0);
                Invoke("DeactivateGroundPound", 0.2f);
            }
        }
        if (pressedGroundPound && !isGrounded)
        {
            isGroundPounding = true;
            pressedGroundPound = false;
            speedXBeforeGP = tempVelocity.x;
        }
        else {
            pressedGroundPound = false;
        }
    }

    private void DeactivateGroundPound() {
        justGroundPounded = false;
        if (horizontalInput != 0 && !superJump)
        {
            int signal = horizontalInput > 0 ? 1 : -1;
            isGroundPounding = false;
            tempVelocity.x = Mathf.Abs(speedXBeforeGP) * signal;
        }
        if(superJump){
            isGroundPounding = false;
            isJumpBuffered = false;
            tempVelocity.x = 0;
            tempVelocity.y = jumpForce * 1.25f;
            isJumping = true;
        }
    }

    private void Crouch() {
        if (verticalInput < 0)
        {
            if (transform.localScale == Vector3.one) {
                if(isGrounded) tempVelocity.y += -20;
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);
                isCrouching = true;
            }
        }
        else {
            if (!isHittingHead){
                transform.localScale = Vector3.one;
                
                isCrouching = false;
            }
        }
    }

    private void Move()
    {
        bool isTurning = false;
        if (lastFrameDirection == 5) lastFrameDirection = (int)Mathf.Sign(horizontalInput);
        if (lastFrameDirection != Mathf.Sign(horizontalInput)) isTurning = true;
         if ((Mathf.Abs(horizontalInput) > 0) && !hitStun)
        {
            // Crouching
            if (isCrouching && !isJumping)
            {
                if (Mathf.Abs(tempVelocity.x) <= maxSpeed * 0.5f) tempVelocity += horizontalInput * Vector2.right * speed;
                else { tempVelocity.x -= horizontalInput * maxSpeed/20; }
            }
            // Checking collisions
            if (horizontalInput > 0 && hitWallRight) { tempVelocity -= horizontalInput * Vector2.right * speed * 2; return; }
            if (horizontalInput < 0 && hitWallLeft) { tempVelocity -= horizontalInput * Vector2.right * speed * 2; return; }

            // Walking
            if (!pressedRun && Mathf.Abs(tempVelocity.x) > maxSpeed) {
                tempVelocity.x = maxSpeed * horizontalInput;
            }
            if (Mathf.Abs(tempVelocity.x) <= maxSpeed && !pressedRun)
            {
                tempVelocity += horizontalInput * Vector2.right * speed;
            }
            if ((Mathf.Sign(horizontalInput) != Mathf.Sign(tempVelocity.x)) || isTurning) //FIXMEEEE AND ALSO DO HIT PROTECTION
            {
                tempVelocity += horizontalInput * Vector2.right * deceleration / 4;
            }
            if ((Mathf.Abs(tempVelocity.x) <= Mathf.Abs(speedBeforeJump + (deceleration / 4 * airAcceleration))) && isJumping) {
                tempVelocity += horizontalInput * Vector2.right * deceleration / 4 * airAcceleration;
            }

            // Running
            if (pressedRun) { 
                if (Mathf.Abs(tempVelocity.x) <= maxSpeedRun) tempVelocity += horizontalInput * Vector2.right * speed;
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
         lastFrameDirection = (int)Mathf.Sign(horizontalInput);
    }

    private void CheckCollision()
    {
        Vector2 topCenter = new Vector2(col.bounds.center.x, col.bounds.max.y);

        bool hitCeiling = Physics2D.Raycast(topCenter, Vector2.up, 0.05f, ~PlayerLayer & ~TriggerLayer); 
        hitCeiling = hitCeiling && !Physics2D.Raycast(topCenter, Vector2.up, 0.05f, ~PlatformLayer); 

        float horizontalExtent = col.bounds.extents.x + 0.01f;

        hitWallRight = Physics2D.Raycast(col.bounds.center, Vector2.right, horizontalExtent, ~PlayerLayer & ~TriggerLayer);
        hitWallLeft = Physics2D.Raycast(col.bounds.center, Vector3.left, horizontalExtent, ~PlayerLayer & ~TriggerLayer);


        bool hitGround = Physics2D.CapsuleCast(col.bounds.center, col.bounds.size - new Vector3(0.2f, 0f, 0f), col.direction, 0, Vector2.down, 0.05f, ~PlayerLayer & ~TriggerLayer);

        isHittingHead = hitCeiling;
        if (hitGround && tempVelocity.y <=0)
        {
            isGrounded = true;
            isJumping = false;
            lastTimeTouchedGround = Time.time;
        }
        else
        {
            isGrounded = false;
        }
        if (!isGrounded && hitCeiling)
        {
            tempVelocity.y = -gravity * 5;
        }
        bool isStuck = Physics2D.CapsuleCast(col.bounds.center, col.bounds.size - new Vector3(0f, 0.2f, 0f), col.direction, 0, Vector2.up, 0.1f, PlatformLayer);

        if (isGrounded && isStuck && isCollidingWithOneWayPlatforms) { // To prevent the player from getting stuck in platforms, as I am using custom physics
            isGrounded = false;
            isJumping = true;
        }

    }

    public void ChangeMaxSpeed(float percentage, float duration) {
        timeSinceStartedBoost += Time.deltaTime;
        if (timeSinceStartedBoost < duration)
        {
            maxSpeedRun = maxSpeedRun * percentage;
        }
        else { maxSpeedRun = originalMaxSpeed; }
    }

    public void HitJump() {
        isGroundPounding = false;
        coyoteActive = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isCollidingWithOneWayPlatforms = collision.collider.CompareTag("Platform") ? true : false;

        if (collision.collider.CompareTag("Enemy") && !Physics2D.CapsuleCast(col.bounds.center, col.bounds.size - new Vector3(0.2f, 0f, 0f), col.direction, 0, Vector2.down, 0.05f, EnemyLayer)) {
            if (!hitStun) {
            tempVelocity.y = jumpForce* 0.7f;
            reverseDirection =  -1 * (int)Mathf.Sign(tempVelocity.x);
            tempVelocity.x = 0;
            }
            hitStun = true;
        }
    }

    private void CheckHitStun() {
        if (hitStun) {
            tempVelocity.x += Mathf.Abs(tempVelocity.x) <= maxSpeed? reverseDirection  * (maxSpeed/3) : 0;
            hitStun = !isGrounded ? true : false;
        }
    }
}
