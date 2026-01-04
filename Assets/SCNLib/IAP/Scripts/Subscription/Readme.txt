Lưu ý subscription không thể test bằng popup mua IAP fake được mà phải thực hiện thông trên device thật, và sử dụng bản APK tải về từ internal test,... trên google play console
Code test

using SCN.IAP;
using UnityEngine;

public class TestSubscription : MonoBehaviour
{
    private void Awake()
    {
        IAPManager.OnFinishCheckSubscriptionStatus += UpdateStatus;
    }

    private void OnDestroy()
    {
        IAPManager.OnFinishCheckSubscriptionStatus -= UpdateStatus;
    }

    //kiểm tra trạng thái subscription mỗi khi mở game hoặc khi người dùng vừa mua sub để mở hoặc khóa nội dung lại nếu ko sub nữa
    void UpdateStatus(bool isSubcribed)
    {
        if (isSubcribed)
        {
            //mở khóa nội dung subscription
            Debug.Log("Unlock Subscription contents");
        }
        else
        {
            //khóa các nội dung subscription
            Debug.Log("Lock Subscribtion contents");
        }
    }

    //có thể dùng bool IAPManager.Instance.IsSubscribed để kiểm tra trạng thái sub
    void OnStartGame()
    {
        if (IAPManager.Instance.IsSubscribed)
        {
            //sử dụng nội dung đã mở khóa sub
	    //ví dụ ko show ads
        }
        else
        {
            //sử dụng nội dung chưa sub
            //ví dụ show ads
        }
        //VD neu muon goi check Sub dau scene de kiem tra lai trang thai Sub
        IAPManager.Instance.CheckSubscriptionStatus();
    }
    
    //Code gọi màn hình popup Subscription
    void ShowSubscriptionPopup()
    {
        ParentGateManager.Instance.OpenDialog(() =>
        {
            IAPManager.Instance.OpenSubscriptionPanel();
        });
    }
}

