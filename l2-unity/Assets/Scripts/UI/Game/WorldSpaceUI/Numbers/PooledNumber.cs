using System;
using TMPro;
using UnityEngine;

public class PooledNumber : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private TextMeshProUGUI _value;
    [SerializeField] private RectTransform _rectTransform;

    [Header("Motion Settings")]
    [SerializeField] private NumberAnimationStyle _bounceStyle;
    [SerializeField] private float _duration = 1.25f;          // RO-style: fast!
    [SerializeField] private float _horizontalDistance = 5f; // how far to the right
    [SerializeField] private float _bounceHeight = 6f;       // how high the bounce goes
    [SerializeField] public float _fadingStart = 0.25f;
    [SerializeField] public float _fadingSpeed = 0.4f;
    [SerializeField] private Vector2 _startOffset;

    private bool _pendingStart = true;
    private float _startTime;

    private void Update()
    {
        if (_pendingStart)
            return;

        float elapsed = Time.time - _startTime;
        float t = Mathf.Clamp01(elapsed / _duration);

        if (t >= 1f)
        {
            PoolMyself();
            return;
        }

        switch (_bounceStyle)
        {
            case NumberAnimationStyle.BounceLeft:
                Bounce(true, t);
                break;
            case NumberAnimationStyle.BounceRight:
                Bounce(false, t);
                break;
            default:
                ScrollUp(t);
                break;
        }
    }

    private void Bounce(bool left, float t)
    {
        Vector3 startLocalPos = new Vector3(left ? -_startOffset.x : _startOffset.x, _startOffset.y, 0);

        // Ease-out time (fast at start, slower at end)
        // This mimics that "pop then slow" feeling.
        float moveT = t * t * (3f - 2f * t); // SmoothStep

        // Move to the right over time
        float xOffset = Mathf.Lerp(0f, _horizontalDistance, moveT);

        // Vertical bounce
        float bounce = Mathf.Sin(moveT * Mathf.PI + Mathf.PI / 3f);  // 0 -> 1 -> 0
        float yOffset = bounce * _bounceHeight;

        // Apply position
        Vector3 totalOffset = new Vector3(left ? -xOffset : xOffset, yOffset, 0f);

        _rectTransform.localPosition = startLocalPos + totalOffset;

        // Optional: scale "pop" 
        float scale = Mathf.Lerp(1f, 0.6f, t + 0.25f);
        _rectTransform.localScale = Vector3.one * scale;

        // Optional: fade out at the very end

        float alpha = 1f;
        // float fadingStart = 0.45f;
        // float fadingStart = 0.45f;
        if (t > _fadingStart)
        {
            float fadeT = (t - _fadingStart) / _fadingSpeed; // 0 -> 1 over last 0.2 of duration
            alpha = Mathf.Lerp(1f, 0f, fadeT);
        }

        Color c = _value.color;
        c.a = alpha;
        _value.color = c;
    }

    private void ScrollUp(float t)
    {
        Vector3 startLocalPos = new Vector3(_startOffset.x, _startOffset.y, 0);

        // Ease-out time (fast at start, slower at end)
        // This mimics that "pop then slow" feeling.
        float moveT = t * t * (3f - 2f * t); // SmoothStep

        // Move to the right over time
        float yOffset = Mathf.Lerp(0f, _bounceHeight, moveT);

        // Apply position
        Vector3 totalOffset = new Vector3(0, yOffset, 0f);

        _rectTransform.localPosition = startLocalPos + totalOffset;

        // Optional: fade out at the very end (last 20% of life)
        float alpha = 1f;
        if (t > 0.6f)
        {
            float fadeT = (t - 0.6f) / 0.2f; // 0 -> 1 over last 0.2 of duration
            alpha = Mathf.Lerp(1f, 0f, fadeT);
        }

        Color c = _value.color;
        c.a = alpha;
        _value.color = c;
    }

    public void Animate(NumberAnimationStyle bounceStyle, NumberType numberType, float value)
    {
        UpdateText(numberType, value);

        _pendingStart = true;

        _bounceStyle = bounceStyle;

        gameObject.SetActive(true);

        _startTime = Time.time;

        // Always reset transform state for pooled reuse
        _rectTransform.localScale = Vector3.one;
        _rectTransform.localPosition = new Vector3(_startOffset.x, _startOffset.y, 0f);

        // Reset alpha (pool reuse can leave it faded)
        Color c = _value.color;
        c.a = 1f;
        _value.color = c;

        _pendingStart = false;
    }

    private void UpdateText(NumberType numberType, float value)
    {
        _value.text = ((int)value).ToString();

        switch (numberType)
        {
            case NumberType.Damaged:
                _value.color = Color.red;
                break;
            case NumberType.Heal:
                _value.color = Color.green;
                break;
            default:
                _value.color = Color.white;
                break;
        }
    }

    private void PoolMyself()
    {
        _pendingStart = true;
        gameObject.SetActive(false);
        NumbersManager.Instance.PoolEffect(this);
    }
}