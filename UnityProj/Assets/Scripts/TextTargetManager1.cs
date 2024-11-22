using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; 
using System.IO;

public class TargetManager : MonoBehaviour
{
    public int participantID;
    [SerializeField] private GameObject target;
    [SerializeField] private List<float> targetSizes;
    [SerializeField] private List<float> targetAmplitudes;
    [SerializeField] private float numTrials = 10;
    [SerializeField] private int numTargets = 16;
    [SerializeField] private int numRows = 4;  
    [SerializeField] private int numColumns = 4;  
    [SerializeField] private float gridOffsetY = 100f;
    private int currentTrial = 0;
    private List<float> trialTimes = new List<float>(); // List to store trial times
    private float trialStartTime; // Timer start time
    private string outputFilePath; // Path to save CSV file
    private List<float> randomSizes;
    private Vector2 screenCentre;
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
        screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        participantID = PlayerPrefs.GetInt("ParticipantID", 1); // Retrieve participant ID from PlayerPrefs

        // Initialize CSV file path
        outputFilePath = Path.Combine(Application.dataPath, $"Participant_{participantID}_TrialData.csv");

        // Check if the file exists if so delete it
        if (File.Exists(outputFilePath))
        {
            File.Delete(outputFilePath);
            Debug.Log($"Existing file deleted: {outputFilePath}");
        }

        // Start the first trial
        StartTrial();

    }

    private void Update()
    {
        CheckTargets();
    }

    private void StartTrial()
    {
        trialStartTime = Time.time; // Record trial start time
        SpawnTargets();
    }

    private void EndTrial()
    {
        float trialEndTime = Time.time; // Record trial end time
        float trialDuration = trialEndTime - trialStartTime; // Calculate trial duration
        trialTimes.Add(trialDuration); // Add duration to list

        currentTrial++;
        if (currentTrial < numTrials)
        {
            GameObject[] textTargets = GameObject.FindGameObjectsWithTag("TextTarget");
            foreach (GameObject textTarget in textTargets)
            {
                Destroy(textTarget);
            }
            StartTrial(); // Start the next trial
        }
        else
        {
            SaveToCSV(); // Save data to CSV after all trials
            SceneManager.LoadScene("EndScreen"); // Load EndScreen
        }
    }

    private void SaveToCSV()
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            writer.WriteLine("ParticipantID,TrialTime");

            foreach (float trialTime in trialTimes)
            {
                writer.WriteLine($"{participantID},{trialTime}");
            }
        }

        Debug.Log($"Trial data saved to {outputFilePath}");
    }


    private void SpawnTargets()
    {
        List<GameObject> targetList = new();
        List<Vector3> points = GenerateGridPoints();
        List<float> randomSizes = GenerateRandomSizes();
        List<char> leftChars = new List<char> { 'Q', 'W', 'E', 'R', 'A', 'S', 'D', 'F', 'Z', 'X', 'C', 'V' };
        List<char> rightChars = new List<char> { 'Y', 'U', 'I', 'O', 'P', 'J', 'K', 'K', 'N', 'M' };
        List<string> usedCombinations = new List<string>();

        for (int i = 0; i < numTargets; i++)
        {
            GameObject targetObject = Instantiate(target, points[i], Quaternion.identity, transform);
            targetObject.transform.localScale = Vector3.one * randomSizes[i];

            string charPair; // Generate a unique character combination
            do
            {
                char leftChar = leftChars[Random.Range(0, leftChars.Count)];
                char rightChar = rightChars[Random.Range(0, rightChars.Count)];
                charPair = $"{leftChar}{rightChar}";
            } 
            while (usedCombinations.Contains(charPair)); // Repeat until a unique pair is found

            // Add the unique combination to the list
            usedCombinations.Add(charPair);

            // Set the unique combination to the TextMeshPro component
            TextMeshPro textComponent = targetObject.GetComponentInChildren<TextMeshPro>();
            if (textComponent != null)
            {
                textComponent.text = charPair;
            }
              
            targetList.Add(targetObject);

        }

        // Randomly select one target to be the goal
        if (targetList.Count > 0)
        {
            int goalIndex = Random.Range(0, targetList.Count);
            GameObject goalTarget = targetList[goalIndex];

            // Set the goal target's tag to "Goal" and set its color to green
            goalTarget.tag = "Goal";

            SpriteRenderer spriteRenderer = goalTarget.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.green;
            }
        }
    }

    private void CheckTargets()
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Check if Enter key is pressed
        {
            GameObject goalTarget = GameObject.FindWithTag("Goal");
            if (goalTarget != null)
            {
                Destroy(goalTarget);
                EndTrial(); // End the current trial
            }
        }
    }

    List<Vector3> GenerateRandomPoints()
    {
        List<Vector3> pointList = new();
        for (int i = 0; i < numTargets; i++)
        {
            float randomX = Random.Range(0, Screen.width);
            float randomY = Random.Range(0, Screen.height);
            float z = 10f;
            Vector3 randomScreenPoint = new(randomX, randomY, z);
            Vector3 randomWorldPoint = mainCamera.ScreenToWorldPoint(randomScreenPoint);
            pointList.Add(randomWorldPoint);
        }
        return pointList;
    }

    List<float> GenerateRandomSizes()
    {
        List<float> sizes = new();
        for (int i = 0; i < numTargets; i++)
        {
            int randomIndex = Random.Range(0, targetSizes.Count);
            sizes.Add(targetSizes[randomIndex]);
        }

        return sizes;
    }

    List<Vector3> GenerateGridPoints()
    {
        List<Vector3> pointList = new();
        
        // Calculate the horizontal and vertical spacing
        float gridWidth = Screen.width / numColumns;
        float gridHeight = Screen.height / numRows * 0.8f;

        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numColumns; col++)
            {
                if (pointList.Count >= numTargets) return pointList;

                float x = col * gridWidth + gridWidth / 2; 
                float y = row * gridHeight + gridHeight / 2 + gridOffsetY; 

                Vector3 screenPoint = new Vector3(x, y, 10f); 
                Vector3 worldPoint = mainCamera.ScreenToWorldPoint(screenPoint);
                pointList.Add(worldPoint);
            }
        }

        return pointList;
    }
}
