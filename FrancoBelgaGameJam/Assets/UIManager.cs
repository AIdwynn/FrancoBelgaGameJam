using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI turnText, zapText;
    [SerializeField] Transform hpIconParent;
    [SerializeField] GameObject hpIcon;
    [SerializeField] SkillIcon[] icons;
    [SerializeField] Color ready, unusable, disabled;

    public void UpdateHP(int value)
    {
        for (var i = hpIconParent.childCount - 1; i >= 0; i--)
        {
            var objectA = hpIconParent.GetChild(i);
            Obliterate(objectA.gameObject);
        }

        for (int i = 0; i < value; i++)
        {
            GameObject go = Instantiate(hpIcon);
            go.transform.SetParent(hpIconParent, false);
        }
    }

    public void Obliterate(GameObject go)
    {
        go.transform.SetParent(null);

        Destroy(go);
    }

    public void UpdateTurnText(string content)
    {
        turnText.text = content;
        turnText.transform.DOKill();
        turnText.transform.localScale = Vector3.one;
        turnText.transform.DOPunchScale(Vector3.one, 0.4f);
    }

    public void UpdateZapAmount(int amount)
    {
        zapText.text = amount.ToString();
        zapText.transform.DOKill();
        zapText.transform.localScale = Vector3.one;
        zapText.transform.DOPunchScale(Vector3.one, 0.4f);
    }

    #region Items
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
    #endregion
}


[System.Serializable]
public class SkillIcon
{
    public string name;
    public Image icon;
}
