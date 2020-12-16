﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SRackInfos
{
    public string name;
    public Transform parent;
    public string orient;
    public Vector2 pos; // tile
    public Vector3 size; // cm 
    public int height; // U
    public string template;
}

[System.Serializable]
public struct SDeviceInfos
{
    public string name;
    public Transform parent;
    // public int posU;
    public float posU; // should be int, authorize until non IT objects can be created
    public string slot;
    public float sizeU;
    public string template;
    public string side;

}

[System.Serializable]
public struct SSiteInfos
{
    public string name;
    public Transform parent;
    public string orient;
}

[System.Serializable]
public struct SBuildingInfos
{
    public string name;
    public Transform parent;
    public Vector3 pos;
    public Vector3 size;
}

[System.Serializable]
public struct SRoomInfos
{
    public string name;
    public Transform parent;
    public Vector3 pos; // tile
    public Vector3 size; // tile
    public string orient;
    public string template;
}

public struct SSeparatorInfos
{
    public string name;
    public Vector2 pos1XYm;
    public Vector2 pos2XYm;
    public Transform parent;
}

