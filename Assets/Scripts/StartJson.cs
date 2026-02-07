using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;


public class StartJson : MonoBehaviour
{

    [System.Serializable]
    public class HorLineInfo
    {
        public int horLineId;
        public bool isActive;
        public int standFlagId;
        public int moneyValue;
        public int heartValue;
        public int healthValue;
        public string eventText;
    }

    [System.Serializable]
    public class VarLineInfo
    {
        public int varLineId;
        public int eventId;
        public int startCharaId;
    }

        [System.Serializable]
    public class CharaData
    {
        public int id;
        public string name;
        public string comment;
        public int likeCharaId;
        public int disLikeCharaId;
        public int StartPosition;
        public int money;
        public int heart0;
        public int heart1;
        public int heart2;
        public int heart3;
        public int heart4;
        public int heart5;

        public int health;
    }

    [System.Serializable]
    //フラグ管理用
    public class FlagData
    {
        public bool isFlagNeeded;
        public bool isFlagSet;
        public int flagId;
    }

    [System.Serializable]
    public class SaveData
    {
        public int VarticalLineNumber;
        public List<CharaData> charaList;
        public List<FlagData> flagList;
        public List<VarLineInfo> VarLineInfo;
        public List<HorLineInfo> HorLineInfo; 
    }


    void Start()
    {
        // 現在開いているシーンの名前を取得
        string sceneName = SceneManager.GetActiveScene().name;

        string jsonFolderPath = Path.Combine(Application.dataPath, "Json");

        // フォルダが存在しなければ作成
        if (!Directory.Exists(jsonFolderPath))
        {
            Directory.CreateDirectory(jsonFolderPath);
        }

        string jsonFilePath = Path.Combine(Application.dataPath, "Json/" + sceneName + ".json");

        if (!File.Exists(jsonFilePath))
        {
            // 初期データを作成
            SaveData initialData = new SaveData
            {
                VarticalLineNumber = -1,
                charaList = new List<CharaData>(),
                flagList = new List<FlagData>()
            };

            for (int i = 0; i < 6; i++)
            {
                string charaName = GetCharaName(i);
                int likeCharaId = GetLikeCharaId(i);
                int disLikeCharaId = GetDisLikeCharaId(i);

                initialData.charaList.Add(new CharaData
                {
                    id = i,
                    name = charaName,
                    comment = null,
                    likeCharaId = likeCharaId,     
                    disLikeCharaId = disLikeCharaId  ,
                    StartPosition= -1
                });
            }

            for (int i = 0; i < 10; i++)
            {
                initialData.flagList.Add(new FlagData
                {
                    isFlagNeeded = false,
                    isFlagSet = false,
                    flagId = i
                });
            }

            // Newtonsoft.Jsonを使ってシリアライズ
            string json = JsonConvert.SerializeObject(initialData, Formatting.Indented);
            File.WriteAllText(jsonFilePath, json);

            Debug.Log($"初期JSONファイル作成: {jsonFilePath}");
        }
        else
        {
            Debug.Log($"JSONファイルは既に存在します: {jsonFilePath}");
        }
    }

    private string GetCharaName(int id)
    {
        switch (id)
        {
            case 0:
                return "イチカ";
            case 1:
                return "フタバ";
            case 2:
                return "ミツヤ";
            case 3:
                return "シロウ";
            case 4:
                return "イツキ";
            case 5:
                return "ムツミ";
            default:
                return "NewCharactor";
        }
    }

    private int GetLikeCharaId(int id)
    {
        switch (id)
        {
            case 0:
                return 1;
            case 1:
                return 2;
            case 2:
                return 3;
            case 3:
                return 4;
            case 4:
                return 5;
            case 5:
                return 0;
            default:
                return -1;
        }
    }

    private int GetDisLikeCharaId(int id)
    {
        switch (id)
        {
            case 0:
                return 2;
            case 1:
                return 3;
            case 2:
                return 4;
            case 3:
                return 5;
            case 4:
                return 0;
            case 5:
                return 1;
            default:
                return -1;
        }
    }
}

