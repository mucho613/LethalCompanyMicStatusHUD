using Dissonance;
using HarmonyLib;
using UnityEngine;
using BepInEx;
using TMPro;
using UnityEngine.InputSystem;
using System.Linq;

namespace MicStatusHUD { 
    [HarmonyPatch(typeof(HUDManager))]
    public class MicStatusHUDPatch
    {
        private static TextMeshProUGUI micStatus;
        private static TextMeshProUGUI levelMeter;

        public static HUDManager hud = HUDManager.Instance;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(HUDManager.Start))]
        static void createMicStatusHUD(ref HUDManager __instance)
        {
            GameObject levelMeterHUD = new GameObject("LevelMeterHUD");
            levelMeterHUD.AddComponent<RectTransform>();
            TextMeshProUGUI levelMeterTemp = levelMeterHUD.AddComponent<TextMeshProUGUI>();
            RectTransform levelMeterRectTransform = levelMeterTemp.rectTransform;
            levelMeterRectTransform.SetParent(__instance.PTTIcon.transform, false);
            levelMeterRectTransform.anchoredPosition = new Vector2(150f, 20f);
            levelMeterTemp.font = __instance.controlTipLines[0].font;
            levelMeterTemp.fontSize = 10f;
            levelMeterTemp.fontWeight = FontWeight.Bold;
            levelMeterTemp.enabled = true;
            levelMeterTemp.color = Color.white;
            levelMeterTemp.overflowMode = TextOverflowModes.Truncate;
            levelMeterTemp.alignment = TextAlignmentOptions.Center;
            levelMeter = levelMeterTemp;

            GameObject micStatusHUD = new GameObject("MicStatusHUD");
            micStatusHUD.AddComponent<RectTransform>();
            TextMeshProUGUI micStatusTemp = micStatusHUD.AddComponent<TextMeshProUGUI>();
            RectTransform micStatusRectTransform = micStatusTemp.rectTransform;
            micStatusRectTransform.SetParent(__instance.PTTIcon.transform, false);
            micStatusRectTransform.anchoredPosition = new Vector2(150f, 20f);
            micStatusTemp.font = __instance.controlTipLines[0].font;
            micStatusTemp.fontSize = 20f;
            micStatusTemp.fontWeight = FontWeight.Bold;
            micStatusTemp.enabled = true;
            micStatusTemp.overflowMode = TextOverflowModes.Overflow;
            micStatusTemp.alignment = TextAlignmentOptions.Center;
            micStatus = micStatusTemp;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(HUDManager.Update))]
        private static void Update(ref HUDManager __instance)
        {
            if (micStatus == null || levelMeter == null)
            {
                return;
            }

            if (StartOfRound.Instance.voiceChatModule == null)
            {
                return;
            }

            VoicePlayerState player = StartOfRound.Instance.voiceChatModule.FindPlayer(StartOfRound.Instance.voiceChatModule.LocalPlayerName);
            float detectedAmplitude = Mathf.Clamp(player.Amplitude * 30f, 0f, 1f);

            // Mic status text
            micStatus.color = Color.Lerp(Color.white, Color.red, detectedAmplitude);

            // Mic input level meter
            levelMeter.text = string.Concat(Enumerable.Repeat("|", (int)(detectedAmplitude * 30f)));
            levelMeter.color = Color.Lerp(Color.white, Color.red, detectedAmplitude);

            if (IngamePlayerSettings.Instance.settings.pushToTalk)
            {
                if (__instance.PTTIcon.enabled)
                {
                    micStatus.text = "";
                    levelMeter.enabled = true;
                    return;
                }
                else
                {
                    micStatus.text = "PUSH TO TALK";
                    levelMeter.enabled = false;
                    return;
                }
            } 

            if (!IngamePlayerSettings.Instance.settings.micEnabled)
            {
                micStatus.text = "MUTING";
                levelMeter.enabled = false;
                return;
            }

            micStatus.text = "";
            levelMeter.enabled = true;
        }
    }
}
