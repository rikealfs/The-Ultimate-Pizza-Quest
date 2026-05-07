using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DoughKneadingMinigame : MonoBehaviour
{
    public enum MinigamePhase
    {
        Kneading,
        Rolling,
        Sauce,
        Complete
    }

    [Header("Phase Panels")]
    public GameObject kneadingPanel;
    public GameObject rollingPanel;

    // Enum to represent swipe directions
    public enum SwipeDirection
    {
        Up, Down, Left, Right,
        UpLeft, UpRight, DownLeft, DownRight,
        Clockwise, CounterClockwise
    }

    // references to UI elements
    [Header("UI")]
    public Image arrowImage;
    public Slider progressBar;
    public TMP_Text feedbackText;
    public RectTransform doughImage;
    public TMP_Text instructionsText;

    [Header("Sprites")]
    public Sprite arrowSprite;
    public Sprite circleSprite;

    [Header("Settings")]
    // Number of correct swipes needed to complete the minigame
    public int swipesToComplete = 8;
    // Minimum distance for a swipe to be registered
    public float minSwipeDistance = 50f;
    // Minimum distance from the dough center before circle tracking starts
    public float minCircleRadius = 60f;
    // Minimum accumulated angle needed to count as a circle swipe
    public float circleAngleThreshold = 220f;

    private Vector2 swipeStart;
    private bool swiping = false;
    private int correctSwipes = 0;
    private SwipeDirection currentDirection;

    private float previousAngle;
    private float accumulatedAngle = 0f;
    private bool trackingCircle = false;

    private bool minigameComplete = false;

    [Header("Dough Animation")]
    public float pulseScale = 0.9f;
    public float returnSpeed = 4f;
    public float spinAmount = 21f;

    private Quaternion doughTargetRotation;
    private Vector3 doughTargetScale;
    private Vector3 doughNormalScale;

    [Header("Rolling UI")]
    public RectTransform rolledDoughImage;
    public RectTransform targetRectangle;
    public RectTransform rollingPinImage;
    public RectTransform crossArrowImage;

    [Header("Rolling Settings")]
    public float phaseDelay = 1.5f;
    public int swipesPerStretch = 3;
    public int goalHorizontalSteps = 4;
    public int goalVerticalSteps = 4;
    public int maxExtraSteps = 2;
    public float stretchAmount = 60f;

    private MinigamePhase currentPhase = MinigamePhase.Kneading;

    public int horizontalSwipeCount = 0;
    public int verticalSwipeCount = 0;

    private int horizontalSteps = 0;
    private int verticalSteps = 0;

    private bool rollingTouchActive = false;
    private Vector2 rollingLastPosition;
    private string lockedRollDirection = "";
    private float phaseTimer = 0f;
    private bool waitingForRollingPhase = false;

    private Vector2 rolledDoughStartSize;

    private Vector2 rollingPinStartPos;
    public float rollingPinMoveRange = 150f;

    [Header("Sauce UI")]
    public RectTransform sauceLayer;
    public RectTransform sauceMask;
    public GameObject sauceBrushPrefab;

    [Header("Sauce Settings")]
    public int sauceStampsNeeded = 40;
    public float sauceBrushSpacing = 30f;

    private int sauceStampCount = 0;
    private Vector2 lastSaucePosition;
    private bool hasLastSaucePosition = false;

    public float saucePhaseDelay = 1.5f;

    private bool waitingForSaucePhase = false;
    private float saucePhaseTimer = 0f;

    void Start()
    {
        // Initialize progress bar and feedback
        progressBar.maxValue = swipesToComplete;
        progressBar.value = 0;
        feedbackText.text = "";
        //feedbackText.text = "Starting";
        // Set the first arrow direction
        SetNextArrow();
        doughNormalScale = doughImage.localScale;
        doughTargetScale = doughNormalScale;
        doughTargetRotation = doughImage.rotation;
        kneadingPanel.SetActive(true);
        rollingPanel.SetActive(false);
        rollingPinImage.gameObject.SetActive(false);
        rolledDoughStartSize = rolledDoughImage.sizeDelta;
        rollingPinStartPos = rollingPinImage.anchoredPosition;
        sauceMask.gameObject.SetActive(false);
        sauceLayer.gameObject.SetActive(false);
    }

    void Update()
    {
        if (waitingForRollingPhase)
        {
            phaseTimer -= Time.deltaTime;

            if (phaseTimer <= 0f)
            {
                StartRollingPhase();
            }

            return;
        }

        if (waitingForSaucePhase)
        {
            saucePhaseTimer -= Time.deltaTime;

            if (saucePhaseTimer <= 0f)
            {
                StartSaucePhase();
            }

            return;
        }

        if (currentPhase == MinigamePhase.Kneading)
        {
            HandleTouchSwipe();
        }
        else if (currentPhase == MinigamePhase.Rolling)
        {
            HandleRollingInput();
        }
        else if (currentPhase == MinigamePhase.Sauce)
        {
            HandleSauceInput();
        }

            AnimateDough();
    }

    // Handle touch input for swipe detection
    void HandleTouchSwipe()
    {
        //feedbackText.text = "Touches: " + Input.touchCount + "\n";
        // Only process if there's at least one touch
        if (Input.touchCount == 0) return;

        // If the minigame is already complete, ignore input
        if (minigameComplete) return;

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
            accumulatedAngle = 0f;
            BeginCircleTracking(touch.position);
            //feedbackText.text += "Swipe START: " + swipeStart + "\n";
        }
        else if (touch.phase == TouchPhase.Moved && swiping)
        {
            UpdateCircleTracking(touch.position);
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

            // Check if the current prompt is a circle swipe
            if (currentDirection == SwipeDirection.Clockwise || currentDirection == SwipeDirection.CounterClockwise)
            {
                CheckCircleSwipe();
            }
            else
            {
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

            trackingCircle = false;
            accumulatedAngle = 0f;
        }
    }

    // Optional: Handle mouse input for testing in the editor
    void HandleMouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            swipeStart = Input.mousePosition;
            swiping = true;
            accumulatedAngle = 0f;
            BeginCircleTracking(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && swiping)
        {
            UpdateCircleTracking(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0) && swiping)
        {
            Vector2 swipeEnd = Input.mousePosition;
            Vector2 swipe = swipeEnd - swipeStart;
            swiping = false;

            if (currentDirection == SwipeDirection.Clockwise || currentDirection == SwipeDirection.CounterClockwise)
            {
                CheckCircleSwipe();
            }
            else
            {
                if (swipe.magnitude >= minSwipeDistance)
                {
                    SwipeDirection detected = GetSwipeDirection(swipe);
                    CheckSwipe(detected);
                }
            }

            trackingCircle = false;
            accumulatedAngle = 0f;
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

    // Begin tracking the circle swipe around the dough center
    void BeginCircleTracking(Vector2 screenPosition)
    {
        if (doughImage == null)
        {
            trackingCircle = false;
            return;
        }

        Vector2 center = RectTransformUtility.WorldToScreenPoint(null, doughImage.position);
        Vector2 fromCenter = screenPosition - center;

        if (fromCenter.magnitude >= minCircleRadius)
        {
            previousAngle = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
            trackingCircle = true;
        }
        else
        {
            trackingCircle = false;
        }
    }

    // Update the current circle swipe angle
    void UpdateCircleTracking(Vector2 screenPosition)
    {
        if (!trackingCircle) return;

        Vector2 center = RectTransformUtility.WorldToScreenPoint(null, doughImage.position);
        Vector2 fromCenter = screenPosition - center;

        if (fromCenter.magnitude < minCircleRadius) return;

        float currentAngleValue = Mathf.Atan2(fromCenter.y, fromCenter.x) * Mathf.Rad2Deg;
        float deltaAngle = Mathf.DeltaAngle(previousAngle, currentAngleValue);

        accumulatedAngle += deltaAngle;
        previousAngle = currentAngleValue;
    }

    // Check if the circle swipe direction matches the current circle direction
    void CheckCircleSwipe()
    {
        if (currentDirection == SwipeDirection.CounterClockwise && accumulatedAngle >= circleAngleThreshold)
        {
            CheckSwipe(SwipeDirection.CounterClockwise);
        }
        else if (currentDirection == SwipeDirection.Clockwise && accumulatedAngle <= -circleAngleThreshold)
        {
            CheckSwipe(SwipeDirection.Clockwise);
        }
        else
        {
            // Provide negative feedback for an incorrect swipe
            feedbackText.text = "Wrong direction!";
        }
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
            doughTargetScale = doughNormalScale * pulseScale;
            doughTargetRotation *= Quaternion.Euler(0f, 0f, spinAmount);
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
        currentDirection = (SwipeDirection)Random.Range(0, 10);

        float angle = 0f;

        // Reset rotation and scale before changing the prompt image
        arrowImage.rectTransform.rotation = Quaternion.identity;
        arrowImage.rectTransform.localScale = Vector3.one;

        // Rotate the arrow image based on the current direction
        switch (currentDirection)
        {
            case SwipeDirection.Up:
                arrowImage.sprite = arrowSprite;
                angle = 0f;
                break;
            case SwipeDirection.Down:
                arrowImage.sprite = arrowSprite;
                angle = 180f;
                break;
            case SwipeDirection.Left:
                arrowImage.sprite = arrowSprite;
                angle = 90f;
                break;
            case SwipeDirection.Right:
                arrowImage.sprite = arrowSprite;
                angle = -90f;
                break;
            case SwipeDirection.UpRight:
                arrowImage.sprite = arrowSprite;
                angle = -45f;
                break;
            case SwipeDirection.UpLeft:
                arrowImage.sprite = arrowSprite;
                angle = 45f;
                break;
            case SwipeDirection.DownRight:
                arrowImage.sprite = arrowSprite;
                angle = -135f;
                break;
            case SwipeDirection.DownLeft:
                arrowImage.sprite = arrowSprite;
                angle = 135f;
                break;
            case SwipeDirection.Clockwise:
                arrowImage.sprite = circleSprite;
                arrowImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
                angle = 0f;
                break;
            case SwipeDirection.CounterClockwise:
                arrowImage.sprite = circleSprite;
                arrowImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
                angle = 0f;
                break;
        }
        arrowImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
        //feedbackText.text += "\nNew Direction: " + currentDirection + "\n";
    }


    void CompleteMinigame()
    {
        minigameComplete = true;
        swiping = false;
        trackingCircle = false;

        feedbackText.text = "Dough complete!";
        arrowImage.gameObject.SetActive(false);
        waitingForRollingPhase = true;
        phaseTimer = phaseDelay;
        // Todo: transition to next phase
    }

    void AnimateDough()
    {
        if (doughImage == null) return;

        doughImage.localScale = Vector3.Lerp(
            doughImage.localScale,
            doughTargetScale,
            Time.deltaTime * returnSpeed
        );

        doughImage.rotation = Quaternion.Lerp(
            doughImage.rotation,
            doughTargetRotation,
            Time.deltaTime * returnSpeed
        );

        doughTargetScale = Vector3.Lerp(
            doughTargetScale,
            doughNormalScale,
            Time.deltaTime * returnSpeed
        );
    }

    void StartRollingPhase()
    {
        waitingForRollingPhase = false;
        currentPhase = MinigamePhase.Rolling;

        kneadingPanel.SetActive(false);
        rollingPanel.SetActive(true);

        feedbackText.text = "";

        instructionsText.text = "Roll out the dough!";
        crossArrowImage.gameObject.SetActive(true);
    }

    void HandleRollingInput()
    {
        if (Input.touchCount == 0)
        {
            if (rollingTouchActive)
            {
                rollingTouchActive = false;
                lockedRollDirection = "";
                rollingPinImage.gameObject.SetActive(false);
                if (currentPhase == MinigamePhase.Rolling)
                {
                    crossArrowImage.gameObject.SetActive(true);
                }
                rollingPinImage.anchoredPosition = rollingPinStartPos;

                CheckRollingComplete();
            }

            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            rollingTouchActive = true;
            rollingLastPosition = touch.position;
            lockedRollDirection = "";
            rollingPinImage.gameObject.SetActive(true);
            crossArrowImage.gameObject.SetActive(false);
        }
        else if (touch.phase == TouchPhase.Moved && rollingTouchActive)
        {
            Vector2 movement = touch.position - rollingLastPosition;

            if (movement.magnitude < minSwipeDistance) return;

            string movementDirection;

            if (Mathf.Abs(movement.x) > Mathf.Abs(movement.y))
            {
                movementDirection = "Horizontal";
            }
            else
            {
                movementDirection = "Vertical";
            }

            if (lockedRollDirection == "")
            {
                lockedRollDirection = movementDirection;
            }

            if (movementDirection == lockedRollDirection)
            {
                RegisterRollingSwipe(movementDirection);
                rollingLastPosition = touch.position;
            }
            UpdateRollingPinPosition(touch.position);
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            rollingTouchActive = false;
            lockedRollDirection = "";
            rollingPinImage.gameObject.SetActive(false);
            rollingPinImage.anchoredPosition = rollingPinStartPos;
            if (currentPhase == MinigamePhase.Rolling)
            {
                crossArrowImage.gameObject.SetActive(true);
            }
                CheckRollingComplete();
        }
    }

    void RegisterRollingSwipe(string direction)
    {
        if (direction == "Horizontal")
        {
            horizontalSwipeCount++;

            rollingPinImage.rotation = Quaternion.Euler(0f, 0f, 90f);

            if (horizontalSwipeCount >= swipesPerStretch)
            {
                horizontalSwipeCount = 0;

                if (horizontalSteps < goalHorizontalSteps + maxExtraSteps)
                {
                    horizontalSteps++;
                    StretchDoughHorizontal();
                }
            }
        }
        else if (direction == "Vertical")
        {
            verticalSwipeCount++;

            rollingPinImage.rotation = Quaternion.Euler(0f, 0f, 0f);

            if (verticalSwipeCount >= swipesPerStretch)
            {
                verticalSwipeCount = 0;

                if (verticalSteps < goalVerticalSteps + maxExtraSteps)
                {
                    verticalSteps++;
                    StretchDoughVertical();
                }
            }
        }
    }
    void StretchDoughHorizontal()
    {
        Vector2 size = rolledDoughImage.sizeDelta;
        size.x += stretchAmount;
        rolledDoughImage.sizeDelta = size;
    }

    void StretchDoughVertical()
    {
        Vector2 size = rolledDoughImage.sizeDelta;
        size.y += stretchAmount;
        rolledDoughImage.sizeDelta = size;
    }

    void CheckRollingComplete()
    {
        bool wideEnough = horizontalSteps >= goalHorizontalSteps;
        bool tallEnough = verticalSteps >= goalVerticalSteps;

        if (!wideEnough || !tallEnough) return;


        bool perfect =
            horizontalSteps == goalHorizontalSteps &&
            verticalSteps == goalVerticalSteps;

        bool even =
            horizontalSteps == verticalSteps;

        if (perfect)
        {
            feedbackText.text = "Perfect!";
        }
        else if (even)
        {
            feedbackText.text = "Not bad!";
        }
        else
        {
            feedbackText.text = "Oh no! Wonky!";
        }

        rollingPinImage.gameObject.SetActive(false);
        crossArrowImage.gameObject.SetActive(false);

        waitingForSaucePhase = true;
        saucePhaseTimer = saucePhaseDelay;
        instructionsText.text = "";

    }

    void UpdateRollingPinPosition(Vector2 touchPosition)
    {
        Vector2 center = RectTransformUtility.WorldToScreenPoint(null, rolledDoughImage.position);
        Vector2 offset = touchPosition - center;

        if (lockedRollDirection == "Horizontal")
        {
            float clampedX = Mathf.Clamp(offset.x, -rollingPinMoveRange, rollingPinMoveRange);
            rollingPinImage.anchoredPosition = new Vector2(rollingPinStartPos.x + clampedX, rollingPinStartPos.y);
        }
        else if (lockedRollDirection == "Vertical")
        {
            float clampedY = Mathf.Clamp(offset.y, -rollingPinMoveRange, rollingPinMoveRange);
            rollingPinImage.anchoredPosition = new Vector2(rollingPinStartPos.x, rollingPinStartPos.y + clampedY);
        }

    }
    void StartSaucePhase()
    {
        waitingForSaucePhase = false;
        currentPhase = MinigamePhase.Sauce;

        feedbackText.text = "";
        instructionsText.text = "Spread the sauce over the pizza!";

        sauceMask.gameObject.SetActive(true);
        sauceLayer.gameObject.SetActive(true);

        sauceStampCount = 0;
        hasLastSaucePosition = false;
    }

    void HandleSauceInput()
    {
        if (Input.touchCount == 0)
        {
            hasLastSaucePosition = false;
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            TryPlaceSauce(touch.position);
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            TryPlaceSauce(touch.position);
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            hasLastSaucePosition = false;
        }
    }

    void TryPlaceSauce(Vector2 screenPosition)
    {
        Vector2 localPoint;

        bool inside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            sauceMask,
            screenPosition,
            null,
            out localPoint
        );

        if (!inside) return;

        if (!sauceMask.rect.Contains(localPoint)) return;

        if (hasLastSaucePosition)
        {
            float distance = Vector2.Distance(localPoint, lastSaucePosition);

            if (distance < sauceBrushSpacing)
            {
                return;
            }
        }

        GameObject sauce = Instantiate(sauceBrushPrefab, sauceLayer);
        RectTransform sauceRect = sauce.GetComponent<RectTransform>();
        sauceRect.anchoredPosition = localPoint;

        lastSaucePosition = localPoint;
        hasLastSaucePosition = true;

        sauceStampCount++;

        if (sauceStampCount >= sauceStampsNeeded)
        {
            CompleteSaucePhase();
        }
    }

    void CompleteSaucePhase()
    {
        currentPhase = MinigamePhase.Complete;

        instructionsText.text = "";
        feedbackText.text = "Sauce complete!";

        Debug.Log("Sauce phase complete.");
    }
}