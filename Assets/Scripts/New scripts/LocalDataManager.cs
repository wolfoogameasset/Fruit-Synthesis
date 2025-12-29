using SCN.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.BinaryData
{
    /// <summary>
    /// Script mau de quan ly data
    /// </summary>
    [CreateAssetMenu(fileName = fileNameSO, menuName = "SCN/Scriptable Objects/Data")]
    public class LocalDataManager : ScriptableObject
    {
        const string fileNameSO = "User Data manager";

        static LocalDataManager instance;
        public static LocalDataManager Instance
        {
            get
            {
                if (instance == null) Setup();
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        [SerializeField] string dataFileName = "gachatoon_fruitsynthesis.scn";

        static void Setup()
        {
            // Load SO Data
            instance = LoadSource.LoadObject<LocalDataManager>(fileNameSO);

            instance.userLocalData = BinaryDataManager.LoadData<UserLocalData>(instance.dataFileName);

            // Auto save khi quit game
            DDOL.Instance.OnApplicationPauseE += pause => { Debug.Log("local data " + pause); if (pause) instance.SaveLocalData(); };
            DDOL.Instance.OnApplicationQuitE += () => { instance.SaveLocalData(); };

#if UNITY_EDITOR
            DDOL.Instance.OnUpdateE += () => { if (Input.GetKeyDown(KeyCode.S)) instance.SaveLocalData(); };
#endif
        }

        [SerializeField] UserLocalData userLocalData;
        public UserLocalData UserLocalData => userLocalData;

        public void SaveLocalData()
        {
            Debug.Log("Save data");
            BinaryDataManager.SaveData<UserLocalData>(userLocalData, dataFileName);
        }
    }

    /// <summary>
    /// Cau truc mau cua 1 class data
    /// </summary>
    [System.Serializable]
    public class UserLocalData : BinaryData
    {
        [SerializeField] int currentCoins;
        [SerializeField] int highScore;

        public int CurrentCoins { get => currentCoins; set => currentCoins = value; }
        public int HighScore { get => highScore; set => highScore = value; }

        public override void SetupDefault()
        {
            CurrentCoins = 0;
            HighScore = 0;
        }
    }
}