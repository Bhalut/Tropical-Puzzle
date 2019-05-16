using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderAsync : Singleton<SceneLoaderAsync>
{
    // Loading Progress: private setter, public getter
    private float _loadingProgress;
    public float LoadingProgress { get { return _loadingProgress; } }

    public void LoadScene()
    {
        // kick-off the one co-routine to rule them all
        StartCoroutine(LoadScenesInOrder("Main"));
    }

    public IEnumerator LoadScenesInOrder(string scene)
    {
        yield return new WaitForSeconds(0.25f);
        // LoadSceneAsync() returns an AsyncOperation, 
        // so will only continue past this point when the Operation has finished
        yield return SceneManager.LoadSceneAsync("Loading");

        // as soon as we've finished loading the loading screen, start loading the game scene
        yield return StartCoroutine(LoadScene(scene));
    }

    private IEnumerator LoadScene(string sceneName)
    {
        var asyncScene = SceneManager.LoadSceneAsync(sceneName);

        // this value stops the scene from displaying when it's finished loading
        asyncScene.allowSceneActivation = false;

        while (!asyncScene.isDone)
        {
            // loading bar progress
            _loadingProgress = Mathf.Clamp01(asyncScene.progress / 0.9f) * 100;

            // scene has loaded as much as possible, the last 10% can't be multi-threaded
            if (asyncScene.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f);
                // we finally show the scene
                asyncScene.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
