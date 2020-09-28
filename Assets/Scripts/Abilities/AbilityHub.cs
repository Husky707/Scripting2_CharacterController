using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHub : MonoBehaviour
{

    [SerializeField] private List<eAbilities> AbilityList = new List<eAbilities>();
    [SerializeField] private List<GameObject> ObjectList = new List<GameObject>();

    private Dictionary<eAbilities, GameObject> HubList;
    bool isInit = false;
    private void Awake()
    {
        InitDict();
    }

    private void InitDict()
    {
        if (isInit)
            return;
        isInit = true;

        int count = AbilityList.Count;
        HubList = new Dictionary<eAbilities, GameObject>(count);
        for(int i = 0; i < count; i++)
        {
            HubList.Add(AbilityList[i], ObjectList[i]);
        }
    }

    public GameObject GetTarget(eAbilities ability)
    {
        InitDict();

        if (HubList == null)
            Debug.Log("Getting targets from null dictionary");
        if(!HubList.ContainsKey(ability))
        {
            Debug.Log("Dictionary did not have the key for the ability");
            return null;
        }

        return HubList[ability];
    }
}
