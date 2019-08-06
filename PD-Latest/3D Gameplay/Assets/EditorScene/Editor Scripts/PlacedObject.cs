﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class PlacedObject : MonoBehaviour {

    // UI
    private Slider timelineSlider;
    public Button saveButton;

    // Audio
    public AudioSource menuSFXAudioSource; // Audio source
    public AudioClip colorChangedSound; // Sound that plays when changing the hit object color

    // Gameobjects
    private GameObject timelineObject;
    public GameObject editorHitObjectCursor; // Editor hit object used for tracking the position and saving the position   
    private GameObject raycastTimelineObject; // The timeline object that was clicked on in the timeline bar
    private GameObject instantiatedEditorHitObject; // The instantiated editor hit object that is added to the scene when a timeline bar has been clicked
    public GameObject[] placedHitObjects = new GameObject[6];
    public GameObject[] timelineObjects = new GameObject[6];
    private List<GameObject> previewHitObjectList = new List<GameObject>(); // Preview hit objects that have been spawned when the preview button has been pressed and the song timer has reached the spawn time for the hit object
    public List<EditorHitObject> editorHitObjectList = new List<EditorHitObject>(); // List of editorHitObjects (includes spawn time, object type and positions)
    public List<GameObject> instantiatedTimelineObjectList = new List<GameObject>(); // List of instantiated timeline objects that are added to the list when instantiated
    
    // Integers
    private int hitObjectTypeBlueValue, hitObjectTypeRedValue, hitObjectTypePurpleValue, hitObjectTypeYellowValue, hitObjectTypeGreenValue,
        hitObjectTypeOrangeValue; // Number values for the hit object types - array
    private int specialTimeKeyPresses;
    private int instantiatedTimelineObjectType;
    private int latestBeatsnapIndex; // The last beatsnap to have played (time/index in the tick list for metronome)
    private int raycastTimelineObjectListIndex; // The index of the timeline bar clicked in the editor, used to delete and update existing notes spawn times, position etc by getting the index on click
    private int timelineObjectIndex; // The index for all editor objects, increases by 1 everytime one is instantiated
    private int nullTimelineObjectIndex; // Index for checking null gameobjects
    private int hitObjectSavedType; // Saved hit object type
    private int currentTickIndex; // Current tick in the song
    private int previousTickIndex; // Previous tick in the song
    private float timelineBarHandlePositionX, timelineBarHandlePositionY, timelineBarHandlePositionZ; // Timeline bar position 
    private float currentSongTimePercentage; // The current time in the song turned to percentage value
    private float currentTickTime; // Current tick time
    private float nextTickTime; // Next tick time
    private float userPressedTime; // Time the user pressed the key down
    private float closestTickTime; // Closest tick time based on the user pressing the key down
    private float calculatedTickSpawnTime; // Calculated time for the hit object spawn
    private float hitObjectSpawnTime; // Hit object spawn time
    private float deactivateObjectTimer; // Timer for controlling checks on deactivating timeline hit objects
    private List<float> tickTimesList = new List<float>(); // Tick times for comparing and calculating the closest tick time based on user key press time
    private List<int> nullObjectsList = new List<int>(); // List of all null gameobjects
    
    // Vectors
    private Vector3 instantiatePosition, timelineObjectPosition;
    private Vector3 timelineBarHandlePosition; // Timeline bar position

    // Bools
    private bool hasPressedSpacebar; // Tracking when the spacebar has been pressed to start the song and timer
    private bool hasCreatedLeaderboard; // Allowing the user to create a leaderboard once
    private bool hasSaved; // Allowing the user to save the beatmap once
    private bool pressedKeyS, pressedKeyD, pressedKeyF, pressedKeyJ, pressedKeyK, pressedKeyL; // Keys pressed in the beatmap
    private bool instantiatedEditorHitObjectExists; // Used to check if a timeline bar has been clicked, instantiating a hitobject to appear on screen, if another timeline bar is pressed
    private bool objectSpawnTimeIsTaken; // Check if spawn time already exists or taken by another hit object

    // Colors
    public Color greenTimelineBarColor, yellowTimelineBarColor, orangeTimelineBarColor, blueTimelineBarColor, purpleTimelineBarColor, redTimelineBarColor;

    // Transform
    public Transform canvas, timeline;

    // Scripts
    private SongProgressBar songProgressBar;
    private EditorSoundController editorSoundController; // The editorSoundController
    private MetronomePro_Player metronomePro_Player; // Get the current song time, position of the handle and slide value for placed diamond bars on the timeline
    private MetronomePro metronomePro; // Controls metronome
    private EditorUIManager editorUIManager; // UI manager for controlling UI elements
    private BeatsnapManager beatsnapManager; // Beatsnap
    private DestroyTimelineObject destroyTimelineObject; // Destroy timeline object script attached to instantiated timeline objects


    // Propertiess

    public bool PressedKeyS
    {
        get { return pressedKeyS; }
    }

    public bool PressedKeyD
    {
        get { return pressedKeyD; }
    }

    public bool PressedKeyF
    {
        get { return pressedKeyF; }
    }

    public bool PressedKeyJ
    {
        get { return pressedKeyJ; }
    }

    public bool PressedKeyK
    {
        get { return pressedKeyK; }
    }

    public bool PressedKeyL
    {
        get { return pressedKeyL; }
    }


    // Use this for initialization
    void Start () {

        // Intialize

        ResetKeysPressed();
        hitObjectTypeBlueValue = 0;
        hitObjectTypePurpleValue = 1;
        hitObjectTypeRedValue = 2;
        hitObjectTypeGreenValue = 3;
        hitObjectTypeYellowValue = 4;
        hitObjectTypeOrangeValue = 5;
        specialTimeKeyPresses = 0;
        nullTimelineObjectIndex = 0;
        hitObjectSavedType = 0;
        hitObjectSpawnTime = 0;
        timelineBarHandlePositionY = 9999;
        hasPressedSpacebar = false;
        objectSpawnTimeIsTaken = false;


        // Reference
        songProgressBar = FindObjectOfType<SongProgressBar>();
        editorSoundController = FindObjectOfType<EditorSoundController>(); 
        metronomePro_Player = FindObjectOfType<MetronomePro_Player>();
        metronomePro = FindObjectOfType<MetronomePro>();
        editorUIManager = FindObjectOfType<EditorUIManager>();
        beatsnapManager = FindObjectOfType<BeatsnapManager>();
    }
	
	// Update is called once per frame
	void Update () {

        // Timer increment
        deactivateObjectTimer += Time.deltaTime;

        // Check timeline objects every 5 seconds
        if (deactivateObjectTimer > 5)
        {
            // Disable the timeline objects 
            DisableTimelineObjects();

            // Reset timer
            deactivateObjectTimer = 0;
        }
 

        // BLUE Key Pressed
        if (Input.GetKeyDown(KeyCode.J))
        {
            // Add a new editor hit object to the editorHitObjectList, and instantiate a new timeline object for this hit object on the timeline
            AddEditorHitObjectToList(hitObjectTypeBlueValue);
        }
        // PURPLE Key Pressed
        else if (Input.GetKeyDown(KeyCode.K))
        {
            // Add a new editor hit object to the editorHitObjectList, and instantiate a new timeline object for this hit object on the timeline
            AddEditorHitObjectToList(hitObjectTypePurpleValue);
        }
        // RED Key Pressed
        else if (Input.GetKeyDown(KeyCode.L))
        {
            // Add a new editor hit object to the editorHitObjectList, and instantiate a new timeline object for this hit object on the timeline
            AddEditorHitObjectToList(hitObjectTypeRedValue);
        }
        // GREEN Key Pressed
        else if (Input.GetKeyDown(KeyCode.U) || Input.GetKeyDown(KeyCode.S))
        {
            // Add a new editor hit object to the editorHitObjectList, and instantiate a new timeline object for this hit object on the timeline
            AddEditorHitObjectToList(hitObjectTypeGreenValue);
        }
        // YELLOW Key Pressed
        else if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.D))
        {
            // Add a new editor hit object to the editorHitObjectList, and instantiate a new timeline object for this hit object on the timeline
            AddEditorHitObjectToList(hitObjectTypeYellowValue);
        }
        // ORANGE Key Pressed
        else if (Input.GetKeyDown(KeyCode.O) || Input.GetKeyDown(KeyCode.F))
        {
            // Add a new editor hit object to the editorHitObjectList, and instantiate a new timeline object for this hit object on the timeline
            AddEditorHitObjectToList(hitObjectTypeOrangeValue);
        }
    }

    // Check the keys contained within the beatmap
    public void CheckKeysForBeatmap()
    {
        for (int i = 0; i < editorHitObjectList.Count; i++)
        {
            int keyPressed = editorHitObjectList[i].hitObjectType;

            switch (keyPressed)
            {
                case 0:
                    pressedKeyJ = true;
                    break;
                case 1:
                    pressedKeyK = true;
                    break;
                case 2:
                    pressedKeyL = true;
                    break;
                case 3:
                    pressedKeyS = true;
                    break;
                case 4:
                    pressedKeyD = true;
                    break;
                case 5:
                    pressedKeyF = true;
                    break;
            }
        }
    }

    // Destroy all previewHitObjects that appear on screen
    private void DestroyAllPreviewHitObjects()
    {
        for (int i = 0; i < previewHitObjectList.Count; i++)
        {
            Destroy(previewHitObjectList[i]);
        }
    }

    // Delete all objects in the instantiated timeline list then clear it
    private void DestroyAllInstantiatedTimelineObjects()
    {
        for (int i = 0; i < instantiatedTimelineObjectList.Count; i++)
        {
            Destroy(instantiatedTimelineObjectList[i]);
        }
    }

    // Reset the editor
    public void ResetEditor()
    {
        // Clear all editor hit objects
        editorHitObjectList.Clear();

        // Destroy all preview hit objects
        DestroyAllPreviewHitObjects();

        // Clear the previewHitObject list
        previewHitObjectList.Clear();

        // Clear all null object list
        nullObjectsList.Clear();

        // Delete all objects in the instantiated timeline list then clear it
        DestroyAllInstantiatedTimelineObjects();

        // Clear the instantaitedTimelineObjectList also
        instantiatedTimelineObjectList.Clear();

        // Reset the raycast timeline object index
        raycastTimelineObjectListIndex = 0;

        // Reset object saved type
        hitObjectSavedType = 0;

        // Reset null index
        nullTimelineObjectIndex = 0;

        // Reset the song to 0 and the metronome
        metronomePro_Player.StopSong();

        // Reset tick times list
        tickTimesList.Clear();

        // Reset keys pressed
        ResetKeysPressed();

        // DestroyInstantiatedEditorHitObject
        DestroyInstantiatedEditorHitObject();

        // Set editor hit object to false
        instantiatedEditorHitObjectExists = false;
    }

    // Destroy the instantiatedEditorHitObject that is spawned when a timeline object has been clicked
    public void DestroyInstantiatedEditorHitObject()
    {
        Destroy(instantiatedEditorHitObject);
    }

    // Get the index of the timeline object clicked on
    public int GetIndexOfRaycastTimelineObject(GameObject _gameObject)
    {
        // Check if an editor hit object already exists on the map, if it does delete it before adding a new one to the scene to edit
        if (instantiatedEditorHitObjectExists == true)
        {
            // Destroy the current hit editor object
            Destroy(instantiatedEditorHitObject);
            // Set back to false
            instantiatedEditorHitObjectExists = false;
        }

        // The timeline object reference is passed and assigned
        raycastTimelineObject = _gameObject;

        // Get the index number for the timeline object passed in the instantiated list it was inserted into when instantiated
        raycastTimelineObjectListIndex = instantiatedTimelineObjectList.IndexOf(raycastTimelineObject);

        // Instantiate the hit object saved with this timeline index
        InstantiateEditorHitObject();

        return raycastTimelineObjectListIndex;
    }

    // Save the changed instantiated editor objects position
    public void SaveNewInstantiatedEditorObjectsPosition()
    {
        // Set the saved editor position to the new position of the current object/replace the old value
        editorHitObjectList[raycastTimelineObjectListIndex].hitObjectPosition = instantiatedEditorHitObject.transform.position;
    }

    // Removes the timeline object from the list
    public void RemoveTimelineObject()
    {
        // The index of null objects found
        nullTimelineObjectIndex = 0;

        // Check if any objects are null in the instantiatedTimelineObjectList
        for (int i = 0; i < instantiatedTimelineObjectList.Count; i++)
        {
            // Check if any objects are null in the list
            if (instantiatedTimelineObjectList[i] == null)
            {
                // Set the index to the null object index found
                nullTimelineObjectIndex = i;
                // Add the index of a null object to the null object list
                nullObjectsList.Add(nullTimelineObjectIndex);
            }
        }
    
        // Remove all null objects and their associated information lists for the hit objects
        for (int i = nullObjectsList.Count - 1; i > -1; i--)
        {
            // Get the null object index from the list
            nullTimelineObjectIndex = nullObjectsList[i];

            // Remove the timeline object from the list
            instantiatedTimelineObjectList.RemoveAt(nullTimelineObjectIndex);

            // Remove the editorHitObject tied to the timeline from the list
            editorHitObjectList.RemoveAt(nullTimelineObjectIndex);
        }

        // Clear the list for next time
        nullObjectsList.Clear();

        // Reset for next time
        nullTimelineObjectIndex = 0;
    }

    // Update editorHitObject in the list's spawn time to the new value
    public void UpdateEditorHitObjectSpawnTime(float _spawnTime, int _hitObjectIndex)
    {
        // Slider change updates the spawn time for this editor hit object
        editorHitObjectList[_hitObjectIndex].hitObjectSpawnTime = _spawnTime;
    }

    // Get the information for the editor hit object which is instantiated when the timeline bar has been clicked
    public Vector3 GetHitObjectPositionInformation()
    {
        // Get the position for the hit object saved
        Vector3 hitObjectSavedPosition = editorHitObjectList[raycastTimelineObjectListIndex].hitObjectPosition;
        // Return the position back to the instantiation function for the hit object
        return hitObjectSavedPosition;
    }

    // Get the hit object type information
    public int GetHitObjectTypeInformation()
    {
        // Get the object type for the hit object saved
        // Return the type back to the instantiation function for the hit object
        return editorHitObjectList[raycastTimelineObjectListIndex].hitObjectType;
    }

    // Instantiate the editor hit object in the editor scene for the timeline object selected with its correct positioning, disable the fade script
    public void InstantiateEditorHitObject()
    {
        // Run functions to get the position, type and spawn time of this editor object based off the timelineobjectindex
        // Get the hit object position saved
        Vector3 hitObjectSavedPosition = GetHitObjectPositionInformation();

        // Get the hit object type saved
        hitObjectSavedType = GetHitObjectTypeInformation();

        // Instantiate the editor hit object with its loaded information previously saved
        instantiatedEditorHitObject = Instantiate(placedHitObjects[hitObjectSavedType], hitObjectSavedPosition, Quaternion.Euler(-90, 45, 0));

        // Set to true as object has been instantiated
        instantiatedEditorHitObjectExists = true;
    }

    // Instantiate a timeline object at the current song time
    public void InstantiateTimelineObject(int _instantiatedTimelineObjectType, float _hitObjectSpawnTime)
    {
        // Get the handle position currently in the song to spawn the timeline object at
        timelineBarHandlePositionX = metronomePro_Player.songPointSliderHandle.transform.position.x;
        // Decrease the Y position to prevent overlap
        timelineBarHandlePositionY = 0;
        // Get the Z position
        timelineBarHandlePositionZ = metronomePro_Player.songPointSliderHandle.transform.position.z;
        // Assign the new position
        timelineBarHandlePosition = new Vector3(timelineBarHandlePositionX, timelineBarHandlePositionY, timelineBarHandlePositionZ);

        // Instantiate the type of object
        timelineObject = Instantiate(timelineObjects[_instantiatedTimelineObjectType], timelineBarHandlePosition,
        Quaternion.Euler(90, 0, 0), timeline);

        // Get the timeline slider from the timeline object instantiated
        timelineSlider = timelineObject.GetComponent<Slider>();

        // Add the instantiated timeline object to the list of instantiated timeline objects
        instantiatedTimelineObjectList.Add(timelineObject);

        // Get the reference to the destroy timeline object script attached to the timeline object
        destroyTimelineObject = timelineObject.GetComponent<DestroyTimelineObject>();
        // Set the spawn time inside the object to the spawn time calculated from ticks previously
        destroyTimelineObject.timelineHitObjectSpawnTime = _hitObjectSpawnTime;

        // Calculate the slider value based off the timeline hit object spawn time
        // Update the instantiated timeline hit object's slider to the correct value calculated from ticks
        timelineSlider.value = CalculateTimelineHitObjectSliderValue(_hitObjectSpawnTime); ;

        // Increase the timeline object index
        timelineObjectIndex++;
    }

    // Calculate the timeline editor hit object sliders value based off the tick time converted to percentage of 0-1 slider value
    private float CalculateTimelineHitObjectSliderValue(float _spawnTime)
    {
        // Get how much % the spawn time is out of the entire clip length
        currentSongTimePercentage = (_spawnTime / metronomePro.songAudioSource.clip.length);

        // Calculate and return the percentage of 1 based on percentage of currentSongTimePercentage
        return (currentSongTimePercentage / 1);
    }

    // Get current beatsnap tick time
    private float GetCurrentBeatsnapTime()
    {
        // The current tick index and time
        currentTickIndex = metronomePro.CurrentTick;
        currentTickTime = (float)metronomePro.songTickTimes[currentTickIndex];
        tickTimesList.Add(currentTickTime);

        // The next tick index and time
        previousTickIndex = metronomePro.CurrentTick - 1;
        nextTickTime = (float)metronomePro.songTickTimes[previousTickIndex];
        tickTimesList.Add(nextTickTime);

        // Get the time the user pressed the key down
        userPressedTime = metronomePro_Player.songAudioSource.time;

        // Check which time the users press was closest to
        closestTickTime = tickTimesList.Select(p => new { Value = p, Difference = Math.Abs(p - userPressedTime) })
        .OrderBy(p => p.Difference)
        .First().Value;

        // Snap the hit object to this time
        calculatedTickSpawnTime = closestTickTime;

        // Reset list
        tickTimesList.Clear();

        return calculatedTickSpawnTime;


        /*
        List<float> tickTimesList = new List<float>();

        // The current tick index and time
        int currentTickIndex = metronomePro.CurrentTick;
        float currentTickTime = (float)metronomePro.songTickTimes[currentTickIndex];
        tickTimesList.Add(currentTickTime);

        // The next tick index and time
        int previousTickIndex = metronomePro.CurrentTick - 1;
        float nextTickTime = (float)metronomePro.songTickTimes[previousTickIndex];
        tickTimesList.Add(nextTickTime);

        // Get the time the user pressed the key down
        float userPressedTime = metronomePro_Player.songAudioSource.time;

        // Check which time the users press was closest to
        float closestTime = tickTimesList.Select(p => new { Value = p, Difference = Math.Abs(p - userPressedTime) })
        .OrderBy(p => p.Difference)
        .First().Value;

        // Snap the hit object to this time
        float hitObjectSpawnTime = closestTime;

        return hitObjectSpawnTime;
        */
    }

    // Instantiate placed hit object at the position on the mouse
    public void InstantiateEditorPlacedHitObject(int _editorHitObjectType)
    {
        // Instantiate a new placed object onto the grid where the current cursor position is
        GameObject instantiatedEditorPlacedHitObject = Instantiate(placedHitObjects[_editorHitObjectType], Vector3.zero, Quaternion.Euler(-90, 45, 0));

        // Set the object to be in the canvas
        instantiatedEditorPlacedHitObject.transform.SetParent(canvas);

        // Update the position
        instantiatedEditorPlacedHitObject.transform.position = editorHitObjectCursor.transform.position;
    }

    // Check if the spawn time for the hit object is taken or available
    private void CheckIfSpawnTimeIsTaken()
    {
        // Check through all the editor hit objects in the list, check if the spawn time exists already
        for (int i = 0; i < editorHitObjectList.Count; i++)
        {
            if (hitObjectSpawnTime == editorHitObjectList[i].hitObjectSpawnTime)
            {
                objectSpawnTimeIsTaken = true;
                break;
            }
        }
    }

    // Add a new editor hit object to the editorHitObjectList saving the spawn times, positions and object type. Instantiate a timeline object for this object also
    public void AddEditorHitObjectToList(int _objectType)
    {
        // Check if another hit object has the same spawn time based off ticks, if another hit object exists do not instantiate or add to the list
        hitObjectSpawnTime = GetCurrentBeatsnapTime();

        // Reset
        objectSpawnTimeIsTaken = false;

        // Check if the spawn time for the hit object is taken or available
        CheckIfSpawnTimeIsTaken();

        // If the objects spawn time does not exist/is not taken, allow instantiation of another hit object
        if (objectSpawnTimeIsTaken == false)
        {
            // Set the instantiate position to the editor hit object position but with a Y of 0
            InstantiateEditorPlacedHitObject(_objectType);

            // Call the instantiateTimelineObject function and pass the object type to instantiate a timeline object of the correct note color type
            InstantiateTimelineObject(_objectType, hitObjectSpawnTime);

            // Create a new editor hit object (class object) and assign all the variables such as position, spawn time and type
            EditorHitObject newEditorHitObject = new EditorHitObject();
            newEditorHitObject.hitObjectPosition = editorHitObjectCursor.transform.position;
            newEditorHitObject.hitObjectType = _objectType;
            newEditorHitObject.hitObjectSpawnTime = hitObjectSpawnTime;

            // Add the newEditorHitObject to the editorHitObjectList
            editorHitObjectList.Add(newEditorHitObject);


            // Reorder the editorHitObject list
            SortListOrders();
        }
    }

    // Sort all lists based on spawn time so they're in order
    public void SortListOrders()
    {
        editorHitObjectList = editorHitObjectList.OrderBy(w => w.hitObjectSpawnTime).ToList();
    }

    // Disable timeline objects based on song time
    private void DisableTimelineObjects()
    {
        if (editorHitObjectList.Count != 0)
        {
            for (int i = 0; i < editorHitObjectList.Count; i++)
            {
                // Get spawn time for timeline object
                // Check the current time
                // If current time is greater by 10 seconds of the hit object spawn time
                // Deactivate the timeline object

                float timelineObjectSpawnTime = editorHitObjectList[i].hitObjectSpawnTime;
                float currentSongTime = metronomePro.songAudioSource.time;
                int deactivateValue = 5;
                float deactivateObjectTime = (timelineObjectSpawnTime + deactivateValue);

                // If the current song time is greater than the time to deactivate the hit object based off its spawn time
                if (currentSongTime > deactivateObjectTime)
                {
                    // Deactivate the timeline hit object
                    instantiatedTimelineObjectList[i].gameObject.SetActive(false);
                }
                else
                {
                    // Keep the game object active
                    if (instantiatedTimelineObjectList[i].gameObject.activeSelf == false)
                    {
                        instantiatedTimelineObjectList[i].gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    // Save all list information to the database script
    public void SaveListsToDatabase()
    {
        // Sort the editorHitObjects based on the spawn time
        SortListOrders();

        // Save to the database everything - spawn times, object type, positions
        for (int i = 0; i < editorHitObjectList.Count; i++)
        {
            // Add the positions to the database
            Database.database.positionX.Add(editorHitObjectList[i].hitObjectPosition.x);
            Database.database.positionY.Add(editorHitObjectList[i].hitObjectPosition.y);
            Database.database.positionZ.Add(editorHitObjectList[i].hitObjectPosition.z);
            // Add the spawn times to the database
            Database.database.hitObjectSpawnTime.Add(editorHitObjectList[i].hitObjectSpawnTime);
            // Add the object type to the database
            Database.database.objectType.Add(editorHitObjectList[i].hitObjectType);
        }
    }

    // Disable the save button
    public void DisableSaveButton()
    {
        saveButton.interactable = false;
    }

    // Reset keys pressed if the map has been reset
    public void ResetKeysPressed()
    {
        pressedKeyS = false;
        pressedKeyD = false;
        pressedKeyF = false;
        pressedKeyJ = false;
        pressedKeyK = false;
        pressedKeyL = false;
    }


    /*
    // Change instantiated hit objects material
    public void ChangeInstantiatedEditorHitObjectMaterial(string materialTypePass)
    {
        // Get the timelinebar selected handle image
        Image instantiatedTimelineImage = instantiatedTimelineObjectList[raycastTimelineObjectListIndex].GetComponentInChildren<Image>();

        // Get the renderer attached to the editor hit object
        Renderer rend = instantiatedEditorHitObject.GetComponentInChildren<Renderer>();
        // If the object has a renderer component change its material
        if (rend != null)
        {
            // Change the material based on the type passed
            switch (materialTypePass)
            {
                case "GREEN":
                    instantiatedTimelineImage.color = greenTimelineBarColor;
                    rend.material = greenEditorHitObjectMaterial;
                    // Save the new color/type for the hit object in the list // the number is the type
                    editorHitObjectList[raycastTimelineObjectListIndex].hitObjectType = 3;
                    break;
                case "YELLOW":
                    instantiatedTimelineImage.color = yellowTimelineBarColor;
                    rend.material = yellowEditorHitObjectMaterial;
                    // Save the new color/type for the hit object in the list // the number is the type
                    editorHitObjectList[raycastTimelineObjectListIndex].hitObjectType = 4;
                    break;
                case "ORANGE":
                    instantiatedTimelineImage.color = orangeTimelineBarColor;
                    rend.material = orangeEditorHitObjectMaterial;
                    // Save the new color/type for the hit object in the list // the number is the type
                    editorHitObjectList[raycastTimelineObjectListIndex].hitObjectType = 5;
                    break;
                case "BLUE":
                    instantiatedTimelineImage.color = blueTimelineBarColor;
                    rend.material = blueEditorHitObjectMaterial;
                    // Save the new color/type for the hit object in the list // the number is the type
                    editorHitObjectList[raycastTimelineObjectListIndex].hitObjectType = 0;
                    break;
                case "PURPLE":
                    instantiatedTimelineImage.color = purpleTimelineBarColor;
                    rend.material = purpleEditorHitObjectMaterial;
                    // Save the new color/type for the hit object in the list // the number is the type
                    editorHitObjectList[raycastTimelineObjectListIndex].hitObjectType = 1;
                    break;
                case "RED":
                    instantiatedTimelineImage.color = redTimelineBarColor;
                    rend.material = redEditorHitObjectMaterial;
                    // Save the new color/type for the hit object in the list // the number is the type
                    editorHitObjectList[raycastTimelineObjectListIndex].hitObjectType = 2;
                    break;
            }
        }
    }



    // Check for color change input when a hit object has spawned
    private void CheckForColorChangeInput()
    {
        // Check if number keys have been pressed, if so change the color of the hit object and timeline bars
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PlayColorChangedSound();
            ChangeInstantiatedEditorHitObjectMaterial("GREEN");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlayColorChangedSound();
            ChangeInstantiatedEditorHitObjectMaterial("YELLOW");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PlayColorChangedSound();
            ChangeInstantiatedEditorHitObjectMaterial("ORANGE");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            PlayColorChangedSound();
            ChangeInstantiatedEditorHitObjectMaterial("BLUE");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PlayColorChangedSound();
            ChangeInstantiatedEditorHitObjectMaterial("PURPLE");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PlayColorChangedSound();
            ChangeInstantiatedEditorHitObjectMaterial("RED");
        }
    }

    // Play color changed sound effect
    private void PlayColorChangedSound()
    {
        menuSFXAudioSource.PlayOneShot(colorChangedSound);
    }

    */



}
