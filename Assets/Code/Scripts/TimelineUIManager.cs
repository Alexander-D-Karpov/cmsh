using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class TimelineUIManager : MonoBehaviour
{
    public GameObject buttonPrefab; // The prefab for the timeline buttons
    public Transform buttonContainer; // The UI container where buttons will be instantiated
    public PlayerMovement playerMovement; // Reference to the player movement script
    public RectTransform scrollContent; // The content of the scroll view
    private List<GameObject> poiButtons = new List<GameObject>(); // List of all the instantiated buttons
    private PointOfInterest[] pois;
    private Dictionary<PointOfInterest, RectTransform> poiButtonMap = new Dictionary<PointOfInterest, RectTransform>();

    void CreateButtonForPOI(PointOfInterest poi)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, buttonContainer);
        buttonObj.name = "Button_" + poi.poiName;
        buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = poi.poiName;
        Button button = buttonObj.GetComponent<Button>();
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();

        // Add listener that uses the button's RectTransform to center it
        button.onClick.AddListener(() => OnPOIButtonClicked(poi));

        // Store the button's RectTransform in the dictionary using the POI as a key
        poiButtonMap[poi] = buttonRect;
    }


    void OnPOIButtonClicked(PointOfInterest poi)
    {
        playerMovement.TeleportAndLookAt(poi.transform.position, poi.viewTarget.position);

        // Retrieve the RectTransform from the dictionary and center on it
        if (poiButtonMap.TryGetValue(poi, out RectTransform buttonRect))
        {
            StartCoroutine(CenterOnButtonCoroutine(buttonRect));
        }
    }

    IEnumerator CenterOnButtonCoroutine(RectTransform buttonRectTransform)
    {
        yield return new WaitForEndOfFrame(); // Ensure UI elements are updated

        // Check if scrolling is needed
        float totalContentWidth = scrollContent.GetComponentsInChildren<RectTransform>().Sum(rt => rt.rect.width);
        float viewportWidth = scrollContent.parent.GetComponent<RectTransform>().rect.width;
        if (totalContentWidth <= viewportWidth)
        {
            // Not enough content to require scrolling, so exit coroutine
            yield break;
        }

        // Calculate the desired position of the button
        float buttonPositionInScroll = buttonRectTransform.localPosition.x
                                       - (scrollContent.rect.width * scrollContent.pivot.x)
                                       + (buttonRectTransform.rect.width * buttonRectTransform.pivot.x);
        float contentMoveOffset = viewportWidth * 0.5f - buttonPositionInScroll;
        float newContentPosX = scrollContent.anchoredPosition.x + contentMoveOffset;

        // Clamp the position to prevent over-scrolling
        float contentBounds = Mathf.Max((scrollContent.rect.width - viewportWidth) * 0.5f, 0);
        newContentPosX = Mathf.Clamp(newContentPosX, -contentBounds, contentBounds);

        // Soften the animation
        float duration = 1.0f; // Increase duration for a slower scroll
        float timeElapsed = 0f;

        // The start and end positions for the animation
        Vector2 startPos = scrollContent.anchoredPosition;
        Vector2 endPos = new Vector2(newContentPosX, scrollContent.anchoredPosition.y);

        // Animate the movement
        while (timeElapsed < duration)
        {
            scrollContent.anchoredPosition = Vector2.Lerp(startPos, endPos, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        scrollContent.anchoredPosition = endPos;
    }

    void Start()
    {
        // Find all POIs and order them by the 'order' field
        pois = FindObjectsOfType<PointOfInterest>().OrderBy(poi => poi.order).ToArray();
        foreach (var poi in pois)
        {
            CreateButtonForPOI(poi);
        }

        // Optionally, center the first button on start
        if (pois.Length > 0)
        {
            // Use the RectTransform from the poiButtonMap, which maps POIs to their buttons
            RectTransform firstButtonRect = poiButtonMap[pois[0]];
            StartCoroutine(CenterOnButtonCoroutine(firstButtonRect));
        }
    }
}
