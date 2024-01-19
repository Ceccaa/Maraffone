using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSmister : MonoBehaviour
{
    [SerializeField] string[] stack;
    [SerializeField] Player[] players;
    [SerializeField] GameObject botPrefab;
    [SerializeField] GameObject humanPrefab;
    public BoxCollider2D playBox;
    public static Player mainPlayer;
    public int curTurn = 0;
    public static CardSmister cs;
    public char handSeed;
    public int tempOrder;
    public int highestBid = -1;
    [SerializeField] int highestBidder;
    [SerializeField] Transform turnPointer;
    [Header("CardAnimation")]
    bool inAnimation = false;
    bool inTakeAnimation = false;
    Transform takeTarget;
    Card animCard;
    GameObject animHand;
    [SerializeField] float animationSpeed;
    [Header("Debug")]
    public char briscola = 'd';
    public bool inMenu = false;


    private void Awake()
	{
        cs = this;
        tempOrder = 0;
        highestBid = -1;
        highestBidder = -1;
    }

	void Start()
    {
        handSeed = 'z';
        players = new Player[4];
        players[0] = Instantiate(humanPrefab, new Vector3(0,-2.7f,0), Quaternion.identity).GetComponent<Player>();
        mainPlayer = players[0];
        for(int i = 1; i < 4; i++)
		{
            players[i] = Instantiate(botPrefab, new Vector3(0, -150, 0), Quaternion.identity).GetComponent<Player>();
            players[i].playerIndex = i;
        }
        players[1].transform.position = new Vector3(-4.5f, 1.5f, 0);
        players[2].transform.position = new Vector3(0, 6.5f, 0);
        players[3].transform.position = new Vector3(4.5f, 1.5f, 0);

        stack = new string[40];
        for(int i = 0; i < 10; i++)
		{
            stack[i] = "b";
            stack[i] += i;
            stack[i+10] = "d";
            stack[i+10] += i;
            stack[i+20] = "s";
            stack[i+20] += i;
            stack[i+30] = "c";
            stack[i+30] += i;
        }

        GiveCards();

		if (curTurn == mainPlayer.playerIndex)
        {
            GetComponent<MenuVisualizer>().ShowBriscolaMenu();
		}
		else
		{
            briscola = players[curTurn].GetBotSeed();
            GetComponent<MenuVisualizer>().UpdateBriscolaIcon();
        }

        turnPointer.rotation =  Quaternion.Euler(0,0,-90*curTurn);

        StartGame();
        //StartCoroutine(CommunicateSeed());
    }

    public void ShowPlayedMenu()
	{
        inMenu = true;
        GetComponent<MenuVisualizer>().ShowPlayedMenu();
    }

    public void HidePlayedMenu()
    {
        if (!GetComponent<MenuVisualizer>().playedView.activeSelf) return;
        inMenu = false;
        GetComponent<MenuVisualizer>().HidePlayedMenu();
    }

    IEnumerator CommunicateSeed()
	{
        string seme;
		switch (briscola)
		{
            case 'b':
                seme = "Bastoni";
                break;
            case 'c':
                seme = "Coppe";
                break;
            case 'd':
                seme = "Denare";
                break;
            case 's':
                seme = "Spade";
                break;
            default:
                seme = "Bastoni";
                break;
        }
        Camera.main.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = $"La briscola è {seme}";
        yield return new WaitForSeconds(2.5f);
        Camera.main.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "";
        StartGame();
    }

    public void StartGame()
	{
        players[curTurn].TriggerTurn();
	}

    void GiveCards()
	{
        int n = stack.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            string value = stack[k];
            stack[k] = stack[n];
            stack[n] = value;
        }

        for (int i = 0; i < 40; i++)
        {
            if (stack[i] == "d3")
            {
                curTurn = i / 10;
                break;
            }
        }

        foreach (Player p in players)
        {
            p.SetHand(stack);
        }
    }

    public void Play(Card card)
    {
        if (handSeed == 'z')
		{
            handSeed = card.seed;
            GetComponent<MenuVisualizer>().UpdateHandIcon();
        }
            
        int val = card.value;
        if (val < 3)
            val += 10;
        if (card.seed == briscola)
		{
            if(highestBid < val * 10000)
			{
                highestBid = val * 10000;
                highestBidder = curTurn;
            }
        }
        else if(card.seed == handSeed)
		{
            if (highestBid < val * 100)
            {
                highestBid = val * 100;
                highestBidder = curTurn;
            }
        }
		else
		{
            if (highestBid < val)
            {
                highestBid = val;
                highestBidder = curTurn;
            }
        }

        var render = card.GetComponent<SpriteRenderer>();
        render.sortingOrder = -100 + tempOrder;
        tempOrder++;
        card.transform.rotation = Quaternion.Euler(card.transform.rotation.x, card.transform.rotation.y, -curTurn * 90 + Random.Range(-15f, 15f));

        if (players[curTurn].isBot)
		{
            card.transform.position = players[curTurn].transform.position;
            animCard = card;
            animCard.InitSprite();
            
            inAnimation = true;
		}
		else
		{
            NextTurn();
		}
	}

    void NextTurn()
	{
        if (tempOrder == 4)
        {
            tempOrder = 0;
            handSeed = 'z';
            GetComponent<MenuVisualizer>().UpdateHandIcon();
            players[highestBidder].punteggio += CalcolaPunteggio();
            animHand = new GameObject();
            animHand.transform.position = playBox.transform.position;
            foreach (Transform child in playBox.GetComponentsInChildren<Transform>())
            {
                if (child == playBox.transform) continue;
                child.parent = animHand.transform;
            }
            inTakeAnimation = true;
            takeTarget = players[highestBidder].transform;
            highestBid = -1;
            curTurn = highestBidder - 1;
            highestBidder = -1;
            return;
        }
        curTurn = ++curTurn % 4;
        //turnPointer.rotation = Quaternion.Euler(0, 0, -90 * curTurn);
        StopCoroutine("turnPoint");
        StartCoroutine("turnPoint");
        bool stop = players[curTurn].TriggerTurn();
        if (stop)
        {
            Debug.Log("That's all folks");
            GameEnd();
            tempOrder = 0;
        }
    }

    public IEnumerator turnPoint()
	{
        while ((int)turnPointer.rotation.eulerAngles.z != -90 * curTurn)
		{
            turnPointer.rotation = Quaternion.Euler(0, 0, (int)Mathf.Round(Mathf.Lerp(turnPointer.rotation.eulerAngles.z, 360-(90 * curTurn), 4f * Time.deltaTime)));
            yield return null;
        }
	}

    public int CalcolaPunteggio()
	{
        int tot = 0;
        Card[] played = new Card[4];
        int i1 = 0;
        foreach (Card c in playBox.transform.GetComponentsInChildren<Card>())
        {
            played[i1++] = c;
        }

        for(int i = 0; i < 4; i++)
		{
            if (played[i] == null) continue;
            if (played[i].value == 0) tot += 3;
            else if (played[i].value < 3 || played[i].value > 6) tot++;
		}

        return tot;
    }

    void GameEnd()
	{
        int maxP = (players[0].punteggio + players[2].punteggio)/3;
        int winner = 0;
        if(maxP < (players[1].punteggio + players[3].punteggio)/3) winner = 1;
        if((mainPlayer.playerIndex-winner)%2 == 0)
		{
            Camera.main.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "La tua squadra vince!";
        }
		else
		{
            Camera.main.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "La squadra avversaria vince!";
        }
    }

	private void Update()
	{
		if (inAnimation)
		{
            animCard.transform.position = Vector3.Lerp(animCard.transform.position, playBox.transform.position, animationSpeed*Time.deltaTime);
            if((animCard.transform.position - playBox.transform.position).magnitude <= 0.02f)
			{
                animCard.transform.position = playBox.transform.position;
                inAnimation = false;
                NextTurn();
			}
		}
        else if (inTakeAnimation)
		{
            Vector2 trgt = takeTarget.position;
            if(takeTarget == mainPlayer.transform) trgt += new Vector2(0, -4f);
            animHand.transform.position = Vector3.Lerp(animHand.transform.position, trgt, animationSpeed * Time.deltaTime);
            if ((animHand.transform.position - new Vector3(trgt.x, trgt.y)).magnitude <= 0.02f)
            {
                Destroy(animHand);
                inTakeAnimation = false;
                NextTurn();
            }
        }
	}
}
