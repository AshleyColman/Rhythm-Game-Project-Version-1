﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;

public class BeatmapSetup : MonoBehaviour {

    // Create folder variables
    public string beatmapFolderName;
    public string folderDirectory;

    public int songClipChosenIndex;

    public string beatmapEasyDifficultyLevel;
    public string beatmapAdvancedDifficultyLevel;
    public string beatmapExtraDifficultyLevel;

    // Beatmap difficulty
    public GameObject difficultyButtonsPanel;
    public string beatmapDifficulty;

    // Song preview start time in th song select screen
    public float songPreviewStartTime;
    // The textbox to get the vlaue of the start time
    public TMP_InputField songPreviewStartTimeInputField;
    // The song preview start time panel
    public GameObject songPreviewStartTimePanel;

    // Song name
    public string songName;

    // Song artist
    public string songArtist;

    // Beatmap creator
    public string beatmapCreator;

    public static Beatmap beatmap;
    string FILE_EXTENSION = ".dia";

    // The editor UI components
    public TextMeshProUGUI editorTitle;

    // Is true when in the setup screen, used for allowing keyboard press without starting the editor
    public bool settingUp;

    // Get reference to the background manager
    private BackgroundManager backgroundManager;
    // Get the editor background image
    public Image backgroundImage;

    private bool hasUpdatedUIText;

    public GameObject songSelectPanel;

    public Button finishedButton;

    public Button hitObjectButton;

    public GameObject difficultyLevelPanel;

    public Button saveButton;

    public Button resetButton;
    public Button newSongButton;

    public Button successSaveButton;

    // The settings panel that's displayed when the user has finished creating the beatmap or is editing beatmap information
    public GameObject settingsPanel;

    public GameObject saveButtonPanel;

    private void Awake()
    {
        beatmap = new Beatmap();
    }

    private void Start()
    {
        // Setting up at the start 
        settingUp = true;

        // Get the user logged in and set the creator to that user
        if (MySQLDBManager.loggedIn)
        {
            beatmapCreator = MySQLDBManager.username;
        }
        else
        {
            beatmapCreator = "GUEST";
        }

        // Get the reference to the background image manager
        backgroundManager = FindObjectOfType<BackgroundManager>();
    }

    // Get the song preview time from the input field box
    public void ActivateSongPreviewStartTimePanel()
    {
        // Enable the song preview start time panel
        songPreviewStartTimePanel.gameObject.SetActive(true);
    }

    // Save the text field information then disable
    public void GetSongPreviewStartTime()
    {
        // Get the time from the input field
        songPreviewStartTime = float.Parse(songPreviewStartTimeInputField.text);

        // Disble the song preview start time panel
        songPreviewStartTimePanel.gameObject.SetActive(false);

        // Enable the save button
        saveButton.interactable = true;

        // Activate save button panel and disable settings panel
        ActivateSaveButtonPanel();
    }

    // Activate the save button panel
    public void ActivateSaveButtonPanel()
    {
        saveButtonPanel.gameObject.SetActive(true);

        // Disable the settings panel
        settingsPanel.gameObject.SetActive(false);
    }

    // Deactivate the save button panel
    public void DeactivateSaveButtonPanel()
    {
        saveButtonPanel.gameObject.SetActive(false);
    }

    // Insert all song information when the song selected has been clicked
    public void InsertSongName(string songNamePass)
    {
        songName = songNamePass;

        // No longer setting up
        settingUp = false;

        // Update the editor UI
        UpdateEditorUI();
    }

    // Insert all song information when the song selected has been clicked
    public void InsertSongArtist(string songArtistPass)
    {
        songArtist = songArtistPass;

        // No longer setting up
        settingUp = false;
        
        // Update the editor UI
        UpdateEditorUI();
    }


    // Get the song chosen to load 
    public void GetSongChosen(int songChosenIndexPass)
    {
        // Get the index of the song chosens
        songClipChosenIndex = songChosenIndexPass;
    }

    // Create the folder based on the name inserted in the folderNameInputField
    public void CreateBeatmapFolder()
    {
        // Disable the save button
        saveButton.interactable = false;

        // Folder name is based off the user name and the song name charted
        beatmapFolderName = beatmapCreator + "_" +songName + "_" + songArtist;

        // Create a new folder for the beatmap
        var folder = Directory.CreateDirectory(@"C:\Beatmaps\" + beatmapFolderName); 
        // Save the folder directory to save the files into later
        folderDirectory = @"C:\Beatmaps\" + beatmapFolderName + @"\";
    }

    // Insert beatmap difficulty
    public void InsertBeatmapDifficulty(string beatmapDifficultyPass)
    {
        // The beatmap difficulty is the difficulty button chosen during the setup menu
        beatmapDifficulty = beatmapDifficultyPass;


        // Disable the difficulty buttons
        difficultyButtonsPanel.gameObject.SetActive(false);

        // Activate the difficulty level buttons
        difficultyLevelPanel.gameObject.SetActive(true);
    }

    // Insert the beatmap difficulty level
    public void InsertBeatmapDifficultyLevel(string beatmapDifficultyLevelPass)
    {
        // If the beatmap difficulty selected previously is easy
        if (beatmapDifficulty == "easy")
        {
            // Insert the advanced difficulty level
            beatmapEasyDifficultyLevel = beatmapDifficultyLevelPass;
        }
        // If the beatmap difficulty selected previously is advanced
        else if (beatmapDifficulty == "advanced")
        {
            // Insert the advanced difficulty level
            beatmapAdvancedDifficultyLevel = beatmapDifficultyLevelPass;
        }
        // If the beatmap difficulty selected previously is extra
        else if (beatmapDifficulty == "extra")
        {
            // Insert the extra difficulty level
            beatmapExtraDifficultyLevel = beatmapDifficultyLevelPass;
        }

        // Disable the difficulty level panel
        difficultyLevelPanel.gameObject.SetActive(false);

        // Activate the song preview start time input field and button
        ActivateSongPreviewStartTimePanel();
    }

    // Update the song name, artist and difficulty with the values entered
    public void UpdateEditorUI()
    {
        // Disable the song select menu
        songSelectPanel.gameObject.SetActive(false);
    }

    // Change the beatmap song selected
    public void OpenSongSelectMenu()
    {
        // Set the song panel to active
        songSelectPanel.gameObject.SetActive(true);
    }

    // Activate the difficulty buttons
    public void ActivateDifficultyTypeButtons()
    {
        // Activate settings panel
        settingsPanel.gameObject.SetActive(true);

        difficultyButtonsPanel.gameObject.SetActive(true);
        finishedButton.interactable = false;
        resetButton.interactable = false;
        newSongButton.interactable = false;
        newSongButton.interactable = false;
        hitObjectButton.interactable = false;
    }

    // Activate success beatmap save button
    public void ActivateSuccessBeatmapSaveButton()
    {
        successSaveButton.gameObject.SetActive(true);
    }

}