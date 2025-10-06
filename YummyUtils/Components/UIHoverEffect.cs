using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverEffect : MonoBehaviour,IPointerEnterHandler, IPointerExitHandler
{

    public bool enableScale = true;
    public float enterDuration = 0.1f;
    public float exitDuration = 0.1f;
    public float scaleTimes = 1.1f;
    public GameObject hoverObj;
    float _stScalex;
    float _stScaley;
    //public AnimationCurve scaleCurve;
    IEnumerator<WaitForSeconds> DoScale(float startScale, float endScale,float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            var value = Mathf.Lerp(startScale, endScale, t / duration);
            //var value = scaleCurve.Evaluate(t / duration);
            transform.localScale = new Vector3(value * _stScalex, value * _stScaley, 0);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        if(enableScale)
            StartCoroutine(DoScale(1, scaleTimes, enterDuration));
        if(hoverObj!= null)
            hoverObj.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        if(enableScale)
            StartCoroutine(DoScale(scaleTimes,1,exitDuration));
        if(hoverObj!= null)
            hoverObj?.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        _stScalex = transform.localScale.x;
        _stScaley = transform.localScale.y;
       if(hoverObj!= null)
        hoverObj?.SetActive(false);
    }
    
}
