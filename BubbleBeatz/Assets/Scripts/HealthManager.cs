using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public Image heart1;
    public Image heart2;
    public Image heart3;

    private int currentHealth = 3;
    [SerializeField] private PlayerMovement playerHealthStuff;
    private void Start()
    {
        if (playerHealthStuff == null)
        {
            playerHealthStuff = FindAnyObjectByType<PlayerMovement>();
        }
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
        Debug.Log("Calling method from other script");
        if (playerHealthStuff != null)
        {
            playerHealthStuff.HandleDeath();
        }
        else
        {
            Debug.LogWarning("cant find reference to playermovement");
        }
    }
}