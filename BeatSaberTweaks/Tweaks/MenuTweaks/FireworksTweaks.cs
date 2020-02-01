using System.Text;
using IPA;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using System.Collections;
using System;
using System.Linq;

namespace BeatSaberTweaks
{
    class FireworksTweaks : MonoBehaviour
    {
        public static FireworksTweaks Instance;

        private ResultsViewController resultsViewController;
        private FireworksController fireworksController;
        private bool isEnabled = true;
        private bool loaded = false;
        public bool leftGameCoreScene = false;

        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("Fireworks Tweaks").AddComponent<FireworksTweaks>().transform.parent = parent;
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
                Plugin.Log("Tweaks (FireworksTweaks) messed up: " + e, Plugin.LogLevel.Error);
            }
        }

        private IEnumerator WaitForLoad()
        {
            while (!loaded)
            {
                resultsViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();
                fireworksController = ReflectionUtil.GetPrivateField<FireworksController>(resultsViewController, "_fireworksController");

                if (resultsViewController == null)
                {
                    Plugin.Log("resultsViewController is null!", Plugin.LogLevel.DebugOnly);
                    yield return new WaitForSeconds(0.01f);
                }
                else if (fireworksController == null)
                {
                    Plugin.Log("fireworksController is null!", Plugin.LogLevel.DebugOnly);
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    Plugin.Log("Found FireworksController!", Plugin.LogLevel.DebugOnly);
                    loaded = true;
                }
            }
            Init();
        }

        private void Init()
        {
            Plugin.Log("FireworksTweaks Initialized!", Plugin.LogLevel.DebugOnly);

            isEnabled = Settings.FireworksEnable;
        }

        public void Update()
        {
            Plugin.Log("Updating fireworks tweaks!", Plugin.LogLevel.DebugOnly);
            if (!loaded) return;

            if (resultsViewController == null)
            {
                Plugin.Log("resultsViewController is null!", Plugin.LogLevel.DebugOnly);
                resultsViewController = Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();
            }
            else if (fireworksController == null)
            {
                Plugin.Log("fireworksController is null!", Plugin.LogLevel.DebugOnly);
                fireworksController = Resources.FindObjectsOfTypeAll<FireworksController>().FirstOrDefault();
            }
            else if (resultsViewController.enabled && fireworksController.enabled)
            {
                Plugin.Log("fireworksController.enabled", Plugin.LogLevel.DebugOnly);
                if (!isEnabled)
                {
                    Plugin.Log("isEnabled is false", Plugin.LogLevel.DebugOnly);
                    fireworksController.enabled = false;

                    // Delete any fireworks that were generated before it could be disabled
                    var fireworkItemController = Resources.FindObjectsOfTypeAll<FireworkItemController>().FirstOrDefault();
                    GameObject.Destroy(fireworkItemController);
                } else
                {
                    Plugin.Log("isEnabled is true", Plugin.LogLevel.DebugOnly);
                }
            }
        }
    }
}
