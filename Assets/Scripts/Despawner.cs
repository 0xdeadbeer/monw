using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawner : MonoBehaviour
{
    public MonrAPI monr;
    public Picker wordPicker;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Card cardInUse = monr.inUseCardsQueue[0];
        monr.inUseCardsQueue.Remove(cardInUse);

        StartCoroutine(monr.LearnCard(cardInUse, 0));
        
        wordPicker.pickCard(monr.inUseCardsQueue, monr.allCardsQueue);
    }
}