using UnityEngine.EventSystems;
using UnityEngine;

public class ObjectHover : MonoBehaviour
{
    public Canvas canvas; // Assign this via the inspector
    public float maxAnnotationDistance = 5f; // Maximum distance to show annotation
    private Annotation currentAnnotation;
    private Camera mainCamera;

    void Start()
    {
        // Cache the main camera for efficiency
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Check for mouse over object
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject())
        {
            // Check if the hit object is within the max annotation distance
            if (Vector3.Distance(hit.transform.position, mainCamera.transform.position) <= maxAnnotationDistance)
            {
                // Get the Annotation component from the hit object
                Annotation annotation = hit.collider.GetComponent<Annotation>();
                if (annotation != null)
                {
                    // If we hit a new object, hide the previous bubble
                    if (currentAnnotation != null && currentAnnotation != annotation)
                    {
                        currentAnnotation.HideBubble();
                    }

                    // Show the text bubble and update its position
                    annotation.ShowBubble();
                    currentAnnotation = annotation;
                }
            }
            else if (currentAnnotation != null)
            {
                // If the hit object is beyond the max distance, hide the annotation
                currentAnnotation.HideBubble();
                currentAnnotation = null;
            }
        }
        else
        {
            // If we're not hovering over anything, hide the bubble
            if (currentAnnotation != null)
            {
                currentAnnotation.HideBubble();
                currentAnnotation = null;
            }
        }

        // Check for key press to hide the bubble
        if (Input.GetKeyDown(KeyCode.H)) // Replace 'H' with the key you want to use
        {
            if (currentAnnotation != null)
            {
                currentAnnotation.HideBubble();
                currentAnnotation = null;
            }
        }
    }
}
