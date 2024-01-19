using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotAI : MonoBehaviour
{
    public enum BotDifficulty { Easy, Medium, Hard };
    public BotDifficulty difficulty;

    public int AIGuess(Card[] hand)
    {
        Card[] played = new Card[3];
        int i1 = 0;
        if (CardSmister.cs.tempOrder != 0)
        {
            foreach (Card c in CardSmister.cs.playBox.transform.GetComponentsInChildren<Card>())
            {
                played[i1++] = c;
            }
        }

        int feedLength = 0;
        foreach (Card c in hand)
        {
            if (c != null && c.playable) feedLength++;
        }
        Card[] feed = new Card[feedLength];
        int[] feedPos = new int[feedLength];
        feedLength = 0;
        for (int i = 0; i < 10; i++)
        {
            if (hand[i] != null && hand[i].playable)
            {
                feed[feedLength] = hand[i];
                feedPos[feedLength] = i;
                feedLength++;
            }
        }

        var guess = difficulty switch
        {
            BotDifficulty.Easy => Random.Range(0, feed.Length),
            BotDifficulty.Medium => PredictCard(feed),
            BotDifficulty.Hard => Random.Range(0, feed.Length),
            _ => Random.Range(0, feed.Length),
        };
        Debug.Log(guess);
        return feedPos[guess];
    }



    public int PredictCard(Card[] hand)
    {
        //DA IMPLEMENTARE LOWEST CARD INDEX E BEST CARD INDEX, QUANDO SI PUò GIOCARE QUALSIASI SEME PERCHè SI VOLA.


        int guess = -1;
        int lowestValue = int.MaxValue;
        int highestValue = -1;
        int bestCardIndex = -1;
        int lowestCardIndex = -1;
        int pointer = 0;
        int convinientCardIndex = 0;
        int convinientCardValue = 0;
        Card[] cardsOnTable = CardSmister.cs.playBox.GetComponentsInChildren<Card>();


        //order cardsontable
        foreach (Card c in cardsOnTable)
        {
            if (c.playedBy > GetComponent<Player>().playerIndex) c.playedBy -= 4;
        }
        if (cardsOnTable.Length > 1)
        {
            if (cardsOnTable[0].playedBy > cardsOnTable[1].playedBy)
            {
                Card temp = cardsOnTable[0];
                cardsOnTable[0] = cardsOnTable[1];
                cardsOnTable[1] = temp;

            }

            if (cardsOnTable.Length > 2 && cardsOnTable[1].playedBy > cardsOnTable[2].playedBy)
            {
                Card temp = cardsOnTable[1];
                cardsOnTable[1] = cardsOnTable[2];
                cardsOnTable[2] = temp;

            }

            if (cardsOnTable[0].playedBy > cardsOnTable[1].playedBy)
            {
                Card temp = cardsOnTable[0];
                cardsOnTable[0] = cardsOnTable[1];
                cardsOnTable[1] = temp;

            }
        }

        foreach (Card c in cardsOnTable)
        {
            if (c.playedBy < 0) c.playedBy += 4;
        }



        if (cardsOnTable.Length > 0)
        {

            //calcola carta piu alta e bassa in mano
            for (int j = 0; j < hand.Length; j++)
            {
                if (hand[j] != null && hand[j].playable)
                {
                    if (hand[j].effectiveValue > highestValue)
                    {
                        highestValue = hand[j].effectiveValue;
                        bestCardIndex = j;
                        pointer = j;
                    }
                    if (hand[j].effectiveValue < lowestValue)
                    {
                        lowestValue = hand[j].effectiveValue;
                        lowestCardIndex = j;
                    }
                    if (hand[j].effectiveValue >= 3 && hand[j].effectiveValue <= 6 && hand[j].seed != CardSmister.cs.briscola)
                    {
                        convinientCardValue = hand[j].effectiveValue;
                        convinientCardIndex = j;
                    }

                }
            }


            if (cardsOnTable.Length == 3)
            {
                Debug.Log("ramo 1/3");
                if (cardsOnTable[1].effectiveValue > cardsOnTable[0].effectiveValue && cardsOnTable[1].effectiveValue > cardsOnTable[2].effectiveValue && (cardsOnTable[0].seed != CardSmister.cs.briscola || cardsOnTable[2].seed != CardSmister.cs.briscola))
                {
                    guess = bestCardIndex;
                }
                else
                {
                    Debug.Log("ramo 2/3");
                    if (cardsOnTable[1].effectiveValue > cardsOnTable[0].effectiveValue || cardsOnTable[1].effectiveValue > cardsOnTable[2].effectiveValue && cardsOnTable[1].seed != CardSmister.cs.briscola && (cardsOnTable[0].seed == CardSmister.cs.briscola || cardsOnTable[2].seed == CardSmister.cs.briscola))
                    {
                        guess = lowestCardIndex; Debug.Log("a");// se è maggiore ma un avversario briscola
                    }
                    else if (cardsOnTable[1].effectiveValue > cardsOnTable[0].effectiveValue && cardsOnTable[1].effectiveValue > cardsOnTable[2].effectiveValue && cardsOnTable[1].seed == CardSmister.cs.briscola && (cardsOnTable[0].seed == CardSmister.cs.briscola || cardsOnTable[2].seed == CardSmister.cs.briscola))
                    {
                        guess = bestCardIndex; Debug.Log("b"); //se è maggiore (con briscola) e uno degli avversari briscola
                    }
                    else if ((cardsOnTable[1].effectiveValue < cardsOnTable[0].effectiveValue || cardsOnTable[1].effectiveValue < cardsOnTable[2].effectiveValue) && cardsOnTable[1].seed == CardSmister.cs.briscola && (cardsOnTable[0].seed != CardSmister.cs.briscola || cardsOnTable[2].seed != CardSmister.cs.briscola))
                    {
                        guess = bestCardIndex; Debug.Log("c"); //se è minore (briscola) ma gli avversari non hanno briscolato
                    }
                    else if (cardsOnTable[1].effectiveValue < cardsOnTable[0].effectiveValue && cardsOnTable[1].effectiveValue < cardsOnTable[2].effectiveValue && cardsOnTable[1].seed != CardSmister.cs.briscola && (cardsOnTable[0].seed == CardSmister.cs.briscola || cardsOnTable[2].seed == CardSmister.cs.briscola))
                    {
                        guess = lowestCardIndex; Debug.Log("d"); //se è minore, senza briscola e uno degli avversari briscola
                    }
                    else if (cardsOnTable[1].effectiveValue < cardsOnTable[0].effectiveValue && cardsOnTable[1].effectiveValue < cardsOnTable[2].effectiveValue && cardsOnTable[1].seed != CardSmister.cs.briscola && (cardsOnTable[0].seed == CardSmister.cs.briscola && cardsOnTable[2].seed == CardSmister.cs.briscola))
                    {
                        guess = lowestCardIndex; Debug.Log("e");//se è minore, senza briscola e gli avversari briscolano entrambi
                    }
                    else if (cardsOnTable[1].effectiveValue < cardsOnTable[0].effectiveValue && cardsOnTable[1].effectiveValue < cardsOnTable[2].effectiveValue && cardsOnTable[1].seed == CardSmister.cs.briscola && (cardsOnTable[0].seed == CardSmister.cs.briscola || cardsOnTable[2].seed == CardSmister.cs.briscola))
                    {
                        guess = lowestCardIndex; Debug.Log("f"); // se è minore con briscola, ma gli avversari briscola sopra 
                    }
                    else if (cardsOnTable[1].effectiveValue < cardsOnTable[0].effectiveValue && cardsOnTable[1].effectiveValue < cardsOnTable[2].effectiveValue && cardsOnTable[1].seed != CardSmister.cs.briscola && (cardsOnTable[0].seed == CardSmister.cs.briscola && cardsOnTable[2].seed == CardSmister.cs.briscola))
                    {
                        guess = lowestCardIndex; Debug.Log("g");// se è minore con briscola, ma gli avversari briscola sopra entrambi
                    }
                    else if (cardsOnTable[1].effectiveValue > cardsOnTable[0].effectiveValue || cardsOnTable[1].effectiveValue > cardsOnTable[2].effectiveValue && cardsOnTable[1].seed != CardSmister.cs.briscola && (cardsOnTable[0].seed == CardSmister.cs.briscola && cardsOnTable[2].seed == CardSmister.cs.briscola))
                    {
                        guess = lowestCardIndex; Debug.Log("h");//se è maggiore senza briscola, ma entrambi gli avversari briscolano
                    }
                    else
                    {
                        guess = lowestCardIndex; Debug.Log("i");//dato che l'algoritmo è ancora rudimentale, in caso di casi non coperti, butta la carta più piccola.
                        //TODO
                    }
                }
            }
            else if (cardsOnTable.Length == 2)
            {
                Debug.Log("ramo 1/2");
                if (cardsOnTable[0].effectiveValue > cardsOnTable[1].effectiveValue)
                {
                    if (cardsOnTable[1].seed != CardSmister.cs.briscola)
                    {
                        guess = bestCardIndex;
                    }
                    else if (cardsOnTable[1].seed == CardSmister.cs.handSeed && cardsOnTable[0].seed == CardSmister.cs.handSeed)
                    {
                        guess = bestCardIndex;
                    }
                    else
                    {
                        //la carta giocata è maggiore, ma l'avversario ha una briscola
                        guess = lowestCardIndex;
                    }
                }
                else
                {
                    Debug.Log("ramo 2/2");
                    if (highestValue > cardsOnTable[1].effectiveValue)
                    {
                        if (cardsOnTable[1].seed != CardSmister.cs.briscola)
                        {
                            guess = bestCardIndex;
                        }
                        else if (cardsOnTable[1].seed == CardSmister.cs.handSeed && hand[pointer].seed == CardSmister.cs.handSeed)
                        {
                            guess = bestCardIndex;
                        }
                        else
                        {
                            //la carta che voglio giocare è maggiore, ma l'avversario ha una briscola
                            guess = lowestCardIndex;
                        }
                    }
                    else
                    {
                        if (highestValue == CardSmister.cs.briscola && cardsOnTable[1].effectiveValue != CardSmister.cs.briscola)
                        {
                            //la carta che ho è minore, ma è una briscola e l'avversario non ha giocato briscola.
                            guess = bestCardIndex;
                        }
                        else
                        {
                            //butto la carta più piccola perchè l'avversario ha per forza qualcosa di maggiore.
                            guess = lowestCardIndex;
                        }
                    }
                }
            }
            else if (cardsOnTable.Length == 1)
            {

                if (highestValue > cardsOnTable[0].effectiveValue)
                {
                    guess = bestCardIndex;
                }
                else
                {
                    if (bestCardIndex != -1)
                    {
                        guess = lowestCardIndex;
                    }
                    else
                    {
                        Debug.Log("non ci dovevi entrare brutto finocchio. perchè fai errori? Ora randomizzi a caso eh");
                        guess = Random.Range(0, hand.Length); //da rivedere sta logica
                    }
                }

            }

        }
        else
        {

            //if compagno nella mossa precedente ha bussato e la squadra è riuscita a prendersi la mano, ritorna con lo stesso seme. (se non si ha il seme, buttare random?!?!?!? boh)
            //if compagno vola nella mossa precedente, ritornare nel seme in cui ha volato. (se non si ha il seme, buttare random?!?!?!? boh)
            //if compagno striscia nella mossa precendente, ritornare nello stesso seme.



            //ATTENZIONE ERRORE OUT OF BOUND DA CORREGGERE ASSOLUTAMENTE!!!!! (EDIT: ERRORE FIXATO, MA DA RIVEDERE PERCHè NON HO CAPITO COME HO FIXATO LOL)
            if (hand.Length == 10)
            {
                Debug.Log("Prima giocata");
                guess = convinientCardIndex;
            }
            else
            {
                //TODO
                Debug.Log("Uscita random Generated !!!!");
                guess = Random.Range(0, hand.Length);

            }
        }

        return guess;

        //palle e cozze
    }



    public char ChooseSeed(Card[] hand)
    {

        //Miglioramenti: in caso di parità di numero di carte dello stesso seme (es. 4-4-2), andare a scegliere la quantità maggiore di carte dello stesso seme che insieme totalizzano più punti. In caso di altra parità scegliere una dei due a caso(in  realtà andrebbe implementato il valore di forza per ogni carta, mi sparo).


        int b = 0;
        int c = 0;
        int d = 0;
        int s = 0;
        int seed = 'z';
        char selSeed = 'z';
        int TreDueAssoCountB = 0;
        int TreDueAssoCountC = 0;
        int TreDueAssoCountD = 0;
        int TreDueAssoCountS = 0;

        for (int i = 0; i > hand.Length; i++)
        {

            //vede quanti Tre Due Assi ha in mano, in cerca di una maraffa
            if (hand[i].value == 0 || hand[i].value == 1 || hand[i].value == 2 && hand[i].seed == 'b')
            {
                TreDueAssoCountB++;
            }
            else if (hand[i].value == 0 || hand[i].value == 1 || hand[i].value == 2 && hand[i].seed == 'c')
            {
                TreDueAssoCountC++;
            }
            else if (hand[i].value == 0 || hand[i].value == 1 || hand[i].value == 2 && hand[i].seed == 'd')
            {
                TreDueAssoCountD++;
            }
            else if (hand[i].value == 0 || hand[i].value == 1 || hand[i].value == 2 && hand[i].seed == 's')
            {
                TreDueAssoCountS++;
            }



            //nel dubbio si conta anche tutte le carte appartenenti a ciascun seme
            if (hand[i].seed == 'b')
            {
                b++;
            }
            else if (hand[i].seed == 'c')
            {
                c++;
            }
            else if (hand[i].seed == 'd')
            {
                d++;
            }
            else
            {
                s++;
            }


            //alla fine se ha trovato una maraffa gli fa la briscola, se no la briscola la fa ne seme con più carte.

            if (TreDueAssoCountB == 3)
            {
                seed = 0;
            }
            else if (TreDueAssoCountC == 3)
            {
                seed = 1;
            }
            else if (TreDueAssoCountD == 3)
            {
                seed = 2;
            }
            else if (TreDueAssoCountS == 3)
            {
                seed = 3;
            }
            else
            {
                int max = Mathf.Max(b, Mathf.Max(c, Mathf.Max(d, s)));

                if (max == b)
                {
                    seed = 0;
                }
                else if (max == c)
                {
                    seed = 1;
                }
                else if (max == d)
                {
                    seed = 2;
                }
                else if (max == s)
                {
                    seed = 3;
                }
            }



        }


        //qui fa l'effettiva dichiarazione del seme della briscola

        switch (seed)
        {
            case 0:
                selSeed = 'b';
                break;
            case 1:
                selSeed = 'c';
                break;
            case 2:
                selSeed = 'd';
                break;
            case 3:
                selSeed = 's';
                break;
            default:
                selSeed = 'b';
                break;
        }
        return selSeed;
    }
}
