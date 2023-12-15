using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTemplate : MonoBehaviour
{
    [SerializeField] protected int healthHit;
    [SerializeField] protected int damage;
    [SerializeField] protected float playerSpeedBoost;
    [SerializeField] protected float playerSpeedBoostDuration;
    private CharacterController playerController;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponentInChildren<CharacterController>();
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
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
            boxCollider.enabled = false;
            rb.isKinematic = true;
            Destroy(root, playerSpeedBoostDuration);
        }
    }

    protected void EnemyBehaviour() { }

    private bool CheckHead() {
        return Physics2D.BoxCast(boxCollider.bounds.center,boxCollider.bounds.size, transform.rotation.eulerAngles.z, Vector2.up, 0.02f, playerController.PlayerLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && CheckHead()) {
            healthHit -= 1;
        }
    }
}