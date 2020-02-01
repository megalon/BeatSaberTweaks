using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPA;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using System.Collections;

namespace BeatSaberTweaks
{
    class MusicVolume : MonoBehaviour
    {
        public static MusicVolume Instance;

        static GameSongController songController = null;
        static AudioTimeSyncController audioTimeSyncController = null;
        static AudioSource audioSource = null;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("Music Volume").AddComponent<MusicVolume>().transform.parent = parent;
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        public static void UpdateMusicVolume()
        {
            Plugin.Log("Updating music volume", Plugin.LogLevel.DebugOnly);
            if (audioSource != null && SceneUtils.isGameScene(SceneManager.GetActiveScene()))
            {
                float newVolume = Settings.MusicVolume;
                audioSource.volume = newVolume;
                Plugin.Log("Setting music volume to " + newVolume, Plugin.LogLevel.DebugOnly);
            }
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            try
            {
                if (SceneUtils.isGameScene(scene))
                {
                    StartCoroutine(WaitForLoad());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Tweaks Music Volume error: " + e);
            }
        }
        
        private IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                songController = Resources.FindObjectsOfTypeAll<GameSongController>().FirstOrDefault();

                if (songController == null)
                {
                    Plugin.Log("Music Volume songController is null!", Plugin.LogLevel.DebugOnly);
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    Plugin.Log("Music Volume found songController!", Plugin.LogLevel.DebugOnly);
                    loaded = true;
                }
            }
            LoadingDidFinishEvent();
        }

        private void LoadingDidFinishEvent()
        {
            Plugin.Log("Music Volume LoadingDidFinishEvent: ", Plugin.LogLevel.DebugOnly);
            try
            {
                audioTimeSyncController = ReflectionUtil.GetPrivateField<AudioTimeSyncController>(songController, "_audioTimeSyncController");
                try
                {
                    audioSource = ReflectionUtil.GetPrivateField<AudioSource>(audioTimeSyncController, "_audioSource");
                    try
                    {
                        UpdateMusicVolume();
                    }
                    catch (Exception e)
                    {
                        Plugin.Log("Music Volume LoadingDidFinishEvent normalVolume error: " + e, Plugin.LogLevel.Error);
                    }
                }
                catch (Exception e)
                {
                    Plugin.Log("Music Volume LoadingDidFinishEvent audioSource error: " + e, Plugin.LogLevel.Error);
                }
            }
            catch (Exception e)
            {
                Plugin.Log("Music Volume LoadingDidFinishEvent audioTimeSyncController error: " + e, Plugin.LogLevel.Error);
            }
        }
    }
}