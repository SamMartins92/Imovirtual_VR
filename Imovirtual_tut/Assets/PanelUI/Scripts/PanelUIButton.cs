using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PanelUIButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
{
    public AudioClip ClickSound;
    public AudioClip HoverSound;

    public bool SoundCooldown = true;

    public Button buttonComponent;
    public Image buttonIcon;
    public TMPro.TMP_Text buttonText;
    public PanelUIManager manager;
    public UnityAction downAction;
    public UnityAction upAction;

    private bool canPlay = false;

    private void Awake()
    {
        buttonComponent = GetComponent<Button>();
        if (SoundCooldown)
            StartCoroutine(AvoidEarlyClick());
        else
            canPlay = true;
    }

    private IEnumerator AvoidEarlyClick()
    {
        yield return new WaitForSeconds(.01f);
        canPlay = true;
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {

        if (buttonComponent.interactable)
        {
            manager?.UIAudio?.PlayOneShot(ClickSound);
            downAction?.Invoke();
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (canPlay && buttonComponent.interactable)
        {
            manager?.UIAudio?.PlayOneShot(HoverSound);
        }
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        upAction?.Invoke();
    }

    void OnDestroy()
    {
        Debug.Log("Destroying");
        downAction = null;
        upAction = null;
    }
}
