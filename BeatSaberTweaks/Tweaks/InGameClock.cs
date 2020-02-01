using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using VRUI;
using VRUIControls;
using TMPro;
using System.Threading;

namespace BeatSaberTweaks
{
    public class InGameClock : MonoBehaviour
    {
        public static InGameClock Instance;

        private static GameObject ClockCanvas = null;
        private static TextMeshProUGUI text = null;
        private static Vector3 timePos;
        private static Quaternion timeRot;
        private static float timeSize;

        private bool _IsPlayerIngame;

        float timer = 0;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            Plugin.Log("Creating InGameClock.", Plugin.LogLevel.DebugOnly);
            new GameObject("In Game Time").AddComponent<InGameClock>().transform.parent = parent;
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Plugin.Log("InGameClock awake.", Plugin.LogLevel.DebugOnly);
                Instance = this;
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
                DontDestroyOnLoad(gameObject);
                timePos = Settings.ClockPosition;
                timeRot = Settings.ClockRotation;
                timeSize = Settings.ClockFontSize;
            }
            else
            {
                Destroy(this);
            }
        }

        public void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            Plugin.Log("InGameClock SceneManagerOnActiveSceneChanged: " + arg0.name + " " + scene.name, Plugin.LogLevel.DebugOnly);
            try
            {
                if (SceneUtils.isMenuScene(scene) && ClockCanvas == null)
                {
                    Plugin.Log("Creating the clock object... ", Plugin.LogLevel.DebugOnly);
                    ClockCanvas = new GameObject();
                    DontDestroyOnLoad(ClockCanvas);
                    ClockCanvas.AddComponent<Canvas>();

                    ClockCanvas.name = "Clock Canvas";
                    ClockCanvas.transform.position = timePos;
                    ClockCanvas.transform.rotation = timeRot;
           //         ClockCanvas.transform.localScale = new Vector3(0.02f, 0.02f, 1.0f);

                    text = CustomUI.BeatSaber.BeatSaberUI.CreateText(ClockCanvas.transform as RectTransform, "Clock Text", new Vector2(0, 0.3f));
                    text.name = "Clock Text";

                    text.alignment = Utilites.TextAlignUtil.textAlignFromString(Settings.ClockAlignment);
                    text.color = Color.white;
                    text.transform.localScale *= .02f;
                    text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
                    text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);
                    text.fontSize = timeSize;
                    text.text = "";

                    UpdateClock();
                    
                    ClockCanvas.SetActive(Settings.ShowClock);
                }
                _IsPlayerIngame = SceneUtils.isGameScene(scene);
            }
            catch (Exception e)
            {
                Plugin.Log("InGameClock error: " + e, Plugin.LogLevel.DebugOnly);
            }
        }

        public void Update()
        {
            bool shouldShow = Settings.ShowClock && (!_IsPlayerIngame || !Settings.HideClockIngame);
            if (ClockCanvas != null && shouldShow != ClockCanvas.activeSelf)
            {
                ClockCanvas.SetActive(shouldShow);
            }

            timer += Time.deltaTime;
            if (text != null && timer > 60.0f)
            {
                timer = 0;
                UpdateClock();
            }
        }

        public static void UpdateClock()
        {
            Plugin.Log("InGameClock UpdateClock function called.", Plugin.LogLevel.DebugOnly);
            string time;
            if (Settings.Use24hrClock)
            {
                time = DateTime.Now.ToString("HH:mm");
            }
            else
            {
                time = DateTime.Now.ToString("h:mm tt");
            }
            text.text = time;
        }
    }
}
