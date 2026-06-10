using System.Collections;
using UnityEngine;

/// <summary>Leans the board mesh toward the tilt direction and eases it back, selling the physical tip.</summary>
public class BoardTiltAnimator : MonoBehaviour
{
    [Header("Tilt Feel")]
    [SerializeField] private float tiltAngle      = 15f;
    [SerializeField] private float tiltDuration   = 0.15f;
    [SerializeField] private float holdDuration   = 0.05f;
    [SerializeField] private float returnDuration = 0.2f;

    [Tooltip("Transform that leans. Defaults to this object's transform if unset.")]
    [SerializeField] private Transform boardTransform;

    private void Awake()
    {
        if (boardTransform == null) boardTransform = transform;
    }

    /// <summary>Leans toward `dir`, holds, then returns to flat. Yields until fully returned.</summary>
    public IEnumerator PlayTilt(TiltDirection dir)
    {
        Vector3 target;
        switch (dir)
        {
            case TiltDirection.Left:  target = new Vector3(0f, 0f,  tiltAngle); break;
            case TiltDirection.Right: target = new Vector3(0f, 0f, -tiltAngle); break;
            case TiltDirection.Up:    target = new Vector3(-tiltAngle, 0f, 0f); break;
            case TiltDirection.Down:  target = new Vector3( tiltAngle, 0f, 0f); break;
            default:                  target = Vector3.zero; break;
        }

        yield return LerpRotation(Vector3.zero, target, tiltDuration);
        if (holdDuration > 0f) yield return new WaitForSeconds(holdDuration);
        yield return LerpRotation(target, Vector3.zero, returnDuration);
    }

    private IEnumerator LerpRotation(Vector3 from, Vector3 to, float duration)
    {
        if (duration <= 0f)
        {
            boardTransform.localEulerAngles = to;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            boardTransform.localEulerAngles = Vector3.Lerp(from, to, t);
            yield return null;
        }
        boardTransform.localEulerAngles = to;
    }
}
