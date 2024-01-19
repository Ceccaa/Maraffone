using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSpriteSelector : MonoBehaviour
{
	[SerializeField] Sprite[] list;
	static public CardSpriteSelector csp;

	private void Awake()
	{
		csp = this;
	}

	public Sprite GetSprite(char seed, int value)
	{
		int idx = value;
		switch (seed)
		{
			case 'b':
				break;
			case 'c':
				idx += 10;
				break;
			case 'd':
				idx += 20;
				break;
			case 's':
				idx += 30;
				break;
			default:
				return list[40];
		}
		return list[idx];
	}
}
