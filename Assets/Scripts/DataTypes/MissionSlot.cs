using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MissionSlot
{
    [SerializeField]
    private RectTransform slotObject;
    public RectTransform Slotobject
    {
        get { return slotObject; }
        set { slotObject = value; }
    }
    
    [SerializeField]
    private ColorType colorType;
    public ColorType ColorType
    {
        get { return colorType; }
        set { colorType = value; }
    }
}
