using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using Unity.Mathematics;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    // global settings
    public Camera camera;
    public Picker wordsPicker;
    public MonrAPI monr;
    private float screenSizeFactor;
    public GameObject endCanvasPrefab;
    
    // game settings
    public int gameDifficulty = 10; // 1-factor-difficulty-range
    public GameObject wordPrefab;

    // src: https://www.youtube.com/watch?v=WR0vA2uKk20
    float ConvertPixelsToUnits(float pixelValue)
    {
        float ortho = camera.orthographicSize;
        float pixelHeight = camera.pixelHeight;
        return (pixelValue * ortho * 2) / pixelHeight;
    }

    public IEnumerator GenerateWords()
    {
        while (monr.inUseCardsQueue.Count != 0 || monr.toPickCardsQueue.Count != 0)
        {
            if (monr.toPickCardsQueue.Count == 0)
            {
                yield return null;
                continue;
            }

            if (Random.Range(1, 101) > (25 + gameDifficulty))
            {
                yield return null;
                continue;
            }

            Vector3 randomUnitVector = Random.insideUnitSphere;
            randomUnitVector.x *= screenSizeFactor;
            Vector3 newWordPosition = this.transform.position + randomUnitVector;

            GameObject newWordInstance = Instantiate(wordPrefab, newWordPosition, quaternion.identity, transform);
            Enemy enemyProperty = newWordInstance.GetComponent<Enemy>();
            TMP_Text instanceQuestion = enemyProperty.questionTMP;

            Card instanceCard = monr.pushCardToUse(0);
            int instanceCardIndex = monr.inUseCardsQueue.IndexOf(instanceCard);

            if (instanceCardIndex == -1)
            {
                Debug.LogError($"Cannot find card with id {instanceCard.id} in the use queue");
                yield break;
            }

            instanceQuestion.text = instanceCard.question;
            monr.inUseCardsQueue[instanceCardIndex].assignedObject = newWordInstance;

            if (wordsPicker.currentlyPickedCard is null)
            {
                wordsPicker.pickCard(monr.inUseCardsQueue, monr.allCardsQueue);
            }

            // failed attempt at semi-randomly also delaying the new word
            // yield return new WaitForSeconds((2.0f * Random.Range(0f, 1f)) + (100f/gameDifficulty));
            yield return new WaitForSeconds(2.0f);
        }

        Instantiate(endCanvasPrefab);
    }

    void Start()
    {
        // basic setup 
        screenSizeFactor = ConvertPixelsToUnits((Screen.currentResolution.width / 2f) / 6f);
    }
}
