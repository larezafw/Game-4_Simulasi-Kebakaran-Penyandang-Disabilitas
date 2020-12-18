using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainmenuManager : MonoBehaviour
{
    public GameObject credit_UI;
    public Button[] allButton;
    public Image progressBar;
    public TextMeshProUGUI percentageText;
    List<AsyncOperation> scheduledScene;

    void Start()
    {
        AudioManager.Instance.PlayMenuSound();
        credit_UI.SetActive(false);
        progressBar.transform.parent.gameObject.SetActive(false);
        scheduledScene = new List<AsyncOperation>();
    }

    public void ShowCreditButton()
    {
        AudioManager.Instance.PlayButtonSound(); 
        credit_UI.SetActive(true);
    }

    public void HideCreditButton()
    {
        AudioManager.Instance.PlayButtonSound();
        credit_UI.SetActive(false);
    }

    public void StartGameButton()
    {
        AudioManager.Instance.PlayButtonSound();
        foreach (Button button in allButton) button.interactable = false;
        progressBar.transform.parent.gameObject.SetActive(true);
        scheduledScene.Add(SceneManager.LoadSceneAsync(Keyword.SCENE_INGAME));
        StartCoroutine(ShowProgressBar());
    }

    IEnumerator ShowProgressBar()
    {
        float totalProgress = 0;
        for(int i = 0; i < scheduledScene.Count; i++)
        {
            while (!scheduledScene[i].isDone)
            {
                totalProgress =scheduledScene[i].progress;
                progressBar.fillAmount = totalProgress;
                percentageText.SetText((totalProgress*100).ToString("0.00") + "%");
                yield return null;
            }
        }
    }
}
