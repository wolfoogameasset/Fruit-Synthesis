using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SCN.Common;
using SCN.FirebaseLib.FA;

namespace SCN.Ads
{
    public static class NetEditor
    {
        [MenuItem("SCN/Net/Admob settings android")]
        static void SelectAdmobAndroidConfig()
        {
            Master.CreateAndSelectAssetInResource<AdmobConfig>(AdmobConfig.AssetNameAndroid);
        }

        [MenuItem("SCN/Net/Admob settings ios")]
        static void SelectSettingAdmobIOSConfig()
        {
            Master.CreateAndSelectAssetInResource<AdmobConfig>(AdmobConfig.AssetNameIOS);
        }
        

        [MenuItem("SCN/Net/GA setting")]
        static void SelectSettingGAConfig()
		{
            Master.CreateAndSelectAssetInResource<GAManager>(GAManager.AssetName);
        }
    }
}