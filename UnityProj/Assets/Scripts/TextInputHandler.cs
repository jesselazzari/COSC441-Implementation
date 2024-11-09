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

    void Update()
    {
        // Keep the input field focused
        if (!inputField.isFocused)
        {
            inputField.ActivateInputField();
        }
    }
    void HandleInput(string userInput)
    {
        if (userInput.Length == 2)
        {
            // Find all GameObjects with the "TextTarget" tag
            GameObject[] targets = GameObject.FindGameObjectsWithTag("TextTarget");

            foreach (GameObject target in targets)
            {
                TextMeshPro textComponent = target.GetComponentInChildren<TextMeshPro>();

                if (textComponent.text == userInput)
                    {
                        Debug.Log("Match found, destroying target: " + target.name);
                        Destroy(target); // Destroy the target if the text matches user input
                    }
            }
        }
        else
        {
            Debug.Log("Input must be exactly 2 characters");
        }

        inputField.text = ""; // Clear the input field
    }
}
