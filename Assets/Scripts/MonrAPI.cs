using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class MonrAPI : MonoBehaviour
{
    // global settings
    public string URL;
    public string deck;
    public float reviewAheadTime = 10f; // minutes
    public Spawner gameSpawner;
    string dateFormat = "yyyy-MM-dd HH:mm:ss";

    // game settings
    public List<Card> allCardsQueue = new List<Card>();
    public List<Card> toPickCardsQueue = new List<Card>();
    public List<Card> inUseCardsQueue = new List<Card>();

    public Card pushCardToUse(int index)
    {
        if (toPickCardsQueue.Count == 0)
        {
            return null;
        }

        if (toPickCardsQueue.Count <= index)
        {
            index = toPickCardsQueue.Count - 1;
        }

        Card instanceCard = toPickCardsQueue[index];
        toPickCardsQueue.RemoveAt(index);
        inUseCardsQueue.Add(instanceCard);

        return instanceCard;
    }

    public Card pushCardToPick(int index)
    {
        if (inUseCardsQueue.Count == 0)
        {
            return null;
        }

        if (inUseCardsQueue.Count <= index)
        {
            index = inUseCardsQueue.Count - 1;
        }

        Card instanceCard = inUseCardsQueue[index];
        inUseCardsQueue.RemoveAt(index);
        toPickCardsQueue.Add(instanceCard);

        return instanceCard;
    }

    // TODO: insert cards in the correct spot of the queue
    /*public Card pushCardToPickByDate(Card card)
    {
         
    }*/

    public IEnumerator ReturnCards()
    {
        object data = new
        {
            deck = deck
        };
        string jsonData = JsonConvert.SerializeObject(data);
        byte[] dataRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest www = new UnityWebRequest($"{URL}/cards/return", "POST");
        www.uploadHandler = (UploadHandler) new UploadHandlerRaw(dataRaw);
        www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        
        yield return www.SendWebRequest();
        
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed returning all the cards");
            yield break;
        }
        
        JArray resultJArray = JArray.Parse(www.downloadHandler.text);
        try
        {
            foreach (JObject item in resultJArray)
            {
                Card itemObject = item.ToObject<Card>();
                allCardsQueue.Add(itemObject);
            }
        }
        catch (InvalidCastException)
        {
            Debug.LogError("Failed casting JSON to cards");
            yield break;
        }

        // TODO: SHOULD WE DO SOMETHING ABOUT THIS?
        if (allCardsQueue.Count < 5)
        {
            Debug.LogWarning("Seems like there's too little cards to review today");
        }

        StartCoroutine(FetchCards());
    }

    public IEnumerator FetchCards()
    {
        object data = new
        {
            deck = deck,
            interval_date = DateTime.Now.AddMinutes(reviewAheadTime).ToString(dateFormat),
            sort_column = "interval_date",
            sort_direction = "ASC"
        };
        string jsonData = JsonConvert.SerializeObject(data);
        byte[] dataRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest www = new UnityWebRequest($"{URL}/cards/fetch", "POST");
        www.uploadHandler = (UploadHandler) new UploadHandlerRaw(dataRaw);
        www.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed fetching the cards");
            yield break;
        }
        
        JArray resultJArray = JArray.Parse(www.downloadHandler.text);
        try
        {
            foreach (JObject item in resultJArray)
            {
                Card itemObject = item.ToObject<Card>();
                toPickCardsQueue.Add(itemObject);
            }
        }
        catch (InvalidCastException)
        {
            Debug.LogError("Failed casting JSON to cards");
            yield break;
        }
        
        StartCoroutine(gameSpawner.GenerateWords());
    }

    public IEnumerator LearnCard(Card card, int grade)
    {
        object data = new
        {
            card_id = card.id, 
            grade = grade
        };

        string jsonData = JsonConvert.SerializeObject(data);
        byte[] dataRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest www = new UnityWebRequest($"{URL}/cards/learn", "POST");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(dataRaw);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed changing card's interval (id: {card.id})");
            yield break;
        }

        // update the object
        try
        {
            JsonConvert.PopulateObject(www.downloadHandler.text, card);
        }
        catch
        {
            yield break;
        }
        
        // play swipe animations 
        GameObject wordObject = card.assignedObject.transform.GetChild(0).gameObject;
        Animator wordAnimator = wordObject.GetComponent<Animator>();

        wordObject.GetComponent<Collider2D>().enabled = false; // disable collider to not interfere with the game

        card.assignedObject = null;
        int location = toPickCardsQueue.BinarySearch(card, new CardComparer());
        
        if (grade == 0) // card failed (swipe left)
        {
            wordAnimator.Play("WrongSwipe");
            toPickCardsQueue.Insert(~location, card);
        } 
        else if (card.repetitions <= 2 || card.intervalDate <= DateTime.Now.AddMinutes(reviewAheadTime)) // card okay - but still in review for today (swipe down)
        {
            wordAnimator.Play("GoodSwipe");
            toPickCardsQueue.Insert(~location, card);
        }
        else // card okay - we'll see it next time (swipe right)
        {
            wordAnimator.Play("AdvanceSwipe");
        }
    }

    void Start()
    {
        StartCoroutine(ReturnCards());
    }
}
