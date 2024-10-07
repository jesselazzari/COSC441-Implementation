using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class BubbleCursor : MonoBehaviour
{
    public int missedClicks = 0;
    [SerializeField] public float radius;
    [SerializeField] private ContactFilter2D contactFilter;
    [SerializeField] private GameObject cursorSprite;

    private Camera mainCam;
    private List<Collider2D> results = new();
    private Collider2D previousDetectedCollider = new();

    private void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        // Calculating the radius
        // Calculates ConDi, IntDj 
        (float containmentDistance, float intersectDistance) = CalculateContainmentAndIntersectDistances();

        // Radius is set to the minimum of these two distances
        // Unfortunately I did not have time to fix the morph bubble edge case where the cursor should overlap the closest target (when ConDi > IntDj)
        radius = Mathf.Min(containmentDistance, intersectDistance);

        //Adjust cursor sprite to be the radius
        float diameter = radius * 2;
        cursorSprite.transform.localScale = new Vector3(diameter, diameter, 1f);
        
        Collider2D detectedCollider = null;

        Physics2D.OverlapCircle(transform.position, radius, contactFilter, results);

        //Get Mouse Position on screen, and get the corresponding position in a Vector3 World Co-Ordinate
        Vector3 mousePosition = Input.mousePosition;

        //Change the z position so that cursor does not get occluded by the camera
        mousePosition.z += 9f;
        mousePosition.x = Mathf.Clamp(mousePosition.x, 0f, Screen.width);
        mousePosition.y = Mathf.Clamp(mousePosition.y, 0f, Screen.height);
        transform.position = mainCam.ScreenToWorldPoint(mousePosition);

        //Detect how many targets
        //Change previous target back to default colour
        if (results.Count < 1)
        {
            UnHoverPreviousTarget();
            return;
        }
        else if (results.Count > 1)
        {
            UnHoverPreviousTarget();
            return;
        }
        else
        {
            detectedCollider = results[0];
            UnHoverPreviousTarget(detectedCollider);
            HoverTarget(detectedCollider);
        }

        //On Mouse Click, select the closest target
        if (Input.GetMouseButtonDown(0))
        {
            SelectTarget(detectedCollider);
            if(detectedCollider.gameObject.tag != "Goal" && detectedCollider.gameObject.tag != "Start")
                {
                    missedClicks++;
                }
        }

        previousDetectedCollider = detectedCollider;
    }
    
    private (float containmentDistance, float intersectDistance) CalculateContainmentAndIntersectDistances()
    {
        // Get all colliders in the game area
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        List<Collider2D> targetColliders = new List<Collider2D>();

        foreach (Collider2D collider in allColliders)
        {
            targetColliders.Add(collider);
        }

        // Sort colliders based on their distance to the cursor
        targetColliders.Sort((a, b) => Vector2.Distance(transform.position, a.transform.position).CompareTo(Vector2.Distance(transform.position, b.transform.position)));

        float containmentDistance = 0f;
        float intersectDistance = 0f;

        if (targetColliders.Count > 0)
        {
            // Closest target
            Collider2D closestTarget = targetColliders[0];
            float distanceToClosestTarget = Vector2.Distance(transform.position, closestTarget.transform.position);
            float closestTargetRadius = closestTarget.bounds.extents.magnitude; 
            containmentDistance = distanceToClosestTarget + closestTargetRadius; 

            // Second closest target (if it exists)
            if (targetColliders.Count > 1)
            {
                Collider2D secondClosestTarget = targetColliders[1];
                float distanceToSecondClosestTarget = Vector2.Distance(transform.position, secondClosestTarget.transform.position);
                float secondClosestTargetRadius = secondClosestTarget.bounds.extents.magnitude; 
                intersectDistance = distanceToSecondClosestTarget - secondClosestTargetRadius;
            }
        }

        return (containmentDistance, intersectDistance);
    }

    private void HoverTarget(Collider2D collider)
    {
        if (collider.TryGetComponent(out Target target))
        {
            target.OnHoverEnter();
        }
        else
        {
            Debug.LogWarning("Not a valid Target?");
        }
    }

    private void UnHoverPreviousTarget()
    {
         if (previousDetectedCollider != null)
        {
            if (previousDetectedCollider.TryGetComponent(out Target t))
            {
                t.OnHoverExit();
            }
        }
    }

    private void UnHoverPreviousTarget(Collider2D collider)
    {
        //Checking if the target detected in previous and current frame are the same
        //If target changes, change the previous target back to default colour
         if (previousDetectedCollider != null &&  collider != previousDetectedCollider)
        {
            if (previousDetectedCollider.TryGetComponent(out Target t))
            {
                t.OnHoverExit();
            }
        }
    }

    void SelectTarget(Collider2D collider)
    {
        if (collider.TryGetComponent(out Target target))
        {
            target.OnSelect();
        }
    }

    //Debug code
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}

