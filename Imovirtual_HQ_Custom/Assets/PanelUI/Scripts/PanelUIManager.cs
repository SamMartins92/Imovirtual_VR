using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Utility;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PanelUIManager : MonoBehaviour
{
    public MenuItemManager menuManager;
    [SerializeField] private ExperimentManager expManager;

    [SerializeField] private GameObject ButtonMenuPrefab;
    [SerializeField] private GameObject ButtonActionPrefab;
    [SerializeField] private Sprite backButtonArrow;
    [SerializeField] private Sprite backButtonClose;

    [SerializeField] private GameObject RightHandLaser;
    [SerializeField] private GameObject RighHand;
    [SerializeField] private GameObject LeftHandLaser;
    [SerializeField] private GameObject LeftHand;


    [Header("Components")]
    [SerializeField] private Image TitleIcon;
    [SerializeField] private TMPro.TMP_Text TitleName;
    [SerializeField] private TMPro.TMP_Text TitleNameWBread;
    [SerializeField] private TMPro.TMP_Text TitleNameWBreadText;
    [SerializeField] private Button backButton;
    [SerializeField] private Image backButtonIcon;
    [SerializeField] private GameObject buttonsParent;
    [SerializeField] private GameObject UIParent;
    [SerializeField] private CanvasGroup canvasGroup;
    public AudioSource UIAudio;


    [Header("UI Behaviour")]
    public UIBehaviour currentBehaviour = UIBehaviour.Fixed;
    public bool spawnPositionFrontTargetCamera;
    public bool spawnRotationFacingTargetCamera;
    [SerializeField] private SmoothFollow smoothFollow;
    public Transform TargetCamera;
    public float distanceToTargetCamera;
    public float followScale = 0.315f;
    public float followRemoteScale = 0.25f;
    public Vector3 offsetForRemote;
    public AnimationCurve FadeInAnimation;
    public AnimationCurve FadeOutAnimation;
    public float FadeInTime;
    public float FadeOutTime;
    public AudioClip FadeInSound;


    public enum UIBehaviour { Fixed, LookAtTargetCamera, FollowTargetCamera, OnRemote };


    private Stack<MenuEntry> menuHistory;
    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;
    private bool isShowing = false;

    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    private SteamVR_Input_Sources selectedHand = SteamVR_Input_Sources.RightHand;


    private void Awake()
    {
        initialPosition = UIParent.transform.position;
        initialRotation = UIParent.transform.rotation;
        initialScale = UIParent.transform.localScale;

        menuHistory = new Stack<MenuEntry>();
    }

    void Start()
    {
        //SteamVR_Input.htc_viu.ActivateSecondary();

        //try
        //{
        //    selectedHand = SteamVR_Input_Sources.LeftHand;
        //    switch (currentBehaviour)
        //    {
        //        case UIBehaviour.FollowTargetCamera:
        //        case UIBehaviour.LookAtTargetCamera:
        //        case UIBehaviour.Fixed:
        //            LeftHandLaser.SetActive(true);
        //            RightHandLaser.SetActive(true);
        //            break;
        //        case UIBehaviour.OnRemote:
        //            if (selectedHand == SteamVR_Input_Sources.LeftHand)
        //                RightHandLaser.SetActive(true);
        //            else if (selectedHand == SteamVR_Input_Sources.RightHand)
        //                LeftHandLaser.SetActive(true);
        //            break;
        //    }
        //    ShowPanel(menuManager.Menu);
        //}
        //catch { } 


    }

    public bool SetupBehaviour(UIBehaviour b)
    {
        if (isShowing)
            return false;

        currentBehaviour = b;
        spawnPositionFrontTargetCamera = (currentBehaviour == UIBehaviour.FollowTargetCamera) ? true : false;
        spawnRotationFacingTargetCamera = (currentBehaviour == UIBehaviour.FollowTargetCamera) ? true : false;
        return true;

    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Comma))
        //{
        //    SetupBehaviour(UIBehaviour.Fixed);
        //}
        //if (Input.GetKeyDown(KeyCode.Period))
        //{
        //    SetupBehaviour(UIBehaviour.FollowTargetCamera);
        //}
        //if (Input.GetKeyDown(KeyCode.Minus))
        //{
        //    SetupBehaviour(UIBehaviour.OnRemote);
        //}

        if (!isShowing)
        {
            if (/*SteamVR_Input._default.inActions.Teleport.GetStateDown(SteamVR_Input_Sources.LeftHand)*/false)
            {
                selectedHand = SteamVR_Input_Sources.RightHand;
                ShowPanel(menuManager.Menu1);

                switch (currentBehaviour)
                {
                    case UIBehaviour.FollowTargetCamera:
                    case UIBehaviour.LookAtTargetCamera:
                    case UIBehaviour.Fixed:
                        LeftHandLaser.SetActive(true);
                        //RightHandLaser.SetActive(true);
                        break;
                    case UIBehaviour.OnRemote:
                        if (selectedHand == SteamVR_Input_Sources.LeftHand)
                            RightHandLaser.SetActive(true);
                        else if (selectedHand == SteamVR_Input_Sources.RightHand)
                            LeftHandLaser.SetActive(true);
                        break;
                }
            }

            if (/*SteamVR_Input._default.inActions.Teleport.GetStateDown(SteamVR_Input_Sources.RightHand)*/false)
            {
                selectedHand = SteamVR_Input_Sources.LeftHand;
                ShowPanel(menuManager.Menu1);

                switch (currentBehaviour)
                {
                    case UIBehaviour.FollowTargetCamera:
                    case UIBehaviour.LookAtTargetCamera:
                    case UIBehaviour.Fixed:
                        //LeftHandLaser.SetActive(true);
                        RightHandLaser.SetActive(true);
                        break;
                    case UIBehaviour.OnRemote:
                        if (selectedHand == SteamVR_Input_Sources.LeftHand)
                            RightHandLaser.SetActive(true);
                        else if (selectedHand == SteamVR_Input_Sources.RightHand)
                            LeftHandLaser.SetActive(true);
                        break;
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                ShowPanel(menuManager.Menu1);

            }
        }
    }

    private void FixedUpdate()
    {
        // In case of UIBehaviour.LookAtTargetCamera updates Picker rotation
        if (currentBehaviour == UIBehaviour.LookAtTargetCamera)
        {
            UIParent.transform.LookAt(TargetCamera);

        }
        else if (currentBehaviour == UIBehaviour.OnRemote && selectedHand == SteamVR_Input_Sources.LeftHand)
        {
            UIParent.transform.position = LeftHand.transform.position + Vector3.up * offsetForRemote.y;
            UIParent.transform.localPosition += Camera.main.transform.forward * offsetForRemote.x;
            UIParent.transform.LookAt(TargetCamera);
        }
        else if (currentBehaviour == UIBehaviour.OnRemote && selectedHand == SteamVR_Input_Sources.RightHand)
        {
            UIParent.transform.position = RighHand.transform.position + Vector3.up * offsetForRemote.y;
            UIParent.transform.localPosition += Camera.main.transform.forward * offsetForRemote.x;
            UIParent.transform.LookAt(TargetCamera);

        }

    }

    public void ShowPanel(MenuEntry menuRoot)
    {
        if (!isShowing && fadeInCoroutine == null)
        {
            GenerateMenu((menuRoot == null) ? menuManager.Menu1 : menuRoot);

            SpawnPickerBehaviour();

            fadeInCoroutine = StartCoroutine(FadePickerAnimationCoroutine());

            isShowing = true;
            expManager.LogEvent(@"\Menu\Open");

        }
    }

    public void HidePanel()
    {
        try
        {
            LeftHandLaser.SetActive(false);
            RightHandLaser.SetActive(false);
        }
        catch { }

        if (isShowing && fadeOutCoroutine == null)
        {
            
            if (currentBehaviour == UIBehaviour.FollowTargetCamera)
            {
                smoothFollow.enabled = false;
                TargetCamera.rotation = Quaternion.LookRotation(-TargetCamera.forward, Vector3.up);
            }
            //currentBehaviour = UIBehaviour.Fixed;

            fadeOutCoroutine = StartCoroutine(FadePickerAnimationCoroutine(false));

            isShowing = false;
            expManager.LogEvent(@"\Menu\Close");
            menuHistory.Clear();
            //ClearFileList();
        }
    }

    public void GoBack()
    {
        if (menuHistory != null && menuHistory.Count > 0)
        {
            expManager.LogEvent(@"\Menu\Back");
            MenuEntry m = menuHistory.Pop();
            GenerateMenu(m);
        }
        else
        {
            HidePanel();
        }
    }

    public void GenerateMenu(MenuEntry menuRoot)
    {
        expManager.LogEvent(@"\Menu\GoTo\" + menuRoot.ItemName);
        ClearButtons();

        UpdateBackButton();

        TitleName.text = menuRoot.ItemName;
        TitleIcon.sprite = menuRoot.ItemIcon;

        //if (menuRoot.ItemAction != null)
        //{
        //    GenerateItemAction(menuRoot, buttonsParent.transform);
        //}

        if (menuRoot.Childs != null && menuRoot.Childs.Count != 0)
        {
            foreach (MenuEntry m in menuRoot.Childs)
            {
                if (m.Childs != null && m.Childs.Count > 0)
                {
                    GenerateItemMenu(m, menuRoot, buttonsParent.transform);
                }
                else
                {
                    GenerateItemAction(m, buttonsParent.transform);
                }
            }
        }

        buttonsParent.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;


    }

    private void ClearButtons()
    {
        foreach (Transform t in buttonsParent.transform)
        {
            Destroy(t.gameObject);
        }
    }

    private void GenerateItemAction(MenuEntry m, Transform parent)
    {
        var button = GameObject.Instantiate(ButtonActionPrefab, parent);
        PanelUIButton btn = button.GetComponent<PanelUIButton>();
        btn.manager = this;

        btn.downAction =
                    delegate
                    {
                        if (isShowing)
                            m.DoDownAction();
                    };
        btn.upAction =
                    delegate
                    {
                        if (isShowing)
                            m.DoUpAction();
                    };

        if (m.ItemIcon != null)
        {
            btn.buttonIcon.sprite = m.ItemIcon;
        }
        else
        {
            btn.buttonIcon.enabled = false;
        }
        btn.buttonText.text = m.ItemName;
    }

    private void GenerateItemMenu(MenuEntry m, MenuEntry mparent, Transform parent)
    {
        var button = GameObject.Instantiate(ButtonMenuPrefab, parent);
        PanelUIButton btn = button.GetComponent<PanelUIButton>();
        btn.manager = this;

        btn.buttonComponent.onClick.AddListener(
            delegate
            {
                if (isShowing)
                {
                    menuHistory.Push(mparent);
                    GenerateMenu(m);
                }
            });

        btn.buttonText.text = m.ItemName;
        btn.buttonIcon.sprite = m.ItemIcon;
    }

    private void UpdateBackButton()
    {
        if (menuHistory != null && menuHistory.Count > 0)
        {
            backButtonIcon.sprite = backButtonArrow;
            //backButton.interactable = true;
        }
        else
        {
            backButtonIcon.sprite = backButtonClose;
            //backButton.interactable = false;
        }
    }

    // Defines how the Picker is spawned
    private void SpawnPickerBehaviour()
    {
        //Spawn posição: local ou frente camara
        if (spawnPositionFrontTargetCamera)
        {
            Vector3 targetPosition = TargetCamera.position + TargetCamera.forward * distanceToTargetCamera;
            targetPosition.y = TargetCamera.position.y;
            UIParent.transform.position = targetPosition;
        }
        else
        {
            UIParent.transform.position = initialPosition;
        }


        //Spawn rotação: local ou lookat camara
        if (spawnRotationFacingTargetCamera)
        {
            UIParent.transform.LookAt(TargetCamera);
        }
        else
        {
            UIParent.transform.rotation = initialRotation;
        }

        switch (currentBehaviour)
        {
            case UIBehaviour.FollowTargetCamera:

                //Rotate FollowTarget by 180 degrees
                TargetCamera.rotation = Quaternion.LookRotation(-TargetCamera.forward, Vector3.up);
                smoothFollow.target = TargetCamera;
                smoothFollow.distance = distanceToTargetCamera;
                smoothFollow.enabled = true;

                UIParent.transform.localScale = new Vector3(followScale, followScale, followScale);
                break;

            case UIBehaviour.OnRemote:
                UIParent.transform.localScale = new Vector3(followRemoteScale, followRemoteScale, followRemoteScale);

                break;

            default:
                UIParent.transform.localScale = initialScale;
                UIParent.transform.position = initialPosition;
                UIParent.transform.rotation = initialRotation;
                break;

        }

    }

    // Animates the Picker UI to fade in or out
    private IEnumerator FadePickerAnimationCoroutine(bool fadeIn = true)
    {

        if (fadeIn)
        {
            yield return new WaitUntil(() => fadeOutCoroutine == null);

            UIParent.SetActive(true);

            if (canvasGroup.alpha != 1)
            {


                canvasGroup.alpha = 0;
                //cg.interactable = false;


                UIAudio.PlayOneShot(FadeInSound);
                float t = 0;
                while (canvasGroup.alpha < 1)
                {
                    t += Time.deltaTime;
                    canvasGroup.alpha = FadeInAnimation.Evaluate(t / FadeInTime);
                    if (canvasGroup.alpha > 1) canvasGroup.alpha = 1;
                    yield return null;
                }
            }

            fadeInCoroutine = null;
        }
        else
        {
            yield return new WaitUntil(() => fadeInCoroutine == null);

            if (canvasGroup.alpha != 0)
            {

                canvasGroup.alpha = 1;
                UIAudio.PlayOneShot(FadeInSound);
                float t = 0;
                while (canvasGroup.alpha > 0)
                {
                    t += Time.deltaTime;
                    canvasGroup.alpha = FadeOutAnimation.Evaluate(t / FadeOutTime);
                    if (canvasGroup.alpha < 0) canvasGroup.alpha = 0;
                    yield return null;
                }
            }

            UIParent.SetActive(false);

            fadeOutCoroutine = null;
        }
        //cg.interactable = true;
    }

}
