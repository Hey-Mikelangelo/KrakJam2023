using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    public Animator transition;
    public float transitionTime = 1f;
    [SerializeField] private FadingScreen fadingScreen;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //LoadNextLevel();
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
