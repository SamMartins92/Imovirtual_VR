using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class MenuEntry
{

    [SerializeField] string itemName;
    [SerializeField] Sprite itemIcon;
    [SerializeField] UnityEvent itemDownAction;// = new UnityEvent();
    [SerializeField] UnityEvent itemUpAction;// = new UnityEvent();
    [SerializeField] UnityEvent onPropertyChange;
    [SerializeField] List<MenuEntry> childs;


    public string ItemName
    {
        get { return itemName; }
        set
        {
            itemName = value;
            if (onPropertyChange != null)
            {
                onPropertyChange.Invoke();
            }
        }
    }

    public Sprite ItemIcon
    {
        get { return itemIcon; }
        set
        {
            itemIcon = value;
            if (onPropertyChange != null)
            {
                onPropertyChange.Invoke();
            }
        }
    }

    public UnityEvent ItemDownAction
    {
        get { return itemDownAction; }
        set
        {
            itemDownAction = value;
            if (onPropertyChange != null)
            {
                onPropertyChange.Invoke();
            }
        }
    }

    public UnityEvent ItemUpAction
    {
        get { return itemUpAction; }
        set
        {
            itemUpAction = value;
            if (onPropertyChange != null)
            {
                onPropertyChange.Invoke();
            }
        }
    }

    public List<MenuEntry> Childs
    {
        get { return childs; }
        set
        {
            childs = value;
        }
    }

    public UnityEvent OnPropertyChange
    {
        private get { return onPropertyChange; }
        set { onPropertyChange = value; }
    }

    public MenuEntry(string Name, Sprite Icon, UnityAction OnDown, UnityAction  OnUp, List<MenuEntry> Childs)
    {
        this.ItemName = Name;
        this.ItemIcon = Icon;
        if (OnDown != null)
        {
            this.itemDownAction = new UnityEvent();
            this.ItemDownAction.AddListener(OnDown);
        }
        if (OnUp != null)
        {
            this.itemUpAction = new UnityEvent();
            this.ItemUpAction.AddListener(OnUp);
        }
        this.Childs = Childs;
    }

    public MenuEntry()
    {

    }

    public bool DoDownAction()
    {
        if (ItemDownAction == null)
            return false;
        else
            ItemDownAction.Invoke();

        return true;
    }

    public bool DoUpAction()
    {
        if (ItemUpAction == null)
            return false;
        else
            ItemUpAction.Invoke();

        return true;
    }
}
