using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI recipeText;

    public TextMeshProUGUI warningText;

    [Header("Recipe UI")]
    public Image mushroomIcon;
    public TextMeshProUGUI mushroomText;

    public Image pepperoniIcon;
    public TextMeshProUGUI pepperoniText;

    public Image pepperIcon;
    public TextMeshProUGUI pepperText;

    public Image flashImage;

    [Header("Splatter Effect")]
    public GameObject splatPrefab;
    public Sprite[] bombSplats; 
    public Sprite[] pineappleSplats;
    public int splatCount = 6;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip splatSound;
    public AudioClip chaosSound1;
    public AudioClip chaosSound2;
    public AudioClip correctSound;

    [Header("Camera Shake")]
    public Transform cameraTransform;
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.15f;

    [Header("End Screen")]
    public GameObject endPanel;
    public TextMeshProUGUI endText;

    Canvas canvas;
    RectTransform canvasRect;

    void Start()
    {
        canvas = FindFirstObjectByType<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();

        UpdateAllIngredients(); 
    }

    void Update()
{
    var gm = CollectGameManager.Instance;
    if (gm == null) return;

    timerText.text = "Time: " + Mathf.Ceil(gm.timeLeft);

    UpdateIngredientUI(ToppingType.Mushroom, mushroomText);
    UpdateIngredientUI(ToppingType.Pepperoni, pepperoniText);
    UpdateIngredientUI(ToppingType.Pepper, pepperText);
}

void UpdateAllIngredients()
{
    UpdateIngredientUI(ToppingType.Mushroom, mushroomText);
    UpdateIngredientUI(ToppingType.Pepperoni, pepperoniText);
    UpdateIngredientUI(ToppingType.Pepper, pepperText);
}

void UpdateIngredientUI(ToppingType type, TextMeshProUGUI text)
{
    int required = CollectGameManager.Instance.GetRequired(type);
    int collected = CollectGameManager.Instance.GetCollected(type);

    int remaining = Mathf.Max(0, required - collected);

    text.text = "x" + remaining;

    if (remaining > 0)
    {
        text.transform.localScale = Vector3.one * 1.2f;
    }
    else
    {
        text.transform.localScale = Vector3.one;
    }


    if (remaining == 0)
    {
        text.color = Color.gray;
    }
}

    public void PlayCorrectSound()
{
    if (audioSource != null && correctSound != null)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f); // slight variation
        audioSource.PlayOneShot(correctSound);
    }
}

    public void ShowWarning(string message)
    {
        warningText.transform.SetAsLastSibling();
        warningText.transform.localScale = Vector3.one * 1.2f;
        StartCoroutine(WarningRoutine(message));
    }

    IEnumerator WarningRoutine(string msg)
    {
        warningText.text = msg;
        warningText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        warningText.gameObject.SetActive(false);
    }

public void FlashScreen(Color color, float duration = 0.3f)
{
    StartCoroutine(FlashRoutine(color, duration));
}

IEnumerator FlashRoutine(Color color, float duration)
{
    flashImage.gameObject.SetActive(true);
    flashImage.color = new Color(color.r, color.g, color.b, 0);

    float t = 0;

    // fade in
    while (t < 1)
    {
        t += Time.deltaTime * 10f;
        flashImage.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0, 0.6f, t));
        yield return null;
    }

    t = 0;

    // fade out
    while (t < 1)
    {
        t += Time.deltaTime * 5f;
        flashImage.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0.6f, 0, t));
        yield return null;
    }

    flashImage.gameObject.SetActive(false);
}

    public void TriggerSplatter(Sprite[] splatSet)
{
    // shake
    if (cameraTransform != null)
    {
        StartCoroutine(ShakeCamera());
    }

    for (int i = 0; i < splatCount; i++)
    {
        GameObject splat = Instantiate(splatPrefab, canvas.transform);

        Image img = splat.GetComponent<Image>();
        img.sprite = splatSet[Random.Range(0, splatSet.Length)];

        RectTransform rt = splat.GetComponent<RectTransform>();

        float size = Random.Range(0.6f, 1.4f);

        float width = canvasRect.rect.width;
        float height = canvasRect.rect.height;

        rt.anchoredPosition = new Vector2(
            Random.Range(-width / 2, width / 2),
            Random.Range(-height / 2, height / 2)
        );

        rt.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        rt.localScale = Vector3.one * 0.1f;

        StartCoroutine(AnimateSplatter(rt, splat, size));
    }
}

    IEnumerator AnimateSplatter(RectTransform rt, GameObject splat, float targetSize)
    {
        CanvasGroup cg = splat.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = splat.AddComponent<CanvasGroup>();

        cg.alpha = 0;

        float t = 0;

        // pop in
        while (t < 1)
        {
            t += Time.deltaTime * 5f;

            cg.alpha = Mathf.Lerp(0, 0.8f, t);
            rt.localScale = Vector3.Lerp(Vector3.one * 0.1f, Vector3.one * targetSize, t);

            yield return null;
        }

        // slight overshoot
        rt.localScale *= 1.1f;

        yield return new WaitForSeconds(1.5f);

        // fade out
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f;
            cg.alpha = Mathf.Lerp(0.8f, 0, t);
            yield return null;
        }

        Destroy(splat);
    }

    IEnumerator ShakeCamera()
    {
        Vector3 originalPos = cameraTransform.localPosition;

        float t = 0;

        while (t < shakeDuration)
        {
            t += Time.deltaTime;

            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            cameraTransform.localPosition = originalPos + new Vector3(x, y, 0);

            yield return null;
        }

        cameraTransform.localPosition = originalPos;
    }

    public void TriggerBombEffect()
    {
        TriggerSplatter(bombSplats);

        if (audioSource != null && splatSound != null)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(splatSound);
        }
    }

    public void TriggerPineappleEffect()
    {
        TriggerSplatter(pineappleSplats);

        FlashScreen(Color.yellow);
        ShowWarning("PINEAPPLE CHAOS!");

        if (audioSource != null && chaosSound1 != null && chaosSound2 != null)
        {
            audioSource.PlayOneShot(chaosSound1);
            audioSource.PlayOneShot(chaosSound2);
        }

        if (cameraTransform != null)
        {
            StartCoroutine(ShakeCamera());
        }
    }

    public void ShowEndScreen(bool win)
    {
        endPanel.SetActive(true);
        endText.text = win ? "YOU WIN!" : "YOU LOSE!";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainWorld()
    {
        SceneManager.LoadScene("ToppingStartingHub");
    }
}