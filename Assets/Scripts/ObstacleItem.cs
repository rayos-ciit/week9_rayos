using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObstacleItem : MonoBehaviour
{
    #region Serialized Fields

    [Header("Obstacle Settings")]
    [SerializeField] private Vector3 _baseScale = Vector3.one;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    #endregion

    #region Private Fields

    private PerspectiveCamera _perspectiveCamera;
    private Vector3 _logicalPosition;
    private float _moveSpeedZ;
    private bool _hasTriggeredHit;

    #endregion

    #region Properties

    public Vector3 LogicalPosition => _logicalPosition;
    public bool HasTriggeredHit => _hasTriggeredHit;
    public SpriteRenderer Renderer => _spriteRenderer;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    #endregion

    #region Public Methods


    public void Initialize(
        PerspectiveCamera perspectiveCamera,
        Vector3 startLogicalPosition,
        float moveSpeedZ,
        Sprite obstacleSprite)
    {
        _perspectiveCamera = perspectiveCamera;
        _logicalPosition = startLogicalPosition;
        _moveSpeedZ = moveSpeedZ;
        _hasTriggeredHit = false;

        if (_spriteRenderer != null && obstacleSprite != null)
        {
            _spriteRenderer.sprite = obstacleSprite;
        }

        UpdatePerspectiveTransform();
    }

    public void TickMovement(float deltaTime)
    {
        //screen moves (decreasing Z) using Vector3 scalar operations
        Vector3 forwardStep = new Vector3(0f, 0f, -1f) * (_moveSpeedZ * deltaTime);
        _logicalPosition = _logicalPosition + forwardStep;

        UpdatePerspectiveTransform();
    }

    public void MarkAsHit()
    {
        _hasTriggeredHit = true;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        if (_spriteRenderer == null)
        {
            return;
        }

        _spriteRenderer.sortingOrder = sortingOrder;
    }

    #endregion

    #region Private Methods

    private void UpdatePerspectiveTransform()
    {
        if (_perspectiveCamera == null)
        {
            return;
        }

        float perspectiveScale = _perspectiveCamera.CalculatePerspectiveScale(_logicalPosition.z);
        transform.position = _perspectiveCamera.ProjectToScreenPosition(_logicalPosition);
        transform.localScale = _baseScale * perspectiveScale;
    }

    #endregion
}