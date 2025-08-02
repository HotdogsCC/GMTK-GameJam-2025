using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Serialization;

/// <summary>
/// Defines a transform and its target rotations for left/right states.
/// </summary>
[System.Serializable]
public struct IndicatorTransform
{
    public Transform transform;
    public Vector3 leftRotation;
    public Vector3 rightRotation;
}

/// <summary>
/// Controls a set of transforms to visually indicate a switched state.
/// </summary>
public class SwitchIndicator : MonoBehaviour
{
    [Tooltip("The list of transforms to rotate.")]
    [SerializeField] private List<IndicatorTransform> indicators = new List<IndicatorTransform>();

    [Tooltip("How long the switch animation takes in seconds.")]
    [SerializeField] private float switchDuration = 0.3f;

    private Coroutine _currentSwitchCoroutine;

    /// <summary>
    /// Instantly sets all indicators to their 'left' rotation.
    /// </summary>
    public void SetLeft()
    {
        StopSwitching();
        foreach (var indicator in indicators)
        {
            if (indicator.transform != null)
                indicator.transform.localRotation = Quaternion.Euler(indicator.leftRotation);
        }
    }

    /// <summary>
    /// Instantly sets all indicators to their 'right' rotation.
    /// </summary>
    public void SetRight()
    {
        StopSwitching();
        foreach (var indicator in indicators)
        {
            if (indicator.transform != null)
                indicator.transform.localRotation = Quaternion.Euler(indicator.rightRotation);
        }
    }
    
    /// <summary>
    /// Starts a smooth transition to the 'left' rotation.
    /// </summary>
    public void SwitchToLeft()
    {
        StopSwitching();
        _currentSwitchCoroutine = StartCoroutine(SwitchRotationCoroutine(true));
    }

    /// <summary>
    /// Starts a smooth transition to the 'right' rotation.
    /// </summary>
    public void SwitchToRight()
    {
        StopSwitching();
        _currentSwitchCoroutine = StartCoroutine(SwitchRotationCoroutine(false));
    }

    private void StopSwitching()
    {
        if (_currentSwitchCoroutine != null)
        {
            StopCoroutine(_currentSwitchCoroutine);
            _currentSwitchCoroutine = null;
        }
    }

    private IEnumerator SwitchRotationCoroutine(bool toLeft)
    {
        float elapsedTime = 0f;

        // Store the starting rotation for each indicator before the animation begins
        var startRotations = new List<Quaternion>();
        foreach (var indicator in indicators)
        {
            startRotations.Add(indicator.transform != null ? indicator.transform.localRotation : Quaternion.identity);
        }

        // Animate over the specified duration
        while (elapsedTime < switchDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / switchDuration);

            // Update each indicator's rotation
            for (int i = 0; i < indicators.Count; i++)
            {
                if (indicators[i].transform == null) continue;

                Quaternion targetRotation = toLeft ? 
                    Quaternion.Euler(indicators[i].leftRotation) : 
                    Quaternion.Euler(indicators[i].rightRotation);
            
                // Interpolate from the original starting rotation to the target
                indicators[i].transform.localRotation = Quaternion.Slerp(startRotations[i], targetRotation, t);
            }
        
            yield return null;
        }

        // After the loop, snap all indicators to their final rotation to ensure accuracy
        for (int i = 0; i < indicators.Count; i++)
        {
            if (indicators[i].transform == null) continue;
            Quaternion finalRotation = toLeft ? 
                Quaternion.Euler(indicators[i].leftRotation) : 
                Quaternion.Euler(indicators[i].rightRotation);
            
            indicators[i].transform.localRotation = finalRotation;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (indicators == null) return;

        foreach (var indicator in indicators)
        {
            if (indicator.transform == null) continue;

            const float gizmoLength = 0.3f;
            Quaternion parentRotation = indicator.transform.parent ? indicator.transform.parent.rotation : Quaternion.identity;

            // Draw Left state axes (Blue)
            Quaternion leftWorldRotation = parentRotation * Quaternion.Euler(indicator.leftRotation);
            DrawGizmoAxes(indicator.transform.position, leftWorldRotation, Color.blue, gizmoLength);
            
            // Draw Right state axes (Red)
            Quaternion rightWorldRotation = parentRotation * Quaternion.Euler(indicator.rightRotation);
            DrawGizmoAxes(indicator.transform.position, rightWorldRotation, Color.red, gizmoLength);
        }
    }
    
    private void DrawGizmoAxes(Vector3 position, Quaternion rotation, Color color, float length)
    {
        Gizmos.color = color;
        // X-axis
        Gizmos.DrawRay(position, rotation * Vector3.right * length);
        Gizmos.DrawRay(position, rotation * Vector3.right * -length);
        // Y-axis
        Gizmos.DrawRay(position, rotation * Vector3.up * length);
        Gizmos.DrawRay(position, rotation * Vector3.up * -length);
        // Z-axis
        Gizmos.DrawRay(position, rotation * Vector3.forward * length);
        Gizmos.DrawRay(position, rotation * Vector3.forward * -length);
    }
}