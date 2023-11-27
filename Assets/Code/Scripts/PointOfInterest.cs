using UnityEngine;

public class PointOfInterest : MonoBehaviour
{
    public string poiName; // The name of the point of interest
    public Transform viewTarget; // Where the player should look when this point is selected
    public int order; // Optional: Order in the timeline
    public string sceneName; // Optional: Name of the scene to load when this point is selected
}
