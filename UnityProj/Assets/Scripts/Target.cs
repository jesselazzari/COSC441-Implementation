using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private SpriteRenderer sprite;
    private bool onSelect;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        
    }

    public void OnHighlight()
    {

    }

    public void OnHoverEnter()
    {
        
        if (onSelect) return;
        // On hover set start and goal target to be a darker shade of green or red
        // On hover distractor targets are set to dark grey
        if (gameObject.tag == "Goal")sprite.color = new Color(0f, 0.392f, 0f);
        else if (gameObject.tag == "Start")sprite.color = new Color(0.545f, 0f, 0f);
        else sprite.color = new Color(0.169f, 0.169f, 0.169f); 
    }

    public void OnHoverExit()
    {
        if (onSelect) return;
        // On hover exit set targets back to their normal colors
        if (gameObject.tag == "Goal")sprite.color = Color.green;
        else if (gameObject.tag == "Start")sprite.color = Color.red;
        else sprite.color = Color.grey;
    }

    public void OnSelect()
    {
        // Check if the current object is the goal target
        if (gameObject.tag == "Goal")
        {
            // Check if the "Start" target was destroyed 
            GameObject startTarget = GameObject.FindGameObjectWithTag("Start");
            if (startTarget != null) // If Start target exists, prevent selection
            {
                Debug.Log("You need to select the red start target before selecting the green goal target.");
                return;
            }
            
            // Proceed to select the Goal target
            onSelect = true;
            sprite.color = Color.grey; // Change color to indicate selection
            StartCoroutine(DestroyGameObject(0.1f));
        }
        else if (gameObject.tag == "Start")
        {
            // Allow the selection and destruction of the Start target
            onSelect = true;
            sprite.color = Color.grey; // Change color to indicate selection
            StartCoroutine(DestroyGameObject(0.1f));
        }
    }

    public IEnumerator DestroyGameObject(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }

}
