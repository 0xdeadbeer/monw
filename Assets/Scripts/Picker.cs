using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Picker : MonoBehaviour
{
    public TMP_Text answer1;
    public TMP_Text answer2;

    public Card currentDecoyCard;
    private Card _currentlyPickedCard;
    public Card currentlyPickedCard
    {
        get
        {
            return _currentlyPickedCard;
        }
        set
        {
            _currentlyPickedCard = value;
            if (Random.Range(1, 101) > 50)
            {
                answer1.text = _currentlyPickedCard?.answer ?? "";
                answer2.text = currentDecoyCard?.answer ?? "";
            }
            else
            {
                answer1.text = currentDecoyCard?.answer ?? "";
                answer2.text = _currentlyPickedCard?.answer ?? "";
            }
        }
    }

    public Card pickCard(List<Card> queue, List<Card> randomCardsQueue)
    {
        if (queue.Count == 0 || randomCardsQueue.Count == 0)
        {
            currentDecoyCard = null;
            currentlyPickedCard = null;
            return null;
        }
        
        int decoyChoice = Random.Range(0, randomCardsQueue.Count-1);
        this.currentDecoyCard = randomCardsQueue[decoyChoice];
        this.currentlyPickedCard = queue.First();
        
        return currentlyPickedCard;
    }
}
