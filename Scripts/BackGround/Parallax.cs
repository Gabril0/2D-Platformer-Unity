using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxSpeed = 0.5f;
    [SerializeField] private bool isRepeatable;
    private Transform cameraPosition;
    private Vector3 lastPosition = Vector3.zero;
    private SpriteRenderer spriteRenderer;
    private float distanceTraveled = 0;
    private float initialDistanceOffset;
    void Start()
    {
        cameraPosition = GameObject.Find("Main Camera").GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialDistanceOffset = cameraPosition.position.x - transform.position.x;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraPosition != null){
            if (isRepeatable)
            {
                Debug.Log(lastPosition.x + " " + cameraPosition.position.x);
                if (lastPosition == Vector3.zero) { lastPosition = cameraPosition.position; }
                if (lastPosition.x != cameraPosition.position.x)
                {
                    transform.position += (cameraPosition.position - lastPosition) * 0.5f;
                    distanceTraveled += (cameraPosition.position.x - lastPosition.x) * 0.5f;
                }


                lastPosition = cameraPosition.position;
                BackGroundRepeat();
            }
            else { }
        }
    }

    private void BackGroundRepeat() {
        if (distanceTraveled > spriteRenderer.size.x / 2) {
            transform.position = new Vector3(cameraPosition.position.x + initialDistanceOffset, transform.position.y, transform.position.z);
            distanceTraveled = 0;
        }
    }
}
