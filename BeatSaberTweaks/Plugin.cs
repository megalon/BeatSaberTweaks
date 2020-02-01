using System;
using System.Collections.Generic;
using System.Linq;
using IPA;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeatSaberTweaks
{
    public class Plugin : IBeatSaberPlugin
    {
        public static string versionNumber = "4.4.4";

        public string Name => "Beat Saber Tweaks";
        public string Version => versionNumber;

        private bool _init = false;
        private static SoloFreePlayFlowCoordinator _soloFlowCoordinator;
        private static PartyFreePlayFlowCoordinator _partyFlowCoordinator;

        private static PracticeViewController _practiceViewController;
        private static StandardLevelDetailViewController _soloDetailView;
        private static bool debug = false;
        public static bool party { get; private set; } = false;
        public static bool saveRequested = false;

        public enum LogLevel
        {
            DebugOnly = 0,
            Info = 1,
            Error = 2
        }

        public void OnApplicationStart()
        {
            if (_init) return;
            _init = true;

            Settings.Load();
            TweakManager.OnLoad();
        }


        private void _practiceViewController_didPressPlayButtonEvent()
        {
            Log("Play Button Press ", Plugin.LogLevel.Info);
            party = _partyFlowCoordinator.isActivated;
            Log(party.ToString(), Plugin.LogLevel.Info);
        }

        private void _soloDetailView_didPressPlayButtonEvent(StandardLevelDetailViewController obj)
        {
            Log("Play Button Press " , Plugin.LogLevel.Info);
            party = _partyFlowCoordinator.isActivated;
            Log(party.ToString(), Plugin.LogLevel.Info);
        }      

        public void OnApplicationQuit()
        {
            Settings.Save();
        }


        public void OnUpdate()
        {

        }

        public void OnFixedUpdate()
        {
        }

        public static void Log(string input, Plugin.LogLevel logLvl)
        {
            if (logLvl >= LogLevel.Info || debug) Console.WriteLine("[BeatSaberTweaks]: " + input);
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {

        }

        public void OnSceneUnloaded(Scene scene)
        {
   
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            if (nextScene.name == "MenuCore")
            {


                if (_soloFlowCoordinator == null)
                {
                    _soloFlowCoordinator = Resources.FindObjectsOfTypeAll<SoloFreePlayFlowCoordinator>().FirstOrDefault();
                    if (_soloFlowCoordinator == null) return;
                    _soloDetailView = _soloFlowCoordinator.GetPrivateField<StandardLevelDetailViewController>("_levelDetailViewController");
                    _practiceViewController = _soloFlowCoordinator.GetPrivateField<PracticeViewController>("_practiceViewController");
                    if (_soloDetailView != null)
                        _soloDetailView.didPressPlayButtonEvent += _soloDetailView_didPressPlayButtonEvent;
                    else
                        Log("Detail View Null", Plugin.LogLevel.Info);
                    if (_practiceViewController != null)
                        _practiceViewController.didPressPlayButtonEvent += _practiceViewController_didPressPlayButtonEvent;
                    else
                        Log("Practice View Null", Plugin.LogLevel.Info);

                }

                if (_partyFlowCoordinator == null)
                {
                    _partyFlowCoordinator = Resources.FindObjectsOfTypeAll<PartyFreePlayFlowCoordinator>().FirstOrDefault();
                }

                if (saveRequested)
                {
                    Settings.Save();
                    saveRequested = false;
                }
            }
        }
    }
}
