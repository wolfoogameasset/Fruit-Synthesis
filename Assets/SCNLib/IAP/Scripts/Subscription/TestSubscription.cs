using SCN.IAP;
using UnityEngine;

public class TestSubscription : MonoBehaviour
{
    //luu y, subscription ko test duoc tren editor fake buy IAP ma phai can co sub that va su dung ban APK tai ve tu internal test,... tren google play consle ve

    private void Awake()
    {
        IAPManager.OnFinishCheckSubscriptionStatus += UpdateStatus;
    }

    private void OnDestroy()
    {
        IAPManager.OnFinishCheckSubscriptionStatus -= UpdateStatus;
    }

    //kiem tra trang thai subscription moi khi mo game len de khoa bot noi dung lai neu ko sub nua/ hoac dung de mo khoa ngay khi khach hang mua sub
    void UpdateStatus(bool isSubcribed)
    {
        if (isSubcribed)
        {
            //mo khoa noi dung subscription
            Debug.Log("Unlock Subscription contents");
        }
        else
        {
            //khoa cac noi dung subscription
            Debug.Log("Lock Subscription contents");
        }
    }

    //co the dung bool IAPManager.Instance.IsSubscribed de check da sub chua (da kiem tra luc mo game)
    void OnStartGame()
    {
        if (IAPManager.Instance.IsSubscribed)
        {
            //su dung noi dung da mo khoa sub
            //vi du ko show ads
        }
        else
        {
            //su dung noi dung chua sub
            //vi du show ads
        }

        //VD neu muon goi check Sub dau scene de kiem tra lai trang thai Sub
        IAPManager.Instance.CheckSubscriptionStatus();
    }

    //Code goi man hinh popup subscription
    void ShowSubscriptionPopup()
    {
        ParentGateManager.Instance.OpenDialog(() =>
        {
            IAPManager.Instance.OpenSubscriptionPanel();
        });
    }
}
