using UnityEngine;

public class MenuFunctions : MonoBehaviour
{
    public void Play()
    { 
        SceneLoaderAsync.Instance.LoadScene("Main");
    }

    public void Back()
    {
        SceneLoaderAsync.Instance.LoadScene("Menu");
    }

    public void Exit() 
    { 
        Application.Quit();
    }
}
