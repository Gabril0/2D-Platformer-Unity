using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private float deceleration = 1.5f;
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        move();
        ApplyGravity();
        CheckGround();
    }

    private void ApplyGravity()
    {
        Vector2 newVelocity = rb.velocity + Vector2.down * gravity;
        rb.velocity = new Vector2(rb.velocity.x, newVelocity.y); // To not change X velocity
    }

    private void move()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        Debug.Log(rb.velocity);

        if (Mathf.Abs(horizontalInput) > 0)
        {
            if (Mathf.Abs(rb.velocity.x) <= maxSpeed)
            {
                rb.velocity += horizontalInput * Vector2.right * speed;
            }
        }
        else
        {
            if (Mathf.Abs(rb.velocity.x) > 0)
            {
                rb.velocity -= rb.velocity.normalized * deceleration;

                if (rb.velocity.magnitude < 0.1f)
                {
                    rb.velocity = Vector2.zero;
                }
            }
        }
    }


    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer);

        if (hit.collider != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }
    }
}
