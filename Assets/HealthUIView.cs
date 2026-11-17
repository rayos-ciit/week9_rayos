using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthUIView : MonoBehaviour
{
    #region Serialized Fields

    [Header("Injected Dependencies")]
    [SerializeField] private PlayerHealthSystem _playerHealth;

    [Header("UI Elements")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private TMP_Text _healthLabel;

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        if (_playerHealth == null)
        {
            return;
        }

        _playerHealth.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        if (_playerHealth == null)
        {
            return;
        }

        _playerHealth.OnHealthChanged -= HandleHealthChanged;
    }

    #endregion

    #region Private Methods

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        UpdateSlider(currentHealth, maxHealth);
        UpdateText(currentHealth, maxHealth);
    }

    private void UpdateSlider(float currentHealth, float maxHealth)
    {
        if (_healthSlider == null || maxHealth <= 0f)
        {
            return;
        }

        _healthSlider.value = currentHealth / maxHealth;
    }

    private void UpdateText(float currentHealth, float maxHealth)
    {
        if (_healthLabel == null)
        {
            return;
        }

        int roundedCurrent = Mathf.CeilToInt(currentHealth);
        int roundedMax = Mathf.CeilToInt(maxHealth);
        _healthLabel.text = $"HP: {roundedCurrent} / {roundedMax}";
    }

    #endregion
}