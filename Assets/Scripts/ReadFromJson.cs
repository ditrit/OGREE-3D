﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadFromJson
{
    #region Room
    [System.Serializable]
    public struct SRoomFromJson
    {
        public string slug;
        public string orientation;
        public float[] sizeWDHm;
        public int[] technicalArea;
        public int[] reservedArea;
        public SSeparator[] separators;
        public STiles[] tiles;
        public SAisles[] aisles;
    }

    [System.Serializable]
    public struct SSeparator
    {
        public string name;
        public float[] pos1XYm;
        public float[] pos2XYm;
    }

    [System.Serializable]
    public struct STiles
    {
        public string location;
        public string name;
        public string label;
        public string type;
        public string color;
    }

    [System.Serializable]
    public struct SAisles
    {
        public string name;
        public string locationY; // should be posY
        public string orientation;
    }
    #endregion

    #region Rack
    [System.Serializable]
    private struct SRackFromJson
    {
        public string name;
        public string slug;
        public string vendor;
        public string model;
        public string type;
        public string role;
        public string orientation;
        public string side;
        public string fulllength;
        public int[] sizeWDHmm;
        public SRackSlot[] components;
    }

    [System.Serializable]
    private struct SRackSlot
    {
        public string location;
        public string family;
        public string role;
        public string installed;
        public int[] elemPos;
        public int[] elemSize;
        public string mandatory;
        public string labelPos;
        public string color;
    }
    #endregion

    #region Component
    [System.Serializable]
    private struct SComponent
    {
        public string name;
        public string slug;
        public string vendor;
        public string model;
        public string type;
        public string role;
        public string orientation;
        public string side;
        public string fulllength;
        public int[] sizeWDHmm;
        public SComponentChild components;
    }

    [System.Serializable]
    private struct SComponentChild
    {
        public string location;
        public string type;
        public string role;
        public string position;
        public int[] elemPos;
        public int[] elemSize;
        public string mandatory;
        public string labelPos;
    }
    #endregion


    ///<summary>
    /// Create a rack from _json data and add it to GameManager.rackTemplates.
    ///</summary>
    ///<param name="_json">Json to parse</param>
    public void CreateRackTemplate(string _json)
    {
        SRackFromJson rackData = JsonUtility.FromJson<SRackFromJson>(_json);
        if (GameManager.gm.rackTemplates.ContainsKey(rackData.slug))
            return;

        SRackInfos infos = new SRackInfos();
        infos.name = rackData.slug;
        infos.parent = GameManager.gm.templatePlaceholder;
        infos.orient = "front";
        infos.size = new Vector2(rackData.sizeWDHmm[0] / 10, rackData.sizeWDHmm[1] / 10);
        infos.height = (int)(rackData.sizeWDHmm[2] / GameManager.gm.uSize / 1000);
        Rack rack = ObjectGenerator.instance.CreateRack(infos, false);

        rack.transform.localPosition = Vector3.zero;
        rack.vendor = rackData.vendor;
        rack.model = rackData.model;
        foreach (SRackSlot comp in rackData.components)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = comp.location;
            go.transform.parent = rack.transform;
            go.transform.localScale = new Vector3(comp.elemSize[0], comp.elemSize[2], comp.elemSize[1]) / 1000;
            go.transform.localPosition = go.transform.parent.GetChild(0).localScale / -2;
            go.transform.localPosition += go.transform.localScale / 2;
            go.transform.localPosition += new Vector3(comp.elemPos[0], comp.elemPos[2], comp.elemPos[1]) / 1000;
            go.transform.localEulerAngles = Vector3.zero;

            Slot s = go.AddComponent<Slot>();
            s.installed = comp.installed;
            s.mandatory = comp.mandatory;
            s.labelPos = comp.labelPos;

            GameObject textHolder = new GameObject();
            textHolder.name = "textHolder";

            TextMeshPro text = textHolder.AddComponent<TextMeshPro>();
            text.text = go.name;
            text.fontSize = 0.5f;
            text.alignment = TextAlignmentOptions.MidlineGeoAligned;

            switch (comp.labelPos)
            {
                case "front":
                    textHolder.transform.SetParent(go.transform);
                    textHolder.transform.localPosition = new Vector3(0, 0, go.transform.localScale.z / -2);
                    textHolder.transform.localPosition += new Vector3(0, 0, 0.06f);
                    textHolder.transform.localEulerAngles = Vector3.zero;
                    text.rectTransform.sizeDelta = new Vector2(go.transform.localScale.x, go.transform.localScale.y);
                    break;
                case "rear":
                    textHolder.transform.SetParent(go.transform);
                    textHolder.transform.localPosition = new Vector3(0, 0, go.transform.localScale.z / 2);
                    textHolder.transform.localPosition += new Vector3(0, 0, -0.06f);
                    textHolder.transform.localEulerAngles = new Vector3(0, 180, 0);
                    text.rectTransform.sizeDelta = new Vector2(go.transform.localScale.x, go.transform.localScale.y);
                    break;
                case "top":
                    textHolder.transform.localEulerAngles = new Vector3(90, 0, 0);
                    textHolder.transform.SetParent(go.transform);
                    textHolder.transform.localPosition = new Vector3(0, go.transform.localScale.y * 10, 0);
                    textHolder.transform.localPosition += new Vector3(0, 0.1f, 0);
                    text.rectTransform.sizeDelta = new Vector2(go.transform.localScale.x, go.transform.localScale.y);
                    break;
                case "left":
                    textHolder.transform.localEulerAngles = new Vector3(0, -90, 0);
                    textHolder.transform.SetParent(go.transform);
                    textHolder.transform.localPosition = new Vector3(go.transform.localScale.x * -10, 0, 0);
                    textHolder.transform.localPosition += new Vector3(-0.07f, 0, 0);
                    text.rectTransform.sizeDelta = new Vector2(go.transform.localScale.z, go.transform.localScale.y);
                    break;
                case "right":
                    textHolder.transform.localEulerAngles = new Vector3(0, 90, 0);
                    textHolder.transform.SetParent(go.transform);
                    textHolder.transform.localPosition = new Vector3(go.transform.localScale.x * 10, 0, 0);
                    textHolder.transform.localPosition += new Vector3(0.07f, 0, 0);
                    text.rectTransform.sizeDelta = new Vector2(go.transform.localScale.z, go.transform.localScale.y);
                    break;
                case "frontrear":
                    // place textHolder as front...
                    textHolder.transform.SetParent(go.transform);
                    textHolder.transform.localPosition = new Vector3(0, 0, go.transform.localScale.z / -2);
                    textHolder.transform.localPosition += new Vector3(0, 0, 0.06f);
                    textHolder.transform.localEulerAngles = Vector3.zero;
                    text.rectTransform.sizeDelta = new Vector2(go.transform.localScale.x, go.transform.localScale.y);
                    // ... and create a new one for rear
                    GameObject textHolderBis = new GameObject();
                    textHolderBis.name = "textHolderBis";
                    TextMeshPro textBis = textHolderBis.AddComponent<TextMeshPro>();
                    textBis.text = go.name;
                    textBis.fontSize = 0.5f;
                    textBis.alignment = TextAlignmentOptions.MidlineGeoAligned;
                    textHolderBis.transform.SetParent(go.transform);
                    textHolderBis.transform.localPosition = new Vector3(0, 0, go.transform.localScale.z / 2);
                    textHolderBis.transform.localPosition += new Vector3(0, 0, -0.06f);
                    textHolderBis.transform.localEulerAngles = new Vector3(0, 180, 0);
                    textBis.rectTransform.sizeDelta = new Vector2(go.transform.localScale.x, go.transform.localScale.y);
                    break;
            }

            go.GetComponent<Renderer>().material = GameManager.gm.defaultMat;
            Material mat = go.GetComponent<Renderer>().material;
            Color myColor = new Color();
            ColorUtility.TryParseHtmlString($"#{comp.color}", out myColor);
            mat.color = new Color(myColor.r, myColor.g, myColor.b, 0.5f);
        }

#if !DEBUG
        Renderer[] renderers = rack.transform.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
            r.enabled = false;
#endif

        GameManager.gm.allItems.Remove(rack.GetComponent<HierarchyName>().fullname);
        GameManager.gm.rackTemplates.Add(rack.name, rack.gameObject);
    }

    ///<summary>
    /// Create a room from _json data...
    ///</summary>
    ///<param name="_json">Json to parse</param>
    public void CreateRoomTemplate(string _json)
    {
        SRoomFromJson roomData = JsonUtility.FromJson<SRoomFromJson>(_json);
        if (GameManager.gm.roomTemplates.ContainsKey(roomData.slug))
            return;

        GameManager.gm.roomTemplates.Add(roomData.slug, roomData);
    }

}
