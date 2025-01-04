// Manages soundtracks (loading, playback, etc.)
using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;
using ChatbotApp.Utilities;

namespace ChatbotApp.Features
{
    public class SoundtrackManager
    {
        private readonly List<string> soundtracks;
        private readonly string soundtrackDirectory;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private readonly ErrorLogClient errorLogClient;

        public SoundtrackManager()
        {
            // Adjusted path to match your project structure
            soundtrackDirectory = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "ChatbotApp", "Resources", "Soundtracks");
            soundtracks = new List<string>();
            errorLogClient = new ErrorLogClient();
        }

        public void InitializeSoundtracks()
        {
            try
            {
                if (!Directory.Exists(soundtrackDirectory))
                {
                    errorLogClient.AppendToErrorLog($"Soundtrack directory does not exist: {soundtrackDirectory}", "SoundtrackManager.cs");
                    return;
                }

                soundtracks.AddRange(Directory.GetFiles(soundtrackDirectory, "*.mp3"));

                if (soundtracks.Count == 0)
                {
                    errorLogClient.AppendToErrorLog("No soundtracks found in the soundtrack directory.", "SoundtrackManager.cs");
                }
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error initializing soundtracks: {ex.Message}", "SoundtrackManager.cs");
            }
        }

        public List<string> GetSoundtrackNames()
        {
            var soundtrackNames = new List<string>();

            foreach (var track in soundtracks)
            {
                soundtrackNames.Add(Path.GetFileName(track));
            }

            return soundtrackNames;
        }

        public void PlaySoundtrack(string soundtrackName)
        {
            var filePath = Path.Combine(soundtrackDirectory, soundtrackName);

            if (!File.Exists(filePath))
            {
                errorLogClient.AppendToErrorLog($"Soundtrack file not found: {filePath}", "SoundtrackManager.cs");
                return;
            }

            StopPlayback();

            try
            {
                audioFile = new AudioFileReader(filePath);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Play();

                errorLogClient.AppendToDebugLog($"Playing soundtrack: {soundtrackName}", "SoundtrackManager.cs");
            }
            catch (Exception ex)
            {
                errorLogClient.AppendToErrorLog($"Error playing soundtrack: {ex.Message}", "SoundtrackManager.cs");
            }
        }

        public void PausePlayback()
        {
            if (outputDevice?.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Pause();
                errorLogClient.AppendToDebugLog("Playback paused.", "SoundtrackManager.cs");
            }
        }

        public void StopPlayback()
        {
            if (outputDevice?.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Stop();
            }

            outputDevice?.Dispose();
            outputDevice = null;

            audioFile?.Dispose();
            audioFile = null;
        }
    }
}
