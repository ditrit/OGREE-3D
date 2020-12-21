﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGenerator : MonoBehaviour
{
    public static BuildingGenerator instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this);
    }

    ///<summary>
    /// Instantiate a buildingModel (from GameManager) and apply _data to it.
    ///</summary>
    ///<param name="_data">Informations about the building</param>
    ///<returns>The created Building</returns>
    public Building CreateBuilding(SBuildingInfos _data)
    {
        if (_data.parent.GetComponent<OgreeObject>().category != "site")
        {
            GameManager.gm.AppendLogLine("Building must be child of a site", "yellow");
            return null;
        }
        string hierarchyName = $"{_data.parent.GetComponent<HierarchyName>()?.fullname}.{_data.name}";
        if (GameManager.gm.allItems.Contains(hierarchyName))
        {
            GameManager.gm.AppendLogLine($"{hierarchyName} already exists.", "yellow");
            return null;
        }

        GameObject newBD = Instantiate(GameManager.gm.buildingModel);
        newBD.name = _data.name;
        newBD.transform.parent = _data.parent;
        newBD.transform.localEulerAngles = Vector3.zero;

        // originalSize
        Vector3 originalSize = newBD.transform.GetChild(0).localScale;
        newBD.transform.GetChild(0).localScale = new Vector3(originalSize.x * _data.size.x, originalSize.y, originalSize.z * _data.size.z);

        Vector3 origin = newBD.transform.GetChild(0).localScale / 0.2f;
        newBD.transform.localPosition = new Vector3(origin.x, 0, origin.z);
        newBD.transform.localPosition += new Vector3(_data.pos.x, 0, _data.pos.z);

        Building building = newBD.GetComponent<Building>();
        building.name = newBD.name;
        building.parentId = _data.parent.GetComponent<OgreeObject>().id;
        building.category = "building";
        building.domain = _data.parent.GetComponent<OgreeObject>().domain;

        BuildWalls(building.walls, new Vector3(newBD.transform.GetChild(0).localScale.x * 10, _data.size.y, newBD.transform.GetChild(0).localScale.z * 10), 0);
        building.attributes.Add("posXY", new Vector2(_data.pos.x, _data.pos.y).ToString());
        building.attributes.Add("posXYUnit", "m");
        building.attributes.Add("posZ", _data.pos.z.ToString());
        building.attributes.Add("posZUnit", "m");
        building.attributes.Add("size", new Vector2(_data.size.x, _data.size.z).ToString());
        building.attributes.Add("sizeUnit", "m");
        building.attributes.Add("height", _data.size.y.ToString());
        building.attributes.Add("heightUnit", "m");

        newBD.AddComponent<HierarchyName>();
        GameManager.gm.allItems.Add(hierarchyName, newBD);

        return building;
    }

    ///<summary>
    /// Instantiate a roomModel (from GameManager) and apply _data to it.
    ///</summary>
    ///<param name="_data">Informations about the room</param>
    ///<returns>The created Room</returns>
    public Room CreateRoom(SRoomInfos _data)
    {
        if (_data.parent.GetComponent<Building>() == null || _data.parent.GetComponent<Room>())
        {
            GameManager.gm.AppendLogLine("Room must be child of a Building", "yellow");
            return null;
        }
        string hierarchyName = $"{_data.parent.GetComponent<HierarchyName>()?.fullname}.{_data.name}";
        if (GameManager.gm.allItems.Contains(hierarchyName))
        {
            GameManager.gm.AppendLogLine($"{hierarchyName} already exists.", "yellow");
            return null;
        }

        GameObject newRoom = Instantiate(GameManager.gm.roomModel);
        newRoom.name = _data.name;
        newRoom.transform.parent = _data.parent;

        Vector3 size;
        string orient;
        if (string.IsNullOrEmpty(_data.template))
        {
            size = _data.size;
            orient = _data.orient;
            newRoom.GetComponent<Room>().attributes.Add("template", "");
        }
        else if (GameManager.gm.roomTemplates.ContainsKey(_data.template))
        {
            size = new Vector3(GameManager.gm.roomTemplates[_data.template].sizeWDHm[0],
                            GameManager.gm.roomTemplates[_data.template].sizeWDHm[2],
                            GameManager.gm.roomTemplates[_data.template].sizeWDHm[1]);
            orient = GameManager.gm.roomTemplates[_data.template].orientation;
            newRoom.GetComponent<Room>().attributes.Add("template", _data.template);
        }
        else
        {
            GameManager.gm.AppendLogLine($"Unknown template \"{_data.template}\"", "yellow");
            return null;
        }

        Room room = newRoom.GetComponent<Room>();
        room.name = newRoom.name;
        room.parentId = _data.parent.GetComponent<OgreeObject>().id;
        room.category = "room";
        room.domain = _data.parent.GetComponent<OgreeObject>().domain;

        Vector3 originalSize = room.usableZone.localScale;
        room.usableZone.localScale = new Vector3(originalSize.x * size.x, originalSize.y, originalSize.z * size.z);
        room.reservedZone.localScale = room.usableZone.localScale;
        room.technicalZone.localScale = room.usableZone.localScale;
        room.tilesEdges.localScale = room.usableZone.localScale;
        room.tilesEdges.GetComponent<Renderer>().material.mainTextureScale = new Vector2(size.x, size.z) / 0.6f;
        room.tilesEdges.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(size.x / 0.6f % 1, size.z / 0.6f % 1);
        BuildWalls(room.walls, new Vector3(room.usableZone.localScale.x * 10, size.y, room.usableZone.localScale.z * 10), -0.001f);

        Vector3 bdOrigin = _data.parent.GetChild(0).localScale / -0.2f;
        Vector3 roOrigin = room.usableZone.localScale / 0.2f;
        newRoom.transform.position = _data.parent.position;
        newRoom.transform.localPosition += new Vector3(bdOrigin.x, 0, bdOrigin.z);
        newRoom.transform.localPosition += _data.pos;

        room.attributes.Add("posXY", JsonUtility.ToJson(new Vector2(_data.pos.x, _data.pos.y)));
        room.attributes.Add("posXYUnit", "m");
        room.attributes.Add("posZ", _data.pos.z.ToString());
        room.attributes.Add("posZUnit", "m");
        room.attributes.Add("size", JsonUtility.ToJson(new Vector2(size.x, size.z)));
        room.attributes.Add("sizeUnit", "m");
        room.attributes.Add("height", size.y.ToString());
        room.attributes.Add("heightUnit", "m");
        switch (orient)
        {
            case "EN":
                room.attributes.Add("orientation", "EN");
                newRoom.transform.eulerAngles = new Vector3(0, 0, 0);
                newRoom.transform.position += new Vector3(roOrigin.x, 0, roOrigin.z);
                break;
            case "WS":
                room.attributes.Add("orientation", "WS");
                newRoom.transform.eulerAngles = new Vector3(0, 180, 0);
                newRoom.transform.position += new Vector3(-roOrigin.x, 0, -roOrigin.z);
                break;
            case "NW":
                room.attributes.Add("orientation", "NW");
                newRoom.transform.eulerAngles = new Vector3(0, -90, 0);
                newRoom.transform.position += new Vector3(-roOrigin.z, 0, roOrigin.x);
                break;
            case "SE":
                room.attributes.Add("orientation", "SE");
                newRoom.transform.eulerAngles = new Vector3(0, 90, 0);
                newRoom.transform.position += new Vector3(roOrigin.z, 0, -roOrigin.x);
                break;
        }

        // Set UI room's name
        room.nameText.text = newRoom.name;
        room.nameText.rectTransform.sizeDelta = new Vector2(size.x, size.z);

        // Add room to GUI room filter
        Filters.instance.AddIfUnknown(Filters.instance.roomsList, newRoom.name);
        Filters.instance.UpdateDropdownFromList(Filters.instance.dropdownRooms, Filters.instance.roomsList);

        room.UpdateZonesColor();

        string hn = newRoom.AddComponent<HierarchyName>().fullname;
        GameManager.gm.allItems.Add(hn, newRoom);

        if (!string.IsNullOrEmpty(_data.template) && GameManager.gm.roomTemplates.ContainsKey(_data.template))
        {
            ReadFromJson.SRoomFromJson template = GameManager.gm.roomTemplates[_data.template];
            room.SetAreas(new SMargin(template.reservedArea), new SMargin(template.technicalArea));

            foreach (ReadFromJson.SSeparator sep in template.separators)
                CreateSeparatorFromJson(sep, newRoom.transform);
        }

        return room;
    }

    ///<summary>
    /// Instantiate a separatorModel (from GameManager) and apply _data to it.
    ///</summary>
    ///<param name="_data">Informations about the separator</param>
    public void CreateSeparator(SSeparatorInfos _data)
    {
        float length = Vector3.Distance(_data.pos1XYm, _data.pos2XYm);
        float height = _data.parent.GetComponent<Room>().walls.GetChild(0).localScale.y;
        float angle = Vector3.SignedAngle(Vector3.right, _data.pos2XYm - _data.pos1XYm, Vector3.up);
        // Debug.Log($"[{_data.name}]=> {angle}");

        GameObject separator = Instantiate(GameManager.gm.separatorModel);
        separator.name = _data.name;
        separator.transform.parent = _data.parent;

        // Set textured box
        separator.transform.GetChild(0).localScale = new Vector3(length, height, 0.001f);
        separator.transform.GetChild(0).localPosition = new Vector3(length, height, 0) / 2;
        Renderer rend = separator.transform.GetChild(0).GetComponent<Renderer>();
        rend.material.mainTextureScale = new Vector2(length, height) * 1.5f;

        // Place the separator in the right place
        Vector3 roomScale = _data.parent.GetComponent<Room>().technicalZone.localScale * -5;
        separator.transform.localPosition = new Vector3(roomScale.x, 0, roomScale.z);
        separator.transform.localPosition += new Vector3(_data.pos1XYm.x, 0, _data.pos1XYm.y);
        separator.transform.localEulerAngles = new Vector3(0, -angle, 0);

        string hn = separator.AddComponent<HierarchyName>().fullname;
        GameManager.gm.allItems.Add(hn, separator);
    }

    ///<summary>
    /// Set walls children of _root. They have to be in Front/Back/Right/Left order.
    ///</summary>
    ///<param name="_root">The root of walls.</param>
    ///<param name="_dim">The dimensions of the building/room</param>
    ///<param name="_offset">The offset for avoiding walls overlaping</param>
    private void BuildWalls(Transform _root, Vector3 _dim, float _offset)
    {
        Transform wallFront = _root.GetChild(0);
        Transform wallBack = _root.GetChild(1);
        Transform wallRight = _root.GetChild(2);
        Transform wallLeft = _root.GetChild(3);

        wallFront.localScale = new Vector3(_dim.x, _dim.y, 0.01f);
        wallBack.localScale = new Vector3(_dim.x, _dim.y, 0.01f);
        wallRight.localScale = new Vector3(_dim.z, _dim.y, 0.01f);
        wallLeft.localScale = new Vector3(_dim.z, _dim.y, 0.01f);

        wallFront.localPosition = new Vector3(0, wallFront.localScale.y / 2, _dim.z / 2 + _offset);
        wallBack.localPosition = new Vector3(0, wallFront.localScale.y / 2, -(_dim.z / 2 + _offset));
        wallRight.localPosition = new Vector3(_dim.x / 2 + _offset, wallFront.localScale.y / 2, 0);
        wallLeft.localPosition = new Vector3(-(_dim.x / 2 + _offset), wallFront.localScale.y / 2, 0);
    }

    ///<summary>
    /// Convert ReadFromJson.SSeparator to SSeparatorInfos and call CreateSeparator().
    ///</summary>
    ///<param name="_sepData">Data from json</param>
    ///<param name="_root">The room of the separator</param>
    private void CreateSeparatorFromJson(ReadFromJson.SSeparator _sepData, Transform _root)
    {
        SSeparatorInfos infos = new SSeparatorInfos();
        infos.name = _sepData.name;
        infos.pos1XYm = new Vector2(_sepData.pos1XYm[0], _sepData.pos1XYm[1]);
        infos.pos2XYm = new Vector2(_sepData.pos2XYm[0], _sepData.pos2XYm[1]);
        infos.parent = _root;

        CreateSeparator(infos);
    }
}
