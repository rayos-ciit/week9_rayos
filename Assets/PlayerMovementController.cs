using System;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    #region Serialized Fields

    [Header("Dependencies")]
    [SerializeField] private PerspectiveCamera _perspectiveCamera;

    [Header("Lane Configuration")]
    [SerializeField] private float _laneWidth = 2.5f;
    [SerializeField] private float _laneSwitchDuration = 0.22f;
    [SerializeField] private float _groundYPosition = -2.0f;
    [SerializeField] private float _playerDepthZ = 0f;

    [Header("Jump Configuration")]
    [SerializeField] private float _jumpHeight = 2.2f;
    [SerializeField] private float _jumpDuration = 0.55f;

    [Header("Visual Scale")]
    [SerializeField] private Vector3 _baseScale = Vector3.one;

    #endregion

    #region Private Fields

    private const int MinLaneIndex = 0;
    private const int CenterLaneIndex = 1;
    private const int MaxLaneIndex = 2;

    private int _currentLaneIndex = CenterLaneIndex;
    private float _laneTransitionStartX;
    private float _laneTransitionTargetX;
    private float _laneTransitionElapsed = 1f;
    private bool _isTransitioningLane;
    private int _lastMoveDirection;

    private bool _isJumping;
    private float _jumpElapsed;

    private Vector3 _logicalPosition;

    #endregion

    #region Properties

    public Vector3 LogicalPosition => _logicalPosition;
    public bool IsJumping => _isJumping;
    public bool IsTransitioningLane => _isTransitioningLane;
    public int LastMoveDirection => _lastMoveDirection;
    public float GroundYPosition => _groundYPosition;

    #endregion

    #region Events

    public event Action<int> OnLaneSwitchStarted;
    public event Action OnJumpStarted;
    public event Action OnJumpLanded;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        InitializePosition();
    }

    private void Update()
    {
        HandleLaneInput();
        HandleJumpInput();
        UpdateHorizontalEasing();
        UpdateJumpArc();
        ApplyPerspectiveTransform();
    }

    #endregion

    #region Private Methods

    private void InitializePosition()
    {
        float initialX = CalculateLaneWorldX(_currentLaneIndex);
        _laneTransitionStartX = initialX;
        _laneTransitionTargetX = initialX;
        _logicalPosition = new Vector3(initialX, _groundYPosition, _playerDepthZ);
        ApplyPerspectiveTransform();
    }

    private void HandleLaneInput()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TrySwitchLane(-1);
            return;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            TrySwitchLane(1);
        }
    }

    private void TrySwitchLane(int direction)
    {
        int targetLaneIndex = Mathf.Clamp(_currentLaneIndex + direction, MinLaneIndex, MaxLaneIndex);
        if (targetLaneIndex == _currentLaneIndex)
        {
            return;
        }

        _currentLaneIndex = targetLaneIndex;
        _laneTransitionStartX = _logicalPosition.x;
        _laneTransitionTargetX = CalculateLaneWorldX(_currentLaneIndex);
        _laneTransitionElapsed = 0f;
        _isTransitioningLane = true;
        _lastMoveDirection = direction;

        OnLaneSwitchStarted?.Invoke(direction);
    }

    private void HandleJumpInput()
    {
        if (_isJumping)
        {
            return;
        }

        if (!Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.W) && !Input.GetKeyDown(KeyCode.UpArrow))
        {
            return;
        }

        _isJumping = true;
        _jumpElapsed = 0f;
        OnJumpStarted?.Invoke();
    }

    private void UpdateHorizontalEasing()
    {
        if (!_isTransitioningLane)
        {
            return;
        }

        _laneTransitionElapsed += Time.deltaTime;
        float normalizedTime = Mathf.Clamp01(_laneTransitionElapsed / _laneSwitchDuration);
        float easedTime = MathUtility.EaseOutCubic(normalizedTime);

        float currentX = MathUtility.InterpolateScalar(_laneTransitionStartX, _laneTransitionTargetX, easedTime);
        _logicalPosition = new Vector3(currentX, _logicalPosition.y, _logicalPosition.z);

        if (normalizedTime >= 1f)
        {
            _isTransitioningLane = false;
            _lastMoveDirection = 0;
        }
    }

    private void UpdateJumpArc()
    {
        if (!_isJumping)
        {
            return;
        }

        _jumpElapsed += Time.deltaTime;
        float normalizedJumpTime = Mathf.Clamp01(_jumpElapsed / _jumpDuration);

        //sine wave arc
        float verticalOffset = Mathf.Sin(normalizedJumpTime * Mathf.PI) * _jumpHeight;
        float currentY = _groundYPosition + verticalOffset;
        _logicalPosition = new Vector3(_logicalPosition.x, currentY, _logicalPosition.z);

        if (normalizedJumpTime < 1f)
        {
            return;
        }

        _isJumping = false;
        _logicalPosition = new Vector3(_logicalPosition.x, _groundYPosition, _logicalPosition.z);
        OnJumpLanded?.Invoke();
    }

    private void ApplyPerspectiveTransform()
    {
        if (_perspectiveCamera == null)
        {
            return;
        }

        float scaleFactor = _perspectiveCamera.CalculatePerspectiveScale(_logicalPosition.z);
        transform.position = _perspectiveCamera.ProjectToScreenPosition(_logicalPosition);
        transform.localScale = _baseScale * scaleFactor;
    }

    private float CalculateLaneWorldX(int laneIndex)
    {
        intlaneOffsetFromCenter = laneIndex - CenterLaneIndex;
        return intlaneOffsetFromCenter * _laneWidth;
    }

    #endregion
}