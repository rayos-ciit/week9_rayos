using System;
using UnityEngine;

public class PlayerHealthSystem : MonoBehaviour
{
    #region Serialized Fields

    [Header("Health Settings")]
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _damagePerHit = 5f;
    [SerializeField] private float _regenerationRatePerSecond = 1f;

    #endregion

    #region Private Fields

    private float _currentHealth;
    private bool _isDead;

    #endregion

    #region Properties

    public float CurrentHealth => _currentHealth;
    public float MaxHealth => _maxHealth;
    public bool IsDead => _isDead;

    #endregion

    #region Events

    public event Action<float, float> OnHealthChanged;
    public event Action OnDamageTaken;
    public event Action OnPlayerDied;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    private void Start()
    {
        NotifyHealthChanged();
    }

    private void Update()
    {
        RegenerateHealthOverTime();
    }

    #endregion

    #region Public Methods

    public void TakeHitDamage()
    {
        ApplyDamage(_damagePerHit);
    }

    public void ApplyDamage(float damageAmount)
    {
        if (_isDead)
        {
            return;
        }

        _currentHealth = Mathf.Clamp(_currentHealth - damageAmount, 0f, _maxHealth);
        OnDamageTaken?.Invoke();
        NotifyHealthChanged();

        if (_currentHealth > 0f)
        {
            return;
        }

        _isDead = true;
        OnPlayerDied?.Invoke();
    }

    #endregion

    #region Private Methods

    private void RegenerateHealthOverTime()
    {
        if (_isDead)
        {
            return;
        }

        if (_currentHealth >= _maxHealth)
        {
            return;
        }

        float regenAmount = _regenerationRatePerSecond * Time.deltaTime;
        _currentHealth = Mathf.Clamp(_currentHealth + regenAmount, 0f, _maxHealth);
        NotifyHealthChanged();
    }

    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
    }

    #endregion
}