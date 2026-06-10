using System;
using UnityEngine;

/// <summary>Reads the device accelerometer, low-pass filters it, and fires OnTilt for deliberate 4-way tilts.</summary>
public class TiltInputHandler : MonoBehaviour
{
    public static TiltInputHandler Instance { get; private set; }

    /// <summary>Fired once per registered, debounced tilt gesture.</summary>
    public event Action<TiltDirection> OnTilt;

    [Header("Sensitivity")]
    [SerializeField] private float tiltThreshold = 0.35f;  // magnitude an axis must cross to register
    [SerializeField] private float smoothing     = 0.1f;   // low-pass filter blend factor
    [SerializeField] private float cooldown      = 0.6f;    // seconds locked after a registered tilt
    [SerializeField] private float reArmFraction = 0.4f;    // device must fall below threshold*this before re-firing

    [Header("Runtime")]
    [SerializeField] private bool inputEnabled = true;

    private Vector3 _smoothedAccel;
    private float   _lastTiltTime = -999f;
    private bool    _armed = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    /// <summary>Allows external systems (e.g. accessibility D-pad, menus) to suspend tilt reading.</summary>
    public void SetInputEnabled(bool enabled) => inputEnabled = enabled;

    private void Update()
    {
        if (!inputEnabled) return;

        // Low-pass filter to remove hand shake.
        _smoothedAccel = Vector3.Lerp(_smoothedAccel, Input.acceleration, smoothing);

        float absX = Mathf.Abs(_smoothedAccel.x);
        float absY = Mathf.Abs(_smoothedAccel.y);
        float reArmLevel = tiltThreshold * reArmFraction;

        // Re-arm only once the device has returned near neutral.
        if (!_armed && absX < reArmLevel && absY < reArmLevel)
            _armed = true;

        if (Time.time - _lastTiltTime < cooldown) return;
        if (!_armed) return;

        TiltDirection? dir = DetectTilt(absX, absY, _smoothedAccel);
        if (dir.HasValue)
        {
            _lastTiltTime = Time.time;
            _armed = false;
            OnTilt?.Invoke(dir.Value);
        }
    }

    private TiltDirection? DetectTilt(float absX, float absY, Vector3 accel)
    {
        if (absX < tiltThreshold && absY < tiltThreshold) return null;

        // Dominant axis wins — no diagonal moves.
        if (absX >= absY)
            return accel.x < 0 ? TiltDirection.Left : TiltDirection.Right;
        return accel.y < 0 ? TiltDirection.Down : TiltDirection.Up;
    }
}
