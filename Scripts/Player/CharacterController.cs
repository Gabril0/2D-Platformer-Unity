using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class CharacterController : MonoBehaviour
{
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float deceleration = 1.5f;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private LayerMask playerLayer;

    private Vector2 tempVelocity;

    private Rigidbody2D rb;
    private CapsuleCollider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
    }

    void FixedUpdate()
    {
        move();
        ApplyGravity();
        Jump();
        CheckCollision();
        //TODO
        //Coyote
        //Jump Buffer
        //Jump Height
        //Trail
        //Apex Modifiers
        //Edge detection
        //Wall stop

        rb.velocity = tempVelocity;
    }

    private void ApplyGravity()
    {
        Vector2 newVelocity = tempVelocity + Vector2.down * gravity;
        tempVelocity = new Vector2(tempVelocity.x, newVelocity.y); // To not change X velocity
    }

    private void Jump() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector2 newVelocity = tempVelocity + Vector2.up * jumpForce;
            tempVelocity = new Vector2(tempVelocity.x, newVelocity.y);
        }
    }

    private void move()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        //Debug.Log(tempVelocity);

         if (Mathf.Abs(horizontalInput) > 0)
        {
            if (Mathf.Abs(tempVelocity.x) <= maxSpeed)
            {
                tempVelocity += horizontalInput * Vector2.right * speed;
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
        if (hitGround) {
            tempVelocity.y = 0;
        }
    }

}
