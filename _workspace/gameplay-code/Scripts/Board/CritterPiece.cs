using System.Collections;
using UnityEngine;

/// <summary>A single critter on the board: holds its color/grid position and plays slide and exit animations.</summary>
public class CritterPiece : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private int id;
    [SerializeField] private CritterColor color;

    [Header("Settle Feel")]
    [SerializeField] private float overshootFraction = 0.04f; // OutBack-style elastic overshoot
    [SerializeField] private float settleDuration    = 0.08f;

    [Header("Exit Feel")]
    [SerializeField] private float exitScaleUp  = 1.25f;
    [SerializeField] private float exitDuration = 0.25f;

    /// <summary>Unique id within the level.</summary>
    public int Id => id;

    /// <summary>This critter's color; determines which gate it may exit through.</summary>
    public CritterColor Color => color;

    /// <summary>Current grid cell (valid while on the board).</summary>
    public Vector2Int GridPosition { get; private set; }

    private Transform _tf;
    private Vector3   _baseScale;

    private void Awake()
    {
        _tf = transform;
        _baseScale = _tf.localScale;
    }

    /// <summary>Initializes identity and starting grid cell (called by LevelManager).</summary>
    public void Init(int critterId, CritterColor critterColor, Vector2Int startCell)
    {
        id = critterId;
        color = critterColor;
        GridPosition = startCell;
    }

    /// <summary>Updates the cached grid cell after a resolved move.</summary>
    public void SetGridPosition(Vector2Int cell) => GridPosition = cell;

    /// <summary>Slides this critter to a world position at constant speed (cells/sec * cellSize), with an elastic settle.</summary>
    public IEnumerator SlideTo(Vector3 worldPos, float speed)
    {
        if (_tf == null) { _tf = transform; _baseScale = _tf.localScale; }

        Vector3 start = _tf.position;
        float distance = Vector3.Distance(start, worldPos);

        if (distance > 0.0001f && speed > 0f)
        {
            float duration = distance / speed;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                // OutCubic for an ease-out feel.
                float eased = 1f - Mathf.Pow(1f - t, 3f);
                _tf.position = Vector3.Lerp(start, worldPos, eased);
                yield return null;
            }
        }

        _tf.position = worldPos;
        yield return Settle(worldPos);
    }

    private IEnumerator Settle(Vector3 finalPos)
    {
        if (overshootFraction <= 0f || settleDuration <= 0f) yield break;

        Vector3 over = _baseScale * (1f + overshootFraction);
        float half = settleDuration * 0.5f;

        float e = 0f;
        while (e < half)
        {
            e += Time.deltaTime;
            _tf.localScale = Vector3.Lerp(_baseScale, over, e / half);
            yield return null;
        }
        e = 0f;
        while (e < half)
        {
            e += Time.deltaTime;
            _tf.localScale = Vector3.Lerp(over, _baseScale, e / half);
            yield return null;
        }
        _tf.localScale = _baseScale;
    }

    /// <summary>Plays the exit animation (scale up briefly then shrink to nothing) and disables the object.</summary>
    public IEnumerator PlayExitAnimation()
    {
        if (_tf == null) { _tf = transform; _baseScale = _tf.localScale; }

        Vector3 peak = _baseScale * exitScaleUp;
        float up = exitDuration * 0.3f;
        float down = exitDuration - up;

        float e = 0f;
        while (e < up)
        {
            e += Time.deltaTime;
            _tf.localScale = Vector3.Lerp(_baseScale, peak, e / up);
            yield return null;
        }
        e = 0f;
        while (e < down)
        {
            e += Time.deltaTime;
            _tf.localScale = Vector3.Lerp(peak, Vector3.zero, e / down);
            yield return null;
        }
        _tf.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    /// <summary>Resets visual scale (used when recycling a pooled critter for a new level).</summary>
    public void ResetVisual()
    {
        if (_tf == null) { _tf = transform; }
        _tf.localScale = _baseScale == Vector3.zero ? Vector3.one : _baseScale;
        gameObject.SetActive(true);
    }
}
