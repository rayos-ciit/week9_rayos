using UnityEngine;

public static class MathUtility
{
    #region Public Methods

    public static Vector3 InterpolateVector(Vector3 startValue, Vector3 endValue, float interpolationFactor)
    {
        float clampedFactor = Mathf.Clamp01(interpolationFactor);
        Vector3 difference = endValue - startValue;
        return startValue + (difference * clampedFactor);
    }

    public static float InterpolateScalar(float startValue, float endValue, float interpolationFactor)
    {
        float clampedFactor = Mathf.Clamp01(interpolationFactor);
        return startValue + ((endValue - startValue) * clampedFactor);
    }

    //decelerates towards the target: 1 - (1 - t)^3
    public static float EaseOutCubic(float normalizedTime)
    {
        float clampedTime = Mathf.Clamp01(normalizedTime);
        float inverseTime = 1f - clampedTime;
        return 1f - (inverseTime * inverseTime * inverseTime);
    }

    public static float EaseInOutSine(float normalizedTime)
    {
        float clampedTime = Mathf.Clamp01(normalizedTime);
        return -(Mathf.Cos(Mathf.PI * clampedTime) - 1f) / 2f;
    }
    #endregion
}