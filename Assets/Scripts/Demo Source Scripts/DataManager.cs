using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class DataManager {
    private static string KEY_MONEY = "key_current_money";

    private const string KEY_RED_BAG_GIVE_UP = "key_red_bag_give_up";

    private const string KEY_MAX_SCORE = "key_max_score"; 
    private const string KEY_MAX_LEVEL = "key_max_level"; 

    private const string KEY_CREATE_TIME = "key_create_time";

    private const string KEY_LOGIN_TIME = "key_login_time";

    private const string KEY_RV_COUNT = "key_rv_count";

    private const string KEY_COMPOSITE_COUNT = "key_composite_count";

    private const string KEY_LEVEL_NUN = "key_level_nun";

    private const string KEY_INTER_COUNT = "key_inter_count";

    private const string KEY_TWO_OPEN = "KEY_TWO_OPEN";

    private const string KEY_NAT = "KEY_NAT";
    public static void saveInfo(LevelPlayerInfo user) {
    }

    public static LevelPlayerInfo getInfo() {
        LevelPlayerInfo info = null;

        if (info == null) {
            info = new LevelPlayerInfo();
            info.level = 1;
            info.score = 0;
            info.maxScore = 0;
            info.listSphereInfo = new List<SphereInfo>();
            DataManager.saveInfo(info);
        }

        return info;
    }

    public static void addLevel(int _level) {
        LevelPlayerInfo info = getInfo();
        if (info == null) return;
        info.level += _level;
        saveInfo(info);
        setMaxLevel(info.level);
    }

    public static int getLevel {
        get
        {
            LevelPlayerInfo info = getInfo();
            if (info == null) return 1;
            return info.level;
        }
        set
        {
            LevelPlayerInfo info = getInfo();
            if (info == null) return;
            info.level = value;
            saveInfo(info);
        }
    }
    public static void addExp(int _exp) {
        LevelPlayerInfo info = getInfo();
        if (info == null) return;
        info.exp += _exp;
        saveInfo(info);
    }

    public static int getExp {
        get
        {
            LevelPlayerInfo info = getInfo();
            if (info == null) return 1;
            return info.exp;
        }
        set
        {
            LevelPlayerInfo info = getInfo();
            if (info == null) return;
            info.exp = value;
            saveInfo(info);
        }
    }

    public static void addLevelScore(int _levelScore) {
        LevelPlayerInfo info = getInfo();
        if (info == null) return;
        info.levelScore += _levelScore;
        saveInfo(info);
    }

    public static int getLevelScore {
        get
        {
            LevelPlayerInfo info = getInfo();
            if (info == null) return 0;
            return info.levelScore;
        }
        set
        {
            LevelPlayerInfo info = getInfo();
            if (info == null) return;
            info.levelScore = value;
            saveInfo(info);
        }
    }
    public static void addScore(int _score) {
        LevelPlayerInfo info = getInfo();
        if (info == null) return;
        info.score += _score;
        if (info.score >= info.maxScore)
            info.maxScore += _score;
        saveInfo(info);
        setMaxScore(info.score);
    }

    public static int getMaxScore {
        get
        {
            LevelPlayerInfo info = getInfo();
            if (info == null) return 0;
            return info.maxScore;
        }
        set
        {
            LevelPlayerInfo info = getInfo();
            if (info == null) return;
            info.maxScore = value;
            saveInfo(info);
        }
    }

    public static int getScore {
        get
        {
            LevelPlayerInfo info = getInfo();
            if (info == null) return 0;
            return info.score;
        }
        set
        {
            LevelPlayerInfo info = getInfo();
            if (info == null) return;
            info.score = value;
            saveInfo(info);
        }
    }

    public static void addSphereInfo(SphereInfo _info) {
        LevelPlayerInfo info = getInfo();
        if (info == null) return;
        info.listSphereInfo.Add(_info);
        saveInfo(info);
    }

    public static List<SphereInfo> getSphereInfo {
        get
        {
            LevelPlayerInfo info = getInfo();
            if (info == null)
                return new List<SphereInfo>();
            return info.listSphereInfo;
        }
        set
        {
            LevelPlayerInfo info = getInfo();
            if (info == null) return;
            info.listSphereInfo = value;
            saveInfo(info);
        }
    }

    public static int getGravityNum {
        get { return PlayerPrefs.GetInt("GravityNum", 3); }
    }

    public static void setGravityNum(bool isDefult = false) {
        PlayerPrefs.SetInt("GravityNum", isDefult ? 3 : PlayerPrefs.GetInt("GravityNum", 3) - 1);
    }

    public static int getUniversalNum {
        get { return PlayerPrefs.GetInt("UniversalNum", 3); }
    }

    public static void setUniversalNum(bool isDefult = false) {
        PlayerPrefs.SetInt("UniversalNum", isDefult ? 3 : PlayerPrefs.GetInt("UniversalNum", 3) - 1);
    }

    public static float getCurrentMoney() {
        return float.Parse(PlayerPrefs.GetFloat(KEY_MONEY, 0f).ToString("F2"));
    }

    public static void setCurrentMoney(float money) {
        PlayerPrefs.SetFloat(KEY_MONEY, money);
    }

    public static void addMoney(float money) {
        float m = float.Parse((getCurrentMoney() + money).ToString("F2"));
        PlayerPrefs.SetFloat(KEY_MONEY, m);
    }

    public static void resetGiveUpNum() {
        PlayerPrefs.SetInt(KEY_RED_BAG_GIVE_UP, 0);
        PlayerPrefs.SetInt(KEY_LEVEL_NUN, 0);
    }

    public static void addGiveUpNum() {
        PlayerPrefs.SetInt(KEY_RED_BAG_GIVE_UP, getGiveUpNum() + 1);
    }

    public static int getGiveUpNum() {
        return PlayerPrefs.GetInt(KEY_RED_BAG_GIVE_UP, 0);
    }

    public static void addLevelNum() {
        PlayerPrefs.SetInt(KEY_LEVEL_NUN, getLevelNum() + 1);
    }

    public static int getLevelNum() {
        return PlayerPrefs.GetInt(KEY_LEVEL_NUN, 0);
    }

    public static void setMaxLevel(int level) {
        if (level > getMaxLevel()) {
            PlayerPrefs.SetInt(KEY_MAX_LEVEL, level);
        }
    }

    public static int getMaxLevel() {
        return PlayerPrefs.GetInt(KEY_MAX_LEVEL, 0);
    }

    public static void setMaxScore(int score) {
        if (score > getMaxMaxScore()) {
            PlayerPrefs.SetInt(KEY_MAX_SCORE, score);
              if (score > 1 && PlayerPrefs.GetInt("default_init_85") == 0)
            {
                PlayerPrefs.SetInt("default_init_85", 1);
            }
            if (score > 2 && PlayerPrefs.GetInt("default_init_90") == 0)
            {
                PlayerPrefs.SetInt("default_init_90", 1);
            }

            if (score > 3 && PlayerPrefs.GetInt("default_init_95") == 0)
            {
                PlayerPrefs.SetInt("default_init_95", 1);
            }
        }
    }

    public static int getMaxMaxScore() {
        return PlayerPrefs.GetInt(KEY_MAX_SCORE, 0);
    }

    public static void setCreateTime() {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString(KEY_CREATE_TIME))) {
            PlayerPrefs.SetString(KEY_CREATE_TIME, DateTime.Now.ToString());
        }
    }

    public static string getCreateTime() {
        return PlayerPrefs.GetString(KEY_CREATE_TIME);
    }

    public static void setLoginTime() {
        PlayerPrefs.SetString(KEY_LOGIN_TIME, DateTime.Now.ToString());
        if (PlayerPrefs.GetInt(KEY_TWO_OPEN, 0) == 1) {
            return;
        }

        try {
            GregorianCalendar gc = new GregorianCalendar();
            int createDay = gc.GetDayOfYear(DateTime.Parse(getLoginTime()));
            int loginDay = gc.GetDayOfYear(DateTime.Parse(getCreateTime()));
            if (loginDay - createDay == 1) {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                PlayerPrefs.SetInt(KEY_TWO_OPEN, 1);
            }
        }
        catch (Exception e) {
        }
    }

    public static string getLoginTime() {
        return PlayerPrefs.GetString(KEY_LOGIN_TIME);
    }

    public static void addRv() {
        PlayerPrefs.SetInt(KEY_RV_COUNT, getRvCount() + 1);
        if (getRvCount() == 10 || getRvCount() == 20 || getRvCount() == 30) {
        }
    }

    public static int getRvCount() {
        return PlayerPrefs.GetInt(KEY_RV_COUNT, 0);
    }

    public static void addInter() {
        PlayerPrefs.SetInt(KEY_INTER_COUNT, getInterCount() + 1);
        if (getInterCount() == 10 || getInterCount() == 20 || getInterCount() == 30) {
        }
    }

    public static int getInterCount() {
        return PlayerPrefs.GetInt(KEY_INTER_COUNT, 0);
    }


    public static void compositeBall() {
        PlayerPrefs.SetInt(KEY_COMPOSITE_COUNT, getCompositeBall() + 1);
        if (getCompositeBall() == 1 || getCompositeBall() == 3 || getCompositeBall() == 5 || getCompositeBall() == 7 ||
            getCompositeBall() == 10) {
        }
    }

    public static int getCompositeBall() {
        return PlayerPrefs.GetInt(KEY_COMPOSITE_COUNT, 0);
    }

    public static void SetNat(bool isNat)
    {
        int index = isNat ? 1 : 0;
        PlayerPrefs.SetInt(KEY_NAT, index);
    }

    public static bool IsNat()
    {

        var index = PlayerPrefs.GetInt(KEY_NAT, 1);
        return index == 1;
    }
}