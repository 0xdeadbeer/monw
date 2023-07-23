using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

public class Selector : MonoBehaviour, IPointerClickHandler
{
    public MonrAPI monr;
    public Picker picker;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (picker.currentlyPickedCard is null)
        {
            return;
        }

        string thisAnswer = this.GetComponent<TMP_Text>().text;
        string realAnswer = picker.currentlyPickedCard.answer;
        int grade = 1;
        
        if (thisAnswer != realAnswer)
        {
            grade = 0;
        }

        // remove the card from the use queue
        monr.inUseCardsQueue.Remove(picker.currentlyPickedCard);
        
        // call MonR with the grade
        StartCoroutine(monr.LearnCard(picker.currentlyPickedCard, grade));

        // pick a new card for answers
        picker.pickCard(monr.inUseCardsQueue, monr.allCardsQueue);
    }
}
