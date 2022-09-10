using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;


//[ExecuteInEditMode]
public class MenuItemManager : MonoBehaviour
{
    [SerializeField] public MenuEntry Menu1;
    [SerializeField] public MenuEntry Menu2;
    [SerializeField] public MenuEntry Menu;


    [Header("Sprites")]
    [SerializeField] private Sprite spriteHome;

    [SerializeField] MobiliaOn MobiliaScript;
    private bool mobiliaState = false;
    [SerializeField] DisposicaoSala disposicaoSalaScript;
    private bool isSala1 = true;
    [SerializeField] ChangeColor2 changeColorScript;

    [SerializeField] private Sprite mobiliaOnSprite;
    [SerializeField] private Sprite mobiliaOffSprite;
    [SerializeField] private Sprite disposicaoSprite;
    [SerializeField] private Sprite aparenciaSprite;
    [SerializeField] private Sprite armarioSprite;
    [SerializeField] private Sprite mesaSprite;
    [SerializeField] private Sprite sofaSprite;
    [SerializeField] private Sprite tapeteSprite;
    //[SerializeField] private Sprite teleportSprite;
    [SerializeField] private Sprite conjuntosSprite;
    [SerializeField] private Sprite conjunto1Sprite;
    [SerializeField] private Sprite conjunto2Sprite;
    [SerializeField] private Sprite conjunto3Sprite;




    [SerializeField] Teleport teleportScript;
    [SerializeField] RadialUIManager radialScript;

    private Vector2 scrollPosition = Vector2.zero;
    private int l = 0, i = 0;

    private void Awake()
    {
        PopulateMenu();
        Menu = Menu1;
    }

    public void CreateMenu(MenuEntry menu, int level = 0)
    {
        i++;

        if (menu.Childs != null && menu.Childs.Count != 0)
        {
#if UNITY_EDITOR
            EditorGUILayout.LabelField(menu.ItemName);
#else
            GUI.Label(new Rect(10 * level, i * 22, 150, 20), menu.ItemName);
#endif
            l++;
            foreach (MenuEntry m in menu.Childs)
                CreateMenu(m, l);
            l--;
        }
        else
        {
#if UNITY_EDITOR
            if (GUILayout.Button(menu.ItemName))
            {
                Debug.Log(menu.DoDownAction());
            }
#else
            if (GUI.Button(new Rect(10 * level, i * 22, 150, 20), menu.ItemName))
            {
                Debug.Log(menu.DoDownAction());
            }
#endif
        }
    }

    public void PopulateMenu()
    {
        Menu1 = new MenuEntry("Menu Principal", spriteHome, null, null, new List<MenuEntry>() {

                new MenuEntry("Aparência", aparenciaSprite, null, null, new List<MenuEntry>()
                {
                    new MenuEntry("Mostrar\nMobília", mobiliaOnSprite,  delegate{
                        MobiliaScript.Enable();
                        Menu = Menu2;
                        radialScript.GenerateMenu(Menu2.Childs[0]);
                    }, null, null),
                    new MenuEntry("Armário\nCozinha", armarioSprite, delegate{ changeColorScript.ChangeMovelCozinhaMaterial(); },null, null ),
                })

        });

        Menu2 = new MenuEntry("Menu Principal", spriteHome, null, null, new List<MenuEntry>() {


                new MenuEntry("Aparência", aparenciaSprite, null, null, new List<MenuEntry>()
                {
                    new MenuEntry("Esconder\nMobília", mobiliaOffSprite, delegate{
                        MobiliaScript.Disable();
                        Menu = Menu1;
                        radialScript.GenerateMenu(Menu1.Childs[0]);
                    }, null, null),
                    new MenuEntry("Disposição\nSala",  disposicaoSprite, delegate{
                        if(isSala1)
                        {
                            disposicaoSalaScript.EnableTwo();
                            isSala1 = false;
                        }
                        else
                        {
                            disposicaoSalaScript.EnableOne();
                            isSala1 = true;
                        }
                    },null, null),
                    new MenuEntry("Mesa\nJantar", mesaSprite, delegate{changeColorScript.ChangeMesaJantarMaterial(); },null, null),
                    new MenuEntry("Sofá", sofaSprite ,delegate { changeColorScript.ChangeSofaMaterial(); }, null,null),
                    new MenuEntry("Tapete", tapeteSprite, delegate{ changeColorScript.ChangeTapeteMaterial(); },null, null),
                    new MenuEntry("Armário\nCozinha", armarioSprite, delegate{ changeColorScript.ChangeMovelCozinhaMaterial(); },null, null ),
                    new MenuEntry("Combinações", conjuntosSprite, null ,null, new List<MenuEntry>(){
                        new MenuEntry("Combinação 1", conjunto1Sprite, delegate { changeColorScript.ChangeConjunto(0); },null,null),
                        new MenuEntry("Combinação 2", conjunto2Sprite, delegate { changeColorScript.ChangeConjunto(1); },null,null),
                        new MenuEntry("Combinação 3", conjunto3Sprite, delegate { changeColorScript.ChangeConjunto(2); },null,null),
                    })
                }),


        });
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(MenuItemManager))]
public class MenuItemManagerEditor : Editor
{
    private MenuItemManager menuManagerScript;

    public override void OnInspectorGUI()
    {
        if (menuManagerScript == null)
            menuManagerScript = (MenuItemManager)target;

        serializedObject.Update();

        base.OnInspectorGUI();

        MASSIVE.EditorUtilities.Separator();

        if (GUILayout.Button("Populate/Update Menu"))
        {
            menuManagerScript.PopulateMenu();
        }

        menuManagerScript.CreateMenu(menuManagerScript.Menu1);

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(menuManagerScript);
            if (!Application.isPlaying)
                UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }
}

#endif