using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField participantInputField;
    
    void Start()
    {
        participantInputField.ActivateInputField(); // Set initial focus
    }
    
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
        // Get participant ID from the input field
        if (int.TryParse(participantInputField.text, out int participantID))
        {
            PlayerPrefs.SetInt("ParticipantID", participantID); // Store the ID persistently
            SceneManager.LoadScene("GameScene"); // Load the experiment scene
        }
        else
        {
            Debug.LogError("Invalid Participant ID. Please enter a valid number.");
        }
    }
}
