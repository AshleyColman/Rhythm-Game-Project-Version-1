﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BeatmapButton : MonoBehaviour
{

    private int beatmapButtonIndex;

    private SongSelectMenuFlash songSelectMenuFlash;
    private SongSelectManager songSelectManager;
    private BeatmapRanking beatmapRanking;

    public AudioClip click;

    public GameObject buttonGlow;

    private GameObject menuSFXGameObject;
    private AudioSource menuSFXAudioSource;


    // Use this for initialization
    void Start () {
        songSelectMenuFlash = FindObjectOfType<SongSelectMenuFlash>();
        menuSFXGameObject = GameObject.FindGameObjectWithTag("MenuSFXAudioSource");
        menuSFXAudioSource = menuSFXGameObject.GetComponent<AudioSource>();
        songSelectManager = FindObjectOfType<SongSelectManager>();
        beatmapRanking = FindObjectOfType<BeatmapRanking>();
	}

    // Load the beatmap assigned to the button when clicked
    public void LoadBeatmap()
    {
        if (songSelectMenuFlash == null)
        {
            songSelectMenuFlash = FindObjectOfType<SongSelectMenuFlash>();
        }

        songSelectMenuFlash.LoadBeatmapButtonSong(beatmapButtonIndex);

        PlaySongPreview();
    }

    // Play the song preview when clicked
    private void PlaySongPreview()
    {
        songSelectManager.PlaySongPreview();
    }

    // Set the beatmap butotn index during instantiation
    public void SetBeatmapButtonIndex(int beatmapButtonIndexPass)
    {
        beatmapButtonIndex = beatmapButtonIndexPass;
    }
   
    // Play the click sound
    public void PlayClickSound()
    {
        menuSFXAudioSource.PlayOneShot(click);
    }

    // Stop all coroutines in the beatmap ranking script
    public void StopBeatmapRankingCoroutines()
    {
        // Stop beatmap leaderboard ranking loads
        beatmapRanking.StopAllCoroutines();
    }


}
