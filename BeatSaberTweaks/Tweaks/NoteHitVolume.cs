﻿using System;
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
    public class NoteHitVolume : MonoBehaviour
    {
        public static NoteHitVolume Instance;

        const string goodCutString = "_goodCutVolume";
        const string badCutString = "_badCutVolume";

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("Note Volume").AddComponent<NoteHitVolume>().transform.parent = parent;
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
                Console.WriteLine("Tweaks (NoteVolume) done fucked up: " + e);
            }
        }

        private IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                var resultsViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();

                if (resultsViewController == null)
                {
                    Plugin.Log("resultsViewController is null!", Plugin.LogLevel.DebugOnly);
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    Plugin.Log("Found resultsViewController!", Plugin.LogLevel.DebugOnly);
                    loaded = true;
                }
            }
            LoadingDidFinishEvent();
        }

        private void LoadingDidFinishEvent()
        {
            try
            {
                var pool = Resources.FindObjectsOfTypeAll<NoteCutSoundEffect>();
                foreach (var effect in pool)
                {
                    /*
                    if (normalVolume == 0)
                    {
                        normalVolume = ReflectionUtil.GetPrivateField<float>(effect, goodCutString);
                        normalMissVolume = ReflectionUtil.GetPrivateField<float>(effect, badCutString);

                        Plugin.Log("Normal hit volumes =" + normalVolume, Plugin.LogLevel.DebugOnly);
                        Plugin.Log("Normal miss volumes =" + normalMissVolume, Plugin.LogLevel.DebugOnly);
                    }
                    */
                    ReflectionUtil.SetPrivateField(effect, goodCutString, Settings.NoteHitVolume);
                    ReflectionUtil.SetPrivateField(effect, badCutString, Settings.NoteMissVolume);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
