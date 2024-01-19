using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchBehaviour : MonoBehaviour
{
	public GameObject card;
	public Card cd;
	Collider2D col;
	public Touch touch;
	public Text text;
	public bool c;
	bool moving = false;
	Camera cam;
	[SerializeField] BoxCollider2D playBox;
	[SerializeField] Player player;

	Vector3 orPos;
	Quaternion orRotation;
	Vector2 orScale;

	Vector2 lastTouchPos;

	void Start()
    {
		//Application.targetFrameRate = 30;
		cam = Camera.main;
		Input.multiTouchEnabled = false;
		lastTouchPos = new Vector2();
    }

	private void Update()
	{
		if(player == null)
		{
			player = CardSmister.mainPlayer;
		}
		if(playBox == null)
		{
			playBox = CardSmister.cs.playBox;
		}

		if (Input.touchCount > 0)
		{
			touch = Input.GetTouch(0);
			Vector2 pos = cam.ScreenToWorldPoint(touch.position);

			if(touch.phase == TouchPhase.Began)
			{
				player.StopTurn();

				lastTouchPos = new Vector2();

				if (CardSmister.cs.inMenu) return;

				col = null;
				var a = Physics2D.OverlapPointAll(pos);
				foreach(Collider2D c in a)
				{
					if(c.gameObject == playBox.gameObject && CardSmister.cs.inMenu == false)
					{
						CardSmister.cs.ShowPlayedMenu();
					}
					if(c.GetComponent<Card>() != null)
					{
						col = c;
						cd = col.gameObject.GetComponent<Card>();
						break;
					}
				}
				
				if (col != null && cd != null && cd.playable)
				{
					card = col.gameObject;
					orPos = card.transform.position;
					orScale = card.transform.localScale;
					orRotation = card.transform.rotation;
					card.transform.rotation = Quaternion.Euler(0,0,0);
					card.transform.localScale = new Vector2(orScale.x*1.5f, orScale.y * 1.5f);
				}
				else
				{
					col = null;
				}	
			}
			else if(touch.phase == TouchPhase.Moved)
			{
				moving = true;
				if (col != null)
				{
					col.transform.position = pos;
					col.transform.position = new Vector3(col.transform.position.x, col.transform.position.y, orPos.z);
				}
				else
				{
					player.Turn(lastTouchPos.x - pos.x);
				}
			}
			else if (touch.phase == TouchPhase.Stationary)
			{
				moving = false;
			}
			else if(touch.phase == TouchPhase.Ended)
			{
				if (CardSmister.cs.inMenu == true)
				{
					CardSmister.cs.HidePlayedMenu();
				}
				else if (moving)
				{
					player.SoftTurn();
				}
				if (card == null)
				{
					return;
				}
				card.transform.localScale = orScale;
				card.transform.position = orPos;
				if (CardSmister.cs.inMenu) return;
				if (card.GetComponent<BoxCollider2D>().IsTouching(playBox) && player.playerIndex == CardSmister.cs.curTurn)
				{
					player.PlayedCard(cd.idx);
				}
				else
				{
					card.transform.rotation = orRotation;
				}
				
				card = null;
			}
			lastTouchPos = pos;
		}
		//text.text = card.transform.position.x + "; " + card.transform.position.y + "; " + card.transform.position.z;
	}
}
