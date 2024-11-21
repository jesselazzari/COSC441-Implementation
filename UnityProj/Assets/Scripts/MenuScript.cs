using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    void Update()
    {
        // Check if the Enter key is pressed
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartGame(); // Call the method to start the game
        }
    }

    public void StartGame()
    {
        // Load the scene named "GameScene"
        SceneManager.LoadScene("GameScene");
    }
}
