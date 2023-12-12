using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Rigidbody2D playerRb;
    private CharacterController controller;
    private SpriteRenderer spriteRenderer;
    private TrailRenderer trailRenderer;
    [SerializeField] private GameObject pivot;
    void Awake()
    {
        playerRb = pivot.GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = pivot.GetComponent<TrailRenderer>();
        controller = pivot.GetComponent<CharacterController>();
    }

    void LateUpdate()
    {
        CheckDirection();
        TrailUpdater();

        transform.position = playerRb.transform.position;
    }

    private void CheckDirection() {
        bool lastFlip = spriteRenderer.flipX;
        spriteRenderer.flipX = playerRb.velocity.x > 0 ? false : true;
        if (playerRb.velocity.x == 0) spriteRenderer.flipX = lastFlip;
        
    }

    private void TrailUpdater()
    {
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKeys = trailRenderer.colorGradient.colorKeys;
        GradientAlphaKey[] alphaKeys = trailRenderer.colorGradient.alphaKeys;

        float maxVelocity = controller.maxSpeedRun;

        float velocityRange = Mathf.Clamp01(playerRb.velocity.magnitude / maxVelocity);

        float minAlpha = 0.1f; 
        float maxAlpha = 1.0f; 

        float alpha = Mathf.Lerp(minAlpha, maxAlpha, velocityRange);

        for (int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i].alpha = alpha;
        }

        gradient.SetKeys(colorKeys, alphaKeys);

        trailRenderer.colorGradient = gradient;
    }
}
