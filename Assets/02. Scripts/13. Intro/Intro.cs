using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    public CanvasGroup introCanvasGroup;
    public Image logoImage;

    public Sequence introSequence;
    public float fadeTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        IntroEffect();
    }

    public void IntroEffect()
    {
        introSequence = DOTween.Sequence()
            .Append(logoImage.DOFade(1f, fadeTime)) // show logo
            .AppendInterval(2f) // delay
            .Append(introCanvasGroup.DOFade(0f, fadeTime)) // hide canvas group
            .OnComplete(()=> 
            { 
                introCanvasGroup.gameObject.SetActive(false); 
            });  
    }
}
