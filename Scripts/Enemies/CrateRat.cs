using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateRat : EnemyTemplate
{
    [Header("CrateRat")]
    [SerializeField] private GameObject crate;
    [SerializeField] private float throwCooldown;
    [SerializeField] private Vector2 throwDirection;
    [SerializeField] private float throwForce;
    private Rigidbody2D rbCrate;
    private bool throwLock = false;
    private bool checkGroundPound = false;

    override
    protected void EnemyStart()
    {
        rbCrate = crate.GetComponent<Rigidbody2D>();
    }

    override
    protected void EnemyBehaviour() {
        checkGroundPound = playerController.justGroundPounded;
        if (rbCrate.velocity == Vector2.zero) throwLock = false;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && checkGroundPound && !throwLock){
            throwLock = true;
            Invoke("ThrowBox", throwCooldown);
            
        }
    }

    private void ThrowBox() {
        rbCrate.AddForce(throwForce * throwDirection, ForceMode2D.Impulse);
        
    }
}
