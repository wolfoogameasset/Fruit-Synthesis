using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SCN.SpecialEvent
{
    public class SpecialEvent_Editor : Editor
    {
        private static string managerPrefabPath = "Assets/SCNLib/Special Event/Prefabs/Special Event Manager.prefab";
        private static string buttonPrefabPath = "Assets/SCNLib/Special Event/Prefabs/Special Event Button.prefab";

        [MenuItem("GameObject/SCN/Special Event/Special Event Button", false, 32)]
        private static void Create_SpecialEventButton()
        {
            var asset = AssetDatabase.LoadAssetAtPath(buttonPrefabPath, typeof(GameObject));
            var instantiatedPrefab = PrefabUtility.InstantiatePrefab(asset);
            var selectObj = Selection.activeObject as GameObject;
            GameObject obj = instantiatedPrefab as GameObject;
            obj.GetComponent<Transform>().transform.SetParent(selectObj.transform);
            obj.GetComponent<Transform>().SetAsLastSibling();
            //obj.transform.localPosition = Vector3.zero;
            //obj.transform.localScale = Vector3.one;
            Selection.activeObject = obj;
            EditorUtility.SetDirty(obj);
            EditorSceneManager.MarkSceneDirty(((GameObject)instantiatedPrefab).scene);
        }

        [MenuItem("GameObject/SCN/Special Event/Special Event Button", true, 32)]
        private static bool ValidateUI()
        {
            var selectObj = Selection.activeObject as GameObject;
            if (selectObj.transform.root.GetComponent<Canvas>())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        [MenuItem("GameObject/SCN/Special Event/Special Event Manager", false, 32)]
        private static void Create_SpecialEventManager()
        {
            var asset = AssetDatabase.LoadAssetAtPath(managerPrefabPath, typeof(GameObject));
            var instantiatedPrefab = PrefabUtility.InstantiatePrefab(asset);
            GameObject obj = instantiatedPrefab as GameObject;
            obj.GetComponent<Transform>().SetAsLastSibling();
            Selection.activeObject = obj;
            EditorUtility.SetDirty(obj);
            EditorSceneManager.MarkSceneDirty(((GameObject)instantiatedPrefab).scene);
        }
    }
}