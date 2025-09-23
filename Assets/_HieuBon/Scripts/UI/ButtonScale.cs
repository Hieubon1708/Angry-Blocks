using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HieuBon
{
    public class ButtonScale : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        Vector3 startScale;

        void Awake()
        {
            startScale = transform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(startScale * 0.95f, 0.1f).SetEase(Ease.Linear).SetUpdate(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.DOKill();
            transform.DOScale(startScale, 0.1f).SetEase(Ease.Linear).SetUpdate(true);
        }
    }
}