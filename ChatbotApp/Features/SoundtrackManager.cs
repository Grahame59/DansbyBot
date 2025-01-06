// Manages soundtracks (loading, playback, etc.)
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
            errorLogClient = ErrorLogClient.Instance; // Using the singleton instance
        }

        public async Task InitializeSoundtracksAsync()
        {
            try
            {
                if (!Directory.Exists(soundtrackDirectory))
                {
                    await errorLogClient.AppendToErrorLogAsync($"Soundtrack directory does not exist: {soundtrackDirectory}", "SoundtrackManager.cs");
                    return;
                }

                soundtracks.AddRange(Directory.GetFiles(soundtrackDirectory, "*.mp3"));

                if (soundtracks.Count == 0)
                {
                    await errorLogClient.AppendToErrorLogAsync("No soundtracks found in the soundtrack directory.", "SoundtrackManager.cs");
                }
                else
                {
                    await errorLogClient.AppendToDebugLogAsync($"Loaded {soundtracks.Count} soundtracks.", "SoundtrackManager.cs");
                }
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error initializing soundtracks: {ex.Message}", "SoundtrackManager.cs");
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

        public async Task PlaySoundtrackAsync(string soundtrackName)
        {
            var filePath = Path.Combine(soundtrackDirectory, soundtrackName);

            if (!File.Exists(filePath))
            {
                await errorLogClient.AppendToErrorLogAsync($"Soundtrack file not found: {filePath}", "SoundtrackManager.cs");
                return;
            }

            StopPlayback();

            try
            {
                audioFile = new AudioFileReader(filePath);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
                outputDevice.Volume = 1.0f; // Set the volume to max
                outputDevice.Play();

                await errorLogClient.AppendToDebugLogAsync($"Playing soundtrack: {soundtrackName}", "SoundtrackManager.cs");
            }
            catch (Exception ex)
            {
                await errorLogClient.AppendToErrorLogAsync($"Error playing soundtrack: {ex.Message}", "SoundtrackManager.cs");
            }
        }

        public async Task PausePlaybackAsync()
        {
            if (outputDevice?.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Pause();
                await errorLogClient.AppendToDebugLogAsync("Playback paused.", "SoundtrackManager.cs");
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
