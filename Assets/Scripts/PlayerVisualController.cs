using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerVisualController : MonoBehaviour
{
    #region Serialized Fields

    [Header("Injected Dependencies")]
    [SerializeField] private PlayerMovementController _movementController;
    [SerializeField] private PlayerHealthSystem _healthSystem;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    [Header("Player State Art (Sprites)")]
    [SerializeField] private Sprite _idleRunSprite;
    [SerializeField] private Sprite _moveLeftSprite;
    [SerializeField] private Sprite _moveRightSprite;
    [SerializeField] private Sprite _jumpSprite;
    [SerializeField] private Sprite _hitSprite;

    [Header("Hit Visual Feedback")]
    [SerializeField] private float _hitSpriteDisplayDuration = 0.25f;

    #endregion

    #region Private Fields

    private float _hitFeedbackTimer;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        UpdateHitTimer();
        UpdateActiveSprite();
    }

    #endregion

    #region Private Methods

    private void SubscribeToEvents()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnDamageTaken += HandleDamageTaken;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnDamageTaken -= HandleDamageTaken;
        }
    }

    private void HandleDamageTaken()
    {
        _hitFeedbackTimer = _hitSpriteDisplayDuration;
    }

    private void UpdateHitTimer()
    {
        if (_hitFeedbackTimer <= 0f)
        {
            return;
        }

        _hitFeedbackTimer -= Time.deltaTime;
    }

    private void UpdateActiveSprite()
    {
        if (_spriteRenderer == null || _movementController == null)
        {
            return;
        }

        if (_hitFeedbackTimer > 0f && _hitSprite != null)
        {
            SetSprite(_hitSprite);
            return;
        }

        if (_movementController.IsJumping && _jumpSprite != null)
        {
            SetSprite(_jumpSprite);
            return;
        }

        if (_movementController.IsTransitioningLane)
        {
            Sprite directionalSprite = _movementController.LastMoveDirection < 0 ? _moveLeftSprite : _moveRightSprite;
            if (directionalSprite != null)
            {
                SetSprite(directionalSprite);
                return;
            }
        }

        if (_idleRunSprite != null)
        {
            SetSprite(_idleRunSprite);
        }
    }

    private void SetSprite(Sprite targetSprite)
    {
        if (_spriteRenderer.sprite == targetSprite)
        {
            return;
        }

        _spriteRenderer.sprite = targetSprite;
    }

    #endregion
}