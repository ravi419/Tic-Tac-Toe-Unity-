using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ParticipantHandler : MonoBehaviour
{
    [SerializeField] private Text loadingText;
    // Start is called before the first frame update
    void Start()
    {
        
    }



    public void TwoPlayer()
    {
        PlayerPrefs.SetString("NameOfGame", "TwoPlayer");
        StartCoroutine(LoadScene());
    }


    public void PlayerVsCPU()
    {
        PlayerPrefs.SetString("NameOfGame", "PlayerVsCPU");
        StartCoroutine(LoadScene());
    }

    private IEnumerator  LoadScene()
    {
        AsyncOperation loadMainScene = SceneManager.LoadSceneAsync("TicTacToe");

        while (!loadMainScene.isDone)
        {
            float progress = loadMainScene.progress * 100f;
            loadingText.text = ("Loading scene: " + progress.ToString("0.00") + "%");
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
