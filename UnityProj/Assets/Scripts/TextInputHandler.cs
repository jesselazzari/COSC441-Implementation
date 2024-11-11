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
        inputField.onEndEdit.AddListener(HandleEnter); // Call HandleInput when Enter is pressed
        inputField.onValueChanged.AddListener(HandleHover);
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

    void HandleHover(string text) // Hover icon functionality and converts input to uppercase
    {
        string upperText = text.ToUpper();
        inputField.text = upperText; 

        GameObject[] targets = GameObject.FindGameObjectsWithTag("TextTarget");
        GameObject goalTarget = GameObject.FindGameObjectWithTag("Goal");

        foreach (GameObject target in targets)
        {
            TextMeshPro textComponent = target.GetComponentInChildren<TextMeshPro>();
            SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
            if (textComponent != null && spriteRenderer != null)
            {
                if (textComponent.text == upperText)
                {
                    spriteRenderer.color = new Color(0.169f, 0.169f, 0.169f); // On hover set to darker shade 
                }
                else
                {
                    spriteRenderer.color = Color.white; // Revert back to regular color 
                }
            }
        }

        if (goalTarget != null)
        {
            TextMeshPro goalTextComponent = goalTarget.GetComponentInChildren<TextMeshPro>();
            SpriteRenderer goalSpriteRenderer = goalTarget.GetComponent<SpriteRenderer>();
            if (goalTextComponent != null && goalSpriteRenderer != null)
            {
                if (goalTextComponent.text == upperText)
                {
                    goalSpriteRenderer.color =  goalSpriteRenderer.color = new Color(0f, 0.392f, 0f); // On hover set to darker shade of green
                }
                else
                {
                    goalSpriteRenderer.color = goalSpriteRenderer.color = Color.green; // Revert back to regular color 
                }
            }
        }
    }
    
    void HandleEnter(string userInput)
    {
    if (userInput.Length == 2)
    {
        // Find all GameObjects with the "TextTarget" tag
        GameObject[] targets = GameObject.FindGameObjectsWithTag("TextTarget");
        GameObject goalTarget = GameObject.FindGameObjectWithTag("Goal");

        TextMeshPro goalTextComponent = goalTarget.GetComponentInChildren<TextMeshPro>();

        // Iterate over the targets and check if any match the user input
        foreach (GameObject target in targets)
        {
            TextMeshPro textComponent = target.GetComponentInChildren<TextMeshPro>();

            if (textComponent.text == userInput)
            {
                Debug.Log("Match found, destroying target: " + target.name);
                Destroy(target); // Destroy the target if the text matches user input
            }
        }

        // Check if the goal target matches the user input
        if (goalTextComponent.text == userInput)
        {
            Debug.Log("Goal target found, destroying goal target");
            Destroy(goalTarget); // Destroy the goal target if it matches user input
        }
    }
    else
    {
        Debug.Log("Input must be exactly 2 characters");
    }

    inputField.text = ""; // Clear the input field
    }
}
