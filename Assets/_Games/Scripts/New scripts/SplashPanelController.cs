using DG.Tweening;
using UnityEngine;

namespace SCN.FruitSynthesis
{
    public class SplashPanelController : MonoBehaviour
    {
        [SerializeField] Transform catTrans;
        [SerializeField] Transform startPos;
        [SerializeField] Transform endPos;
        void Start()
        {
            gameObject.AddComponent<RectTransform>();
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(1f, 1f);

            rectTransform.offsetMin = new Vector2(0f, 0f);
            rectTransform.offsetMax = new Vector2(0f, 0f);

            rectTransform.pivot = new Vector2(0.5f, 0.5f);

            catTrans.DOMove(startPos.position, 0).OnComplete(() =>
            {
                catTrans.DOMoveX(endPos.position.x, 1.8f).SetEase(Ease.Linear);
            });
        }
    }
}