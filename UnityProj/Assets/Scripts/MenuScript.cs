using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField participantInputField;
    [SerializeField] private UnityEngine.UI.Button startButton;
    
    void Start()
    {
        participantInputField.ActivateInputField(); // Set initial focus
        startButton.onClick.AddListener(StartGame);
        startButton.interactable = false;
        participantInputField.onValueChanged.AddListener(OnInputFieldChanged);
    }
    
    void Update()
    {
        // Check if the Enter key is pressed
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartGame(); // Call the method to start the game
        }
    }

    private void OnInputFieldChanged(string input)
    {
        // Enable the button only when the input field contains a valid number
        startButton.interactable = int.TryParse(input, out _);
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
