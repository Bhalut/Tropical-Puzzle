using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DisplayLoadingProgress : MonoBehaviour
{
    private Image _circleOutline;

    private void Start ()
    {
        _circleOutline = GetComponent<Image>();

        // Animate the circle outline's color and fillAmount
        _circleOutline.DOColor(RandomColor(), 1.5f).SetEase(Ease.Linear).Pause();
        _circleOutline.DOFillAmount(0, 1.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo)
            .OnStepComplete(()=> {
                _circleOutline.fillClockwise = !_circleOutline.fillClockwise;
                _circleOutline.DOColor(RandomColor(), 1.5f).SetEase(Ease.Linear);
            })
            .Pause();

        DOTween.Play(_circleOutline);
    }

    private static Color RandomColor()
    {
        return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
    }
}
