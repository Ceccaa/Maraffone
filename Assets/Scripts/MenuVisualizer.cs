using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuVisualizer : MonoBehaviour
{
    //seed select menu
    [SerializeField] GameObject briscolaSelect;
    //visualize played cards menu
    public GameObject playedView;
    [SerializeField] Image[] cards;
    //seed icons
    [SerializeField] Sprite[] icons;
    [SerializeField] RawImage briscolaSeed;
    [SerializeField] RawImage handSeed;

	private void Awake()
	{
        briscolaSeed.texture = icons[4].texture;
        handSeed.texture = icons[4].texture;
    }

	public void ShowBriscolaMenu()
    {
        briscolaSelect.SetActive(true);
        CardSmister.cs.inMenu = true;
    }

    public void SelectBriscolaSeed(string seed)
	{
        briscolaSelect.SetActive(false);
        CardSmister.cs.briscola = seed[0];
        CardSmister.cs.inMenu = false;
        UpdateBriscolaIcon();
    }

    public void ShowPlayedMenu()
    {
        int i = 0;
        foreach (Image j in cards) j.gameObject.SetActive(false);
        if (CardSmister.cs.tempOrder != 0)
        {
            foreach (Card c in CardSmister.cs.playBox.transform.GetComponentsInChildren<Card>())
            {
                cards[i].gameObject.SetActive(true);
                cards[i++].sprite = c.GetComponent<SpriteRenderer>().sprite;
            }
        }

        playedView.SetActive(true);
    }

    public void HidePlayedMenu()
    {
        playedView.SetActive(false);
    }

    public void UpdateBriscolaIcon()
	{
        var selSeed = CardSmister.cs.briscola switch
        {
            'b' => 0,
            'c' => 1,
            'd' => 2,
            's' => 3,
            _ => 4,
        };
        briscolaSeed.texture = icons[selSeed].texture;
    }

    public void UpdateHandIcon()
    {
        var selSeed = CardSmister.cs.handSeed switch
        {
            'b' => 0,
            'c' => 1,
            'd' => 2,
            's' => 3,
            _ => 4,
        };
        handSeed.texture = icons[selSeed].texture;
    }
}
