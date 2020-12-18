using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverManager : MonoBehaviour
{
    public Animator gameoverAnim;
    public void SetToGameover()
    {
        AudioManager.Instance.PlayLoseSound();
        gameoverAnim.SetBool(Keyword.ANIM_PARAMETER_ISGAMEOVER, true);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(Keyword.SCENE_MAINMENU);
    }
}
