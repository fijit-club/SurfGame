using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiTweens : MonoBehaviour
{
    private void Start()
    {
        LeanTween.scale(this.gameObject, new Vector3(1.1f, 1.1f, 1.1f), 0.75f).setDelay(0.5f).setEase(LeanTweenType.easeOutElastic);
    }
}
