﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPersonCamera : MonoBehaviour
{



    private GameObject Main_Camera;
    private GameObject DataLoader;


    private float camSens = 0.10f; //How sensitive it with mouse
    private Vector3 pos;
    private Vector3 dir;
    private Vector3 lastMouse;
    private float fov;
    private Ray ray;
    private RaycastHit hit;
    //private float distance;
    private float totalRun = 1.0f;
    private float zoom = 0.0f;
    private float sens = 10f;
    private float minFov = 15f;
    private float maxFov = 90f;

    void start()
    {
        Main_Camera = GameObject.Find("Main_Camera");
        DataLoader = GameObject.Find("DataLoader");
        pos = Main_Camera.transform.position;
        //distance = Main_Camera.transform.position.z - DataLoader.transform.position.z;
        fov = Camera.main.fieldOfView+50;
        dir = (pos - DataLoader.transform.position).normalized;

        lastMouse = dir;
    }

    void Update()
    {
        updateCamera();
        updateView();
    }

    public void updateCamera()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            lastMouse = Input.mousePosition - lastMouse;
            lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
            lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
            transform.eulerAngles = lastMouse;
            lastMouse = Input.mousePosition;


        }

        fov += Input.GetAxis("Mouse ScrollWheel") * sens;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        //float y = transform.position.y;
        //float x = transform.position.x;

        //transform.position = new Vector3(x, y, distance);
        Camera.main.fieldOfView = fov;
    }

    public void updateView()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log(Input.GetMouseButtonDown(0));
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.parent.gameObject.tag == "SingleConnectome")
                {
                    int id = int.Parse(hit.transform.name);
                    SingleConnectome parent = hit.transform.parent.gameObject.GetComponent<SingleConnectome>();
                    parent.DrawEdges(id);
                }
                if (hit.collider.transform.gameObject.name == "AnatomyParent")
                {
                    GameObject parent = hit.collider.transform.parent.parent.gameObject;
                    SingleConnectome name = parent.GetComponentInChildren<SingleConnectome>();
                    name.representationType = SingleConnectome.representationTypes.Anatomy;
                }
                if (hit.collider.transform.gameObject.name == "TsneParent")
                {
                    GameObject parent1 = hit.collider.transform.parent.parent.gameObject;
                    SingleConnectome name = parent1.GetComponentInChildren<SingleConnectome>();
                    name.representationType = SingleConnectome.representationTypes.Tsne;
                }
                if (hit.collider.transform.gameObject.name == "MdsParent")
                {
                    GameObject parent1 = hit.collider.transform.parent.parent.gameObject;
                    SingleConnectome name = parent1.GetComponentInChildren<SingleConnectome>();
                    name.representationType = SingleConnectome.representationTypes.MDS;
                }
                if (hit.collider.transform.gameObject.name == "IsomapParent")
                {
                    GameObject parent1 = hit.collider.transform.parent.parent.gameObject;
                    SingleConnectome name = parent1.GetComponentInChildren<SingleConnectome>();
                    name.representationType = SingleConnectome.representationTypes.Isomap;
                }
                if (hit.collider.transform.gameObject.name == "CAnatomyParent")
                {
                    GameObject parent1 = hit.collider.transform.parent.parent.gameObject;
                    SingleConnectome name = parent1.GetComponentInChildren<SingleConnectome>();
                    name.classificationType = SingleConnectome.classificationTypes.Anatomy;
                }
                if (hit.collider.transform.gameObject.name == "EmbeddnessParent")
                {
                    GameObject parent1 = hit.collider.transform.parent.parent.gameObject;
                    SingleConnectome name = parent1.GetComponentInChildren<SingleConnectome>();
                    name.classificationType = SingleConnectome.classificationTypes.Embeddness;
                }
                if (hit.collider.transform.gameObject.name == "RichClubParent")
                {
                    GameObject parent1 = hit.collider.transform.parent.parent.gameObject;
                    SingleConnectome name = parent1.GetComponentInChildren<SingleConnectome>();
                    name.classificationType = SingleConnectome.classificationTypes.RichClub;
                }
            }

        }
    }

}
