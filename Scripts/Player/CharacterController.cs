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
    private float lastTimeTouchedGround = 0;
    [SerializeField] private float jumpBuffer = 0.2f;
    private float startedJumpBuffer;
    private bool isJumpBuffered = false;

    //Terminal Velocity
    [SerializeField] private float terminalVelocity = 15;

    //Coyote
    [SerializeField] float coyoteTime = 0.1f;
    private bool coyoteActive = false;

    //Run
    private bool pressedRun = false;
    [SerializeField] float maxSpeedRun = 25;

    //GroundPound
    private bool pressedGroundPound = false;
    [SerializeField] float groundPoundForce = 30;
    private bool isGroundPounding = false;
    private float speedXBeforeGP = 0;

    //SideSwitch
    private float lastFrameVelocityX = 0;
    [SerializeField] float sideSwitchTimer = 0.2f;
    private float switchStartTimer = 0;
    private bool canSideSwitch;


    //Collision and Physics
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
        InputManager();
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
        pressedRun = Input.GetKey(KeyCode.LeftShift)? true : false;

        // GroundPound
        if(Input.GetKeyDown(KeyCode.LeftControl)) pressedGroundPound = true;

        horizontalInput = Input.GetAxisRaw("Horizontal");

    }

    void FixedUpdate()
    {
        Debug.Log(tempVelocity);
        CheckCollision();
        Move();
        ApplyGravity();
        Jump();
        GroundPound();
        SideSwitch();
        //TODO
        //FinishSideSwitch
        //Crawl
        //Find a way to transfer speed between sides


        

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
        if (((pressedJump || isJumpBuffered) && isGrounded) || coyoteActive) {
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
        if (isGroundPounding) {
            tempVelocity = new Vector2(0, -groundPoundForce);
            if (isGrounded){
                isGroundPounding = false;
                tempVelocity.x = speedXBeforeGP;
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

    private void SideSwitch() {
        //Compare the transform change with the speed, and then apply a buffer to send flying in the oposite direction while maintaining the last speed
    }

    private void Move()
    {
         if (Mathf.Abs(horizontalInput) > 0)
        {   // About Walking
            if (!pressedRun && tempVelocity.x > maxSpeed) {
                tempVelocity.x = maxSpeed;
            }
            if (Mathf.Abs(tempVelocity.x) <= maxSpeed && !pressedRun)
            {
                tempVelocity += horizontalInput * Vector2.right * speed;
            }
            if (Mathf.Sign(horizontalInput) != Mathf.Sign(tempVelocity.x))
            {
                tempVelocity += horizontalInput * Vector2.right * deceleration/4;
            }
            if ((Mathf.Abs(tempVelocity.x) <= maxSpeed) && isJumping && !pressedRun) {
                tempVelocity += horizontalInput * Vector2.right * deceleration / 4 * airAcceleration;
            }
            // End Walking
            if (pressedRun) { // About running
                if (Mathf.Abs(tempVelocity.x) <= maxSpeedRun) tempVelocity += horizontalInput * Vector2.right * speed;
                if ((Mathf.Abs(tempVelocity.x) <= maxSpeedRun) && isJumping) tempVelocity += horizontalInput * Vector2.right * deceleration / 4 * airAcceleration;
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
        //Vector2 bottomCenter = new Vector2(col.bounds.center.x, col.bounds.min.y);
        //Vector2 directionDown = Vector2.down;

        //bool hitGround = Physics2D.Raycast(bottomCenter, directionDown, 0.05f, ~playerLayer);

        //Vector2 topCenter = new Vector2(col.bounds.center.x, col.bounds.max.y);
        //Vector2 directionUp = Vector2.up;

        //bool hitCeiling = Physics2D.Raycast(topCenter, directionUp, 0.05f, ~playerLayer);
        bool hitGround = Physics2D.CapsuleCast(col.bounds.center, col.bounds.size - new Vector3(0.1f, 0f, 0f), col.direction, 0, Vector2.down, 0.05f, ~playerLayer);
        //bool hitCeiling= Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.up, 0.05f, ~playerLayer);
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
        //if (!isGrounded && hitCeiling)
        //{
        //    //tempVelocity.y = 0;
        //}
    }

}
