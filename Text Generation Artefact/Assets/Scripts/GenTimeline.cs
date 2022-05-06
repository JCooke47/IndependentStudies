using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class GenTimeline : MonoBehaviour
{
    public PlayerController playerController;

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    private static Area lounge = new Area("lounge",
                                            new string[] {"hallway"},
                                            new string[] {"Lamp", "Vase"},
                                            new string[] {"wallet", "ring", "wristwatch"},
                                            new string[] {"chair", "coffee table", "plant pot"},
                                            true,
                                            new string[] {"window"});

    private static Area hallway = new Area("hallway",
                                            new string[] {"lounge", "dining room"},
                                            new string[] {"broom"},
                                            new string[] {"framed picture", "set of keys"},
                                            new string[] {"bannister", "plant pot"},
                                            true,
                                            new string[] {"front door"});

    private static Area diningRoom = new Area("dining room",
                                            new string[] {"hallway", "kitchen", "garden"},
                                            new string[] {"knife", "fork"},
                                            new string[] {"mobile phone", "wine glass"},
                                            new string[] {"chair", "table cloth"},
                                            true,
                                            new string[] {"skylight"});

    private static Area kitchen = new Area("kitchen",
                                            new string[] {"dining room"},
                                            new string[] {"kitchen knife", "rolling pin"},
                                            new string[] {"bottle of whisky"},
                                            new string[] {"fridge", "cupboard"},
                                            true,
                                            new string[] {"window"});

    private static Area garden = new Area("garden",
                                            new string[] {"dining room"},
                                            new string[] {"pair of shears"},
                                            new string[] {"football"},
                                            new string[] {"garden chair", "flower pot"},
                                            true,
                                            new string[] {"fence"});


    private static Area[] rooms= {lounge, hallway, diningRoom, kitchen, garden};  

    public static Dictionary<int, string> timeline = new Dictionary<int, string>();
    private static string scenarioType = "";       // 'murder' or 'robbery'
    public int difficulty = 3;                     // Easy = 1, Medium = 2, Hard = 3
    private static int timelineLength = 0;         // Specifies no. of story elements (difficulty dependent)

    private static int takeWeaponStage = 0;
    private static int commitMurderStage = 0;
    private static int hideWeaponStage = 0;

    private static int stealCount = 0;
    public static List<int> stealStages = new List<int>();
    public static List<string> AssetOrder = new List<string>();


    public void GenerateTask()
    {
        timeline.Clear();

        SetScenarioType();                  // Set scenario to either murder or robbery
        InitialiseTimeline(difficulty);     // Sets no. of story elements (timelineLength)

        Area currentRoom;
        Area nextRoom;
        string nextRoomName;
        float act;
        string clue = "";
        string takenItem = "";
        string escapeMethod;

        AssetOrder.Clear();

        // Culprit enters premesis
        do{
            currentRoom = rooms[Random.Range(0, rooms.Length)];
        } while(currentRoom.EscapePossible != true);

        timeline.Add(1, "Enter:" + currentRoom.Name);
        AssetOrder.Add(currentRoom.Name);

        //print(timelineLength);

        // Next story elements
        for(int i = 2; i < timelineLength; i++)
        {
            act = Random.Range(0.0f, 1.0f);

            if(scenarioType == "murder" && i == takeWeaponStage)
            {
                takenItem = currentRoom.Weapons[Random.Range(0, currentRoom.Weapons.Length)];
                timeline.Add(i, "Take weapon:" + takenItem);
                AssetOrder.Add(currentRoom.Name);
            }

            else if(scenarioType == "murder" && i == commitMurderStage)
            {
                timeline.Add(i, "Murder:" + currentRoom.Name);
                AssetOrder.Add(currentRoom.Name);
            }

            else if(scenarioType == "murder" && i == hideWeaponStage)
            {
                timeline.Add(i, "Hide weapon:" + takenItem);
                AssetOrder.Add(currentRoom.Name);
            }

            else if(scenarioType == "robbery" && stealStages.Contains(i))
            {
                takenItem = currentRoom.Valuables[Random.Range(0, currentRoom.Valuables.Length)];
                timeline.Add(i, "Steal:" + takenItem);
                AssetOrder.Add(currentRoom.Name);
            }

            else if(act > 0.0f && act < 0.5f)   // Culprit moves to another room
            {
                if(timeline[i - 1] == ("Move:" + currentRoom.Name))
                {
                    i = i - 1; // Prevents having two moves back to back
                }
                else
                {
                    nextRoom = currentRoom;
                    nextRoomName = currentRoom.ConnectedRooms[Random.Range(0, currentRoom.ConnectedRooms.Length)];
                    
                    foreach(Area room in rooms)
                    {
                        if(room.Name == nextRoomName)
                        {
                            nextRoom = room;
                        }
                    }

                    timeline.Add(i, "Move:" + nextRoomName);
                    currentRoom = nextRoom; 
                }
                
            }

            else if (act >= 0.5f && act <= 1.0f)    // Culprit leaves a clue
            {
                if(timeline[i - 1] == ("Clue:" + clue))
                {
                    i = i - 1; // Prevents having two clues back to back
                }
                else
                {
                    clue = currentRoom.ClueItems[Random.Range(0, currentRoom.ClueItems.Length)];
                    timeline.Add(i, "Clue:" + clue);
                    AssetOrder.Add(currentRoom.Name);
                }
            }
        }

        // Escape currently assumes every room has an escape method. Also could modify so culprit is hiding somewhere
        escapeMethod = currentRoom.EscapeMethods[Random.Range(0, currentRoom.EscapeMethods.Length)];
        timeline.Add(timelineLength, "Escape:" + escapeMethod);
        AssetOrder.Add(currentRoom.Name);

        //playerController.generatedScenario.text = scenario;
        playerController.generatedScenario.text = "";
        foreach(string roomname in AssetOrder)
        {
            playerController.generatedScenario.text += roomname + "\n";
        }
    }

    private static void SetScenarioType()
    {
        float getScenarioType = Random.Range(0.0f, 1.0f);

        if(getScenarioType < 0.5f)
        {
            scenarioType = "robbery";
        }
        else
        {
            scenarioType = "murder";
        }
    }


    private static void InitialiseTimeline(int difficulty)
    {
        if(difficulty == 1)
        {
            timelineLength = Random.Range(6, 9);
            takeWeaponStage = Random.Range(2, timelineLength - 2);
            stealCount = Random.Range(1, 3);
        }
        else if(difficulty == 2)
        {
            timelineLength = Random.Range(9, 12);
            takeWeaponStage = Random.Range(2, timelineLength - 3);
            stealCount = Random.Range(2, 5);
        }
        else if(difficulty == 3)
        {
            timelineLength = Random.Range(12, 15);
            takeWeaponStage = Random.Range(2, timelineLength - 5);
            stealCount = Random.Range(4, 7);
        }

        stealStages.Clear();

        for(int i = 0; i < stealCount; i++)
        {
            int stage = Random.Range(2, timelineLength);
            while (stealStages.Contains(stage))
            {
                stage = Random.Range(2, timelineLength);
            }
            stealStages.Add(stage);
        }
        
        hideWeaponStage = Random.Range(takeWeaponStage + 3, timelineLength);
        commitMurderStage = Random.Range(takeWeaponStage + 1, hideWeaponStage);
    }
}