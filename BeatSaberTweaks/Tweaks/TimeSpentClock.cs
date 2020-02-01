using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BeatSaberTweaks
{
    public class TimeSpentClock : MonoBehaviour
    {
        public static TimeSpentClock Instance;

        private static GameObject _TimeSpentClockCanvas = null;
        private static TextMeshProUGUI _Text = null;
        private static Vector3 _TimePos;
        private static Quaternion _TimeRot;
        private static float _TimeSize;
        private static bool _HideWhilePlaying;
        private static String _MessageTemplate;

        private DateTime _StartTime;
        private TimeSpan _TimeSpent;
        private Coroutine _CUpdateTimeSpentClock;

        private bool _IsPlayerIngame;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            Plugin.Log("Creating TimeSpentClock.", Plugin.LogLevel.DebugOnly);
            new GameObject("TimeSpentClock").AddComponent<TimeSpentClock>().transform.parent = parent;
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Plugin.Log("TimeSpentClock awake.", Plugin.LogLevel.DebugOnly);
                Instance = this;
                SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
                DontDestroyOnLoad(gameObject);
                _TimePos = Settings.TimeSpentClockPosition;
                _TimeRot = Settings.TimeSpentClockRotation;
                _TimeSize = Settings.TimeSpentClockFontSize;
                _HideWhilePlaying = Settings.HideTimeSpentClockIngame;
                _MessageTemplate = Settings.TimeSpentClockMessageTemplate;
                _StartTime = DateTime.Now;                
                _IsPlayerIngame = false;
            }
            else
                Destroy(this);
        }

        public void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            Plugin.Log("TimeSpentClock SceneManagerOnActiveSceneChanged: " + arg0.name + " " + scene.name, Plugin.LogLevel.DebugOnly);
            try
            {
                if (SceneUtils.isMenuScene(scene) && _TimeSpentClockCanvas == null)
                {
                    Plugin.Log("Creating the TimeSpentClock object... ", Plugin.LogLevel.DebugOnly);
                    _TimeSpentClockCanvas = new GameObject();
                    DontDestroyOnLoad(_TimeSpentClockCanvas);
                    _TimeSpentClockCanvas.AddComponent<Canvas>();

                    _TimeSpentClockCanvas.name = "TimeSpentClock Canvas";
                    _TimeSpentClockCanvas.transform.position = _TimePos;
                    _TimeSpentClockCanvas.transform.rotation = _TimeRot;

                    _Text = CustomUI.BeatSaber.BeatSaberUI.CreateText(_TimeSpentClockCanvas.transform as RectTransform, "Clock Text", new Vector2(0, 0.05f));
                    _Text.name = "TimeSpentClock Text";

                    _Text.alignment = Utilites.TextAlignUtil.textAlignFromString(Settings.TimeSpentClockAlignment);
                    _Text.color = Color.white;
                    _Text.transform.localScale *= .02f;
                    _Text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
                    _Text.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);
                    _Text.fontSize = _TimeSize;
                    _Text.text = "";

                    _CUpdateTimeSpentClock = StartCoroutine(UpdateTimeSpentClock());

                    _TimeSpentClockCanvas.SetActive(Settings.ShowTimeSpentClock);
                }
            }
            catch (Exception e)
            {
                Plugin.Log("TimeSpentClock error: " + e, Plugin.LogLevel.DebugOnly);
            }

            if (_TimeSpentClockCanvas != null)
            {
                _IsPlayerIngame = SceneUtils.isGameScene(scene);
                if (_HideWhilePlaying)
                    _TimeSpentClockCanvas.SetActive(!_IsPlayerIngame);
            }
        }

        public void Update()
        {
            if (_TimeSpentClockCanvas != null && Settings.ShowTimeSpentClock != _TimeSpentClockCanvas.activeSelf && !_IsPlayerIngame)
                _TimeSpentClockCanvas.SetActive(Settings.ShowTimeSpentClock);
        }

        public IEnumerator UpdateTimeSpentClock()
        {
            Plugin.Log("TimeSpentClock UpdateTimeSpentClock function called.", Plugin.LogLevel.DebugOnly);
            while (_Text != null)
            {
                _TimeSpent = DateTime.Now - _StartTime;
                String timeDisplay;
                if (_TimeSpent.Hours == 0)
                {
                    if (_TimeSpent.Minutes == 0)
                        timeDisplay = string.Format("{0:00}s", _TimeSpent.Seconds);
                    else
                        timeDisplay = string.Format("{0:00}m {1:00}s", _TimeSpent.Minutes, _TimeSpent.Seconds);
                }
                else
                    timeDisplay = string.Format("{0:00}h {1:00}m {2:00}s", _TimeSpent.Hours, _TimeSpent.Minutes, _TimeSpent.Seconds);

                _Text.text = _MessageTemplate.Replace("%TIME%", timeDisplay);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
