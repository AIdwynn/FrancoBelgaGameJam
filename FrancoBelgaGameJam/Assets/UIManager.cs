using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] SkillIcon[] icons;
    [SerializeField] Color ready, unusable, disabled;

    public void EquipIcon(string Name)
    {
        foreach (var item in icons)
        { 
            if (item.name == Name) 
            {
                Color col = item.icon.color;
                col.a = 1;
                item.icon.color = col;
            }
            else
            {
                Color col = disabled;
                col.a = 0.4f;
                item.icon.color = col;
            }
        }
    }

    public void ChangeIconState(string Name, AbilityState state)
    {
        switch (state)
        {
            case AbilityState.Ready:
                GetIcon(Name).icon.color = ready;
                break;

            case AbilityState.Unusable:
                GetIcon(Name).icon.color = unusable;
                break;

            case AbilityState.Disabled:
                GetIcon(Name).icon.color = disabled;
                break;
        }
    }

    public SkillIcon GetIcon(string Name) 
    { 
        return Array.Find(icons, icon => icon.name == Name);
    }
}


[System.Serializable]
public class SkillIcon
{
    public string name;
    public Image icon;
}
