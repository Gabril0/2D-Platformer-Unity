using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    //    private Transform playerPosition;
    //    private Rigidbody2D playerRB;
    //    private Vector3 baseOffset = new Vector3(0, 2.5f, 0);
    //    private Vector3 sideOffset = new Vector3(2.5f, 0, 0);
    //    private Camera cam;

    //    private float transitionTime;
    //    private Vector3 tempPosition;
    //    private Vector3 alteredPosition;
    //    void Awake()
    //    {
    //        playerPosition = GameObject.Find("Player").GetComponent<Transform>();
    //        playerRB = GameObject.Find("Player").GetComponent<Rigidbody2D>();
    //        cam = GetComponent<Camera>();
    //        cam.orthographicSize = 8;
    //    }

    //    // Update is called once per frame
    //    void LateUpdate()
    //    {
    //        if (playerPosition != null && playerRB != null) {
    //            tempPosition = transform.position;
    //            tempPosition = new Vector3(playerPosition.position.x, playerPosition.position.y, tempPosition.z) + baseOffset;
    //            ChangeDirection(playerRB.velocity.x);
    //            transform.position = alteredPosition;
    //        }
    //    }

    //    private void ChangeDirection(float speed) {
    //        float speedPercentage = Mathf.InverseLerp(0, 25, Mathf.Abs(speed)); //FIIIIIX DO WITH CINEMACHINE.
    //        if (speed > 0) {
    //            transitionTime += Time.deltaTime;
    //            tempPosition.x = Mathf.Lerp(tempPosition.x, tempPosition.x + (sideOffset.x * speedPercentage), transitionTime);
    //        }
    //        if (speed < 0){
    //            transitionTime += Time.deltaTime;
    //            tempPosition.x = Mathf.Lerp(tempPosition.x, tempPosition.x -(sideOffset.x * speedPercentage), transitionTime);

    //        }
    //        alteredPosition = tempPosition;

    //        if (speed == 0) {
    //            Invoke("ResetPosition", 1);
    //        }
    //    }

    //    private void ResetPosition() {
    //        transitionTime -= Time.deltaTime / 100; // For it to reset slower
    //        tempPosition.x = Mathf.Lerp(alteredPosition.x, playerPosition.position.x, transitionTime);
    //        alteredPosition = tempPosition;
    //        if (transitionTime < 0) transitionTime = 0;
    //    }
}
