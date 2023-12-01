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
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float jumpForce = 1.5f;
    private bool pressedJump = false;
    private bool isJumping = false;

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
            pressedJump = true;
        }
        horizontalInput = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate()
    {
        Move();
        ApplyGravity();
        Jump();
        CheckCollision();
        //TODO
        //Coyote
        //Jump Buffer
        //Jump Height
        //Apex Modifiers
        //Edge detection
        //Wall stop
        //Fall when hitting the head

        rb.velocity = tempVelocity;
    }
    private void ApplyGravity()
    {

        if (!isGrounded && tempVelocity.y <=0){
            tempVelocity.y -= gravity;
            Debug.Log(Time.deltaTime);
        }
        if (!isGrounded && tempVelocity.y > 0)
        {
            tempVelocity.y -= jumpForce / jumpDeceleration;
            Debug.Log(Time.time);
            

        }
        if (isGrounded && !isJumping){
            if(tempVelocity.y < 0) tempVelocity.y = 0;
        }
    }


    private void Jump() {
        if (pressedJump && isGrounded) {
            tempVelocity.y = jumpForce;
            pressedJump = false;
            isJumping = true;
        }
    }

    private void Move()
    {
        //Debug.Log(tempVelocity);
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
        bool hitGround = Physics2D.CapsuleCast(col.bounds.center, col.size, col.direction, 0, Vector2.down,0.05f,~playerLayer);
        if (hitGround)
        {
            isGrounded = true;
            isJumping = false;
        }
        else {
            isGrounded= false;
        }
    }

}
