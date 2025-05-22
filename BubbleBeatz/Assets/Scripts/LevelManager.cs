using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public Transform player;
    public Camera mainCamera;
    public RectTransform uiCanvas;

    public MusicManager musicManager;

    public float levelHeight = 10f;
    public float transitionDuration = 2f;

    public List<Transform> enemyGroups; // Level-based enemy groups (e.g. Enemies lv1, Enemies lv2)
    private int currentLevel = 0;
    private bool transitioning = false;

    void Start()
    {
        ActivateOnlyCurrentEnemyGroup();
    }

    void Update()
    {
        if (!transitioning && AllEnemiesInGroupDefeated())
        {
            StartCoroutine(MoveToNextLevel());
        }
    }

    bool AllEnemiesInGroupDefeated()
    {
        if (currentLevel >= enemyGroups.Count) return false;

        Transform currentGroup = enemyGroups[currentLevel];

        foreach (Transform enemy in currentGroup)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
                return false;
        }

        return true;
    }

    IEnumerator MoveToNextLevel()
    {
        transitioning = true;
        currentLevel++;

        if (currentLevel >= enemyGroups.Count)
        {
            Debug.Log("All levels complete!");
            yield break;
        }

        float targetYOffset = -levelHeight * currentLevel;

        Vector3 playerStartPos = player.position;
        Vector3 camStartPos = mainCamera.transform.position;
        Vector3 uiStartPos = uiCanvas.position;

        Vector3 playerTargetPos = playerStartPos + Vector3.down * levelHeight;
        Vector3 camTargetPos = camStartPos + Vector3.down * levelHeight;
        Vector3 uiTargetPos = uiStartPos + Vector3.down * levelHeight;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;

            player.position = Vector3.Lerp(playerStartPos, playerTargetPos, t);
            mainCamera.transform.position = Vector3.Lerp(camStartPos, camTargetPos, t);
            uiCanvas.position = Vector3.Lerp(uiStartPos, uiTargetPos, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        player.position = playerTargetPos;
        mainCamera.transform.position = camTargetPos;
        uiCanvas.position = uiTargetPos;

        ActivateOnlyCurrentEnemyGroup();

        if (musicManager != null)
        {
            musicManager.PlayNextTrack();
        }

        transitioning = false;
    }

    void ActivateOnlyCurrentEnemyGroup()
    {
        for (int i = 0; i < enemyGroups.Count; i++)
        {
            enemyGroups[i].gameObject.SetActive(i == currentLevel);
        }
    }
}