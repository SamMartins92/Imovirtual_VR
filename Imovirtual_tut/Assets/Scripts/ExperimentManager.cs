using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    public static ExperimentManager instance;
    public RadialUIManager RadialUI;
    public PanelUIManager PanelUI;
    [SerializeField] private AudioSource bell;
    [SerializeField] private AudioClip startTaskSound;

    [Header("Keybinds")]
    [SerializeField] private KeyCode StartTask = KeyCode.Alpha1;
    //[SerializeField] private KeyCode EndTask = KeyCode.Alpha2;
    [SerializeField] private KeyCode HelpRequest = KeyCode.Alpha0;
    [SerializeField] private KeyCode FixedType = KeyCode.Comma;
    [SerializeField] private KeyCode CameraType = KeyCode.Period;
    [SerializeField] private KeyCode RemoteType = KeyCode.Minus;

    public enum SaveLocationOptions { ApplicationDataPath, Desktop, Documents }

    [Header("Log Recorder")]
    [SerializeField] private SaveLocationOptions saveLocation = SaveLocationOptions.Desktop;
    [SerializeField] private string folderName = "VRMenus_Experiments";
    [SerializeField] private bool Record = true;
    private string ticker;
    private string path;
    private Participant currentParticipant;
    private LogEventData lastEvent;
    private bool doingTask = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        currentParticipant = new Participant();

        switch (saveLocation)
        {
            case SaveLocationOptions.ApplicationDataPath:
                path = Application.persistentDataPath;
                break;
            case SaveLocationOptions.Desktop:
                path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                break;
            case SaveLocationOptions.Documents:
                path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                break;
        }
        path = Path.Combine(path, (string.IsNullOrEmpty(folderName)) ? "VRMenus_Experiments" : folderName);

        currentParticipant.id = Directory.CreateDirectory(path).GetFiles().Length + 1;

        menu = PlayerPrefs.GetString("menu", "radial");
        type = PlayerPrefs.GetString("type", "fixed");

        if (menu == "radial")
        {
            PanelUI.gameObject.SetActive(false);
            RadialUI.gameObject.SetActive(true);
        }
        else
        {
            PanelUI.gameObject.SetActive(true);
            RadialUI.gameObject.SetActive(false);
        }

        switch (type)
        {
            case "fixed":
                PanelUI.SetupBehaviour(PanelUIManager.UIBehaviour.Fixed);
                RadialUI.SetupBehaviour(PanelUIManager.UIBehaviour.Fixed);
                break;
            case "camera":
                PanelUI.SetupBehaviour(PanelUIManager.UIBehaviour.FollowTargetCamera);
                RadialUI.SetupBehaviour(PanelUIManager.UIBehaviour.FollowTargetCamera);
                break;
            case "remote":
                PanelUI.SetupBehaviour(PanelUIManager.UIBehaviour.OnRemote);
                RadialUI.SetupBehaviour(PanelUIManager.UIBehaviour.OnRemote);
                break;
        }

        ticker = "Selected '" + menu + "' and '" + type + "' Menu Type";

        currentParticipant.menu = menu;
        currentParticipant.type = type;

        //Valve.VR.SteamVR_Input.htc_viu.ActivateSecondary();
    }
    private string menu;
    private string type;

    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 220, 700), "Experiment Controls");

        GUI.Label(new Rect(20, 40, 200, 20), "ParticipantID: " + currentParticipant.id);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SceneSelector")
        {
            GUI.Label(new Rect(20, 70, 200, 20), "Menu: " + menu);
            GUI.Label(new Rect(20, 100, 200, 20), "Type: " + type);

            if (GUI.Button(new Rect(20, 150, 200, 20), "Paineis"))
            {
                PanelUI.gameObject.SetActive(true);
                RadialUI.gameObject.SetActive(false);
                ticker = "Selected PanelUI";
                menu = "panel";
            }
            if (GUI.Button(new Rect(20, 180, 200, 20), "Radial"))
            {
                PanelUI.gameObject.SetActive(false);
                RadialUI.gameObject.SetActive(true);
                ticker = "Selected RadialUI";
                menu = "radial";
            }

            if (GUI.Button(new Rect(20, 230, 200, 20), "Fixo (" + FixedType + ")"))
            {
                if (PanelUI.SetupBehaviour(PanelUIManager.UIBehaviour.Fixed) && RadialUI.SetupBehaviour(PanelUIManager.UIBehaviour.Fixed))
                {
                    ticker = "Selected Fixed Menu Type";
                    type = "fixed";
                }
                else
                {
                    ticker = "Close menu first";
                }
            }
            if (GUI.Button(new Rect(20, 260, 200, 20), "Camara (" + CameraType + ")"))
            {
                if (PanelUI.SetupBehaviour(PanelUIManager.UIBehaviour.FollowTargetCamera) && RadialUI.SetupBehaviour(PanelUIManager.UIBehaviour.FollowTargetCamera))
                {
                    ticker = "Selected FollowCamera Menu Type";
                    type = "camera";
                }
                else
                {
                    ticker = "Close menu first";
                }
            }
            if (GUI.Button(new Rect(20, 290, 200, 20), "Comando (" + RemoteType + ")"))
            {
                if (PanelUI.SetupBehaviour(PanelUIManager.UIBehaviour.OnRemote) && RadialUI.SetupBehaviour(PanelUIManager.UIBehaviour.OnRemote))
                {
                    ticker = "Selected OnRemote Menu Type";
                    type = "remote";
                }
                else
                {
                    ticker = "Close menu first";
                }
            }
        }
        else
        {
            GUI.Label(new Rect(20, 70, 200, 20), "Task No: " + (currentParticipant.lastTask + 1));
            GUI.Label(new Rect(20, 100, 200, 20), "Help Requests: " + currentParticipant.helpRequests);
        }


        if (!doingTask)
        {
            if (GUI.Button(new Rect(20, 350, 200, 20), "Start Task (" + StartTask + ")"))
            {
                BeginTask();

            }
        }
        else
        {
            if (GUI.Button(new Rect(20, 350, 200, 20), "Task Finished (" + StartTask + ")"))
            {
                FinishTask();
            }
        }

        if (GUI.Button(new Rect(20, 410, 200, 20), "Help Requested (" + HelpRequest + ")"))
        {
            RequestHelp();
        }

        GUI.Label(new Rect(20, 450, 200, 200), ticker);

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "SceneSelector")
        {
            if (GUI.Button(new Rect(20, 690, 200, 20), "Start Experiment"))
            {
                PlayerPrefs.SetString("menu", menu);
                PlayerPrefs.SetString("type", type);
                UnityEngine.SceneManagement.SceneManager.LoadScene("SmartRoomTime");
            }
        }
        else
        {
            if (GUI.Button(new Rect(20, 690, 200, 20), "Finish Experiment"))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("SceneSelector");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(StartTask))
        {
            if (!doingTask)
                BeginTask();
            else
                FinishTask();
        }
        //if (Input.GetKeyDown(EndTask))
        //{
        //    FinishTask();
        //}
        if (Input.GetKeyDown(HelpRequest))
        {
            RequestHelp();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeTimeofDay();
        }

        if (Input.GetKeyDown(FixedType))
        {
            if (PanelUI.SetupBehaviour(PanelUIManager.UIBehaviour.Fixed) && RadialUI.SetupBehaviour(PanelUIManager.UIBehaviour.Fixed))
            {
                ticker = "Selected Fixed Menu Type";
                type = "fixed";
            }
            else
            {
                ticker = "Close menu first";
            }
        }
        if (Input.GetKeyDown(CameraType))
        {
            if (PanelUI.SetupBehaviour(PanelUIManager.UIBehaviour.FollowTargetCamera) && RadialUI.SetupBehaviour(PanelUIManager.UIBehaviour.FollowTargetCamera))
            {
                ticker = "Selected FollowCamera Menu Type";
                type = "camera";
            }
            else
            {
                ticker = "Close menu first";
            }
        }
        if (Input.GetKeyDown(RemoteType))
        {
            if (PanelUI.SetupBehaviour(PanelUIManager.UIBehaviour.OnRemote) && RadialUI.SetupBehaviour(PanelUIManager.UIBehaviour.OnRemote))
            {
                ticker = "Selected OnRemote Menu Type";
                type = "remote";
            }
            else
            {
                ticker = "Close menu first";
            }
        }

        //if (EnviroSky.instance != null && EnviroSky.instance.currentHour > 23)
        //{
        //    EnviroSky.instance.GameTime.ProgressTime = EnviroTime.TimeProgressMode.None;
        //}
    }

    private void ChangeTimeofDay()
    {
        //if (EnviroSky.instance != null)
        //{
        //    if (EnviroSky.instance.currentHour > 23)
        //    {
        //        EnviroSky.instance.GameTime.Hours = 5;
        //    }
        //    EnviroSky.instance.GameTime.ProgressTime = EnviroTime.TimeProgressMode.Simulated;
        //}
    }

    private void RequestHelp()
    {
        currentParticipant.helpRequests++;
        LogEvent("HelpRequested " + (currentParticipant.helpRequests));
        ticker = "HelpRequested " + (currentParticipant.helpRequests);
    }

    private void FinishTask()
    {
        if (currentParticipant.lastTask != currentTask)
        {
            RadialUI.HidePanel();
            PanelUI.HidePanel();

            currentParticipant.lastTask++;
            LogEvent("TaskFinish " + (currentParticipant.lastTask));
            ticker = "TaskFinish " + currentParticipant.lastTask;
            doingTask = false;
        }
        else
        {
            ticker = "Begin task First";
        }
    }

    private void BeginTask()
    {
        if ((currentParticipant.lastTask == 0 && currentTask == 0) || currentParticipant.lastTask == currentTask)
        {
            LogEvent("TaskBegin " + (currentParticipant.lastTask + 1));
            ticker = "TaskBegin " + (currentParticipant.lastTask + 1);
            currentTask++;
            bell.PlayOneShot(startTaskSound);
            if (currentTask == 4)
            {
                ChangeTimeofDay();
            }
            doingTask = true;
        }
        else
        {
            ticker = "Finish Task First";
        }
    }
    private int currentTask = 0;

    private void OnDestroy()
    {
        instance = null;
        if (Record)
            SaveData();
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(currentParticipant, true);
        File.WriteAllText(Path.Combine(path, currentParticipant.id + ".json"), json);
    }

    public void LogEvent(string description)
    {
        Debug.Log("Logger :: " + description);
        LogEventData entry = new LogEventData()
        {
            eventTime = DateTime.Now,
            secondsCumulative = 0,
            secondsPrevious = 0,
            eventDescription = description
        };

        if (lastEvent != null)
            entry.secondsPrevious = (entry.eventTime - lastEvent.eventTime).TotalSeconds;

        if (currentParticipant.experimentLog.Count > 0)
            entry.secondsCumulative = (entry.eventTime - currentParticipant.experimentLog[0].eventTime).TotalSeconds;

        entry.dateTime = entry.eventTime.ToString();
        currentParticipant.experimentLog.Add(entry);

        lastEvent = entry;
    }

    [Serializable]
    public class Participant
    {
        public int id;
        public string menu;
        public string type;
        public int lastTask;
        public int helpRequests;
        public List<LogEventData> experimentLog;

        public Participant()
        {
            id = 0;
            lastTask = 0;
            helpRequests = 0;
            experimentLog = new List<LogEventData>();
        }
    }

    [Serializable]
    public class LogEventData
    {
        public DateTime eventTime;
        public string dateTime;
        public double secondsCumulative;
        public double secondsPrevious;
        public string eventDescription;
    }
}



