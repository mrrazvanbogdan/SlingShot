using System.Collections;
using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int MaxNumberOfShots = 3;
    [SerializeField] private float secondsToWaitBeforeDeathCheck = 3f;
    [SerializeField] private GameObject restartScreenObject;
    [SerializeField] private SlingShotHandler slingShotHandler;
    [SerializeField] private Button nextLevelButton;

    private int usedNumberOfShots;

    private IconHandler iconHandler;

    private List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        iconHandler = FindAnyObjectByType<IconHandler>();

        Enemy[] foundEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        enemies.Clear();
        for (int i = 0; i < foundEnemies.Length; i++)
        {
            enemies.Add(foundEnemies[i]);
        }

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(false);
            nextLevelButton.onClick.AddListener(NextLevel);
        }

    }

    public void UseShot()
    {
        usedNumberOfShots++;
        iconHandler.UseShot(usedNumberOfShots);

        CheckForLastShot();
    }

    public bool HasEnoughShots()
    {
        if (usedNumberOfShots < MaxNumberOfShots)
        {
            return true;
        }

        else
        {
            return false;
        }
    }

    public void CheckForLastShot()
    { 
        if (usedNumberOfShots == MaxNumberOfShots)
        {
            StartCoroutine(CheckAfterWaitTime());
        }
    }

    private IEnumerator CheckAfterWaitTime()
    {
        yield return new WaitForSeconds(secondsToWaitBeforeDeathCheck);

        if (enemies.Count == 0)
        {
            WinGame();
        }

        else
        {
            RestartGame();
        }
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
        CheckForAllDeadEnemies();
    }

    private void CheckForAllDeadEnemies()
    {
        if (enemies.Count == 0)
        {
            WinGame();
        }
    }

    #region Win / Lose

    private void WinGame()
    {
        restartScreenObject.SetActive(true);
        slingShotHandler.enabled = false;

        if (nextLevelButton != null)
        {
            nextLevelButton.gameObject.SetActive(true);
        }

        GameObject quitButton = restartScreenObject.transform.Find("QuitButton").gameObject;
        if (quitButton != null)
        {
            quitButton.SetActive(true);
        }
    }

    public void RestartGame()
    {
        DOTween.Clear(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        DOTween.Clear(true);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        if (currentSceneIndex + 1 < totalScenes)
        {
            
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
           
            SceneManager.LoadScene(0);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion
}
