using UnityEngine;

public class MenuFunctions : MonoBehaviour
{
    public void Play()
    { 
        SceneLoaderAsync.Instance.LoadScene();
    }

    public void Back()
    {
        StartCoroutine(SceneLoaderAsync.Instance.LoadScenesInOrder("Menu"));
    }

    public void Exit() 
    { 
        Application.Quit();
    }
}
