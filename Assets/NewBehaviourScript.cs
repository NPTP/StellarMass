using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public bool check;
    
    private void OnValidate()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject g in gameObjects)
        {
            MonoBehaviour[] behaviours = g.GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour behaviour in behaviours)
            {
                if (behaviour == null)
                {
                    Debug.Log(g.name);
                    // DestroyImmediate(behaviour);
                }
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger enter: {other.name}");
    }
}
