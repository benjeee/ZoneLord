using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AbilityItem {

	public enum AbilityItemType
    {
       None,
       Crow,
       Flare,
       Bubble
    }

    public static AbilityItem NoneItem = new AbilityItem();

    public AbilityItemType ItemType { get; set; }
    public int count;

    public AbilityItem()
    { 
        ItemType = AbilityItemType.None;
    }

    public AbilityItem(AbilityItemType t)
    {
        ItemType = t;
        count = 1;
    }

    public void Add()
    {
        count++;
    }

    public int Use()
    {
        count--;
        return count;
    }
}
