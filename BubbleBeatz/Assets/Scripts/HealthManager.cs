using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image heart1;
    public Image heart2;
    public Image heart3;

    private int currentHealth = 3;

    private void Start()
    {
        UpdateHeartsUI();
    }

    public void TakeDamage()
    {
        if (currentHealth <= 0) return;

        currentHealth--;
        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            PlayerDied();
        }
    }

    void UpdateHeartsUI()
    {
        heart1.enabled = currentHealth >= 1;
        heart2.enabled = currentHealth >= 2;
        heart3.enabled = currentHealth >= 3;
    }

    void PlayerDied()
    {
        Debug.Log("Player Died!");
        // Add your death logic here (disable movement, show Game Over screen, etc.)
    }
}