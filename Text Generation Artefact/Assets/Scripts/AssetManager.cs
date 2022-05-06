using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AssetManager : MonoBehaviour
{
    public GameObject cluePrefab;
    public GameObject loungeClue1;
    public GameObject loungeClue2;
    public GameObject hallClue1;
    public GameObject hallClue2;
    public GameObject diningClue1;
    public GameObject diningClue2;
    public GameObject kitchenClue1;
    public GameObject kitchenClue2;
    public GameObject gardenClue1;
    public GameObject gardenClue2;

    public TextMeshProUGUI journalText;

    public static List<string> AssetRoomOrder = new List<string>();
    public static List<string> AllSentences = new List<string>();
    int collected = 0;
    int duplicates = 0;

    void Start()
    {
        cluePrefab.SetActive(false);
    }

    public void SetAssetOrder()
    {
        AssetRoomOrder = GenTimeline.AssetOrder;
        AllSentences = GenQuests.AllSentences;
        collected = 0;
        SpawnClue(0);
    }

    public void ClueCollected()
    {
        if(collected != (AssetRoomOrder.Count - 1))
        {
            journalText.text += "> " + AllSentences[collected] + "\n";
            collected++;
            SpawnClue(collected);
        }
        else
        {
            journalText.text += "> " + AllSentences[AllSentences.Count - 1] + "\n";
            journalText.text += "\n<< All clues found! >>";
            print("All clues found!");
        }  
    }

    void SpawnClue(int index)
    {
        string room = AssetRoomOrder[index];
        
        if(index != 0)
        {
            if(room == AssetRoomOrder[index - 1])
            {
                duplicates++;
            }
            else
            {
                duplicates = 0;
            }
        }

        if(room == "lounge")
        {
            if(duplicates % 2 == 0)
            {
                loungeClue1.SetActive(true);
            }
            else
            {
                loungeClue2.SetActive(true);
            }
        }
        else if(room == "hallway")
        {
            if(duplicates % 2 == 0)
            {
                hallClue1.SetActive(true);
            }
            else
            {
                hallClue2.SetActive(true);
            }
        }
        else if(room == "dining room")
        {
            if(duplicates % 2 == 0)
            {
                diningClue1.SetActive(true);
            }
            else
            {
                diningClue2.SetActive(true);
            }
        }
        else if(room == "kitchen")
        {
            if(duplicates % 2 == 0)
            {
                kitchenClue1.SetActive(true);
            }
            else
            {
                kitchenClue2.SetActive(true);
            }
        }
        else if(room == "garden")
        {
            if(duplicates % 2 == 0)
            {
                gardenClue1.SetActive(true);
            }
            else
            {
                gardenClue2.SetActive(true);
            }
        }
    }

    public void DisableAllClues()
    {
        journalText.text = "";
        duplicates = 0;
        
        loungeClue1.SetActive(false);
        loungeClue2.SetActive(false);
        diningClue1.SetActive(false);
        diningClue2.SetActive(false);
        hallClue1.SetActive(false);
        hallClue2.SetActive(false);
        kitchenClue1.SetActive(false);
        kitchenClue2.SetActive(false);
        gardenClue1.SetActive(false);
        gardenClue2.SetActive(false);
    }
}
