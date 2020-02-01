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
    class ClickShockwave : MonoBehaviour
    {
        public static ClickShockwave Instance;

        private MenuShockwave menuShockwave;
        private bool clickShockwaveOn = true;
        private bool loaded = false;
        
        public static void OnLoad(Transform parent)
        {
            if (Instance != null) return;
            new GameObject("RippleEffect Tweaks").AddComponent<ClickShockwave>().transform.parent = parent;
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
                Plugin.Log("Tweaks (RippleEffect) messed up: " + e, Plugin.LogLevel.Error);
            }
        }

        private IEnumerator WaitForLoad()
        {
            while (!loaded)
            {
                menuShockwave = Resources.FindObjectsOfTypeAll<MenuShockwave>().FirstOrDefault();

                if (menuShockwave == null)
                    yield return new WaitForSeconds(0.01f);
                else
                {
                    Plugin.Log("Found MenuShockwave!", Plugin.LogLevel.DebugOnly);
                    loaded = true;
                }
            }
            Init();
        }

        private void Init()
        {
            Plugin.Log("ClickShockwave Initialized!", Plugin.LogLevel.DebugOnly);

            clickShockwaveOn = Settings.ClickShockwaveEnable;
        }

        public void Update()
        {
            if (!loaded) return;

            if (menuShockwave == null)
            {
                menuShockwave = Resources.FindObjectsOfTypeAll<MenuShockwave>().FirstOrDefault();
            }
            else if (menuShockwave.enabled != clickShockwaveOn)
            {
                menuShockwave.enabled = clickShockwaveOn;
            }
        }
    }
}
