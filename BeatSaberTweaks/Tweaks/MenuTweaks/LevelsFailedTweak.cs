using System.Text;
using IPA;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using System.Collections;
using System;
using System.Linq;
using TMPro;

namespace BeatSaberTweaks
{
    class LevelsFailedTweak : MonoBehaviour
    {
        public static LevelsFailedTweak Instance;

        private PlayerStatisticsViewController stats;
        private bool showFailCounter = false;
        private bool loaded = false;
        private TextMeshProUGUI _failedLevelsCountText;
        private string failedLevelsCountReplacementText = "HIDDEN";

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("Levels Failed Tweak").AddComponent<LevelsFailedTweak>().transform.parent = parent;
        }

        private void Awake()
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
                if (SceneUtils.isMenuScene(scene))
                {
                    StartCoroutine(WaitForLoad());
                }
            }
            catch (Exception e)
            {
                Plugin.Log("Tweaks (LevelsFailedTweaks) messed up: " + e, Plugin.LogLevel.Error);
            }
        }

        private IEnumerator WaitForLoad()
        {
            while (!loaded)
            {
                stats = Resources.FindObjectsOfTypeAll<PlayerStatisticsViewController>().FirstOrDefault();

                if (stats == null)
                    yield return new WaitForSeconds(0.01f);
                else
                {
                    Plugin.Log("Found PlayerStatisticsViewController!", Plugin.LogLevel.DebugOnly);
                    loaded = true;
                }
            }
            Init();
        }

        private void Init()
        {
            Plugin.Log("Initialized!", Plugin.LogLevel.DebugOnly);
            failedLevelsCountReplacementText = Settings.FailsCounterReplacementText;

            showFailCounter = Settings.ShowFailsCounterEnable;
        }

        public void Update()
        {
            if (!showFailCounter && loaded)
            {
                if (_failedLevelsCountText == null)
                {
                    if (stats == null)
                    {
                        stats = Resources.FindObjectsOfTypeAll<PlayerStatisticsViewController>().FirstOrDefault();
                    }
                    _failedLevelsCountText = ReflectionUtil.GetPrivateField<TextMeshProUGUI>(stats, "_failedLevelsCountText");
                }
                if (_failedLevelsCountText.text != failedLevelsCountReplacementText)
                {
                    Plugin.Log("Replacing the _failedLevelsCountText with: " + failedLevelsCountReplacementText, Plugin.LogLevel.Info);
                    _failedLevelsCountText.text = failedLevelsCountReplacementText;
                    _failedLevelsCountText.ForceMeshUpdate(true);
                }
            }
        }
    }
}
