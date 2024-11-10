using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTargetManager : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private List<float> targetSizes;
    [SerializeField] private List<float> targetAmplitudes;
    [SerializeField] private int numTargets;
    [SerializeField] private int numRows = 5;  
    [SerializeField] private int numColumns = 5;  
    [SerializeField] private float gridOffsetY = 100f;
    private List<float> randomSizes;
    private List<Target> targetList = new();
    private Vector2 screenCentre;
    private Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
        screenCentre = new Vector2(Screen.width/2, Screen.height / 2);
        SpawnTargets();

    }

    private void SpawnTargets()
    {
        List<Vector3> points = GenerateGridPoints();
        List<float> randomSizes = GenerateRandomSizes();
        for (int i = 0; i < numTargets; i++)
        {
            GameObject targetObject = Instantiate(target, points[i], Quaternion.identity, transform);
            targetObject.transform.localScale = Vector3.one * randomSizes[i];
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
        float gridHeight = Screen.height / numRows;

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