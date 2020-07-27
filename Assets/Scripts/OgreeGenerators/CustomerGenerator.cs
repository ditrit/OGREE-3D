﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerGenerator : MonoBehaviour
{
    public static CustomerGenerator instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
            Destroy(this);
    }

    public void CreateCustomer(string _name, bool _changeHierarchy)
    {
        GameObject customer = new GameObject(_name);
        customer.AddComponent<Customer>();

        customer.AddComponent<HierarchyName>();
        if (_changeHierarchy)
            GameManager.gm.currentItem = customer;
    }

    public void CreateDatacenter(SDataCenterInfos _data, bool _changeHierarchy)
    {
        if (_data.parent.GetComponent<Customer>() == null)
        {
            Debug.LogWarning("Datacenter must be child of a customer");
            return;
        }

        GameObject newDC = new GameObject(_data.name);
        newDC.transform.parent = _data.parent;

        Datacenter dc = newDC.AddComponent<Datacenter>();
        dc.address = _data.address;
        dc.zipcode = _data.zipcode;
        dc.city = _data.city;
        dc.country = _data.country;
        dc.description = _data.description;

        switch (_data.orient)
        {
            case "N":
                dc.orientation = EOrientation.N;
                newDC.transform.localEulerAngles = new Vector3(0, 0, 0);
                break;
            case "S":
                dc.orientation = EOrientation.S;
                newDC.transform.localEulerAngles = new Vector3(0, 180, 0);
                break;
            case "W":
                dc.orientation = EOrientation.W;
                newDC.transform.localEulerAngles = new Vector3(0, 90, 0);
                break;
            case "E":
                dc.orientation = EOrientation.E;
                newDC.transform.localEulerAngles = new Vector3(0, -90, 0);
                break;
        }

        newDC.AddComponent<HierarchyName>();
        if (_changeHierarchy)
            GameManager.gm.currentItem = newDC;
    }
}