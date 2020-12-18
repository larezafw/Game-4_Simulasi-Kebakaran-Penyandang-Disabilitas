using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class PlayerController : MonoBehaviour
{
    // MOTION
    [SerializeField] Animator playerAnim;
    [SerializeField] Rigidbody rb;
    [SerializeField] float maxRigidVelocity;
    Vector3 movementInput;

    // CAMERA SETUP
    public CinemachineVirtualCamera virtualCamera;
    Cinemachine3rdPersonFollow thirdPrsonCamera;
    public Transform followTransform;
    bool isLookClosely;
    int cameraSetupOrder;
    float diffVerticalArm;
    float diffCameraDistance;
    float diffCameraSide;

    // HYDRAN
    public Collider FireCollider;
    public ParticleSystem PS_Water;
    public Slider hydranTankSlider;
    HydranTankProperty hydranTank;
    float usingHydran;
    bool isHavingHydran;

    // ENEGRY
    public Slider playerEnergySlider;
    public Gradient energySliderGradien;
    public GameoverManager gameoverManager;
    Energy playerEnergy;
    bool isWearingMask;

    // PICK ITEM
    public GameObject pickableItemInfo;
    public Material pickableMaterial;
    Material propertyMaterial;
    Collider propertyCollider;

    public PickedItemUI pickedItemUI;
    public GameObject HydranObject;
    public GameObject maskObject;
    List<Item> pickedItemList;

    // NARATION
    public AudioSource narationAudioSources;
    public AudioClip[] narationAudioClips;
    public TextMeshProUGUI narationText;
    Queue<NarasiAudio> narationInSchedule;
    NarasiAudio currentNaration;
    bool gameOver;
    bool isNarating;
    float naratingTimer;

    public StoryLineManager storyLineManager;
    public GameObject mainFeatures;

    // TOGGLE INFO & FIRE INDICATOR
    public ToggleManager toggleManager;
    public CanvasGroup fireIndicator;

    #region UNITY CALLBACK

    private void Start()
    {
        AudioManager.Instance.PlayIngameSound();
        SetupEnergySlider();
        SetupHydranSlider();

        thirdPrsonCamera = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();

        fireIndicator.alpha = 0.2f;

        HydranObject.SetActive(false);
        maskObject.SetActive(false);

        pickableItemInfo.SetActive(false);
        pickedItemList = new List<Item>();

        narationText.gameObject.SetActive(false);
        narationInSchedule = new Queue<NarasiAudio>();
        currentNaration = null;

        // Physic Ignore
        Physics.IgnoreLayerCollision(15, 14);
        Physics.IgnoreLayerCollision(15, 11);
        Physics.IgnoreLayerCollision(15, 10);
        Physics.IgnoreLayerCollision(15, 8);
        Physics.IgnoreLayerCollision(11, 11);
        Physics.IgnoreLayerCollision(11, 8);
        Physics.IgnoreLayerCollision(10, 2);
        Physics.IgnoreLayerCollision(10, 8);
        Physics.IgnoreLayerCollision(10, 10);
        Physics.IgnoreLayerCollision(9, 9);
        Physics.IgnoreLayerCollision(8, 8);
        Physics.IgnoreLayerCollision(2, 2);

        PlayCutscene(new Cutscene_1());
    }

    private void Update()
    {
        // PLAYER ENERGY BEHAVIOUR
        if (playerEnergy.isFaulted())
        {
            if (!gameOver)
            {
                gameOver = true;
                StopNarationAudio();
                gameoverManager.SetToGameover();
                
            }
            return;
        }
        playerEnergy.DecreaseOverTime(isWearingMask, isNarating);
        UpdateEnergySlider();

        // NARATION
        if (naratingTimer > 0 && isNarating) naratingTimer -= Time.deltaTime;
        else if (isNarating)
        {
            if (currentNaration != null)
            {
                storyLineManager.NarationEventCheck(currentNaration);
                toggleManager.UpdateTips(currentNaration);
            }

            if (narationInSchedule.Count > 0) PlayNarationAudio(narationInSchedule.Dequeue());
            else StopNarationAudio();
        }

        // HYDRAN USEMENT
        AudioManager.Instance.PlayHydranSound(usingHydran > 0 && isHavingHydran);
        var PS_Water_Emision = PS_Water.emission;
        if (usingHydran > 0 && hydranTank?.PercentageValue() > 0 && isHavingHydran)
        {
            playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISSPRAYING, true);

            PS_Water_Emision.rateOverTime = 30f;
            hydranTank.UseHydran();
            UpdateHydranSlider();
        }
        else
        {
            playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISSPRAYING, false);
            PS_Water_Emision.rateOverTime = 0;
        }
        
        // PLAYER MOTION
        followTransform.transform.rotation *= Quaternion.AngleAxis(movementInput.x * 100f * Time.deltaTime, Vector3.up);
        if (movementInput.z != 0 || usingHydran != 0)
        {
            transform.parent.rotation = Quaternion.Euler(0f, followTransform.rotation.eulerAngles.y, 0);
            followTransform.localEulerAngles = Vector3.zero;
        }

        if (movementInput.z != 0 && usingHydran == 0)
        {
            isLookClosely = false;
            playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISMOVING, true);
            playerAnim.SetFloat(Keyword.ANIM_PARAMETER_MOVEMENTSPEED, Mathf.Lerp(0.5f, 4f, playerEnergy.SpeedPercentageValue(isNarating)));

            Vector3 movementForce = new Vector3(0f, 0f, movementInput.z * playerEnergy.SpeedByEnergyValue(isNarating) * Time.deltaTime);
            transform.parent.Translate(movementForce);
        }
        else playerAnim.SetBool(Keyword.ANIM_PARAMETER_ISMOVING, false);

        // CAMERA SETUP
        if (isLookClosely) CameraAtLookClosely();
        else if (movementInput.z != 0 || usingHydran != 0 && isHavingHydran) CameraAtAction();
        else CameraAtLookAround();
    }

    private void FixedUpdate()
    {
        LimitingVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        // PICKABLE ITEM
        if (other.gameObject.CompareTag(Keyword.TAG_PICKABLE_ITEM))
        {
            pickableItemInfo.SetActive(true);

            propertyCollider = other;
            propertyMaterial = other.GetComponent<Renderer>().material;
            other.GetComponent<Renderer>().material = pickableMaterial;
        }

        // NARATION
        else if (other.gameObject.CompareTag(Keyword.TAG_NARATION_TRIGGER))
        {
            other.enabled = false;

            if (other.gameObject.name == Keyword.NARASI_TRIGGER_1B) ScheduleNaration(new Narasi_1B());
            else if (other.gameObject.name == Keyword.NARASI_TRIGGER_3A) ScheduleNaration(new Narasi_3A());
            else if (other.gameObject.name == Keyword.NARASI_TRIGGER_4B) ScheduleNaration(new Narasi_4B());
            else if (other.gameObject.name == Keyword.NARASI_TRIGGER_4C) ScheduleNaration(new Narasi_4C());
            else Debug.Log("Narasi detection Error");
        }

        // CUTSCENE
        else if (other.gameObject.CompareTag(Keyword.TAG_CUTSCENE_TRIGGER))
        {
            other.enabled = false;

            if (other.name == Keyword.CUTSCENE_2) PlayCutscene(new Cutscene_2());
            else if (other.name==(Keyword.CUTSCENE_ENDING)) storyLineManager.PlayEndingCutscene();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(Keyword.FIRE_PARTICLES))
        {
            playerEnergy.HitFire();

            if (fireIndicator.alpha < 1) fireIndicator.alpha = 1f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(Keyword.TAG_PICKABLE_ITEM))
        {
            pickableItemInfo.SetActive(false);

            propertyCollider = null;
            other.GetComponent<Renderer>().material = propertyMaterial;
        }
        else if (other.gameObject.CompareTag(Keyword.FIRE_PARTICLES))
        {
            fireIndicator.alpha = 0.2f;
        }
    }

    void OnMove(InputValue inputValue)
    {
        Vector2 inputVector = inputValue.Get<Vector2>();
        movementInput = new Vector3(inputVector.x, 0f, inputVector.y);
    }

    void OnLookClosely()
    {
        if (movementInput.z == 0)
        {
            isLookClosely = true;
            toggleManager.OnPointerExit(null);
        }
    }

    void OnFreeLook()
    {
        int toggleFreeLook = PlayerPrefs.GetInt(Keyword.TOGGLE_FREELOOK,0);
        if (toggleFreeLook == 0) toggleFreeLook = 1;
        else toggleFreeLook = 0;

        PlayerPrefs.SetInt(Keyword.TOGGLE_FREELOOK, toggleFreeLook);
        virtualCamera.GetComponent<FreeLookCamera>().SetMode(toggleFreeLook);
        toggleManager.OnPointerExit(null);
    }

    void OnFire(InputValue inputValue)
    {
        usingHydran = inputValue.Get<float>();
    }

    void OnPick()
    {
        // NO ITEM SELECTED
        if (propertyCollider == null)
        {
            Debug.Log("No item selected!");
            return;
        }

        // ITEM SELECTED
        GameObject itemObject = propertyCollider.gameObject;

        if (itemObject.name == Keyword.ITEM_HYDRAN && !isHavingHydran)
        {
            Debug.Log("Hydran picked!");
            pickedItemList.Add(new Item_Hydran());
            isHavingHydran = true;
            HydranObject.SetActive(true);
            OnTriggerExit(propertyCollider);
            Destroy(itemObject);
            SetupHydranSlider();

            ScheduleNaration(new Narasi_4C());
        }
        else if (itemObject.name == Keyword.ITEM_MASK && !isWearingMask)
        {
            Debug.Log("Mask picked");
            pickedItemList.Add(new Item_Mask());
            isWearingMask = true;
            OnTriggerExit(propertyCollider);
            Destroy(itemObject);

            ScheduleNaration(new Narasi_2B());
            ScheduleNaration(new Narasi_2C());
        }
        else if (itemObject.name == Keyword.ITEM_KAYU)
        {
            Debug.Log("Kayu Picked");
            pickedItemList.Add(new Item_Kayu());
            OnTriggerExit(propertyCollider);
            Destroy(itemObject);

            ScheduleNaration(new Narasi_3B());
        }
        else Debug.Log("I dont need it right now");
    }

    void OnShowItem()
    {
        pickedItemUI.Setup(pickedItemList);
        toggleManager.OnPointerExit(null);
    }

    #endregion

    #region PRIVATE FUNCTION
    void ResetInputValue()
    {
        pickedItemUI.HidePickedItem();
        toggleManager.OnPointerExit(null);
        movementInput = new Vector3();
        usingHydran = 0;
        isLookClosely = false;
    }

    void CameraAtLookClosely()
    {
        float targetVerticalArm = 0.4f;
        float targetCameraDistance = 3.75f;
        float targetCameraSide = 1f;
        if (cameraSetupOrder != 0)
        {
            cameraSetupOrder = 0;

            diffVerticalArm = thirdPrsonCamera.VerticalArmLength - targetVerticalArm;
            diffCameraDistance = thirdPrsonCamera.CameraDistance - targetCameraDistance;
            diffCameraSide = thirdPrsonCamera.CameraSide - targetCameraSide;
        }

        if (thirdPrsonCamera.VerticalArmLength > targetVerticalArm) thirdPrsonCamera.VerticalArmLength -= diffVerticalArm * Time.deltaTime;
        if (thirdPrsonCamera.CameraDistance > targetCameraDistance) thirdPrsonCamera.CameraDistance -= diffCameraDistance * Time.deltaTime;
        if (thirdPrsonCamera.CameraSide < targetCameraSide) thirdPrsonCamera.CameraSide -= diffCameraSide * Time.deltaTime;

        CameraConfiguration(targetVerticalArm, targetCameraDistance);
    }

    private void CameraAtLookAround()
    {
        float targetVerticalArm = 1.75f;
        float targetCameraDistance = 6.5f;
        float targetCameraSide = 1f;
        if (cameraSetupOrder != 1)
        {
            cameraSetupOrder = 1;

            diffVerticalArm = thirdPrsonCamera.VerticalArmLength - targetVerticalArm;
            diffCameraDistance = thirdPrsonCamera.CameraDistance - targetCameraDistance;
            diffCameraSide = thirdPrsonCamera.CameraSide - targetCameraSide;
        }
        if (thirdPrsonCamera.VerticalArmLength < targetVerticalArm) thirdPrsonCamera.VerticalArmLength -= diffVerticalArm * Time.deltaTime;
        if (thirdPrsonCamera.CameraDistance < targetCameraDistance) thirdPrsonCamera.CameraDistance -= diffCameraDistance * Time.deltaTime;
        if (thirdPrsonCamera.CameraSide < targetCameraSide) thirdPrsonCamera.CameraSide -= diffCameraSide * Time.deltaTime;

        CameraConfiguration(targetVerticalArm, targetCameraDistance);
    }

    void CameraAtAction()
    {
        float targetVerticalArm = 1f;
        float targetCameraDistance = 4.5f;
        float targetCameraSide = 0.6f;
        if (cameraSetupOrder != 2)
        {
            cameraSetupOrder = 2;

            diffVerticalArm = thirdPrsonCamera.VerticalArmLength - targetVerticalArm;
            diffCameraDistance = thirdPrsonCamera.CameraDistance - targetCameraDistance;
            diffCameraSide = thirdPrsonCamera.CameraSide - targetCameraSide;
        }
        if (thirdPrsonCamera.VerticalArmLength < targetVerticalArm || thirdPrsonCamera.VerticalArmLength > targetVerticalArm) 
            thirdPrsonCamera.VerticalArmLength -= diffVerticalArm * Time.deltaTime;
        if (thirdPrsonCamera.CameraDistance < targetCameraDistance || thirdPrsonCamera.CameraDistance > targetCameraDistance) 
            thirdPrsonCamera.CameraDistance -= diffCameraDistance * Time.deltaTime;
        if (thirdPrsonCamera.CameraSide > targetCameraSide) thirdPrsonCamera.CameraSide -= diffCameraSide * Time.deltaTime;

        CameraConfiguration(targetVerticalArm, targetCameraDistance);
    }

    void CameraConfiguration(float targetVerticalArm, float targetCameraDistance)
    {
        if (diffVerticalArm >= 0 && thirdPrsonCamera.VerticalArmLength < targetVerticalArm) thirdPrsonCamera.VerticalArmLength = targetVerticalArm;
        else if (diffVerticalArm < 0 && thirdPrsonCamera.VerticalArmLength > targetVerticalArm) thirdPrsonCamera.VerticalArmLength = targetVerticalArm;

        if (diffCameraDistance >= 0 && thirdPrsonCamera.CameraDistance < targetCameraDistance) thirdPrsonCamera.CameraDistance = targetCameraDistance;
        else if (diffCameraDistance < 0 && thirdPrsonCamera.CameraDistance > targetCameraDistance) thirdPrsonCamera.CameraDistance = targetCameraDistance;
    }

    void PlayCutscene(Cutscene nextCutscene)
    {
        ResetInputValue();
        StopNarationAudio();
        storyLineManager.PlayCutscene(nextCutscene);
    }

    public void ScheduleNaration(NarasiAudio nextNaration)
    {
        if ((narationAudioSources.isPlaying && narationAudioSources.clip == narationAudioClips[nextNaration.clipOrder])
            || narationInSchedule.Contains(nextNaration)) return;
        narationInSchedule.Enqueue(nextNaration);
        isNarating = true;

        toggleManager.EnterNarating();
        AudioManager.Instance.FireSoundAdjustment(false);
    }

    void PlayNarationAudio(NarasiAudio nextNaration)
    {
        currentNaration = nextNaration;
        narationAudioSources.clip = narationAudioClips[nextNaration.clipOrder];
        narationAudioSources.Play();
        narationText.SetText(nextNaration.narasiContent);
        narationText.gameObject.SetActive(true);
        naratingTimer = narationAudioSources.clip.length + 0.5f;
    }

    void StopNarationAudio()
    {
        currentNaration = null;
        narationInSchedule = new Queue<NarasiAudio>();
        narationAudioSources.Stop();
        narationText.gameObject.SetActive(false);
        isNarating = false;

        toggleManager.ExitNarating();
        AudioManager.Instance.FireSoundAdjustment(true);
    }

    void SetupHydranSlider()
    {
        if (!isHavingHydran) hydranTankSlider.gameObject.SetActive(false);
        else
        {
            hydranTankSlider.gameObject.SetActive(true);
            hydranTank = new HydranTankProperty();
            UpdateHydranSlider();
        }
    }

    void UpdateHydranSlider()
    {
        hydranTankSlider.value = hydranTank.PercentageValue();
    }

    void SetupEnergySlider()
    {
        playerEnergy = new Energy(200f);
        UpdateEnergySlider();
    }

    void UpdateEnergySlider()
    {
        playerEnergySlider.value = playerEnergy.EnergyPercentageValue();
        playerEnergySlider.image.color = energySliderGradien.Evaluate(playerEnergySlider.value);
    }

    void LimitingVelocity()
    {
        if (rb.velocity.magnitude >= maxRigidVelocity) rb.velocity = rb.velocity.normalized * maxRigidVelocity;
    }
    #endregion
}