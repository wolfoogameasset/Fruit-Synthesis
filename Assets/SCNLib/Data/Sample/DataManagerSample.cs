using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.BinaryData
{
    /// <summary>
    /// Script mau de quan ly data
    /// </summary>
    [CreateAssetMenu(fileName = fileNameSO, menuName = "SCN sample/Scriptable Objects/Data")]
    public class DataManagerSample : ScriptableObject
    {
        const string fileNameSO = "Data manager";

        static DataManagerSample instance;
        public static DataManagerSample Instance
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

        [SerializeField] string dataFileName = "data.scn";

        static void Setup()
        {
            // Load SO Data
            instance = LoadSource.LoadObject<DataManagerSample>(fileNameSO);

            instance.localData = BinaryDataManager.LoadData<LocalData>(instance .dataFileName);

            // Auto save khi quit game
            DDOL.Instance.OnApplicationPauseE += pause => { Debug.Log("local data " + pause); if (pause) instance.SaveLocalData(); };
            DDOL.Instance.OnApplicationQuitE += () => { instance.SaveLocalData(); };

#if UNITY_EDITOR
            DDOL.Instance.OnUpdateE += () => { if (Input.GetKeyDown(KeyCode.S)) instance.SaveLocalData(); };
#endif
        }

        [SerializeField] LocalData localData;
        public LocalData LocalData => localData;

        public void SaveLocalData()
        {
            Debug.Log("Save data");
            BinaryDataManager.SaveData<LocalData>(localData, dataFileName);
        }
    }

    /// <summary>
    /// Cau truc mau cua 1 class data
    /// </summary>
    [System.Serializable]
    public class LocalData : BinaryData
    {
        [SerializeField] bool sampleBool;
        [SerializeField] int sampleInt;
        [SerializeField] string sampleString;
        public bool SampleBool
        {
            get => sampleBool;
            set => sampleBool = value;
        }
        public int SampleInt
        {
            get => sampleInt;
            set => sampleInt = value;
        }
        public string SampleString
        {
            get => sampleString;
            set => sampleString = value;
        }

        public override void SetupDefault()
        {
            // Trang thai dau tien cua data, mac dinh khi bat dau
            sampleBool = true;
            sampleInt = 10;
            sampleString = "test";
        }
    }
}