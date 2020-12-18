using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using System;
using UnityEngine.SceneManagement;

public class StoryLineManager : MonoBehaviour
{
    // CUTSCENE CAMERA SETUP
    public GameObject mainFeature;
    public GameObject playerController;
    public Transform followTarget;
    public Transform[] cutsceneAreaPoint;
    
    public CinemachineVirtualCamera virtualCamera;
    Cinemachine3rdPersonFollow thirdPrsonCamera;
    bool isPositioningCamera;
    bool calculated;
    float diffCameraDistance;
    float diffCameraVerticalArm;
    const float targetCameraVerticalArm = 0.4f;
    const float targetCameraDistance = 1.25f;
    const float cutsceneCameraVerticalArm = 0.2f;
    const float cutsceneCameraDistance = 0f;
    const float cutsceneCameraSide = 0.5f;

    // CUTSCENE SETUP
    Cutscene currentCutscene;
    bool isStartingCutscene;
    public CanvasGroup cutscenePanel;
    public AudioSource cutsceneAudioSource;
    public TextMeshProUGUI narasiText;
    public Animator cutsceneAnim;
    public AudioClip[] cutsceneAudioClip;

    // EVENT
    public Collider[] eventTriggerArea;
    public GameObject bathroomBlocker;
    public GameObject maskObject;

    private void Start()
    {
        foreach (Collider col in eventTriggerArea) col.enabled = false;
    }
    private void Update()
    {
        if (isStartingCutscene) StartingCutscene();
        if (isPositioningCamera) SetCutsceneCamera();
    }

    #region NARATION FUNCTION
    public void NarationEventCheck(NarasiAudio narasi)
    {
        if (narasi.clipOrder == 0) eventTriggerArea[0].enabled = true;
        else if (narasi.clipOrder == 1) eventTriggerArea[1].enabled = true;
        else if (narasi.clipOrder == 2) eventTriggerArea[2].enabled = true;
        else if (narasi.clipOrder == 4) eventTriggerArea[3].enabled = true;
        else if (narasi.clipOrder == 5) eventTriggerArea[4].enabled = true;
        else if (narasi.clipOrder == 6) eventTriggerArea[5].enabled = true;
        else if (narasi.clipOrder == 7) Debug.Log("YOU WIN");

        if (narasi.clipOrder == 3) maskObject.SetActive(true);
        else if (narasi.clipOrder == 4) bathroomBlocker.SetActive(false);
    }
    #endregion

    #region CUTSCENE FUNCTION

    public void PlayEndingCutscene()
    {
        cutscenePanel.alpha = 1;
        SceneManager.LoadScene(Keyword.SCENE_ENDING);
    }

    public void PlayCutscene(Cutscene nextCutscene)
    {
        followTarget.parent = transform;
        mainFeature.SetActive(false);
        narasiText.gameObject.SetActive(false);
        playerController.SetActive(false);

        thirdPrsonCamera = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        thirdPrsonCamera.VerticalArmLength = cutsceneCameraVerticalArm;
        thirdPrsonCamera.CameraDistance = cutsceneCameraDistance;
        thirdPrsonCamera.CameraSide = cutsceneCameraSide;

        currentCutscene = nextCutscene;
        transform.parent.position = cutsceneAreaPoint[currentCutscene.areaPointOrder].position;
        transform.parent.rotation = cutsceneAreaPoint[currentCutscene.areaPointOrder].rotation;
        cutsceneAnim.SetInteger(Keyword.ANIM_PARAMETER_CUTSCENEORDER, currentCutscene.cutsceneOrder);
        cutscenePanel.alpha = 1;
        isStartingCutscene = true;
    }

    private void StartingCutscene()
    {
        if (cutscenePanel.alpha > 0) cutscenePanel.alpha -= Time.deltaTime/1.5f;
        else isStartingCutscene = false;
    }

    public void PlayCutsceneNaration()
    {
        NarasiAudio currentNaration= currentCutscene.GetCurrentNaration();
        cutsceneAudioSource.clip = cutsceneAudioClip[currentNaration.clipOrder];
        cutsceneAudioSource.Play();
        narasiText.SetText(currentNaration.narasiContent);
        narasiText.gameObject.SetActive(true);
    }

    public void NarationTransition()
    {
        narasiText.gameObject.SetActive(false);
    }

    public void cutsceneFinished()
    {
        cutsceneAnim.SetInteger(Keyword.ANIM_PARAMETER_CUTSCENEORDER, 0);
        isPositioningCamera = true;
    }

    void SetCutsceneCamera()
    {
        if (!calculated)
        {
            calculated = true;
            diffCameraDistance = thirdPrsonCamera.CameraDistance - targetCameraDistance;
            diffCameraVerticalArm = thirdPrsonCamera.VerticalArmLength - targetCameraVerticalArm;
        }

        if (thirdPrsonCamera.CameraDistance < targetCameraDistance)
        {
            thirdPrsonCamera.VerticalArmLength -= diffCameraVerticalArm * Time.deltaTime * 8;
            thirdPrsonCamera.CameraDistance -= diffCameraDistance * Time.deltaTime * 8;
        }
        else SetControllerToActive();
    }

    void SetControllerToActive()
    {
        isPositioningCamera = false;
        followTarget.parent = playerController.transform;
        playerController.SetActive(true);
        mainFeature.SetActive(true);
        if (currentCutscene.cutsceneOrder == 1) playerController.GetComponent<PlayerController>().ScheduleNaration(new Narasi_1A());
        else if (currentCutscene.cutsceneOrder == 2) playerController.GetComponent<PlayerController>().ScheduleNaration(new Narasi_2A());
    }
    #endregion
}

public class NarasiAudio
{
    public int clipOrder { get; protected set; }
    public string narasiContent { get; protected set; }
    public string tips { get; protected set; }
}

public class Narasi_1A : NarasiAudio
{
    public Narasi_1A()
    {
        clipOrder = 0;
        narasiContent = "Aku harus bergegas menuju Lift";
        tips = "Segera menuju lift";
    }
}

public class Narasi_1B : NarasiAudio
{
    public Narasi_1B()
    {
        clipOrder = 1;
        narasiContent = "Lift ini tidak berfungsi. Sebaiknya aku segera menuju tangga untuk meminta bantuan";
        tips = "Menuju menuju tangga";
    }
}

public class Narasi_2A : NarasiAudio
{
    public Narasi_2A()
    {
        clipOrder = 2;
        narasiContent = "Hal pertama yang harus aku lakukan adalah menutupi hidungku";
        tips = "Temukan sesuatu untuk menutupi hidung";
    }
}

public class Narasi_2B : NarasiAudio
{
    public Narasi_2B()
    {
        clipOrder = 3;
        narasiContent = "Sepertiya kain ini dapat aku gunakan ... Hufft. sekarang jauh lebih baik";
    }
}

public class Narasi_2C : NarasiAudio
{
    public Narasi_2C()
    {
        clipOrder = 4;
        narasiContent = "Oke, sekarang apa yang \n harus aku lakukan?";
        tips = "Coba kelilingi setiap ruangan";
    }
}

public class Narasi_3A : NarasiAudio
{
    public Narasi_3A()
    {
        clipOrder = 5;
        narasiContent = "Fentilasi udara. Seseorang pernah jahil membuang sampah sembarangan disana. Mungkin aku dapat menggunakannya untuk meminta bantuan";
        tips = "Temukan sesuatu untuk meminta bantuan";
    }
}

public class Narasi_3B : NarasiAudio
{
    public Narasi_3B()
    {
        clipOrder = 6;
        narasiContent = "Tampaknya kayu ini akan cukup untuk aku masukkan. Lebih baik aku mencobanya";
        tips = "Kembali ke tempat fentilasi udara";
    }
}

public class Narasi_4A : NarasiAudio
{
    public Narasi_4A()
    {
        clipOrder = 7;
        narasiContent = "Ibu.. Ayah.. Aku sangat takut. Hmmp.. tidak-tidak, aku tidak boleh menyerah";
    }
}

public class Narasi_4B : NarasiAudio
{
    public Narasi_4B()
    {
        clipOrder = 8;
        narasiContent = "Tidak. Aku tidak bisa menggunakan tangga. \nLebih baik mencari cara lain";
    }
}

public class Narasi_4C : NarasiAudio
{
    public Narasi_4C()
    {
        clipOrder = 9;
        narasiContent = "Sebuah hydran. Sepertinya sekaranglah saatnya aku menerapkan apa yang guruku sudah ajarkan";
    }
}

public class Cutscene
{
    public int areaPointOrder { get; protected set; }
    public int cutsceneOrder { get; protected set; }
    public NarasiAudio[] cutsceneNarations { get; protected set; }
    public int currentClipOrder { get; protected set; }

    public NarasiAudio GetCurrentNaration()
    {
        NarasiAudio result = cutsceneNarations[currentClipOrder];
        currentClipOrder += 1;
        return result;
    }
}

public class Cutscene_1 : Cutscene
{
    public Cutscene_1()
    {
        areaPointOrder = 0;
        cutsceneOrder = 1;
        cutsceneNarations = new NarasiAudio[] { new NarasiCutscene_1A(), new NarasiCutscene_1B() };
        currentClipOrder = 0;
    }
}

public class Cutscene_2 : Cutscene
{
    public Cutscene_2()
    {
        areaPointOrder = 1;
        cutsceneOrder = 2;
        cutsceneNarations = new NarasiAudio[] { new NarasiCutscene_2A(), new NarasiCutscene_2B() };
        currentClipOrder = 0;
    }
}

public class NarasiCutscene_1A : NarasiAudio
{
    public NarasiCutscene_1A()
    {
        clipOrder = 0;
        narasiContent = "Apa yang terjadi?";
    }
}

public class NarasiCutscene_1B : NarasiAudio 
{ 
    public NarasiCutscene_1B()
    {
        clipOrder = 1;
        narasiContent = "Hal terakhir yang aku ingat adalah sebuah ledakan. Tampaknya ini sangat buruk";
    }
}

public class NarasiCutscene_2A : NarasiAudio
{
    public NarasiCutscene_2A()
    {
        clipOrder = 2;
        narasiContent = "Halo?! Ada orang di bawah sana?! \nAku memerlukan bantuan";
    }
}

public class NarasiCutscene_2B: NarasiAudio
{
    public NarasiCutscene_2B()
    {
        clipOrder = 3;
        narasiContent = "OK.. Tidak apa.. Tenang.. Jangan panik. Semuanya akan baik-baik saja";
    }
}

