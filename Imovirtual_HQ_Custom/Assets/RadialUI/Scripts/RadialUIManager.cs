using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Utility;
using Valve.VR;
using static PanelUIManager;

public class RadialUIManager : MonoBehaviour
{
    [Header("RadialUI Prefab")]
    [SerializeField] GameObject prefabRadialUIElement;
    [Header("RadialUI Components")]
    [SerializeField] TMPro.TMP_Text BackgroundLabelTextComponent;
    [SerializeField] private Vector2 textOffset;
    [SerializeField] Transform IndicatorParentTransform;
    [SerializeField] Image IndicatorImageComponent;
    [SerializeField] Transform RadialUIElementGroupTransform;
    [SerializeField] GameObject UIParent;
    [SerializeField] CanvasGroup canvasGroupComponent;
    public AudioSource UIAudioComponent;
    [Header("Item Manager")]
    [SerializeField] MenuItemManager itemManager;
    //[SerializeField] private ExperimentManager expManager;
    [Header("Extra Sprites")]
    [SerializeField] Sprite defaultSprite;
    [SerializeField] Sprite backSprite;
    [SerializeField] Sprite closeSprite;

    [Header("UI Behaviour")]
    public UIBehaviour currentBehaviour = UIBehaviour.Fixed;
    public bool spawnPositionFrontTargetCamera = false;
    public bool spawnRotationFacingTargetCamera = false;
    [SerializeField] GameObject RighHand;
    [SerializeField] GameObject LeftHand;
    [SerializeField] SmoothFollow smoothFollow;
    public Transform TargetCamera;
    public float distanceToTargetCamera;
    public float followScale = 0.315f;
    public float followRemoteScale = 0.25f;
    public Vector3 offsetForRemote;
    public bool invertMenu = true;
    public AnimationCurve FadeInAnimation;
    public AnimationCurve FadeOutAnimation;
    public float FadeInTime;
    public float FadeOutTime;
    public bool DoIndicatorAnimation = false;
    public AnimationCurve FadeInIndicatorAnimation;
    public AnimationCurve FadeOutIndicatorAnimation;
    public float FadeInIndicatorTime;
    public float FadeOutIndicatorTime;
    public AudioClip FadeInSound;
    public AudioClip OnHoverSound;
    public AudioClip OnClickSound;
    public float backgroundSizeOffset = 0;


    private Coroutine fadeInCoroutine;
    private Coroutine fadeOutCoroutine;
    private Coroutine fadeInIndicatorCoroutine;
    private Coroutine fadeOutIndicatorCoroutine;
    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Quaternion initialRotation;
    private SteamVR_Input_Sources selectedHand = SteamVR_Input_Sources.Any;
    private Stack Stack;
    private MenuEntry BackOption;
    private List<RadialUIElement> elements = new List<RadialUIElement>();
    private int lastIndex = -1;
    private bool isShowing = false;
    private bool isIndicatorShowing = true;
    float? rawAngle = null;
    private bool isClicking;

    SteamVR_Action_Boolean touchMenuAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("menu", "MenuTouch");
    SteamVR_Action_Boolean clickMenuAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("menu", "MenuClick");
    SteamVR_Action_Vector2 positionMenuAction = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("menu", "MenuPosition");



    public int TotalElements
    {
        get;
        private set;
    }
    public float IconOffset
    {
        get;
        private set;
    }

    public float ButtonAngle
    {
        get; set;
    } = 360;

    private void Awake()
    {
        initialPosition = UIParent.transform.position;
        initialRotation = UIParent.transform.rotation;
        initialScale = UIParent.transform.localScale;

        Stack = new Stack();
    }


    // Use this for initialization
    void Start()
    {
        //SteamVR_Input.htc_viu.ActivateSecondary();
        BackOption = new MenuEntry("Voltar", closeSprite, delegate { GoBack(); }, null, null);

        //selectedHand = SteamVR_Input_Sources.RightHand;
        //ShowPanel(itemManager.Menu);
        SteamVR_Input.GetActionSet("menu").Activate();

        SetupBehaviour(UIBehaviour.OnRemote);
    }

    // Update is called once per frame
    void Update()
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


        if (isShowing)
        {
            (BackgroundLabelTextComponent.transform.parent as RectTransform).sizeDelta = BackgroundLabelTextComponent.GetRenderedValues() + textOffset;
            float angleOffset = 360 / TotalElements;

            if (touchMenuAction.GetState(selectedHand)/*SteamVR_Input.htc_viu.inActions.viu_touch_32.GetState(selectedHand)*/)
            {
                rawAngle = Vector2.SignedAngle(new Vector2(0, 1), positionMenuAction.GetAxis(selectedHand));
            }
            else
            {

                if (Input.GetKeyDown(KeyCode.Keypad4))
                {
                    if (rawAngle == null)
                    {
                        rawAngle = 0;
                    }
                    else
                    {
                        rawAngle += angleOffset;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Keypad6))
                {
                    if (rawAngle == null)
                    {
                        rawAngle = 0;
                    }
                    else
                    {
                        rawAngle -= angleOffset;
                    }
                }
            }
            if (rawAngle != null)
            {
                float currentAngle = NormalizeAngle((float)-rawAngle);

                if (DoIndicatorAnimation && !isIndicatorShowing && fadeInIndicatorCoroutine == null)
                {
                    fadeInIndicatorCoroutine = StartCoroutine(FadeIndicatorAnimationCoroutine());
                }

                IndicatorParentTransform.localEulerAngles = new Vector3(0, 0, -currentAngle);

                int index = (int)((currentAngle + IconOffset) / angleOffset) % TotalElements;
                if (index != lastIndex)
                {
                    if (isClicking)
                    {
                        if (lastIndex >= 0 && lastIndex < elements.Count)
                        {
                            elements[lastIndex].OnClickUp.Invoke();
                        }
                    }
                    if (lastIndex >= 0 && lastIndex < elements.Count)
                    {
                        elements[lastIndex].OnHover(false);
                    }
                    if (index >= 0 && index < elements.Count)
                    {
                        elements[index].OnHover();
                        
                    }
                    lastIndex = index;
                }
            }

            if (Input.GetKeyDown(KeyCode.Keypad5) || clickMenuAction.GetStateDown(selectedHand))
            {

                if (lastIndex >= 0 && lastIndex < elements.Count)
                {
                    elements[lastIndex].OnClickDown.Invoke();
                    isClicking = true;
                }

            }

            if (Input.GetKeyUp(KeyCode.Keypad5) || clickMenuAction.GetStateUp(selectedHand))
            {
                if (lastIndex >= 0 && lastIndex < elements.Count)
                {
                    elements[lastIndex].OnClickUp.Invoke();
                    isClicking = false;
                }
                rawAngle = null;
            }



        }
        else
        {

            //if (clickMenuAction.GetStateDown(SteamVR_Input_Sources.LeftHand))
            //{
            //    selectedHand = SteamVR_Input_Sources.LeftHand;
            //    ShowPanel(itemManager.Menu);
            //}
            if (clickMenuAction.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                selectedHand = SteamVR_Input_Sources.RightHand;
                ShowPanel(itemManager.Menu);
            }
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                ShowPanel(itemManager.Menu);
            }
            if (DoIndicatorAnimation && isIndicatorShowing && fadeOutIndicatorCoroutine == null)
            {
                fadeOutIndicatorCoroutine = StartCoroutine(FadeIndicatorAnimationCoroutine(false));
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
            UIParent.transform.rotation = LeftHand.transform.rotation;
        }
        else if (currentBehaviour == UIBehaviour.OnRemote && selectedHand == SteamVR_Input_Sources.RightHand)
        {
            UIParent.transform.position = RighHand.transform.position + Vector3.up * offsetForRemote.y;
            UIParent.transform.localPosition += Camera.main.transform.forward * offsetForRemote.x;
            UIParent.transform.rotation = RighHand.transform.rotation;

        }
    }

    public void GenerateMenu(MenuEntry root)
    {
        isClicking = false;
        if (root != null)
        {
            //expManager.LogEvent(@"\Menu\GoTo\" + root.ItemName);
            BackgroundLabelTextComponent.text = root.ItemName;
            
            ClearMenu(RadialUIElementGroupTransform);
            TotalElements = 0;
            lastIndex = -1;


            if (Stack.Count == 0)
            {
                BackOption.ItemIcon = closeSprite;
                BackOption.ItemName = "Fechar";
            }
            else
            {
                BackOption.ItemIcon = backSprite;
                BackOption.ItemName = "Voltar";
            }




            GenerateElements(BackOption, root, true);


            foreach (MenuEntry entry in root.Childs)
            {
                GenerateElements(entry, root);
            }


        }
    }

    public void GenerateElements(MenuEntry entry, MenuEntry lastEntry, bool isBackButton = false)
    {

        GameObject element = Instantiate(prefabRadialUIElement, RadialUIElementGroupTransform);
        RadialUIElement RadialElement = element.GetComponent<RadialUIElement>();
        RadialElement.ElementLabel = entry.ItemName;
        if (entry.ItemIcon != null)
        {
            RadialElement.ElementIcon = entry.ItemIcon;
        }
        else
        {
            RadialElement.ElementIcon = defaultSprite;
        }

        RadialElement.Index = TotalElements;
        RadialElement.Manager = this;

        if (entry.Childs != null && entry.Childs.Count != 0)
        {
            RadialElement.OnClickDown.AddListener(delegate
            {
                UIAudioComponent.PlayOneShot(OnClickSound);
                Stack.Push(lastEntry);
                GenerateMenu(entry);
            });
        }
        else
        {
            RadialElement.OnClickDown.AddListener(delegate
            {
                if (entry.DoDownAction())
                {
                    UIAudioComponent.PlayOneShot(OnClickSound);
                }
            });

            RadialElement.OnClickUp.AddListener(delegate
            {
                if (entry.DoUpAction())
                {
                    UIAudioComponent.PlayOneShot(OnClickSound);
                }
            });
            RadialElement.IsAction = true;
        }

        if (isBackButton)
        {
            RadialElement.IsBackButton = true;
        }

        elements.Add(RadialElement);
        TotalElements++;
        ButtonAngle = 360f / TotalElements;
        IconOffset = ButtonAngle / 2f;
    }

    public void GoBack()
    {
        if (Stack != null && Stack.Count != 0)
        {
            //expManager.LogEvent(@"\Menu\Back");
            GenerateMenu(Stack.Pop() as MenuEntry);
        }
        else
        {
            BackgroundLabelTextComponent.text = "";
            HidePanel();
        }

    }

    public void ClearMenu(Transform Parent)
    {
        foreach (Transform t in Parent)
        {
            Destroy(t.gameObject);
        }
        elements.Clear();
    }
    private float NormalizeAngle(float angle)
    {

        angle = angle % 360f;

        if (angle < 0)
        {
            angle += 360;
        }

        return angle;

    }


    public void ShowPanel(MenuEntry menuRoot)
    {
        if (!isShowing && fadeInCoroutine == null)
        {
            GenerateMenu((menuRoot == null) ? itemManager.Menu : menuRoot);

            SpawnPickerBehaviour();

            fadeInCoroutine = StartCoroutine(FadePickerAnimationCoroutine());

            isShowing = true;
            //expManager.LogEvent(@"\Menu\Open");

        }
    }

    public void HidePanel()
    {
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
            //expManager.LogEvent(@"\Menu\Close");
            Stack.Clear();

        }
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

            if (canvasGroupComponent.alpha != 1)
            {


                canvasGroupComponent.alpha = 0;


                UIAudioComponent.PlayOneShot(FadeInSound);
                float t = 0;
                while (canvasGroupComponent.alpha < 1)
                {
                    t += Time.deltaTime;
                    canvasGroupComponent.alpha = FadeInAnimation.Evaluate(t / FadeInTime);
                    if (canvasGroupComponent.alpha > 1)
                    {
                        canvasGroupComponent.alpha = 1;
                    }

                    yield return null;
                }
            }

            fadeInCoroutine = null;
        }
        else
        {
            yield return new WaitUntil(() => fadeInCoroutine == null);

            if (canvasGroupComponent.alpha != 0)
            {

                canvasGroupComponent.alpha = 1;
                UIAudioComponent.PlayOneShot(FadeInSound);
                float t = 0;
                while (canvasGroupComponent.alpha > 0)
                {
                    t += Time.deltaTime;
                    canvasGroupComponent.alpha = FadeOutAnimation.Evaluate(t / FadeOutTime);
                    if (canvasGroupComponent.alpha < 0)
                    {
                        canvasGroupComponent.alpha = 0;
                    }

                    yield return null;
                }
            }

            UIParent.SetActive(false);

            fadeOutCoroutine = null;
        }
    }


    private IEnumerator FadeIndicatorAnimationCoroutine(bool fadeIn = true)
    {
        if (fadeIn)
        {
            yield return new WaitUntil(() => fadeOutIndicatorCoroutine == null);

            if (IndicatorImageComponent.color.a != 1)
            {

                Color tempColor = IndicatorImageComponent.color;
                tempColor.a = 0;
                IndicatorImageComponent.color = tempColor;

                float t = 0;
                while (tempColor.a < 1)
                {
                    t += Time.deltaTime;
                    IndicatorImageComponent.color = new Color(tempColor.r, tempColor.g, tempColor.b, FadeInIndicatorAnimation.Evaluate(t / FadeInIndicatorTime));

                    if (IndicatorImageComponent.color.a > 1)
                    {
                        IndicatorImageComponent.color = new Color(tempColor.r, tempColor.g, tempColor.b, 1);

                    }
                    tempColor = IndicatorImageComponent.color;
                    yield return null;
                }
            }
            isIndicatorShowing = true;
            fadeInIndicatorCoroutine = null;
        }
        else
        {
            yield return new WaitUntil(() => fadeInIndicatorCoroutine == null);

            if (IndicatorImageComponent.color.a != 0)
            {

                Color tempColor = IndicatorImageComponent.color;
                tempColor.a = 1;
                IndicatorImageComponent.color = tempColor;

                float t = 0;
                while (tempColor.a > 0)
                {
                    t += Time.deltaTime;
                    IndicatorImageComponent.color = new Color(tempColor.r, tempColor.g, tempColor.b, FadeOutIndicatorAnimation.Evaluate(t / FadeOutIndicatorTime));

                    if (IndicatorImageComponent.color.a < 0)
                    {
                        IndicatorImageComponent.color = new Color(tempColor.r, tempColor.g, tempColor.b, 0);

                    }
                    tempColor = IndicatorImageComponent.color;
                    yield return null;
                }
            }

            isIndicatorShowing = false;
            fadeOutIndicatorCoroutine = null;
        }
    }

}
