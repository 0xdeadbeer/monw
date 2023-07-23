using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class CardComparer : IComparer<Card>
{
    public int Compare(Card x, Card y)
    {
        return x.intervalDate.CompareTo(y.intervalDate);
    }
}

public class Card
{
    [JsonProperty("id")]
    public int id;
    [JsonProperty("deck_id")]
    public int deckID;

    [JsonProperty("disabled")]
    public int disabled;
    [JsonProperty("state")]
    public int state;
    [JsonProperty("interval_date")]
    public DateTime intervalDate;
    [JsonProperty("interval_number")]
    public float intervalNumber;
    [JsonProperty("ef_number")]
    public float efNumber;
    [JsonProperty("repetitions")]
    public int repetitions;
    
    [JsonProperty("question")]
    public string question;
    [JsonProperty("answer")]
    public string answer;

    public GameObject assignedObject;

    public Card(int id, int deckID, 
                int disabled, int state, 
                DateTime intervalDate, float intervalNumber, 
                float efNumber, int repetitions,
                string question, string answer)
    {
        this.id = id;
        this.deckID = deckID;
        
        this.disabled = disabled;
        this.state = state;
        this.intervalDate = intervalDate;
        this.intervalNumber = intervalNumber;
        this.efNumber = efNumber;
        this.repetitions = repetitions;

        this.question = question;
        this.answer = answer;
    }
}
