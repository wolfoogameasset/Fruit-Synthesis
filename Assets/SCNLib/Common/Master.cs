using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SCN.Common
{
    public enum Direction
	{
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,
    }

    public enum Orientation
	{
        Horizontal = 0,
        Vertical = 1
    }

    public static class Master
    {
        #region Event strigger
        public static void AddEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            var _trigger = trigger.triggers.Find(e => e.eventID == eventType);
            if (_trigger == null)
            {
                EventTrigger.Entry entry = new EventTrigger.Entry
                {
                    eventID = eventType,
                    callback = new EventTrigger.TriggerEvent()
                };
                entry.callback.AddListener(callback);
                trigger.triggers.Add(entry);
            }
            else
            {
                _trigger.callback.AddListener(callback);
            }
        }

        public static void RemoveEventTriggerListener(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction<BaseEventData> callback)
        {
            var _trigger = trigger.triggers.Find(e => e.eventID == eventType);
            if (_trigger == null)
                return;
            _trigger.callback.RemoveListener(callback);
        }

        public static void RemoveAllEventTriggerListener(EventTrigger trigger, EventTriggerType eventType)
        {
            _ = trigger.triggers.RemoveAll(e => e.eventID == eventType);
        }
        #endregion

        #region Get hierarchy object
        public static GameObject GetChildByName(GameObject parent, string childName)
        {
            foreach (Transform item in parent.GetComponentsInChildren<Transform>(true))
            {
                string childNameItem = item.gameObject.name;
                if (childNameItem == childName)
                {
                    return item.gameObject;
                }
            }
            return null;
        }

        public static List<T> FindAllObjectsOfType<T>()
        {
            List<T> results = new List<T>();
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().ToList()
                .ForEach(g => results.AddRange(g.GetComponentsInChildren<T>(true)));
            return results;
        }
        #endregion

        #region convert time
        public static string ConvertSecond(int second)
        {
            System.TimeSpan time = System.TimeSpan.FromSeconds(second);

            if (second < 60)
            {
                return time.ToString(@"ss");
            }
            else if (second >= 60 && second < 3600)
            {
                return time.ToString(@"mm\:ss");
            }
            else if (second >= 3600 && second < 86400)
                return time.ToString(@"hh\:mm\:ss");
            else if (second >= 86400)
                return time.Days + "d " + time.ToString(@"hh\:mm\:ss");

            return "";
        }
        public static string ConvertSecond2(int second)
        {
            System.TimeSpan time = System.TimeSpan.FromSeconds(second);
            return time.ToString(@"mm\:ss");
        }

        public static string ConvertSecondWithPara(int second)
        {
            System.TimeSpan time = System.TimeSpan.FromSeconds(second);

            return second < 60
                ? second.ToString() + "s"
                : second >= 60 && second < 3600
                ? string.Format("{0:D2}m {1:D2}s", time.Minutes, time.Seconds)
                : second >= 3600 && second < 86400
                ? string.Format("{0:D2}h {1:D2}m {2:D2}s", time.Hours, time.Minutes, time.Seconds)
                : second >= 86400 ? "> 2 days" : "";
        }

        public static string ConvertMinute(int minute)
        {
            System.TimeSpan time = System.TimeSpan.FromMinutes(minute);

            if (minute < 60)
            {
                return minute.ToString() + "m";
            }
            else if (minute >= 60 && minute < 1440)
            {
                var st = time.Hours + "h";
                if (time.Minutes != 0) st += " " + time.Minutes + "m";

                return st;
            }
            else if (minute >= 1440)
            {
                var st = time.Days + "d";
                if (time.Hours != 0) st += " " + time.Hours + "h";
                if (time.Minutes != 0) st += " " + time.Minutes + "m";

                return st;
            }

            return "";
        }
        #endregion

        #region Application
        public static bool IsAndroid
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android
                    || Application.platform == RuntimePlatform.WindowsEditor)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool IsPortrait
        {
			get
			{
                return Screen.width < Screen.height;
            }
        }
        #endregion

        #region Others
        public static void ChangeAlpha(Image ima, float a)
        {
            var color = ima.color;
            color.a = a;
            ima.color = color;
        }

        public static void ChangeAlpha(SpriteRenderer spriteRen, float a)
        {
            var color = spriteRen.color;
            color.a = a;
            spriteRen.color = color;
        }

        public static void ChangeAlpha(Text text, float a)
        {
            var color = text.color;
            color.a = a;
            text.color = color;
        }

        public static string ColorRichText(string st, string hexaColor)
        {
            return "<color=#" + hexaColor + ">" + st + "</color>";
        }

        public static Canvas GetTopmostCanvas(Component component)
        {
            Canvas[] parentCanvases = component.GetComponentsInParent<Canvas>();
            if (parentCanvases != null && parentCanvases.Length > 0)
            {
                return parentCanvases[parentCanvases.Length - 1];
            }
            return null;
        }

        public static Canvas GetNearestCanvas(Component component)
        {
            Canvas[] parentCanvases = component.GetComponentsInParent<Canvas>();
            if (parentCanvases != null && parentCanvases.Length > 0)
            {
                return parentCanvases[0];
            }
            return null;
        }

        public static bool HasTouchScreen()
        {
            return Input.GetMouseButton(0);
        }

        /// <summary>
        /// Set pivot without change posion in scene
        /// </summary>
        public static void SetPivot(RectTransform rectTransform, Vector2 pivot)
        {
            Vector3 deltaPosition = rectTransform.pivot - pivot;    // get change in pivot
            deltaPosition.Scale(rectTransform.rect.size);           // apply sizing
            deltaPosition.Scale(rectTransform.localScale);          // apply scaling
            deltaPosition = rectTransform.rotation * deltaPosition; // apply rotation

            rectTransform.pivot = pivot;                            // change the pivot
            rectTransform.localPosition -= deltaPosition;           // reverse the position change
        }

#if UNITY_EDITOR
        public static T CreateSOInstance<T>(string assetPath) where T: ScriptableObject
		{
            T temp = ScriptableObject.CreateInstance<T>();

            UnityEditor.AssetDatabase.CreateAsset(temp, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.FocusProjectWindow();

            return temp;
        }

        public static void CreateAndSelectAssetInResource<T>(string assetName) where T : ScriptableObject
		{
            var settings = LoadSource.LoadObject<T>(assetName);
            if (settings == null)
            {
                UnityEditor.Selection.activeObject = CreateSOInstance<T>($"Assets/Resources/{assetName}.asset");
            }
            else
            {
                UnityEditor.Selection.activeObject = settings;
            }
        }
#endif

#endregion
    }
}