using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxSpeed = 0.5f;
    private Transform cam;
    private float length;
    private float startingPosition;

    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Transform>();
        startingPosition = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x/4;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cam != null){
            float temp = (cam.position.x * (1 - parallaxSpeed));
            float distance = (cam.position.x * parallaxSpeed);

            transform.position = new Vector3(startingPosition + distance, transform.position.y, transform.position.z);

            if (temp > startingPosition + length) startingPosition += length;
            else if(temp < startingPosition - length) startingPosition -= length;
        }
    }
}
