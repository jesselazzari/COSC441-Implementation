using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextInputHandler : MonoBehaviour
{
    public TMP_InputField inputField; 

    // Start is called before the first frame update
    void Start()
    {
        inputField.characterLimit = 2; // Limit input to 2 characters
        inputField.onEndEdit.AddListener(HandleInput); // Call HandleInput when Enter is pressed
        inputField.ActivateInputField(); // Set initial focus
    }

    void HandleInput(string userInput)
    {
        if (userInput.Length == 2) // Check if input has exactly 2 characters
        {
            // Find target based on input (assuming target names or tags match input)
            GameObject target = GameObject.Find(userInput);
            if (target != null)
            {
                Destroy(target); // Destroy the target
            }
            else
            {
                Debug.Log("No matching target found");
            }
        }
        else
        {
            Debug.Log("Input must be exactly 2 characters");
        }

        inputField.text = ""; // Clear the input field
        inputField.ActivateInputField(); // Re-focus input field
    }
}
