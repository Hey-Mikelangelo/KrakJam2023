using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class levelLoaderScript : MonoBehaviour
{
    public Animator transition;
    [SerializeField] private FadingScreen fadingScreen;
    public float transitionTime = 1f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LoadNextLevel();
        }

    }
    public void LoadNextLevel()
    {
        LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private async void LoadLevel(int levelIndex)
    {
        await fadingScreen.FadeToBlockingView(transitionTime);
        SceneManager.LoadScene(levelIndex);
        await fadingScreen.FadeFromBlockingView(transitionTime);
    }
}
