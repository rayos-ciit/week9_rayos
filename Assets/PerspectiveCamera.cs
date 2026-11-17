using UnityEngine;

public class PerspectiveCamera : MonoBehaviour
{
    #region Serialized Fields

    [Header("Perspective Settings")]
    [SerializeField] private float _focalLength = 5f;
    [SerializeField] private Vector3 _vanishingPointOffset = new Vector3(0f, 1.5f, 0f);

    #endregion

    #region Properties

    public float FocalLength => _focalLength;
    public Vector3 VanishingPointOffset => _vanishingPointOffset;

    #endregion

    #region Public Methods

    public float CalculatePerspectiveScale(float depthZ)
    {
        float denominator = _focalLength + depthZ;
        if (denominator <= 0.01f)
        {
            return 0f;
        }

        return _focalLength / denominator;
    }

    public Vector3 ProjectToScreenPosition(Vector3 logicalPosition)
    {
        float perspectiveScale = CalculatePerspectiveScale(logicalPosition.z);

        Vector3 planarOffset = new Vector3(logicalPosition.x, logicalPosition.y, 0f);
        Vector3 scaledOffset = planarOffset * perspectiveScale;
        Vector3 horizonContribution = _vanishingPointOffset * (1f - perspectiveScale);

        return scaledOffset + horizonContribution;
    }

    #endregion
}