using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetManager : MonoBehaviour
{
    private StudyBehavior studyBehavior;
    private float EWToW_Ratio; 
    private float A; 
    private float targetWidth; // Width of goal and start targets
    [SerializeField] private GameObject goalTarget;
    [SerializeField] private GameObject startTarget;
    [SerializeField] private GameObject target;
    [SerializeField] private List<float> distractorTargetSizes;
    [SerializeField] private int numTargets;

    private List<float> randomSizes;
    private List<Target> targetList = new();
    private Vector2 screenCentre;
    private Camera mainCamera;
    private GameObject currentGoalTarget; //Keep track of current goal target
    private GameObject currentStartTarget; //Keep track of current start target
    private int missedClicks;
    
    void Awake()
    {
        studyBehavior = FindObjectOfType<StudyBehavior>();
    }
    private void Start()
    {
        mainCamera = Camera.main;
        screenCentre = new Vector2(Screen.width/2, Screen.height / 2);
        
        A = studyBehavior.CurrentTrial.amplitude;
        EWToW_Ratio = studyBehavior.CurrentTrial.EWToW_Ratio;
        targetWidth = studyBehavior.CurrentTrial.targetSize;
        SpawnAllTargets();
    }

    private void Update()
    {
        // Check if the goal target has been destroyed
        if (currentGoalTarget == null)
        {
            // Clear remaning targets and respawn a new goal target and its 4 distractor targets
            studyBehavior.NextTrial();

            A = studyBehavior.CurrentTrial.amplitude;
            EWToW_Ratio = studyBehavior.CurrentTrial.EWToW_Ratio;
            targetWidth = studyBehavior.CurrentTrial.targetSize;

            ClearTargets();
            SpawnAllTargets();
        }
    }

    private void ClearTargets()
    {
        foreach (Target target in targetList)
        {
            if (target != null)
            {
                Destroy(target.gameObject);
            }
        }
        targetList.Clear(); // Clear the list after destroying targets
    }

    private void SpawnAllTargets()
    {
        // Store the positions of existing targets to avoid overlaps
        List<Vector3> existingPositions = new List<Vector3>();

        //Generate list of random sizes
        List<float> randomSizes = GenerateRandomSizes();
        
        // Spawn the red start target in middle of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 10f);
        Vector3 worldCenter = mainCamera.ScreenToWorldPoint(screenCenter);
        currentStartTarget = Instantiate(startTarget, worldCenter, Quaternion.identity, transform);
        currentStartTarget.transform.localScale = new Vector3(targetWidth, targetWidth, 1); // Set the width of the start target
        
        existingPositions.Add(worldCenter);

        // Generate a random angle in radians for the green goal target
        float randomAngle = Random.Range(0f, 2 * Mathf.PI);

        // Calculate the goal target position at distance A from the start target
        Vector3 goalTargetPosition = worldCenter + new Vector3(A * Mathf.Cos(randomAngle), A * Mathf.Sin(randomAngle), 0);

        // Add goal target to list of existing targets to avoid overlaps
        existingPositions.Add(goalTargetPosition);
    
        currentGoalTarget = Instantiate(goalTarget, goalTargetPosition, Quaternion.identity, transform);
        currentGoalTarget.transform.localScale = new Vector3(targetWidth, targetWidth, 1); // Set the width of the goal target
        
        // Get the width of the goal target
        float goalTargetWidth = currentGoalTarget.transform.localScale.x; 

        // Set distance between distractor and goal target to be given effective width/with ratio
        float distanceFromGoal = EWToW_Ratio * goalTargetWidth;

        Vector3 goalPosition = currentGoalTarget.transform.position;

        // Calculate positions for distractor targets 
        Vector3[] distractorPositions = new Vector3[4];
        distractorPositions[0] = goalPosition + new Vector3(0, distanceFromGoal, 0);
        distractorPositions[1] = goalPosition + new Vector3(distanceFromGoal, 0, 0);
        distractorPositions[2] = goalPosition + new Vector3(0, -distanceFromGoal, 0);
        distractorPositions[3] = goalPosition + new Vector3(-distanceFromGoal, 0, 0);

        foreach (var position in distractorPositions)
        {
            GameObject distractor = Instantiate(target, position, Quaternion.identity, transform);
            distractor.transform.localScale = new Vector3(targetWidth, targetWidth, 1);
            targetList.Add(distractor.GetComponent<Target>());
            existingPositions.Add(position);
        }

        // Spawn in remaining distractor targets while ensuring that they do not overlap
        for (int i = 0; i < numTargets; i++)
        {
            Vector3 point;
            bool positionValid;

            do
            {
                point = GenerateRandomPoint();
                positionValid = true;

                // Check against existing target positions to prevent overlap
                foreach (var existingPosition in existingPositions)
                {
                    if (Vector3.Distance(point, existingPosition) < distanceFromGoal)
                    {
                        positionValid = false; // Position is too close to an existing target
                        break;
                    }
                }

            } while (!positionValid); // Keep generating until a valid position is found

            // After finding a valid position add it to the existing positions list
            existingPositions.Add(point);
            
            GameObject targetObject = Instantiate(target, point, Quaternion.identity, transform);
            targetObject.transform.localScale = Vector3.one * randomSizes[i];
            targetList.Add(targetObject.GetComponent<Target>()); // Add to the target list
        }
    }

    Vector3 GenerateRandomPoint()
    {
        float randomX = Random.Range(0, Screen.width);
        float randomY = Random.Range(0, Screen.height);
        float z = 10f;
        Vector3 randomScreenPoint = new(randomX, randomY, z);
        Vector3 randomWorldPoint = mainCamera.ScreenToWorldPoint(randomScreenPoint);
        return randomWorldPoint;
    }

    List<float> GenerateRandomSizes()
    {
        List<float> sizes = new();
        for (int i = 0; i < numTargets; i++)
        {
            int randomIndex = Random.Range(0, distractorTargetSizes.Count);
            sizes.Add(distractorTargetSizes[randomIndex]);
        }

        return sizes;
    }
}
