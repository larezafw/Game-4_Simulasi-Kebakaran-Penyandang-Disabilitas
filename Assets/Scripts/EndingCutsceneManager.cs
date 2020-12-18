using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingCutsceneManager : MonoBehaviour
{
    public Transform followTarget;
    public Animator cutsceneAnimator;
    public Animator playerAnim;
    public CanvasGroup panel;
    public AudioSource audioSource;
    public AudioClip[] audioClips;
    public TextMeshProUGUI narasiText;
    Cutscene currentEndingCutscene;
    Vector3 cutsceneZoom1 = new Vector3(-0.3f, 1.78f, 0.8f);
    Vector3 cutscene2Zoom2 = new Vector3(-3f, 0.3f, 0.45f);

    private void Start()
    {
        AudioManager.Instance.PlayEndingSound();
        PlayEndingCutsceneClip();
    }

    public void PlayPlayerPutAnimation() => playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISPUT, true);

    public void PlayPlayerShakeAnimation() => playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISSHAKING, true);

    public void PlayPlayerDownAnimation() => playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISDOWN, true);

    public void PlayPlayerMovingANimation()
    {
        playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISMOVING, true);
        playerAnim.SetFloat(Keyword.ANIM_PARAMETER_MOVEMENTSPEED, 3f);
    }

    public void StopPlayerAnimate()
    {
        playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISPUT, false);
        playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISSHAKING, false);
        playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISMOVING, false);
        playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISDOWN, false);
    }

    public void PlayNarationAudio()
    {
        NarasiAudio currentNaration = currentEndingCutscene.GetCurrentNaration();

        audioSource.clip = audioClips[currentNaration.clipOrder];
        audioSource.Play();

        narasiText.SetText(currentNaration.narasiContent);
        narasiText.gameObject.SetActive(true);
    }

    public void StopNarationAudio()
    {
        audioSource.Stop();
        narasiText.gameObject.SetActive(false);
    }

    public void FirstZoom()
    {
        followTarget.position = cutsceneZoom1;
    }
    public void SecondZoom()
    {
        followTarget.position = cutscene2Zoom2;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(Keyword.SCENE_MAINMENU);
        Debug.Log("ENDING");
    }

    public void PlayEndingCutsceneClip()
    {
        StopPlayerAnimate();
        StopNarationAudio();

        if (currentEndingCutscene == null) currentEndingCutscene = new EndingCutscene1();
        else if (currentEndingCutscene.cutsceneOrder == 1)
        {
            playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISDOWN, true);
            currentEndingCutscene = new EndingCutscene2();
        }
        else
        {
            Debug.Log("YOU WIN");
            return;
        }
        cutsceneAnimator.SetInteger(Keyword.ANIM_PARAMETER_CUTSCENEORDER, currentEndingCutscene.cutsceneOrder);
    }
    
}
public class EndingCutscene1 : Cutscene
{
    public EndingCutscene1()
    {
        cutsceneOrder = 1;
        cutsceneNarations = new NarasiAudio[] { new NarasiEndingCutscene_1()};
        currentClipOrder = 0;
    }
}

public class EndingCutscene2 : Cutscene
{
    public EndingCutscene2()
    {
        cutsceneOrder = 2;
        cutsceneNarations = new NarasiAudio[] { new NarasiEndingCutscene_2A(), new NarasiEndingCutscene_2B()};
        currentClipOrder = 0;
    }
}

public class NarasiEndingCutscene_1 : NarasiAudio
{
    public NarasiEndingCutscene_1()
    {
        clipOrder = 0;
        narasiContent = "Semoga seseorang dapat melihatnya. Kumohon";
    }
}

public class NarasiEndingCutscene_2A : NarasiAudio
{
    public NarasiEndingCutscene_2A()
    {
        clipOrder = 1;
        narasiContent = "Apakah ada orang di dalam?!";
    }
}

public class NarasiEndingCutscene_2B : NarasiAudio
{
    public NarasiEndingCutscene_2B()
    {
        clipOrder = 2;
        narasiContent = "Syukurlah. Sebelah sini!";
    }
}