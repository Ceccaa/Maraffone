using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameObject[] gHand;
    [SerializeField] Card[] hand;
	[SerializeField] static public BoxCollider2D playBox;
	public GameObject cardPrefab;
	public Transform pivot;
	public int playerIndex = 0;
	int nCards = 10;
	public bool isBot = false;
	public int punteggio;

	[Header("Hand Aesthetic")]
	[SerializeField] float minTurn;
	[SerializeField] float maxTurn;
	[SerializeField] float handGap;
	[SerializeField] float turnSpeed;
	[SerializeField] float turnFallOff;

	[Header("debug")]
	[SerializeField] float curSpeed;
	float lAmount = 0;

	private void Start()
	{
		punteggio = 0;
		if (playBox == null)
			playBox = CardSmister.cs.playBox;
	}

	public void SetHand(string[] stack)
	{
		Array.Sort(stack, playerIndex*10, 10);
		gHand = new GameObject[10];
		hand = new Card[10];
		nCards = 10;
		for (int i = 0; i < 10; i++)
		{
			gHand[i] = Instantiate(cardPrefab, new Vector3(transform.position.x, transform.position.y, i), Quaternion.identity, pivot);
			if(!isBot) gHand[i].GetComponent<SpriteRenderer>().sortingOrder = 10 - i;
			if(!isBot) pivot.Rotate(new Vector3(0,0, handGap));
		}
		if (!isBot) pivot.Rotate(new Vector3(0, 0, handGap*-5.5f));
		for (int i = 0; i < 10; i++)
		{
			hand[i] = gHand[i].GetComponent<Card>();
			hand[i].playedBy = playerIndex;
			if (isBot)
			{
				Destroy(gHand[i].GetComponent<BoxCollider2D>());
			}
		}
		for (int i = 0; i < 10; i++)
		{
			hand[i].seed = stack[i + playerIndex * 10][0];
			hand[i].value = stack[i + playerIndex * 10][1];
			hand[i].value -= 48;
			hand[i].effectiveValue = hand[i].value;
			if (hand[i].value < 3) hand[i].effectiveValue += 10;
			hand[i].idx = i;
			if (!isBot) hand[i].InitSprite();
		}
	}

	public char GetBotSeed()
	{
		return GetComponent<BotAI>().ChooseSeed(hand);
	}

	public bool TriggerTurn()
	{
		if (nCards <= 0)
			return true;
		char seed = CardSmister.cs.handSeed;
		bool hasSeed = false;

		//controlla se ha il seme della mano
		foreach (Card c in hand)
		{
			if (c == null) continue;
			c.playable = true;
			c.GetComponent<SpriteRenderer>().color = Color.white;
		}
		for (int i = 0; i < 10; i++)
		{
			if(hand[i] != null)
			{
				if(hand[i].seed == seed)
				{
					hasSeed = true;
					break;
				}
			}
		}
		if (hasSeed)
		{
			foreach(Card c in hand)
			{
				if (c == null) continue;
				if (c.seed != seed)
				{
					c.playable = false;
					c.GetComponent<SpriteRenderer>().color = Color.grey;
				}
			}
		}

		if (isBot)
		{
			PlayedCard(GetComponent<BotAI>().AIGuess(hand));
		}
		return false;
	}

	public void PlayedCard(int idx)
	{
		if (CardSmister.cs.curTurn != playerIndex || !hand[idx].playable) return;

		if (playBox == null)
			playBox = CardSmister.cs.playBox;

		gHand[idx].transform.parent = playBox.transform;
		gHand[idx].transform.localPosition = Vector3.zero;
		gHand[idx].transform.rotation = Quaternion.Euler(0, 0, 0);
		Destroy(gHand[idx].GetComponent<BoxCollider2D>());
		gHand[idx] = null;
		Card playing = hand[idx];
		hand[idx] = null;
		nCards--;
		CardSmister.cs.Play(playing);

		if (isBot) return;

		for (int i = idx+1; i < 10; i++)
		{
			if (gHand[i] == null)
				continue;
			gHand[i].transform.parent = null;
		}
		pivot.Rotate(new Vector3(0, 0, -handGap));
		for (int i = idx + 1; i < 10; i++)
		{
			if (gHand[i] == null)
				continue;
			gHand[i].transform.parent = pivot;
		}

		pivot.Rotate(new Vector3(0, 0, pivot.rotation.eulerAngles.z*-1));
		pivot.Rotate(new Vector3(0, 0, (nCards-1) * (handGap/2)));
		maxTurn -= handGap;
	}

	public bool Turn(float amount)
	{
		if(isBot) return true;
		
		if (nCards <= 4)
			return true;
		lAmount = amount;
		bool stop = false;
		pivot.Rotate(new Vector3(0, 0, amount * turnSpeed));
		if(minTurn < 0)
		{
			if (pivot.rotation.eulerAngles.z < (360 + minTurn) % 360 && pivot.rotation.eulerAngles.z > 270)
			{
				pivot.eulerAngles = new Vector3(0, 0, minTurn);
				stop = true;
			}
		}
		else if (pivot.rotation.eulerAngles.z < (360 + minTurn) % 360)
		{
			pivot.eulerAngles = new Vector3(0, 0, minTurn);
			stop = true;
		}

		else if (pivot.rotation.eulerAngles.z > maxTurn && pivot.rotation.eulerAngles.z < 180)
		{
			pivot.eulerAngles = new Vector3(0, 0, maxTurn);
			stop = true;
		}

		return stop;
	}

	public void SoftTurn()
	{
		curSpeed = lAmount;
	}

	public void StopTurn()
	{
		curSpeed = 0;
	}

	private void Update()
	{
		if (isBot) return;

		if(lAmount > 0 && curSpeed < 0.01)
		{
			StopTurn();
			return;
		}
		else if (lAmount < 0 && curSpeed > -0.01)
		{
			StopTurn();
			return;
		}
		
		curSpeed -= curSpeed * turnFallOff * Time.deltaTime;
		if (Turn(curSpeed))
		{
			StopTurn();
		}
	}
}
