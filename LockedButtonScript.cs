using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockedButtonScript : MonoBehaviour
{
    RectTransform rect;
    void OnEnable()
    {
        transform.parent.GetComponent<Button>().enabled = false;
        // rect = GetComponent<RectTransform>();
        // rect.sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;//new Vector2(transform.parent.GetComponent<RectTransform>().sizeDelta.x,transform.parent.GetComponent<RectTransform>().sizeDelta.y);
        // //transform.GetComponentInParent<Button>().interactable = false;
    }

}
