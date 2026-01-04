using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SCN.IAP
{
    [CustomEditor(typeof(IAPManager))]
    public class IAPManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            IAPManager iapManager = (IAPManager)target;
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.richText = true;
            //style.fontSize = 13;
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<color=Yellow>IAP Manager Setup</color>", style);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            iapManager.IAPPopupTheme = (IAPManager.IAPPopupThemeEnum)EditorGUILayout.EnumPopup("Popup Theme", iapManager.IAPPopupTheme);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Parent Gate Popup", GUILayout.MinWidth(600), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.35f));
            EditorGUI.BeginDisabledGroup(true);
            iapManager.ParentGatePopup = (GameObject)EditorGUILayout.ObjectField(iapManager.ParentGatePopup, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Remove Ads Popup", GUILayout.MinWidth(600), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.35f));
            EditorGUI.BeginDisabledGroup(true);
            iapManager.RemoveAdsPopup = (GameObject)EditorGUILayout.ObjectField(iapManager.RemoveAdsPopup, typeof(GameObject), false);
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Subscription Popup", GUILayout.MinWidth(600), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.35f));
            //EditorGUI.BeginDisabledGroup(true);
            iapManager.SubscriptionPopup = (GameObject)EditorGUILayout.ObjectField(iapManager.SubscriptionPopup, typeof(GameObject), false);
            //EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<color=Yellow>Parent Gate</color>", style);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            iapManager.RetryTime = int.Parse(EditorGUILayout.TextField(new GUIContent("Retry Time", "How many fail attemp to lock the Buttons"), iapManager.RetryTime.ToString()));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            iapManager.NextTryTime = int.Parse(EditorGUILayout.TextField(new GUIContent("Next Try Cooldown", "How you need to wait after failed"), iapManager.NextTryTime.ToString()));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            iapManager.SortingLayer = EditorGUILayout.TextField(new GUIContent("Sorting Layer", "Sorting layer of the Popup"), iapManager.SortingLayer);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            iapManager.OrderInLayer = int.Parse(EditorGUILayout.TextField(new GUIContent("Order in Layer", "Order of Popup in Layer"), iapManager.OrderInLayer.ToString()));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<color=Yellow>Remove Ads Popup</color>", style);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            iapManager.IdRemoveAds = EditorGUILayout.TextField(new GUIContent("ID Remove Ads", "Product ID in IAP Catalog"), iapManager.IdRemoveAds);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Remove Ads Banner", GUILayout.MinWidth(600), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.35f));
            iapManager.RemoveAdsBannerImageSprite = (Sprite)EditorGUILayout.ObjectField(iapManager.RemoveAdsBannerImageSprite, typeof(Sprite), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("<color=Yellow>Subscription Popup</color>", style);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Is Using Subscription ", GUILayout.MinWidth(600), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth * 0.35f));
            iapManager.IsUsingSubscription = EditorGUILayout.Toggle(iapManager.IsUsingSubscription);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            iapManager.IdSubscriptionMonthly = EditorGUILayout.TextField(new GUIContent("ID Subscription Monthly", "Product ID in IAP Catalog"), iapManager.IdSubscriptionMonthly);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            iapManager.IdSubscriptionAnnual = EditorGUILayout.TextField(new GUIContent("ID Subscription Annual", "Product ID in IAP Catalog"), iapManager.IdSubscriptionAnnual);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            iapManager.IdUnlockAll = EditorGUILayout.TextField(new GUIContent("ID Non consume Unlock All", "Product ID in IAP Catalog"), iapManager.IdUnlockAll);
            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                iapManager.LoadCurrentIAPTheme();
                EditorUtility.SetDirty(target);
            }
            //base.DrawDefaultInspector();
        }


        [MenuItem("GameObject/SCN/IAP/IAPManager")]
        static void CreateIAPManager()
        {
            if (Selection.activeGameObject == null)
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>("Assets/SCNLib/IAP/Prefabs/IAPManager.prefab"));
                obj.name = "IAP Manager";
                Selection.activeGameObject = obj;
            }
            else
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>("Assets/SCNLib/IAP/Prefabs/IAPManager.prefab")
                    , Selection.activeGameObject.transform);
                obj.name = "IAP Manager";
                Selection.activeGameObject = obj;
            }

            _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
        [MenuItem("GameObject/SCN/IAP/Remove Ads Button")]
        static void CreateRemoveAdsButton()
        {
            if (Selection.activeGameObject == null)
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>("Assets/SCNLib/IAP/Prefabs/RemoveAdsButton.prefab"));
                obj.name = "Remove Ads Button";
                Selection.activeGameObject = obj;
            }
            else
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>("Assets/SCNLib/IAP/Prefabs/RemoveAdsButton.prefab")
                    , Selection.activeGameObject.transform);
                obj.name = "Remove Ads Button";
                Selection.activeGameObject = obj;
            }

            _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
        [MenuItem("GameObject/SCN/IAP/Subscription Button")]
        static void CreateSubscriptionButton()
        {
            if (Selection.activeGameObject == null)
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>("Assets/SCNLib/IAP/Prefabs/SubscriptionButton.prefab"));
                obj.name = "Subscription Button";
                Selection.activeGameObject = obj;
            }
            else
            {
                var obj = Instantiate(LoadSource.LoadAssetAtPath<GameObject>("Assets/SCNLib/IAP/Prefabs/SubscriptionButton.prefab")
                    , Selection.activeGameObject.transform);
                obj.name = "Subscription Button";
                Selection.activeGameObject = obj;
            }

            _ = UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty
                (UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
    }
}