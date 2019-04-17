﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorHitObjectMouseFollow : MonoBehaviour {

    private GameObject editorGhostObject;
    private GameObject instantiatedEditorHitObject;

    // Is true when the hit object is following the mouse, false when it isn't
    private bool raycastObjectDragActive;


    // Update is called once per frame
    void Update()
    {
        // Get the reference to the editor ghost object and the instantiated hit object 
        if (editorGhostObject == null || instantiatedEditorHitObject == null)
        {
            editorGhostObject = GameObject.FindGameObjectWithTag("EditorHitObject");
            instantiatedEditorHitObject = GameObject.FindGameObjectWithTag("InstantiatedEditorHitObject");
        }
        
        // If both references are got update the instantiated editor hit object position to the same as the editor ghost position
        if (editorGhostObject != null && instantiatedEditorHitObject != null)
        {

            // If M key is pressed start updating the instantiated editor hit object position
            if (Input.GetKeyDown(KeyCode.M) && raycastObjectDragActive == false)
            {
                // Set is dragging to true
                raycastObjectDragActive = true;
            }
            // If M key is pressed and previously was following the mouse
            else if (Input.GetKeyDown(KeyCode.M) && raycastObjectDragActive == true)
            {
                // Set is dragging to false
                raycastObjectDragActive = false;

                // Save the current position of the instantiated hit object?
            }


            // If moving the object is true
            if (raycastObjectDragActive == true)
            {
                // Start moving the instantiated hit object to the position of the ghost object with grid snapping
                instantiatedEditorHitObject.transform.position = editorGhostObject.transform.position;
            }
            else
            {
                // Do not move the instantiated hit object
            }
        }



    }
}
