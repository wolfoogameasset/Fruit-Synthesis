using SCN.FirebaseLib.FA;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

public class GAManagerCustom : MonoBehaviour
{
    #region IAP Tracking
    public enum IAP_location
    {
        start_game,
        home,
        play_game,
    }
    public enum IAP_type
    {
        monthly_plan,
        annual_plan,
        unlock_for_life_plan,
    }

    public static void IAP_confirmClick(IAP_location location)
    {
        //GameManager.Instance.IAP_Location = location;
        GAManager.Instance.LogEvent("iap_confirm_click", new Parameter("location", location.ToString()));
        Debug.Log("iap_confirm_click" + " /location: " + location.ToString());
    }
    public static void IAP_confirmSuccess(IAP_location location)
    {
        GAManager.Instance.LogEvent("iap_confirm_success", new Parameter("location", location.ToString()));
        Debug.Log("iap_confirm_success" + " /location: " + location.ToString());
    }
    public static void IAP_click(IAP_location location, IAP_type type, float value, string currency)
    {
        GAManager.Instance.LogEvent("iap_click",
            new Parameter("location", location.ToString()),
            new Parameter("type", type.ToString()),
            new Parameter("value", value),
            new Parameter("currency", currency)
            );

        Debug.Log("iap_click" + " /location: " + location.ToString() + " /type: " + type.ToString() + " /value: " + value + " /currency: " + currency);
    }
    public static void IAP_success(IAP_location location, IAP_type type, float value, string currency)
    {
        GAManager.Instance.LogEvent("iap_success",
            new Parameter("location", location.ToString()),
            new Parameter("type", type.ToString()),
            new Parameter("value", value),
            new Parameter("currency", currency)
            );
        Debug.Log("iap_success" + " /location: " + location.ToString() + " /type: " + type.ToString() + " /value: " + value + " /currency: " + currency);

    }
    public static void IAP_failed(IAP_location location, string name_error, IAP_type type, float value, string currency)
    {
        GAManager.Instance.LogEvent("iap_failed",
            new Parameter("location", location.ToString()),
            new Parameter("name_error", name_error),
            new Parameter("type", type.ToString()),
            new Parameter("value", value),
            new Parameter("currency", currency)
            );
        Debug.Log("iap_failed" + " /location: " + location.ToString() + " /name_error: " + name_error + " /type: " + type.ToString() + " /value: " + value + " /currency: " + currency);
    }
    #endregion

    #region Ads Tracking
    //Inter
    public enum Ad_Product_minigame
    {
        mini_game_floods,
        mini_game_piggy,
        mini_game_apartment_fire,
        mini_game_factory_fire,
        mini_game_stuck_tree,
        mini_game_mine_collapse,
    }
    #region Inter
    public static void Ad_inter_request()
    {
        GAManager.Instance.LogEvent("ad_inter_request");
    }
    public static void Ad_inter_impress()
    {
        GAManager.Instance.LogEvent("ad_inter_impress");
    }
    public static void Ad_inter_click(bool is_woa_ads, string url_woa_ads)
    {
        GAManager.Instance.LogEvent("ad_inter_click",
            new Parameter("is_woa_ads", is_woa_ads.ToString()),
            new Parameter("url_woa_ads", url_woa_ads)
            );
        Debug.Log("ad_inter_click" + " /is_woa_ads: " + is_woa_ads.ToString() + " /url_woa_ads: " + url_woa_ads);
    }
    public static void Ad_inter_success(Ad_Product_minigame location, bool is_woa_ads, string url_woa_ads)
    {
        GAManager.Instance.LogEvent("ad_inter_success",
            new Parameter("location", location.ToString()),
            new Parameter("is_woa_ads", is_woa_ads.ToString()),
            new Parameter("url_woa_ads", url_woa_ads)
            );

        Debug.Log("ad_inter_success" + " /location: " + location.ToString() + " /is_woa_ads: " + is_woa_ads.ToString() + " /url_woa_ads: " + url_woa_ads);
    }
    public static void Ad_inter_fail(Ad_Product_minigame location, bool is_woa_ads, string url_woa_ads)
    {
        GAManager.Instance.LogEvent("ad_inter_fail",
            new Parameter("location", location.ToString()),
            new Parameter("is_woa_ads", is_woa_ads.ToString()),
            new Parameter("url_woa_ads", url_woa_ads)
            );
        Debug.Log("ad_inter_fail" + " /location: " + location.ToString() + " /is_woa_ads: " + is_woa_ads.ToString() + " /url_woa_ads: " + url_woa_ads);
    }
    #endregion

    #region Banner
    public static void Ad_banner_request()
    {
        GAManager.Instance.LogEvent("ad_banner_request");
    }
    public static void Ad_banner_impress()
    {
        GAManager.Instance.LogEvent("ad_banner_impress");
    }
    public static void Ad_banner_click()
    {
        GAManager.Instance.LogEvent("ad_banner_click");
    }
    #endregion

    #region Reward
    public static void Ad_rv_request()
    {
        GAManager.Instance.LogEvent("ad_rv_request");
    }
    public static void Ad_rv_impress()
    {
        GAManager.Instance.LogEvent("ad_rv_impress");
    }
    public static void Ad_rv_click(bool is_woa_ads, string url_woa_ads)
    {
        GAManager.Instance.LogEvent("ad_rv_click",
            new Parameter("is_woa_ads", is_woa_ads.ToString()),
            new Parameter("url_woa_ads", url_woa_ads)
            );
        Debug.Log("ad_rv_click" + " /is_woa_ads: " + is_woa_ads.ToString() + " /url_woa_ads: " + url_woa_ads);
    }
    public static void Ad_rv_success(Ad_Product_minigame location, bool is_woa_ads, string url_woa_ads)
    {
        GAManager.Instance.LogEvent("ad_rv_success",
            new Parameter("location", location.ToString()),
            new Parameter("is_woa_ads", is_woa_ads.ToString()),
            new Parameter("url_woa_ads", url_woa_ads)
            );

        Debug.Log("ad_rv_success" + " /location: " + location.ToString() + " /is_woa_ads: " + is_woa_ads.ToString() + " /url_woa_ads: " + url_woa_ads);
    }
    public static void Ad_rv_fail(Ad_Product_minigame location, bool is_woa_ads, string url_woa_ads)
    {
        GAManager.Instance.LogEvent("ad_rv_fail",
            new Parameter("location", location.ToString()),
            new Parameter("is_woa_ads", is_woa_ads.ToString()),
            new Parameter("url_woa_ads", url_woa_ads)
            );
        Debug.Log("ad_rv_fail" + " /location: " + location.ToString() + " /is_woa_ads: " + is_woa_ads.ToString() + " /url_woa_ads: " + url_woa_ads);
    }
    #endregion
    #region Native
    public static void Ad_native_request()
    {
        GAManager.Instance.LogEvent("ad_native_request");
    }
    public static void Ad_native_impress(string location, bool is_woa_ads, string game_woa_ads)
    {
        GAManager.Instance.LogEvent("ad_native_impress",
            new Parameter("location", location.ToString()),
            new Parameter("is_woa_ads", is_woa_ads.ToString()),
            new Parameter("game_woa_ads", game_woa_ads)
            );
    }
    public static void Ad_native_click(string location, bool is_woa_ads, string game_woa_ads)
    {
        GAManager.Instance.LogEvent("ad_native_click",
            new Parameter("location", location.ToString()),
            new Parameter("is_woa_ads", is_woa_ads.ToString()),
            new Parameter("game_woa_ads", game_woa_ads)
            );
    }
    #endregion
    #endregion

    #region Product

    public enum Level_result
    {
        win, lose, skip, quit
    }
    public static void Level_start(int current_gold, int map_id, int level_id)
    {
        GAManager.Instance.LogEvent("level_start",
            new Parameter("current_gold", current_gold.ToString()),
            new Parameter("map_id", map_id.ToString()),
            new Parameter("level_id", level_id.ToString())
            );
        Debug.Log("level_start" + " /current_gold: " + current_gold.ToString() + " /map_id: " + map_id.ToString()+ " /level_id: " + level_id.ToString());
    }
    public static void Level_end(Level_result status, int map_id,int level_id, float time_play, int failcount)
    {
        GAManager.Instance.LogEvent("level_end",
            new Parameter("status", status.ToString()),
            new Parameter("map_id", map_id.ToString()),
            new Parameter("level_id", level_id.ToString()),
            new Parameter("time_play", time_play.ToString()),
            new Parameter("failcount", failcount.ToString())
            );
        Debug.Log("level_end" + " /status: " + status.ToString() + " /map_id: " + map_id.ToString()+ " /level_id: " + level_id.ToString() + " /time_play: " + time_play.ToString() + " /failcount: " + failcount.ToString());
    }
    public static void Level_win(float timeplayed, int total_coin_earned)
    {
        GAManager.Instance.LogEvent("level_complete",
            new Parameter("timeplayed", timeplayed.ToString()),
            new Parameter("total_coin_earned", total_coin_earned.ToString())
            );
        Debug.Log("level_complete"  + " /time_play: " + timeplayed.ToString() + " /total_coin_earned: " + total_coin_earned.ToString());
    }
    public static void Open_app()
    {
        GAManager.Instance.LogEvent("open_app");
        Debug.Log("open_app");
    }
    public static void Currency_earn(string virtual_currency_name, long value, string source)
    {
        GAManager.Instance.LogEvent("earn_virtual_currency",
            new Parameter("virtual_currency_name", virtual_currency_name.ToString()),
            new Parameter("value", value),
            new Parameter("source", source.ToString())
            );
        Debug.Log("earn_virtual_currency" + " /virtual_currency_name: " + virtual_currency_name.ToString() + " /value: " + value.ToString() + " /source: " + source.ToString());
    }
    public static void Currency_spend(string virtual_currency_name, long value, string item_name)
    {
        GAManager.Instance.LogEvent("spend_virtual_currency",
            new Parameter("virtual_currency_name", virtual_currency_name.ToString()),
            new Parameter("value", value),
            new Parameter("item_name", item_name.ToString())
            );
        Debug.Log("earn_virtual_currency" + " /virtual_currency_name: " + virtual_currency_name.ToString() + " /value: " + value.ToString() + " /source: " + item_name.ToString());
    }
    public static void ActiveSkill(string skill_id)
    {
        GAManager.Instance.LogEvent("skill",
            new Parameter("skill_id", skill_id.ToString())
            );
        Debug.Log("skill_id" + " /skill_id: "+ skill_id.ToString());
    }
    #endregion
}
