using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    //temp
    public int effectiveValue;
    public char seed;
    public int value;
    public int idx;
    public int playedBy;
    bool hide = false;
    public bool playable = true;
    [SerializeField] GameObject shadow;

    public void InitSprite()
    {
        SpriteRenderer sp = GetComponent<SpriteRenderer>();
        CardSpriteSelector csp = CardSpriteSelector.csp;
        sp.sprite = csp.GetSprite(seed, value);
        if (shadow != null) shadow.GetComponent<SpriteRenderer>().sortingOrder = sp.sortingOrder;
    }
}