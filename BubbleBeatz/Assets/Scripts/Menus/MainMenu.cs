using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(2);
    }

    public void ToOptions()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Back()
    {
        SceneManager.LoadScene(0);

    }
    //<<<<<<< Updated upstream

    public void Death()
    {
        SceneManager.LoadScene(3);

    }

    public void Victory()
    {
        SceneManager.LoadScene(4);
    }
        //=======
        public void LoadHTP()
        {
            SceneManager.LoadScene(3);
        }
        public void backToMenu()
        {
            SceneManager.LoadScene(0);

        }


    }

