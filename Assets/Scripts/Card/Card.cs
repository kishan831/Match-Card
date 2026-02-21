using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class Card : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer frontRenderer;
    [SerializeField] private SpriteRenderer backRenderer;

    [Header("Visual Feedback")]
    [SerializeField] private float matchScalePunch = 1.2f;
    [SerializeField] private float mismatchShakeAmount = 0.1f;
    [SerializeField] private Color matchHighlightColor = Color.green;

    [Header("Settings")]
    [SerializeField] private float flipSpeed = 6f;

    // Card data
    public int CardId { get; private set; }
    public bool IsMatched { get; private set; }
    public int CardDataIndex { get; private set; }
    public bool IsFlipped { get; private set; }
    public bool IsFlipping { get; private set; }

    // Events
    public static event System.Action<Card> OnCardClicked;

    private bool showingFront = false;
    private bool flipRequested = false;
    private bool targetShowFront = false;
    private float currentAngle = 0f;

    public void OnCardSelect()
    {
        if (IsMatched || IsFlipped) return;
        OnCardClicked?.Invoke(this);
    }

    public void Initialize(CardData data, int dataIndex)
    {
        ResetCard();

        CardId = data.cardId;
        CardDataIndex = dataIndex;
        frontRenderer.sprite = data.cardFace;
        IsMatched = false;
        IsFlipped = false;
        IsFlipping = false;
        showingFront = false;
        currentAngle = 0f;
        transform.localEulerAngles = Vector3.zero;

        frontRenderer.gameObject.SetActive(false);
        backRenderer.gameObject.SetActive(true);

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            col.size = backRenderer.bounds.size / transform.localScale.x;
        }
    }

    private void Update()
    {
        if (!flipRequested) return;

        float targetAngle = targetShowFront ? 180f : 0f;
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, flipSpeed * Time.deltaTime);
        if (Mathf.Abs(currentAngle - targetAngle) < 0.5f)
        {
            currentAngle = targetAngle;
        }

        transform.localEulerAngles = new Vector3(0f, currentAngle, 0f);

        // Swap sprites at the halfway point
        bool shouldShowFront = currentAngle > 90f;
        if (shouldShowFront != showingFront)
        {
            showingFront = shouldShowFront;
            frontRenderer.gameObject.SetActive(showingFront);
            backRenderer.gameObject.SetActive(!showingFront);
        }

        // Flip complete
        if (Mathf.Approximately(currentAngle, targetAngle))
        {
            flipRequested = false;
            IsFlipping = false;
            IsFlipped = targetShowFront;
        }
    }

    public void FlipUp()
    {
        if (IsMatched || (IsFlipped && !IsFlipping)) return;
        flipRequested = true;
        targetShowFront = true;
        IsFlipping = true;
    }


    public void SetFlippedInstant()
    {
        currentAngle = 180f;
        transform.localEulerAngles = new Vector3(0f, 180f, 0f);
        showingFront = true;
        IsFlipped = true;
        IsFlipping = false;
        flipRequested = false;

        frontRenderer.gameObject.SetActive(true);
        backRenderer.gameObject.SetActive(false);
    }

    public void FlipDown()
    {
        if (IsMatched) return;
        flipRequested = true;
        targetShowFront = false;
        IsFlipping = true;
    }

    public void SetMatched()
    {
        IsMatched = true;
    }

    public void PlayMatchEffect()
    {
        StartCoroutine(MatchEffectRoutine());
    }

    public void PlayMismatchEffect()
    {
        StartCoroutine(MismatchEffectRoutine());
    }

    private IEnumerator MatchEffectRoutine()
    {
        // Scale punch
        Vector3 originalScale = transform.localScale;
        Vector3 punchScale = originalScale * matchScalePunch;

        // Scale up
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.localScale = Vector3.Lerp(originalScale, punchScale, t);
            yield return null;
        }

        // Scale back
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 6f;
            transform.localScale = Vector3.Lerp(punchScale, originalScale, t);
            yield return null;
        }

        transform.localScale = originalScale;

        // Tint the card
        if (frontRenderer != null)
            frontRenderer.color = matchHighlightColor;

        // Fade tint back to white
        t = 0f;
        Color startColor = matchHighlightColor;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            if (frontRenderer != null)
                frontRenderer.color = Color.Lerp(startColor, Color.white, t);
            yield return null;
        }
    }

    private IEnumerator MismatchEffectRoutine()
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-mismatchShakeAmount, mismatchShakeAmount);
            transform.localPosition = originalPos + new Vector3(x, 0f, 0f);
            yield return null;
        }

        transform.localPosition = originalPos;
    }


    public void ResetCard()
    {
        IsMatched = false;
        IsFlipped = false;
        IsFlipping = false;
        showingFront = false;
        flipRequested = false;
        currentAngle = 0f;
        transform.localEulerAngles = Vector3.zero;

        if (frontRenderer != null)
        {
            frontRenderer.gameObject.SetActive(false);
            frontRenderer.color = Color.white;
        }
        if (backRenderer != null)
        {
            backRenderer.gameObject.SetActive(true);
        }

        StopAllCoroutines();
    }
}