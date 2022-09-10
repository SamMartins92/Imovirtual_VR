using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class RadialUIElement : MonoBehaviour
{

    [SerializeField] Image backgroundComponent;
    [SerializeField] Image iconComponent;
    [SerializeField] Sprite backgroundActionSprite;
    [SerializeField] TMPro.TMP_Text textComponent;
    public Vector2 textOffset;


    private float backgroundSize;
    public float BackgroundSize
    {
        get { return backgroundSize; }
        set
        {
            if (value >= 0 && value <= 1)
            {
                backgroundSize = value;
                backgroundComponent.fillAmount = value;
            }
        }
    }

    private Sprite elementIcon;
    public Sprite ElementIcon
    {
        get { return elementIcon; }
        set
        {
            elementIcon = value;
            iconComponent.sprite = value;

        }
    }

    private string elementLabel;
    public string ElementLabel
    {
        get { return elementLabel; }
        set
        {

            textComponent.text = value;
            elementLabel = value;
        }
    }

    public UnityEvent OnClickDown
    {
        get;
        set;
    } = new UnityEvent();

    public UnityEvent OnClickUp
    {
        get;
        set;
    } = new UnityEvent();

    public UnityEvent OnHoverEnter
    {
        get;
        set;
    } = new UnityEvent();

    public UnityEvent OnHoverExit
    {
        get;
        set;
    } = new UnityEvent();

    
    private int index;
    public int Index
    {
        get { return index; }
        set
        {
            if (value < 0)
            {
                index = 0;
            }
            else
            {
                index = value;
            }
        }
    }

    public bool IsBackButton
    {
        set;
        get;
    } = false;

    public bool IsAction
    {
        set;
        get;
    } = false;

    public RadialUIManager Manager
    {
        get;
        set;
    }

    private void Update()
    {
        BackgroundSize = Manager.ButtonAngle / 360 - Manager.backgroundSizeOffset;

        (this.transform as RectTransform).localRotation = Quaternion.Euler(0, 0, -(Index * Manager.ButtonAngle - Manager.IconOffset + 360 * Manager.backgroundSizeOffset / 2));
        iconComponent.transform.parent.transform.localRotation = Quaternion.Euler(0, 0, -Manager.IconOffset + 360 * Manager.backgroundSizeOffset / 2);


        iconComponent.transform.rotation = transform.parent.rotation;
        float angle = NormalizeAngle(transform.localEulerAngles.z - Manager.IconOffset);
        if (angle > 110 && angle < 250)
            textComponent.transform.localEulerAngles = new Vector3(0, 0, 180);
        (textComponent.transform.parent as RectTransform).sizeDelta = textComponent.GetRenderedValues() + textOffset;

        if (IsAction)
            backgroundComponent.sprite = backgroundActionSprite;

    }

    private void Awake()
    {
        Debug.LogWarning("If it gives Errors on play it's because of UnityEvents Variables");
        //OnHoverEnter.AddListener(delegate
        //{
        //    Manager.UIAudioComponent.PlayOneShot(Manager.OnHoverSound);
        //    ChangeColor(new Color(0, 0, 1, 1), new Color(1, 1, 1, 1), new Color(1, 1, 1, 1));

        //});
        //OnHoverExit.AddListener(delegate { ChangeColor(new Color(1, 1, 1, 1), new Color(0, 0, 0, 1), new Color(0, 0, 0, 1)); });
    }

    private void Start()
    {


        
    }

    public void OnHover(bool enter = true)
    {
        if(enter)
        {
            Manager.UIAudioComponent.PlayOneShot(Manager.OnHoverSound);
            ChangeColor(new Color(0, 0, 1, 1), new Color(1, 1, 1, 1), new Color(1, 1, 1, 1));
            OnHoverEnter.Invoke();
        }
        else
        {
            ChangeColor(new Color(1, 1, 1, 1), new Color(0, 0, 0, 1), new Color(0, 0, 0, 1));
            OnHoverExit.Invoke();
        }
    }

    public void ChangeColor(Color backgroundColor, Color IconColor, Color TextColor)
    {
        backgroundComponent.color = backgroundColor;
        iconComponent.color = IconColor;
        textComponent.transform.parent.GetComponent<Image>().color = backgroundColor;
        textComponent.color = TextColor;
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
}
