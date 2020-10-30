using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

public class MusicManager : MonoBehaviour
{
    // public variables -------------------------
    [Title("Audio Source for the musics")] 
    public AudioSource m_musicAudio; // Instance of the audio source responsible for the music player
    public AudioSource m_ambientAudio; // Instance of the audio source responsible for ambient player
    [Title("Music Tracks")]
    public AudioClip m_mainTheme; // Main theme to play when no other actions in progress
    [Title("Ambient Tracks")] 
    public AudioClip m_field; // Default field ambient track
    [Title("Active Tracks")] 
    [ReadOnly] public AudioClip m_activeMusicTrack; // Music track currently playing
    [ReadOnly] public AudioClip m_activeAmbientTrack; // Ambient track currently playing


    // private variables ------------------------


    // ------------------------------------------
    // Start is called before update
    // ------------------------------------------
    void Start()
    {
        // Play the default theme and ambient
        SelectMusicTrack(m_mainTheme);
        SelectAmbientTrack(m_field);
    }

    // ------------------------------------------
    // Update is called once per frame
    // ------------------------------------------
    void Update()
    {
        
    }

    // ------------------------------------------
    // Methods
    // ------------------------------------------
    // Select the next music track to play -------------------------------------------
    private void SelectMusicTrack(AudioClip nextTrack)
    {
        // Set the new active track
        m_activeMusicTrack = nextTrack;
        
        // Play the new track
        m_musicAudio.clip = m_activeMusicTrack;
        m_musicAudio.Play();
    }
    
    // Select the next ambient track to play -----------------------------------------
    private void SelectAmbientTrack(AudioClip nextTrack)
    {
        // Set the new active track
        m_activeAmbientTrack = nextTrack;
        
        // Play the new track
        m_ambientAudio.clip = m_activeAmbientTrack;
        m_ambientAudio.Play();
    }
}
