using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using VRUI;
using VRUIControls;
using TMPro;
using IPA;
using CustomUI.BeatSaber;
using CustomUI.Settings;
using System.Reflection;
using Harmony;

namespace BeatSaberTweaks
{
    public class TweakManager : MonoBehaviour
    {
        public static TweakManager Instance = null;
        MainMenuViewController _mainMenuViewController = null;
        private static HarmonyInstance harmony = null;

        /*
         * TODO
         * FlyingCars here too
        float carTime = 0;
        */

        public static void OnLoad()
        {
            if (Instance != null) return;
            new GameObject("Tweak Manager").AddComponent<TweakManager>();
            harmony = HarmonyInstance.Create("com.megalon.BeatSaber.BeatSaberTweaks");
            
            PatchWithHarmony();
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
                SceneManager.sceneLoaded += SceneManager_sceneLoaded;
                DontDestroyOnLoad(gameObject);

                Plugin.Log("Tweak Manager started.", Plugin.LogLevel.DebugOnly);

                MoveEnergyBar.OnLoad(transform);
                ScoreMover.OnLoad(transform);
                InGameClock.OnLoad(transform);
                TimeSpentClock.OnLoad(transform);
                IngameTimeSpentClock.OnLoad(transform);
                NoteHitVolume.OnLoad(transform);
                OneColour.OnLoad(transform);
                SongDataModifer.OnLoad(transform);
                MusicVolume.OnLoad(transform);
                FireworksTweaks.OnLoad(transform);
                ClickShockwave.OnLoad(transform);
                LevelsFailedTweak.OnLoad(transform);
            }
            else
            {
                Destroy(this);
            }
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if(arg0.name == "MenuCore")
            CreateUI();
        }

        public void Update()
        {
            /*
                * TODO
                * This is to enable the flying cars easteregg.
                * Is this even still in the game?
            if (SettingsUI.isMenuScene(SceneManager.GetActiveScene()))
            {
                if (_mainMenuViewController.childViewController == null &&
                    (Input.GetAxis("TriggerLeftHand") > 0.75f) &&
                    (Input.GetAxis("TriggerRightHand") > 0.75f))
                {
                    carTime += Time.deltaTime;
                    if (carTime > 5.0f)
                    {
                        carTime = 0;
                        prompt.didFinishEvent += CarEvent;
                        prompt.Init("Flying Cars", "Turn Flying Cars?", "ON", "OFF");
                        _mainMenuViewController.PresentModalViewController(prompt, null, false);
                    }
                }
                else
                {
                    carTime = 0;
                }
            }
            */
        }

        /*
         * TODO
         * More flyingcars stuff
        private void CarEvent(SimpleDialogPromptViewController viewController, bool ok)
        {
            viewController.didFinishEvent -= CarEvent;
            if (viewController.isRebuildingHierarchy)
            {
                return;
            }
            FlyingCar.startflyingCars = ok;
            viewController.DismissModalViewController(null, false);
        }
        */

        public static void PatchWithHarmony()
        {
            try
            {
                Plugin.Log("Applying harmony patch...", Plugin.LogLevel.DebugOnly);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Plugin.Log("Harmony patch applied!", Plugin.LogLevel.DebugOnly);
            }
            catch (Exception e)
            {
                Plugin.Log("This plugin requires Harmony. Run the Mod Manager and install the Harmony library", Plugin.LogLevel.Error);
                Plugin.Log(e.ToString(), Plugin.LogLevel.Error);
            }
        }

        public static bool IsPartyMode()
        {
            Plugin.Log(Plugin.party.ToString(), Plugin.LogLevel.Info);
            if (Plugin.party)
                return true;
            else
                return false;
        }

        public void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            try
            {
                //Console.WriteLine("Active: " + scene.name);
                if (SceneUtils.isMenuScene(scene))
                {
                    Plugin.Log("TweakManager isMenuScene", Plugin.LogLevel.DebugOnly);
                    _mainMenuViewController = Resources.FindObjectsOfTypeAll<MainMenuViewController>().First();
                    //var _menuMasterViewController = Resources.FindObjectsOfTypeAll<StandardLevelSelectionFlowCoordinator>().First();
                    //prompt = ReflectionUtil.GetPrivateField<SimpleDialogPromptViewController>(_menuMasterViewController, "_simpleDialogPromptViewController");
                }
                else
                {
                    Plugin.Log("TweakManager not in menu scene", Plugin.LogLevel.DebugOnly);
                }
            }catch (Exception e)
            {
                Plugin.Log("TweakManager scene changed error: " + e, Plugin.LogLevel.Error);
            }
        }

        private void CreateUI()
        {
            Plugin.Log("TweakManager creating the BSTweaks UI", Plugin.LogLevel.DebugOnly);
            
            // Interface Tweaks [1]
            var subMenuInterfaceTweaks1 = SettingsUI.CreateSubMenu("Interface Tweaks [1]");

            var energyBar = subMenuInterfaceTweaks1.AddBool("Move Energy Bar");
            energyBar.GetValue += delegate { return Settings.MoveEnergyBar; };
            energyBar.SetValue += delegate (bool value) { Settings.MoveEnergyBar = value; };

            var moveScore = subMenuInterfaceTweaks1.AddBool("Move Score");
            moveScore.GetValue += delegate { return Settings.MoveScore; };
            moveScore.SetValue += delegate (bool value) { Settings.MoveScore = value; };

            var showClock = subMenuInterfaceTweaks1.AddBool("Show Clock");
            showClock.GetValue += delegate { return Settings.ShowClock; };
            showClock.SetValue += delegate (bool value) { Settings.ShowClock = value; };

            var hideClockIngame = subMenuInterfaceTweaks1.AddBool("Hide Clock While Playing");
            hideClockIngame.GetValue += delegate { return Settings.HideClockIngame; };
            hideClockIngame.SetValue += delegate (bool value) { Settings.HideClockIngame = value; };


            var clock24hr = subMenuInterfaceTweaks1.AddBool("24hr Clock");
            clock24hr.GetValue += delegate { return Settings.Use24hrClock; };
            clock24hr.SetValue += delegate (bool value)
            {
                Settings.Use24hrClock = value;
                InGameClock.UpdateClock();
            };

            // Interface Tweaks [2]
            var subMenuInterfaceTweaks2 = SettingsUI.CreateSubMenu("Interface Tweaks [2]");

            var showTimeSpentClock = subMenuInterfaceTweaks2.AddBool("Show Time Spent");
            showTimeSpentClock.GetValue += delegate { return Settings.ShowTimeSpentClock; };
            showTimeSpentClock.SetValue += delegate (bool value) { Settings.ShowTimeSpentClock = value; };

            var showIngameTimeSpentClock = subMenuInterfaceTweaks2.AddBool("Show Ingame Time Spent");
            showIngameTimeSpentClock.GetValue += delegate { return Settings.ShowIngameTimeSpentClock; };
            showIngameTimeSpentClock.SetValue += delegate (bool value) { Settings.ShowIngameTimeSpentClock = value; };

            var hideTimeSpentClockIngame = subMenuInterfaceTweaks2.AddBool("Hide Time Spent While Playing");
            hideTimeSpentClockIngame.GetValue += delegate { return Settings.HideTimeSpentClockIngame; };
            hideTimeSpentClockIngame.SetValue += delegate (bool value) { Settings.HideTimeSpentClockIngame = value; };

            var hideIngameTimeSpentClockIngame = subMenuInterfaceTweaks2.AddBool("Hide Ingame Time Spent While Playing");
            hideIngameTimeSpentClockIngame.GetValue += delegate { return Settings.HideIngameTimeSpentClockIngame; };
            hideIngameTimeSpentClockIngame.SetValue += delegate (bool value) { Settings.HideIngameTimeSpentClockIngame = value; };

            // Interface Tweaks [3]
            var subMenuInterfaceTweaks3 = SettingsUI.CreateSubMenu("Interface Tweaks [3]");

            var enableFireworksTweaks = subMenuInterfaceTweaks3.AddBool("Fireworks");
            enableFireworksTweaks.EnabledText = "ON";
            enableFireworksTweaks.DisabledText = "OFF";
            enableFireworksTweaks.GetValue += delegate { return Settings.FireworksEnable; };
            enableFireworksTweaks.SetValue += delegate (bool value) { Settings.FireworksEnable = value; };

            var enableRippleEffect = subMenuInterfaceTweaks3.AddBool("Click ripple effect");
            enableRippleEffect.EnabledText = "ON";
            enableRippleEffect.DisabledText = "OFF";
            enableRippleEffect.GetValue += delegate { return Settings.ClickShockwaveEnable; };
            enableRippleEffect.SetValue += delegate (bool value) { Settings.ClickShockwaveEnable = value; };

            var showFailsCounter = subMenuInterfaceTweaks3.AddBool("Fails counter", "If the fails counter number should be visible on the stats screen.");
            showFailsCounter.EnabledText = "VISIBLE";
            showFailsCounter.DisabledText = "HIDDEN";
            showFailsCounter.GetValue += delegate { return Settings.ShowFailsCounterEnable; };
            showFailsCounter.SetValue += delegate (bool value) { Settings.ShowFailsCounterEnable = value; };

            var hideFailsReplacementText = subMenuInterfaceTweaks3.AddString("Replacement Text", "The text to replace the fails counter with.");
            hideFailsReplacementText.GetValue += delegate { return Settings.FailsCounterReplacementText; };
            hideFailsReplacementText.SetValue += delegate (string value) { Settings.FailsCounterReplacementText = value; };

            // Volume Tweaks
            var subMenuVolumeTweaks = SettingsUI.CreateSubMenu("Volume Tweaks");
            
            var volumeComment = subMenuVolumeTweaks.AddBool("<align=\"center\"><b>The default value is <u>underlined</u>!</b></align>");
            volumeComment.GetValue += delegate { return false; };
            volumeComment.SetValue += delegate (bool value) {  };

            // Hack to convert the boolean toggle into a text only comment
            // This disables the arrows and the value display
            try
            {
                // Note:
                // To get objects from inherited class through reflection, use object.GetType().BaseType
                // Here I do it twice to go up two levels in the heiarchy
                var buttonToDisable = volumeComment.GetType().BaseType.BaseType.GetField("_decButton", BindingFlags.NonPublic | BindingFlags.Instance);
                var decButton = (MonoBehaviour)buttonToDisable.GetValue(volumeComment);
                buttonToDisable = volumeComment.GetType().BaseType.BaseType.GetField("_incButton", BindingFlags.NonPublic | BindingFlags.Instance);
                var incButton = (MonoBehaviour)buttonToDisable.GetValue(volumeComment);

                decButton.gameObject.SetActive(false);
                incButton.gameObject.SetActive(false);

                var textToDisable = volumeComment.GetType().BaseType.BaseType.GetField("_text", BindingFlags.NonPublic | BindingFlags.Instance);
                var uselessText = (MonoBehaviour)textToDisable.GetValue(volumeComment);

                uselessText.gameObject.SetActive(false);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error trying to add comment to volume tweaks settings page:" + e);
            }

            var volumeComment2 = subMenuVolumeTweaks.AddBool("<align=\"center\"><b><color=\"red\">Increasing past the default may cause audio issues!</color></b></align>",
                "These sound effects are loud. When the volume is increased, the audio spam seems to be overloading the audio system, causing audio dropouts, and even lag. It takes a LOT of stacking to make this happen, so it only happens on certain maps.");

            volumeComment2.GetValue += delegate { return false; };
            volumeComment2.SetValue += delegate (bool value) { };

            // Hack to convert the boolean toggle into a text only comment
            // This disables the arrows and the value display
            try
            {
                // Note:
                // To get objects from inherited class through reflection, use object.GetType().BaseType
                // Here I do it twice to go up two levels in the heiarchy
                var buttonToDisable = volumeComment2.GetType().BaseType.BaseType.GetField("_decButton", BindingFlags.NonPublic | BindingFlags.Instance);
                var decButton = (MonoBehaviour)buttonToDisable.GetValue(volumeComment2);
                buttonToDisable = volumeComment2.GetType().BaseType.BaseType.GetField("_incButton", BindingFlags.NonPublic | BindingFlags.Instance);
                var incButton = (MonoBehaviour)buttonToDisable.GetValue(volumeComment2);

                decButton.gameObject.SetActive(false);
                incButton.gameObject.SetActive(false);

                var textToDisable = volumeComment2.GetType().BaseType.BaseType.GetField("_text", BindingFlags.NonPublic | BindingFlags.Instance);
                var uselessText = (MonoBehaviour)textToDisable.GetValue(volumeComment2);

                uselessText.gameObject.SetActive(false);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error trying to add comment to volume tweaks settings page:" + e);
            }

            var noteHit = subMenuVolumeTweaks.AddList("Note Hit Volume", incrementValues(), "The volume for the note cut sound effect.");
            noteHit.GetValue += delegate { return Settings.NoteHitVolume; };
            noteHit.SetValue += delegate (float value) { Settings.NoteHitVolume = value; };
            noteHit.FormatValue += delegate (float value) {
                if (value == 0.5f) return string.Format("<u>{0}%</u>", Mathf.Floor(value * 100));
                return string.Format("{0}%", Mathf.Floor(value * 100));
            };

            var noteMiss = subMenuVolumeTweaks.AddList("Note Miss Volume", incrementValues(), "The volume of the miss / bad cut sound effect.");
            noteMiss.GetValue += delegate { return Settings.NoteMissVolume; };
            noteMiss.SetValue += delegate (float value) { Settings.NoteMissVolume = value; };
            noteMiss.FormatValue += delegate (float value) {
                if (value < 0.91f && value > 0.89f) return string.Format("<u>{0}%</u>", Mathf.Floor(value * 100));
                return string.Format("{0}%", Mathf.Floor(value * 100));
            };

            var musicVol = subMenuVolumeTweaks.AddList("Music Volume", incrementValues(), "The volume of the song you play.");
            musicVol.GetValue += delegate { return Settings.MusicVolume; };
            musicVol.SetValue += delegate (float value) { Settings.MusicVolume = value; };
            musicVol.FormatValue += delegate (float value) {
                if (value == 1f) return string.Format("<u>{0}%</u>", Mathf.Floor(value * 100));
                return string.Format("{0}%", Mathf.Floor(value * 100));
            };

            var previewVol = subMenuVolumeTweaks.AddList("Preview Volume", incrementValues(), "The volume of the beatmap preview in the songs list.");
            previewVol.GetValue += delegate { return Settings.PreviewVolume; };
            previewVol.SetValue += delegate (float value) { Settings.PreviewVolume = value; };
            previewVol.FormatValue += delegate (float value) {
                if (value == 1f) return string.Format("<u>{0}%</u>", Mathf.Floor(value * 100));
                return string.Format("{0}%", Mathf.Floor(value * 100));
            };

            var menuBG = subMenuVolumeTweaks.AddList("Menu BG Music Volume", incrementValues(), "The volume for the menu background music.");
            menuBG.GetValue += delegate { return Settings.MenuBGVolume; };
            menuBG.SetValue += delegate (float value) { Settings.MenuBGVolume = value; };
            menuBG.FormatValue += delegate (float value) {
                if (value == 1f) return string.Format("<u>{0}%</u>", Mathf.Floor(value * 100));
                return string.Format("{0}%", Mathf.Floor(value * 100));
            };

            // Party Mode Tweaks
            var subMenuPartyModeTweaks = SettingsUI.CreateSubMenu("Party Mode Tweaks");

            var noArrows = subMenuPartyModeTweaks.AddBool("No Arrows");
            noArrows.GetValue += delegate { return Settings.NoArrows; };
            noArrows.SetValue += delegate (bool value) { Settings.NoArrows = value; };

            var oneColour = subMenuPartyModeTweaks.AddBool("One Color");
            oneColour.GetValue += delegate { return Settings.OneColour; };
            oneColour.SetValue += delegate (bool value) { Settings.OneColour = value; };

            var removeBombs = subMenuPartyModeTweaks.AddBool("Remove Bombs");
            removeBombs.GetValue += delegate { return Settings.RemoveBombs; };
            removeBombs.SetValue += delegate (bool value) { Settings.RemoveBombs = value; };

            var overrideNoteSpeed = subMenuPartyModeTweaks.AddBool("Override Note Speed");
            overrideNoteSpeed.GetValue += delegate { return Settings.OverrideJumpSpeed; };
            overrideNoteSpeed.SetValue += delegate (bool value) { Settings.OverrideJumpSpeed = value; };

            var noteSpeed = subMenuPartyModeTweaks.AddList("Note Speed", noteSpeeds());
            noteSpeed.GetValue += delegate { return Settings.NoteJumpSpeed; };
            noteSpeed.SetValue += delegate (float value) { Settings.NoteJumpSpeed = value; };
            noteSpeed.FormatValue += delegate (float value) { return string.Format("{0:0}", value); };

            //if (HiddenNotesInstalled)
            //{
            //    var tweaks5 = SettingsUI.CreateSubMenu("Hidden Notes");
            //    tweaks5.AddToggleSetting<HiddenNotesSettingsController>("Hidden Notes");
            //}
        }

        private float[] incrementValues(float startValue = 0.0f, float step = 0.05f, int numberOfElements = 21)
        {
            if (step < 0.01f)
            {
                throw new Exception("Step value specified was too small! Minimum supported step value is 0.01");
            }
            Int64 multiplier = 100;
            // Avoid floating point math as it results in rounding errors
            Int64 fixedStart = (Int64)(startValue * multiplier);
            Int64 fixedStep = (Int64)(step * multiplier);
            var values = new float[numberOfElements];
            for (int i = 0; i < values.Length; i++)
                values[i] = (float)(fixedStart + (fixedStep * i)) / multiplier;
            return values;
        }

        private float[] noteSpeeds()
        {
            float startValue = 5f;
            float step = 1f;
            var numberOfElements = 16;
            var values = new float[numberOfElements];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = startValue + step * i;
            }
            return values;
        }

        public static void LogComponents(Transform t, bool includeScipts = false, string prefix = "")
        {
            Console.WriteLine(prefix + ">" + t.name);

            if (includeScipts)
            {
                foreach (var comp in t.GetComponents<MonoBehaviour>())
                {
                    Console.WriteLine(prefix + "|<-" + comp.GetType());
                }
            }

            foreach (Transform child in t)
            {
                LogComponents(child, includeScipts, prefix + "|");
            }
        }
        /*
        IEnumerator LoadWarning()
        {
            string warningText = "The folling plugins are obsolete:\n";

            foreach(var text in warningPlugins)
            {
                warningText += text + ", ";
            }
            warningText = warningText.Substring(0, warningText.Length - 2);

            warningText +="\nPlease remove them before playing or you my encounter errors.\nDo you want to continue?";

            yield return new WaitForSeconds(0.1f);

            var _menuMasterViewController = Resources.FindObjectsOfTypeAll<StandardLevelSelectionFlowCoordinator>().First();
            var warning = ReflectionUtil.GetPrivateField<SimpleDialogPromptViewController>(_menuMasterViewController , "_simpleDialogPromptViewController");
            warning.gameObject.SetActive(false);
            warning.didFinishEvent += Warning_didFinishEvent;
            warning.Init("Plugin warning", warningText, "YES", "NO");

            yield return new WaitForSeconds(0.1f);

            _mainMenuViewController.PresentModalViewController(warning, null, false);
        }

        private void Warning_didFinishEvent(SimpleDialogPromptViewController viewController, bool ok)
        {
            viewController.didFinishEvent -= Warning_didFinishEvent;
            if (viewController.isRebuildingHierarchy)
            {
                return;
            }
            if (ok)
            {
                viewController.DismissModalViewController(null, false);
            }
            else
            {
                Application.Quit();
            }
        }
        */

        /*
        private void Prompt_didFinishEvent(SimpleDialogPromptViewController viewController, bool ok)
        {
            viewController.didFinishEvent -= Prompt_didFinishEvent;
            if (viewController.isRebuildingHierarchy)
            {
                return;
            }
            if (ok)
            {
                //Console.WriteLine("OK");
            }
            else
            {
                //Console.WriteLine("NO");
            }
            viewController.DismissModalViewController(null, false);
        }
        */
    }
}
