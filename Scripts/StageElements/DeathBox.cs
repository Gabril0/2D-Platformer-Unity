using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;
using UnityEngine;

public class DeathBox : MonoBehaviour
{
    private bool respawnLock = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!respawnLock)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player")){
            Time.timeScale = 0.1f;
            respawnLock = false;
        }
    }
}
