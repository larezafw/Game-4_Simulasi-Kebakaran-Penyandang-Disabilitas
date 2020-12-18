using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ToggleManager : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField]CanvasGroup toggleInfoCanvas;
    public Animator toggleAnim;
    public TextMeshProUGUI toggleInfoText;
    public TextMeshProUGUI tipsInfoText;
    bool isNarating;

    private void Start()
    {
        OnPointerExit(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isNarating) return;

        toggleAnim.SetInteger(Keyword.ANIM_PARAMETER_TOGGLESTATE, 1);
        toggleInfoCanvas.alpha = 1;

        if (PlayerPrefs.GetInt(Keyword.TOGGLE_FREELOOK) == 0) toggleInfoText.SetText("\n \nF - Free Look \n(Non-Aktif) \nV - Zoom \nB - Picked Item");
        else toggleInfoText.SetText(" \n \nF - Free Look \n(Aktif) \nV - Zoom \nB - Picked Item");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        toggleAnim.SetInteger(Keyword.ANIM_PARAMETER_TOGGLESTATE, 0);
        toggleInfoCanvas.alpha = 0;
    }

    public void UpdateTips(NarasiAudio narasi)
    {
        if (narasi.tips == null) return;

        toggleAnim.SetInteger(Keyword.ANIM_PARAMETER_TOGGLESTATE, 2);
        tipsInfoText.SetText(narasi.tips);

    }

    public void EnterNarating()
    {
        isNarating = true;
        OnPointerExit(null);
    }

    public void ExitNarating()
    {
        isNarating = false;
    }
}
