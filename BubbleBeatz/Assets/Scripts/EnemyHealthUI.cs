using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    public EnemyBubbleBobbleAI enemy; // Reference to the enemy script
    public Image[] hearts; // Assign 3 heart images in Inspector
    public Sprite fullHeart;
    public Sprite emptyHeart;
    public Sprite invulnerableHeart; // Optional: for invulnerable state visual

    private void Start()
    {
        if (enemy == null)
        {
            enemy = GetComponentInParent<EnemyBubbleBobbleAI>();
        }

        UpdateHearts(enemy.maxHealth, enemy.CurrentHealth, enemy.isFinalPhase);
    }

    private void Update()
    {
        UpdateHearts(enemyMaxHealth: enemy.maxHealth, currentHealth: enemy.CurrentHealth, isInvulnerable: enemy.isFinalPhase);
    }

    void UpdateHearts(int enemyMaxHealth, int currentHealth, bool isInvulnerable)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = isInvulnerable ? invulnerableHeart : fullHeart;
                hearts[i].enabled = true;
            }
            else if (i < enemyMaxHealth)
            {
                hearts[i].sprite = emptyHeart;
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
}