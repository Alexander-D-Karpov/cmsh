using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

[System.Serializable]
public class TimePoint
{
    public string pointName; // Name of the point
    public float timeValue; // Value on the time tape this point corresponds to
    public Vector3 position; // The position in the scene the camera should move to
}

public class TimeTapeController : MonoBehaviour
{
    public Slider timeTapeSlider;
    public RectTransform pointsContainer; // A container for the points on the scrollbar
    public GameObject pointPrefab; // Prefab for the point UI
    public List<TimePoint> timePoints;
    public Transform player; // Reference to the player transform

    void Start()
    {
        int totalPoints = timePoints.Count;
        for (int i = 0; i < totalPoints; i++)
        {
            AddPointToUI(timePoints[i], i, totalPoints);
        }
    }

    private void AddPointToUI(TimePoint timePoint, int index, int totalPoints)
    {
        GameObject pointUI = Instantiate(pointPrefab, pointsContainer);
        RectTransform rectTransform = pointUI.GetComponent<RectTransform>();
        TextMeshProUGUI textComponent = pointUI.GetComponentInChildren<TextMeshProUGUI>();

        pointUI.name = timePoint.pointName;

        float margin = 10f; // The space between buttons
        float containerWidth = pointsContainer.rect.width;
        float totalMargins = margin * (totalPoints - 1);
        float buttonWidth = (containerWidth - totalMargins) / totalPoints;

        rectTransform.sizeDelta = new Vector2(buttonWidth, 30); // Replace 30 with your desired height

        // Map the timeValue from -1 to 1 range to the container's width
        float normalizedPosition = (timePoint.timeValue + 1) / 2; // This maps -1 to 0 and 1 to 1
        float buttonSpacing = buttonWidth + margin;
        float containerStartX = pointsContainer.rect.xMin + rectTransform.sizeDelta.x / 2;
        float positionX = containerStartX + (normalizedPosition * (containerWidth - rectTransform.sizeDelta.x));

        // Position the button with margins
        rectTransform.anchoredPosition = new Vector2(positionX + (buttonSpacing * index) - (margin * index), rectTransform.anchoredPosition.y);
        rectTransform.localScale = Vector3.one;

        if (textComponent != null)
        {
            textComponent.fontSize = rectTransform.sizeDelta.y * 0.5f; // Set font size to half of button height
        }
        else
        {
            Debug.LogError("No TextMeshProUGUI component found on pointPrefab!");
        }

        // Add click event listener
        Button pointButton = pointUI.GetComponent<Button>();
        if (pointButton != null)
        {
            pointButton.onClick.AddListener(() => MovePlayerToPoint(timePoint.position));
        }
        else
        {
            Debug.LogError("No Button component found on pointPrefab!");
        }
    }

    private void MovePlayerToPoint(Vector3 position)
    {
        // Logic to move the player to the position
        // This could simply set the player's position, or you might implement smooth movement or teleportation
        player.position = position;
    }
}
