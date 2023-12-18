using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LabRat : EnemyTemplate
{
    [Header("LabRat")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float gravity;
    private bool canJump = true;

    override
    protected void EnemyBehaviour()
    {
        Gravity();
        if (IsOnGround() && canJump)
        {
            Jump();
            canJump = false;
            StartCoroutine(ResetJumpCooldown());
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector2(0, jumpForce);
    }

    private void Gravity()
    {
        rb.velocity -= new Vector2(0, gravity);
    }

    private IEnumerator ResetJumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

}
