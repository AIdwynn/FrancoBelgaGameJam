using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AbilityState
{
    Ready,
    Unusable,
    Disabled
}

public class AbilitiesManager : MonoBehaviour
{
    [SerializeField] Ability[] abilities;

    [Header("Settings")]
    [SerializeField] private LayerMask enemiesLayer;

    [Header("References")]
    [SerializeField] private Transform weaponsParent;

    private PlayerController player;
    private int currentIndex;
    private GameObject currentItem;
    private string currentItemName;

    public void Init(PlayerController p)
    {
        player = p;
        EquipAbility(abilities[0]);
    }

    public void UpdateManager()
    {
        Select();

        Ability current = abilities[currentIndex];

        if (current == null)
            return;

        bool canUse = false;
        var UIManager = GameManager.Instance.UIManager;
        RaycastHit hit = new RaycastHit();

        if (current.alwaysActive)
        {
            canUse = true;
            UIManager.ChangeIconState(currentItemName, AbilityState.Ready);
        }
        else
        {
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            if (Physics.SphereCast(ray.origin, current.aimAssist, ray.direction, out hit, current.range, enemiesLayer))
            {
                UIManager.ChangeIconState(currentItemName, AbilityState.Ready);
                canUse = true;
            }
            else
            {
                UIManager.ChangeIconState(currentItemName, AbilityState.Unusable);
            }
        }

        if (canUse && Input.GetMouseButtonDown(0))
        {
            if (hit.transform != null)
            {
                Lifeform target = hit.transform.GetComponent<Lifeform>();

                if (target != null)
                    player.EndTurnAttack(current.Name, current.anticipation, current.recovery, target, current.stuns);
                else
                    player.EndTurnAttack(current.Name, current.anticipation, current.recovery);

            }
            else
            {
                player.EndTurnAttack(current.Name, current.anticipation, current.recovery);
            }
        }
    }

    private void Select()
    {
        if (Input.GetMouseButtonDown(1))
        {
            currentIndex++;

            if (currentIndex >= abilities.Length)
                currentIndex = 0;

            EquipAbility(abilities[currentIndex]);
            player.SwapWeapon();
        }
    }

    public void EquipAbility(Ability ab)
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }

        currentItemName = ab.Name;
        GameManager.Instance.UIManager.EquipIcon(ab.Name);

        GameObject go = Instantiate(ab.Item);
        go.transform.SetParent(weaponsParent, false);

        currentItem = go;
    }
}

[System.Serializable]
public class Ability
{
    public string Name;
    public GameObject Item;
    public string AnimationName;
    public bool alwaysActive, stuns;
    public float range, aimAssist, anticipation, recovery;
}
