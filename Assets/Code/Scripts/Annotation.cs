using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Annotation : MonoBehaviour
{
    [SerializeField]
    private GameObject textBubblePrefab; // The text bubble prefab

    [SerializeField]
    private string annotationText; // The text to display, serialized to appear in the Inspector

    public string AnnotationText
    {
        get { return annotationText; }
        set
        {
            annotationText = value;
            if (textComponent != null)
                textComponent.text = annotationText;
        }
    }

    // Rest of your variables...

    private TextMeshProUGUI textComponent; // Reference to the TextMeshPro component
    private CanvasGroup canvasGroup;
    private GameObject textBubbleInstance;
    private bool isBubbleActive = false;
    private bool isCooldown = false; // Cooldown flag
    public float hideCooldown = 2f; // Cooldown period in seconds

    // Rest of your methods...

    void Awake()
    {
        Canvas canvas = FindObjectOfType<Canvas>(); // Find the canvas in the scene
        if (canvas != null)
        {
            // Instantiate the text bubble prefab as a child of the canvas
            textBubbleInstance = Instantiate(textBubblePrefab, canvas.transform, false);
            textBubbleInstance.SetActive(false); // Start with the bubble hidden

            // Get the CanvasGroup component, add one if it doesn't exist
            canvasGroup = textBubbleInstance.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = textBubbleInstance.AddComponent<CanvasGroup>();
            }

            // Get the TextMeshProUGUI component and initialize the text
            textComponent = textBubbleInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = annotationText; // Set the initial text
            }
        }
        else
        {
            Debug.LogError("No Canvas found in the scene for the annotation bubbles.");
        }
    }

    public void UpdateAnnotationText(string newText)
    {
        if (textComponent != null)
        {
            textComponent.text = newText;
        }
    }

    public void ShowBubble()
    {
        if (!isBubbleActive && !isCooldown)
        {
            textBubbleInstance.SetActive(true);
            StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 1));
            isBubbleActive = true;
        }
    }

    public void HideBubble()
    {
        if (isBubbleActive)
        {
            StartCoroutine(FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0, () => {
                textBubbleInstance.SetActive(false);
                isBubbleActive = false;
                StartCoroutine(Cooldown());
            }));
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end, System.Action onComplete = null, float lerpTime = 0.5f)
    {
        float timeStartedLerping = Time.time;
        float timeSinceStarted = Time.time - timeStartedLerping;
        float percentageComplete = timeSinceStarted / lerpTime;

        while (true)
        {
            timeSinceStarted = Time.time - timeStartedLerping;
            percentageComplete = timeSinceStarted / lerpTime;

            float currentValue = Mathf.Lerp(start, end, percentageComplete);

            cg.alpha = currentValue;

            if (percentageComplete >= 1) break;

            yield return new WaitForEndOfFrame();
        }

        onComplete?.Invoke();
    }

    private IEnumerator Cooldown()
    {
        isCooldown = true;
        yield return new WaitForSeconds(hideCooldown);
        isCooldown = false;
    }
    public void SetText(string text)
    {
        AnnotationText = text;
    }

    public void SetColor(Color color)
    {
        if (textComponent != null)
            textComponent.color = color;
    }

    public void SetFontSize(float fontSize)
    {
        if (textComponent != null)
            textComponent.fontSize = fontSize;
    }
}
