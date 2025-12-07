using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject hintArrow;
    public float arrowFloatDistance = 0.4f;
    public float arrowFloatingDuration = 1.0f;

    private void Awake()
    {
        hintArrow.SetActive(false);
    }

    public void ShowHintArrow(bool show)
    {
        hintArrow.SetActive(show);

        if (show)
        {
            hintArrow.transform.DOLocalMoveY(
                hintArrow.transform.localPosition.y + arrowFloatDistance, arrowFloatingDuration
            ).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public void AdjustArrowDirection(Vector2Int direction)
    {
        hintArrow.transform.rotation = Utils.GetHintArrowAngle(direction);
    }
}