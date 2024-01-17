using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuVisualizer : MonoBehaviour
{
    [SerializeField] GameObject briscolaSelect;

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
    }
}
