using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTemplate : MonoBehaviour
{
    [Header("Basic Stats")]
    [SerializeField] protected int healthHit;
    [SerializeField] public int damage;
    [SerializeField] protected float playerSpeedBoost;
    [SerializeField] protected float playerSpeedBoostDuration;
    protected CharacterController playerController;
    protected CapsuleCollider2D col;
    protected Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponentInChildren<CharacterController>();
        col = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        EnemyStart();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (healthHit > 0)
        {
            EnemyBehaviour();
        }
        else {
            playerController.ChangeMaxSpeed(playerSpeedBoost,playerSpeedBoostDuration);

            //Desativar a colisao
            //Continuar com o sprite renderer ate a animacao de morte terminar, quando terminar desativar o sprite renderer e continuar a dar buff de velocidade para o player
            GameObject root = gameObject.transform.parent.gameObject;
            col.enabled = false;
            rb.isKinematic = true;
            Destroy(root, playerSpeedBoostDuration);
        }
    }

    protected virtual void EnemyBehaviour() { }
    protected virtual void EnemyStart() { }

    private bool CheckHead() {
        return Physics2D.CapsuleCast(col.bounds.center, col.bounds.size, col.direction, 0, Vector2.up, 0.5f, playerController.PlayerLayer); ;
    }

    protected bool IsOnGround() {
        return Physics2D.CapsuleCast(col.bounds.center, col.bounds.size, col.direction, 0, Vector2.down, 0.02f, ~playerController.PlayerLayer & ~playerController.EnemyLayer); ;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && CheckHead()) {
            playerController.HitJump();
            healthHit -= 1;
        }
    }
}
