using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoughKneadingMinigame : MonoBehaviour
{
    // Enum to represent swipe directions
    public enum SwipeDirection
    {
        Up, Down, Left, Right,
        UpLeft, UpRight, DownLeft, DownRight
    }

    // references to UI elements
    [Header("UI")]
    public Image arrowImage;
    public Slider progressBar;
    public TMP_Text feedbackText;

    [Header("Settings")]
    // Number of correct swipes needed to complete the minigame
    public int swipesToComplete = 8;
    // Minimum distance for a swipe to be registered
    public float minSwipeDistance = 50f;

    private Vector2 swipeStart;
    private bool swiping = false;
    private int correctSwipes = 0;
    private SwipeDirection currentDirection;

    void Start()
    {
        // Initialize progress bar and feedback
        progressBar.maxValue = swipesToComplete;
        progressBar.value = 0;
        feedbackText.text = "";
        //feedbackText.text = "Starting";
        // Set the first arrow direction
        SetNextArrow();
    }

    void Update()
    {
        //HandleMouseSwipe();
        HandleTouchSwipe();
    }

    // Handle touch input for swipe detection
    void HandleTouchSwipe()
    {
        //feedbackText.text = "Touches: " + Input.touchCount + "\n";
        // Only process if there's at least one touch
        if (Input.touchCount == 0) return;

        // Get the first touch
        Touch touch = Input.GetTouch(0);
        //feedbackText.text += "Phase: " + touch.phase + "\n";
        //feedbackText.text += "Pos: " + touch.position + "\n";

        // Handle touch phases
        if (touch.phase == TouchPhase.Began)
        {
            // Start tracking the swipe
            swipeStart = touch.position;
            swiping = true;
            //feedbackText.text += "Swipe START: " + swipeStart + "\n";
        }
        // Handle the end of the swipe
        else if (touch.phase == TouchPhase.Ended && swiping)
        {
            // Calculate the swipe vector
            Vector2 swipeEnd = touch.position;
            Vector2 swipe = swipeEnd - swipeStart;
            swiping = false;
            //feedbackText.text += "Swipe END: " + swipeEnd + "\n";
            //feedbackText.text += "Swipe Vector: " + swipe + "\n";
            //feedbackText.text += "Distance: " + swipe.magnitude + "\n";
            // Check if the swipe is long enough to be considered valid
            if (swipe.magnitude >= minSwipeDistance)
            {
                // Determine the swipe direction
                SwipeDirection detected = GetSwipeDirection(swipe);
                //feedbackText.text += "Detected: " + detected + "\n";
                //feedbackText.text += "Expected: " + currentDirection + "\n";
                // Check if the detected swipe matches the current arrow direction
                CheckSwipe(detected);
            }
        }
    }

    // Optional: Handle mouse input for testing in the editor
    void HandleMouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
            swiping = true;
        }
        else if (Input.GetMouseButtonUp(0) && swiping)
        {
            Vector2 swipeEnd = Input.mousePosition;
            Vector2 swipe = swipeEnd - swipeStart;
            swiping = false;

            if (swipe.magnitude >= minSwipeDistance)
            {
                SwipeDirection detected = GetSwipeDirection(swipe);
                CheckSwipe(detected);
            }
        }
    }

    // Convert a swipe vector into a swipe direction enum
    SwipeDirection GetSwipeDirection(Vector2 swipe)
    {
        // Calculate the angle of the swipe in degrees
        float angle = Mathf.Atan2(swipe.y, swipe.x) * Mathf.Rad2Deg;
        // Normalize the angle to be between 0 and 360
        if (angle < 0) angle += 360f;
        if (angle > 360) angle -= 360f;
        // Determine the swipe direction based on the angle
        if (angle >= 337.5f || angle < 22.5f) return SwipeDirection.Right;
        if (angle >= 22.5f && angle < 67.5f) return SwipeDirection.UpRight;
        if (angle >= 67.5f && angle < 112.5f) return SwipeDirection.Up;
        if (angle >= 112.5f && angle < 157.5f) return SwipeDirection.UpLeft;
        if (angle >= 157.5f && angle < 202.5f) return SwipeDirection.Left;
        if (angle >= 202.5f && angle < 247.5f) return SwipeDirection.DownLeft;
        if (angle >= 247.5f && angle < 292.5f) return SwipeDirection.Down;
        return SwipeDirection.DownRight;
    }

    // Check if the detected swipe direction matches the current arrow direction
    void CheckSwipe(SwipeDirection detected)
    {
        // If the swipe is correct, increment the progress and check for completion
        if (detected == currentDirection)
        {
            correctSwipes++;
            progressBar.value = correctSwipes;
            // Provide positive feedback to the player
            feedbackText.text = "Good!";
            // Check if the minigame is complete
            if (correctSwipes >= swipesToComplete)
            {
                CompleteMinigame();
            }
            else
            {
                // Set the next arrow direction for the player to follow
                SetNextArrow();
            }
        }
        else
        {
            // Provide negative feedback for an incorrect swipe
            feedbackText.text = "Wrong direction!";
        }
    }

    // Set the arrow image to point in the next random direction
    void SetNextArrow()
    {
        //feedbackText.text = "Setting next direction...";
        // Randomly select the next swipe direction
        currentDirection = (SwipeDirection)Random.Range(0, 8);

        float angle = 0f;

        // Rotate the arrow image based on the current direction
        switch (currentDirection)
        {
            case SwipeDirection.Up:
                angle = 0f;
                break;
            case SwipeDirection.Down:
                angle = 180f;
                break;
            case SwipeDirection.Left:
                angle = 90f;
                break;
            case SwipeDirection.Right:
                angle = -90f;
                break;
            case SwipeDirection.UpRight:
                angle = -45f;
                break;
            case SwipeDirection.UpLeft:
                angle = 45f;
                break;
            case SwipeDirection.DownRight:
                angle = -135f;
                break;
            case SwipeDirection.DownLeft:
                angle = 135f;
                break;
        }
        arrowImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        //feedbackText.text += "\nNew Direction: " + currentDirection + "\n";
    }


    void CompleteMinigame()
    {
        feedbackText.text = "Dough complete!";
        Debug.Log("Minigame finished.");
        arrowImage.gameObject.SetActive(false);
        // Todo: transition to next phase
    }
}