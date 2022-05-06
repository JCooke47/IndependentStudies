using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GenQuests : MonoBehaviour
{
    public PlayerController playerController;
    public GenTimeline genTimeline;

    public static Dictionary<int, string> fullTimeline = new Dictionary<int, string>();
    public static List<string> AllSentences = new List<string>();

    void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
        genTimeline = FindObjectOfType<GenTimeline>();

        InitializeEnterTree();
        InitializeTakeWeaponTree();
        InitializeMurderTree();
        InitializeHideWeaponTree();
        InitializeClueTree();
        InitializeEscapeTree();
        InitializeStealTree();
    }

    public void GenerateQuests()
    {
        playerController.generatedQuests.text = "";
        fullTimeline = GenTimeline.timeline;
        AllSentences.Clear();

        string currentRoom = "";
        string murderWeapon = "";

        string entrance = "";
        string takeWeapon = "";
        string commitMurder = "";
        string hideWeapon = "";
        string leaveClue = "";
        string escapeArea = "";
        string stealValuable = "";

        for(int i = 1; i <= fullTimeline.Count; i++)    // Iterate through timeline elements
        {
            string actType = fullTimeline[i].Split(':')[0];
            string attribute = fullTimeline[i].Split(':')[1];

            if(actType == "Enter")
            {      
                entrance = GenerateEntranceText(attribute);
                playerController.generatedQuests.text = entrance;
                AllSentences.Add(entrance);
                currentRoom = attribute;
            }
            else if(actType == "Take weapon")
            {
                takeWeapon = GenerateTakeWeaponText(attribute, currentRoom);
                playerController.generatedQuests.text += "\n\n" + takeWeapon;
                AllSentences.Add(takeWeapon);
                murderWeapon = attribute;
            }
            else if(actType == "Murder")
            {
                commitMurder = GenerateMurderText(murderWeapon, attribute);
                playerController.generatedQuests.text += "\n\n" + commitMurder;
                AllSentences.Add(commitMurder);
            }
            else if(actType == "Hide weapon")
            {
                hideWeapon = GenerateHideWeaponText(attribute, currentRoom);
                playerController.generatedQuests.text += "\n\n" + hideWeapon;
                AllSentences.Add(hideWeapon);
            }
            else if(actType == "Clue")
            {
                leaveClue = GenerateClueText(attribute, currentRoom);
                playerController.generatedQuests.text += "\n\n" + leaveClue;
                AllSentences.Add(leaveClue);
            }
            else if(actType == "Escape")
            {
                escapeArea = GenerateEscapeText(attribute, currentRoom);
                playerController.generatedQuests.text += "\n\n" + escapeArea;
                AllSentences.Add(escapeArea);
            }
            else if(actType == "Steal")
            {
                stealValuable = GenerateStealText(attribute, currentRoom);
                playerController.generatedQuests.text += "\n\n" + stealValuable;
                AllSentences.Add(stealValuable);
            }
            else if(actType == "Move")
            {
                currentRoom = attribute;
            }
        }
    }

    public void AdjustOdds(TreeNode node, int selectedIndex)
    {
        float reduceValue = (1.00f / node.Odds.Count)/5;
        float positiveOffset = reduceValue / (node.Odds.Count - 1);
        float probability;
        List<float> probabilities = new List<float>();
        bool bottomCap = false;

        for(int i = 0; i < node.Odds.Count; i++)
        {
            if(i == 0)
            {
                probability = node.Odds[0];                
            }
            else
            {
                probability = node.Odds[i] - node.Odds[i - 1];
            }
            probabilities.Add(probability);
        }


        for(int i = 0; i < probabilities.Count; i++)
        {
            if(i == selectedIndex)
            {
                probabilities[i] -= reduceValue;
            }
            else
            {
                probabilities[i] += positiveOffset;
            }
        }

        foreach(float value in probabilities)
        {
            if(value < 0.05f)
            {
                bottomCap = true;
            }
        }

        if(bottomCap == false)
        {          
            for(int i = 0; i < probabilities.Count; i++)
            {
                if(i == 0)
                {
                    node.Odds[i] = probabilities[i];
                }
                else
                {
                    node.Odds[i] = probabilities[i] + node.Odds[i - 1];
                }  
            }
        }
    }

    public string GenerateEntranceText(string room)
    {
        string enterText = "";
        int part0 = Random.Range(0, enter0Nodes.Count);                 // Build part 0
        enterText += enter0Nodes[part0].Text + " ";

        float rand = Random.Range(0.00f, 1.00f);
        int part1 = 0;
        
        for(int j = 0; j < enter0Nodes[part0].ChildNodes.Count; j++)    // Build part 1
        {
            if(rand < enter0Nodes[part0].Odds[0])
            {
                part1 = 0;
                AdjustOdds(enter0Nodes[part0], 0);
                break;
            }
            else if(rand < enter0Nodes[part0].Odds[j] && rand >= enter0Nodes[part0].Odds[j - 1])
            {
                part1 = j;
                AdjustOdds(enter0Nodes[part0], j);
                break;
            }
        }
        enterText += enter1Nodes[part1].Text + " ";
        

        rand = Random.Range(0.00f, 1.00f);
        int part2 = 0;
        
        for(int j = 0; j < enter1Nodes[part1].ChildNodes.Count; j++)    // Build part 2
        {
            if(rand < enter1Nodes[part1].Odds[0])
            {
                part2 = 0;
                AdjustOdds(enter1Nodes[part1], 0);
            }
            else if(rand < enter1Nodes[part1].Odds[j] && rand >= enter1Nodes[part1].Odds[j - 1])
            {
                part2 = j;
                AdjustOdds(enter1Nodes[part1], j);
            }
        }
        enterText += enter2Nodes[part2].Text + " ";


        rand = Random.Range(0.00f, 1.00f);
        int part3 = 0;
        
        for(int j = 0; j < enter2Nodes[part2].ChildNodes.Count; j++)    // Build part 3
        {
            if(rand < enter2Nodes[part2].Odds[0])
            {
                part3 = 0;
                AdjustOdds(enter2Nodes[part2], 0);
            }
            else if(rand < enter2Nodes[part2].Odds[j] && rand >= enter2Nodes[part2].Odds[j - 1])
            {
                part3 = j;
                AdjustOdds(enter2Nodes[part2], j);
            }
        }
        enterText += enter3Nodes[part3].Text + " ";


        rand = Random.Range(0.00f, 1.00f);
        int part4 = 0;
        
        for(int j = 0; j < enter3Nodes[part3].ChildNodes.Count; j++)    // Build part 4
        {
            if(rand < enter3Nodes[part3].Odds[0])
            {
                part4 = 0;
                AdjustOdds(enter3Nodes[part3], 0);
            }
            else if(rand < enter3Nodes[part3].Odds[j] && rand >= enter3Nodes[part3].Odds[j - 1])
            {
                part4 = j;
                AdjustOdds(enter3Nodes[part3], j);
            }
        }
        enterText += enter4Nodes[part4].Text + " ";
        enterText += "the " + room + "...";
        return enterText;
    }

    public string GenerateTakeWeaponText(string weapon, string room)
    {
        string weaponText = "";
        int part0 = Random.Range(0, weapon0Nodes.Count);
        weaponText += weapon0Nodes[part0].Text + " ";                     // Build part 0

        float rand = Random.Range(0.00f, 1.00f);
        int part1 = 0;
        
        for(int j = 0; j < weapon0Nodes[part0].ChildNodes.Count; j++)    // Build part 1
        {
            if(rand < weapon0Nodes[part0].Odds[0])
            {
                part1 = 0;
                AdjustOdds(weapon0Nodes[part0], 0);
            }
            else if(rand < weapon0Nodes[part0].Odds[j] && rand >= weapon0Nodes[part0].Odds[j - 1])
            {
                part1 = j;
                AdjustOdds(weapon0Nodes[part0], j);
            }
        }
        weaponText += weapon1Nodes[part1].Text + " ";
        

        rand = Random.Range(0.00f, 1.00f);
        int part2 = 0;
        
        for(int j = 0; j < weapon1Nodes[part1].ChildNodes.Count; j++)    // Build part 2
        {
            if(rand < weapon1Nodes[part1].Odds[0])
            {
                part2 = 0;
                AdjustOdds(weapon1Nodes[part1], 0);
            }
            else if(rand < weapon1Nodes[part1].Odds[j] && rand >= weapon1Nodes[part1].Odds[j - 1])
            {
                part2 = j;
                AdjustOdds(weapon1Nodes[part1], j);
            }
        }
        weaponText += weapon2Nodes[part2].Text + " ";

        weaponText += weapon + " ";                                     // Add weapon section

        rand = Random.Range(0.00f, 1.00f);
        int part3 = 0;
        
        for(int j = 0; j < weapon2Nodes[part2].ChildNodes.Count; j++)    // Build part 3
        {
            if(rand < weapon2Nodes[part2].Odds[0])
            {
                part3 = 0;
                AdjustOdds(weapon2Nodes[part2], 0);
            }
            else if(rand < weapon2Nodes[part2].Odds[j] && rand >= weapon2Nodes[part2].Odds[j - 1])
            {
                part3 = j;
                AdjustOdds(weapon2Nodes[part2], j);
            }
        }
        weaponText += weapon3Nodes[part3].Text + " ";

        weaponText += room + " ";                              // Add room section

        rand = Random.Range(0.00f, 1.00f);
        int part4 = 0;
        
        for(int j = 0; j < weapon3Nodes[part3].ChildNodes.Count; j++)    // Build part 4
        {
            if(rand < weapon3Nodes[part3].Odds[0])
            {
                part4 = 0;
                AdjustOdds(weapon3Nodes[part3], 0);
            }
            else if(rand < weapon3Nodes[part3].Odds[j] && rand >= weapon3Nodes[part3].Odds[j - 1])
            {
                part4 = j;
                AdjustOdds(weapon3Nodes[part3], j);
            }
        }
        weaponText += weapon4Nodes[part4].Text + "...";
        return weaponText;
    }

    public string GenerateMurderText(string weapon, string room)
    {
        string murderText = "";
        int part0 = Random.Range(0, murder0Nodes.Count);
        murderText += murder0Nodes[part0].Text + " ";                     // Build part 0

        float rand = Random.Range(0.00f, 1.00f);
        int part1 = 0;
        
        for(int j = 0; j < murder0Nodes[part0].ChildNodes.Count; j++)    // Build part 1
        {
            if(rand < murder0Nodes[part0].Odds[0])
            {
                part1 = 0;
                AdjustOdds(murder0Nodes[part0], 0);
            }
            else if(rand < murder0Nodes[part0].Odds[j] && rand >= murder0Nodes[part0].Odds[j - 1])
            {
                part1 = j;
                AdjustOdds(murder0Nodes[part0], j);
            }
        }
        murderText += murder1Nodes[part1].Text + " ";
        

        rand = Random.Range(0.00f, 1.00f);
        int part2 = 0;
        
        for(int j = 0; j < murder1Nodes[part1].ChildNodes.Count; j++)    // Build part 2
        {
            if(rand < murder1Nodes[part1].Odds[0])
            {
                part2 = 0;
                AdjustOdds(murder1Nodes[part1], 0);
            }
            else if(rand < murder1Nodes[part1].Odds[j] && rand >= murder1Nodes[part1].Odds[j - 1])
            {
                part2 = j;
                AdjustOdds(murder1Nodes[part1], j);
            }
        }
        murderText += murder2Nodes[part2].Text + " ";

        rand = Random.Range(0.00f, 1.00f);
        int part3 = 0;
        
        for(int j = 0; j < murder2Nodes[part2].ChildNodes.Count; j++)    // Build part 3
        {
            if(rand < murder2Nodes[part2].Odds[0])
            {
                part3 = 0;
                AdjustOdds(murder2Nodes[part2], 0);
            }
            else if(rand < murder2Nodes[part2].Odds[j] && rand >= murder2Nodes[part2].Odds[j - 1])
            {
                part3 = j;
                AdjustOdds(murder2Nodes[part2], j);
            }
        }
        murderText += murder3Nodes[part3].Text + " ";

        murderText += room + " ";                              // Add room section

        rand = Random.Range(0.00f, 1.00f);
        int part4 = 0;
        
        for(int j = 0; j < murder3Nodes[part3].ChildNodes.Count; j++)    // Build part 4
        {
            if(rand < murder3Nodes[part3].Odds[0])
            {
                part4 = 0;
                AdjustOdds(murder3Nodes[part3], 0);
            }
            else if(rand < murder3Nodes[part3].Odds[j] && rand >= murder3Nodes[part3].Odds[j - 1])
            {
                part4 = j;
                AdjustOdds(murder3Nodes[part3], j);
            }
        }
        murderText += murder4Nodes[part4].Text + " ";
        murderText += weapon + "...";                                     // Add weapon section
        return murderText;
    }

    public string GenerateHideWeaponText(string weapon, string room)
    {
        string hideText = "";
        int part0 = Random.Range(0, hide0Nodes.Count);
        hideText += hide0Nodes[part0].Text + " ";                     // Build part 0

        float rand = Random.Range(0.00f, 1.00f);
        int part1 = 0;
        
        for(int j = 0; j < hide0Nodes[part0].ChildNodes.Count; j++)    // Build part 1
        {
            if(rand < hide0Nodes[part0].Odds[0])
            {
                part1 = 0;
                AdjustOdds(hide0Nodes[part0], 0);
            }
            else if(rand < hide0Nodes[part0].Odds[j] && rand >= hide0Nodes[part0].Odds[j - 1])
            {
                part1 = j;
                AdjustOdds(hide0Nodes[part0], j);
            }
        }
        hideText += hide1Nodes[part1].Text + " ";
        

        rand = Random.Range(0.00f, 1.00f);
        int part2 = 0;
        
        for(int j = 0; j < hide1Nodes[part1].ChildNodes.Count; j++)    // Build part 2
        {
            if(rand < hide1Nodes[part1].Odds[0])
            {
                part2 = 0;
                AdjustOdds(hide1Nodes[part1], 0);
            }
            else if(rand < hide1Nodes[part1].Odds[j] && rand >= hide1Nodes[part1].Odds[j - 1])
            {
                part2 = j;
                AdjustOdds(hide1Nodes[part1], j);
            }
        }
        hideText += hide2Nodes[part2].Text + " ";

        hideText += weapon + " ";                                     // Add weapon section

        rand = Random.Range(0.00f, 1.00f);
        int part3 = 0;
        
        for(int j = 0; j < hide2Nodes[part2].ChildNodes.Count; j++)    // Build part 3
        {
            if(rand < hide2Nodes[part2].Odds[0])
            {
                part3 = 0;
                AdjustOdds(hide2Nodes[part2], 0);
            }
            else if(rand < hide2Nodes[part2].Odds[j] && rand >= hide2Nodes[part2].Odds[j - 1])
            {
                part3 = j;
                AdjustOdds(hide2Nodes[part2], j);
            }
        }
        hideText += hide3Nodes[part3].Text + " ";
        hideText += room + "...";                                     // Add room section

        return hideText;
    }

    public string GenerateClueText(string clue, string room)
    {
        string clueText = "";
        int part0 = Random.Range(0, clue0Nodes.Count);
        clueText += clue0Nodes[part0].Text + " ";                     // Build part 0

        float rand = Random.Range(0.00f, 1.00f);
        int part1 = 0;
        
        for(int j = 0; j < clue0Nodes[part0].ChildNodes.Count; j++)    // Build part 1
        {
            if(rand < clue0Nodes[part0].Odds[0])
            {
                part1 = 0;
                AdjustOdds(clue0Nodes[part0], 0);
            }
            else if(rand < clue0Nodes[part0].Odds[j] && rand >= clue0Nodes[part0].Odds[j - 1])
            {
                part1 = j;
                AdjustOdds(clue0Nodes[part0], j);
            }
        }
        clueText += clue1Nodes[part1].Text + " ";
        

        rand = Random.Range(0.00f, 1.00f);
        int part2 = 0;
        
        for(int j = 0; j < clue1Nodes[part1].ChildNodes.Count; j++)    // Build part 2
        {
            if(rand < clue1Nodes[part1].Odds[0])
            {
                part2 = 0;
                AdjustOdds(clue1Nodes[part1], 0);
            }
            else if(rand < clue1Nodes[part1].Odds[j] && rand >= clue1Nodes[part1].Odds[j - 1])
            {
                part2 = j;
                AdjustOdds(clue1Nodes[part1], j);
            }
        }
        clueText += clue2Nodes[part2].Text + " ";

        clueText += clue + " ";                                     // Add clue section

        rand = Random.Range(0.00f, 1.00f);
        int part3 = 0;
        
        for(int j = 0; j < clue2Nodes[part2].ChildNodes.Count; j++)    // Build part 3
        {
            if(rand < clue2Nodes[part2].Odds[0])
            {
                part3 = 0;
                AdjustOdds(clue2Nodes[part2], 0);
            }
            else if(rand < clue2Nodes[part2].Odds[j] && rand >= clue2Nodes[part2].Odds[j - 1])
            {
                part3 = j;
                AdjustOdds(clue2Nodes[part2], j);
            }
        }
        clueText += clue3Nodes[part3].Text + " ";
        clueText += room + "...";                                     // Add room section

        return clueText;
    }

    public string GenerateEscapeText(string escapeMethod, string room)
    {
        string escapeText = "";
        int part0 = Random.Range(0, escape0Nodes.Count);
        escapeText += escape0Nodes[part0].Text + " ";                     // Build part 0

        float rand = Random.Range(0.00f, 1.00f);
        int part1 = 0;
        
        for(int j = 0; j < escape0Nodes[part0].ChildNodes.Count; j++)    // Build part 1
        {
            if(rand < escape0Nodes[part0].Odds[0])
            {
                part1 = 0;
                AdjustOdds(escape0Nodes[part0], 0);              
            }
            else if(rand < escape0Nodes[part0].Odds[j] && rand >= escape0Nodes[part0].Odds[j - 1])
            {
                part1 = j;
                AdjustOdds(escape0Nodes[part0], j);
            }
        }
        escapeText += escape1Nodes[part1].Text + " ";
        

        rand = Random.Range(0.00f, 1.00f);
        int part2 = 0;
        
        for(int j = 0; j < escape1Nodes[part1].ChildNodes.Count; j++)    // Build part 2
        {
            if(rand < escape1Nodes[part1].Odds[0])
            {
                part2 = 0;
                AdjustOdds(escape1Nodes[part1], 0);
            }
            else if(rand < escape1Nodes[part1].Odds[j] && rand >= escape1Nodes[part1].Odds[j - 1])
            {
                part2 = j;
                AdjustOdds(escape1Nodes[part1], j);
            }
        }
        escapeText += escape2Nodes[part2].Text + " ";

        rand = Random.Range(0.00f, 1.00f);
        int part3 = 0;
        
        for(int j = 0; j < escape2Nodes[part2].ChildNodes.Count; j++)    // Build part 3
        {
            if(rand < escape2Nodes[part2].Odds[0])
            {
                part3 = 0;
                AdjustOdds(escape2Nodes[part2], 0);
            }
            else if(rand < escape2Nodes[part2].Odds[j] && rand >= escape2Nodes[part2].Odds[j - 1])
            {
                part3 = j;
                AdjustOdds(escape2Nodes[part2], j);
            }
        }
        escapeText += escape3Nodes[part3].Text + " ";
        escapeText += escapeMethod + " in the " + room + ".";               // Add room section

        return escapeText;
    }

    public string GenerateStealText(string valuable, string room)
    {
        string stealText = "";
        int part0 = Random.Range(0, steal0Nodes.Count);
        stealText += steal0Nodes[part0].Text + " ";                     // Build part 0

        float rand = Random.Range(0.00f, 1.00f);
        int part1 = 0;
        
        for(int j = 0; j < steal0Nodes[part0].ChildNodes.Count; j++)    // Build part 1
        {
            if(rand < steal0Nodes[part0].Odds[0])
            {
                part1 = 0;
                AdjustOdds(steal0Nodes[part0], 0);
            }
            else if(rand < steal0Nodes[part0].Odds[j] && rand >= steal0Nodes[part0].Odds[j - 1])
            {
                part1 = j;
                AdjustOdds(steal0Nodes[part0], j);
            }
        }
        stealText += steal1Nodes[part1].Text + " ";
        
        stealText += valuable + " ";                                    // Add valuable section

        rand = Random.Range(0.00f, 1.00f);
        int part2 = 0;
        
        for(int j = 0; j < steal1Nodes[part1].ChildNodes.Count; j++)    // Build part 2
        {
            if(rand < steal1Nodes[part1].Odds[0])
            {
                part2 = 0;
                AdjustOdds(steal1Nodes[part1], 0);
            }
            else if(rand < steal1Nodes[part1].Odds[j] && rand >= steal1Nodes[part1].Odds[j - 1])
            {
                part2 = j;
                AdjustOdds(steal1Nodes[part1], j);
            }
        }
        stealText += steal2Nodes[part2].Text + " ";
        stealText += room + "...";                                     // Add room section

        return stealText;
    }

    public void InitializeEnterTree()
    {
        string[] enter0, enter1, enter2, enter3, enter4;

        enter0 = File.ReadAllText("TextFiles\\Enter\\enter_0.txt").Split(':');
        enter1 = File.ReadAllText("TextFiles\\Enter\\enter_1.txt").Split(':');
        enter2 = File.ReadAllText("TextFiles\\Enter\\enter_2.txt").Split(':');
        enter3 = File.ReadAllText("TextFiles\\Enter\\enter_3.txt").Split(':');
        enter4 = File.ReadAllText("TextFiles\\Enter\\enter_4.txt").Split(':');


        foreach(string element in enter4)
        {
            enter4Nodes.Add(new TreeNode(element));
        }

        float odds4 = 1.00f / enter4Nodes.Count;
        List<float> enter4Odds = new List<float>();

        for(int i = 0; i < enter4Nodes.Count; i++)
        {
            enter4Odds.Add(odds4 * (i + 1));
        }


        foreach(string element in enter3)
        {
            enter3Nodes.Add(new TreeNode(element, enter4Nodes, enter4Odds));
        }

        float odds3 = 1.00f / enter3Nodes.Count;
        List<float> enter3Odds = new List<float>();

        for(int i = 0; i < enter3Nodes.Count; i++)
        {
            enter3Odds.Add(odds3 * (i + 1));
        }


        foreach(string element in enter2)
        {
            enter2Nodes.Add(new TreeNode(element, enter3Nodes, enter3Odds));
        }

        float odds2 = 1.00f / enter2Nodes.Count;
        List<float> enter2Odds = new List<float>();

        for(int i = 0; i < enter2Nodes.Count; i++)
        {
            enter2Odds.Add(odds2 * (i + 1));
        }


        foreach(string element in enter1)
        {
            enter1Nodes.Add(new TreeNode(element, enter2Nodes, enter2Odds));
        }

        float odds1 = 1.00f / enter1Nodes.Count;
        List<float> enter1Odds = new List<float>();

        for(int i = 0; i < enter1Nodes.Count; i++)
        {
            enter1Odds.Add(odds1 * (i + 1));
        }


        foreach(string element in enter0)
        {
            enter0Nodes.Add(new TreeNode(element, enter1Nodes, enter1Odds));
        }
    }

    public void InitializeTakeWeaponTree()
    {
        string[] weapon0, weapon1, weapon2, weapon3, weapon4;

        weapon0 = File.ReadAllText("TextFiles\\TakeWeapon\\weapon_0.txt").Split(':');
        weapon1 = File.ReadAllText("TextFiles\\TakeWeapon\\weapon_1.txt").Split(':');
        weapon2 = File.ReadAllText("TextFiles\\TakeWeapon\\weapon_2.txt").Split(':');
        weapon3 = File.ReadAllText("TextFiles\\TakeWeapon\\weapon_3.txt").Split(':');
        weapon4 = File.ReadAllText("TextFiles\\TakeWeapon\\weapon_4.txt").Split(':');


        foreach(string element in weapon4)
        {
            weapon4Nodes.Add(new TreeNode(element));
        }

        float odds4 = 1.00f / weapon4Nodes.Count;
        List<float> weapon4Odds = new List<float>();

        for(int i = 0; i < weapon4Nodes.Count; i++)
        {
            weapon4Odds.Add(odds4 * (i + 1));
        }


        foreach(string element in weapon3)
        {
            weapon3Nodes.Add(new TreeNode(element, weapon4Nodes, weapon4Odds));
        }

        float odds3 = 1.00f / weapon3Nodes.Count;
        List<float> weapon3Odds = new List<float>();

        for(int i = 0; i < weapon3Nodes.Count; i++)
        {
            weapon3Odds.Add(odds3 * (i + 1));
        }


        foreach(string element in weapon2)
        {
            weapon2Nodes.Add(new TreeNode(element, weapon3Nodes, weapon3Odds));
        }

        float odds2 = 1.00f / weapon2Nodes.Count;
        List<float> weapon2Odds = new List<float>();

        for(int i = 0; i < weapon2Nodes.Count; i++)
        {
            weapon2Odds.Add(odds2 * (i + 1));
        }


        foreach(string element in weapon1)
        {
            weapon1Nodes.Add(new TreeNode(element, weapon2Nodes, weapon2Odds));
        }

        float odds1 = 1.00f / weapon1Nodes.Count;
        List<float> weapon1Odds = new List<float>();

        for(int i = 0; i < weapon1Nodes.Count; i++)
        {
            weapon1Odds.Add(odds1 * (i + 1));
        }


        foreach(string element in weapon0)
        {
            weapon0Nodes.Add(new TreeNode(element, weapon1Nodes, weapon1Odds));
        }
    }

    public void InitializeMurderTree()
    {
        string[] murder0, murder1, murder2, murder3, murder4;

        murder0 = File.ReadAllText("TextFiles\\CommitMurder\\murder_0.txt").Split(':');
        murder1 = File.ReadAllText("TextFiles\\CommitMurder\\murder_1.txt").Split(':');
        murder2 = File.ReadAllText("TextFiles\\CommitMurder\\murder_2.txt").Split(':');
        murder3 = File.ReadAllText("TextFiles\\CommitMurder\\murder_3.txt").Split(':');
        murder4 = File.ReadAllText("TextFiles\\CommitMurder\\murder_4.txt").Split(':');


        foreach(string element in murder4)
        {
            murder4Nodes.Add(new TreeNode(element));
        }

        float odds4 = 1.00f / murder4Nodes.Count;
        List<float> murder4Odds = new List<float>();

        for(int i = 0; i < murder4Nodes.Count; i++)
        {
            murder4Odds.Add(odds4 * (i + 1));
        }


        foreach(string element in murder3)
        {
            murder3Nodes.Add(new TreeNode(element, murder4Nodes, murder4Odds));
        }

        float odds3 = 1.00f / murder3Nodes.Count;
        List<float> murder3Odds = new List<float>();

        for(int i = 0; i < murder3Nodes.Count; i++)
        {
            murder3Odds.Add(odds3 * (i + 1));
        }


        foreach(string element in murder2)
        {
            murder2Nodes.Add(new TreeNode(element, murder3Nodes, murder3Odds));
        }

        float odds2 = 1.00f / murder2Nodes.Count;
        List<float> murder2Odds = new List<float>();

        for(int i = 0; i < murder2Nodes.Count; i++)
        {
            murder2Odds.Add(odds2 * (i + 1));
        }


        foreach(string element in murder1)
        {
            murder1Nodes.Add(new TreeNode(element, murder2Nodes, murder2Odds));
        }

        float odds1 = 1.00f / murder1Nodes.Count;
        List<float> murder1Odds = new List<float>();

        for(int i = 0; i < murder1Nodes.Count; i++)
        {
            murder1Odds.Add(odds1 * (i + 1));
        }


        foreach(string element in murder0)
        {
            murder0Nodes.Add(new TreeNode(element, murder1Nodes, murder1Odds));
        }
    }

    public void InitializeHideWeaponTree()
    {
        string[] hide0, hide1, hide2, hide3;

        hide0 = File.ReadAllText("TextFiles\\HideWeapon\\hide_0.txt").Split(':');
        hide1 = File.ReadAllText("TextFiles\\HideWeapon\\hide_1.txt").Split(':');
        hide2 = File.ReadAllText("TextFiles\\HideWeapon\\hide_2.txt").Split(':');
        hide3 = File.ReadAllText("TextFiles\\HideWeapon\\hide_3.txt").Split(':');

        foreach(string element in hide3)
        {
            hide3Nodes.Add(new TreeNode(element));
        }

        float odds3 = 1.00f / hide3Nodes.Count;
        List<float> hide3Odds = new List<float>();

        for(int i = 0; i < hide3Nodes.Count; i++)
        {
            hide3Odds.Add(odds3 * (i + 1));
        }


        foreach(string element in hide2)
        {
            hide2Nodes.Add(new TreeNode(element, hide3Nodes, hide3Odds));
        }

        float odds2 = 1.00f / hide2Nodes.Count;
        List<float> hide2Odds = new List<float>();

        for(int i = 0; i < hide2Nodes.Count; i++)
        {
            hide2Odds.Add(odds2 * (i + 1));
        }


        foreach(string element in hide1)
        {
            hide1Nodes.Add(new TreeNode(element, hide2Nodes, hide2Odds));
        }

        float odds1 = 1.00f / hide1Nodes.Count;
        List<float> hide1Odds = new List<float>();

        for(int i = 0; i < hide1Nodes.Count; i++)
        {
            hide1Odds.Add(odds1 * (i + 1));
        }


        foreach(string element in hide0)
        {
            hide0Nodes.Add(new TreeNode(element, hide1Nodes, hide1Odds));
        }
    }

    public void InitializeClueTree()
    {
        string[] clue0, clue1, clue2, clue3;

        clue0 = File.ReadAllText("TextFiles\\Clue\\clue_0.txt").Split(':');
        clue1 = File.ReadAllText("TextFiles\\Clue\\clue_1.txt").Split(':');
        clue2 = File.ReadAllText("TextFiles\\Clue\\clue_2.txt").Split(':');
        clue3 = File.ReadAllText("TextFiles\\Clue\\clue_3.txt").Split(':');

        foreach(string element in clue3)
        {
            clue3Nodes.Add(new TreeNode(element));
        }

        float odds3 = 1.00f / clue3Nodes.Count;
        List<float> clue3Odds = new List<float>();

        for(int i = 0; i < clue3Nodes.Count; i++)
        {
            clue3Odds.Add(odds3 * (i + 1));
        }

        foreach(string element in clue2)
        {
            clue2Nodes.Add(new TreeNode(element, clue3Nodes, clue3Odds));
        }

        float odds2 = 1.00f / clue2Nodes.Count;
        List<float> clue2Odds = new List<float>();

        for(int i = 0; i < clue2Nodes.Count; i++)
        {
            clue2Odds.Add(odds2 * (i + 1));
        }


        foreach(string element in clue1)
        {
            clue1Nodes.Add(new TreeNode(element, clue2Nodes, clue2Odds));
        }

        float odds1 = 1.00f / clue1Nodes.Count;
        List<float> clue1Odds = new List<float>();

        for(int i = 0; i < clue1Nodes.Count; i++)
        {
            clue1Odds.Add(odds1 * (i + 1));
        }


        foreach(string element in clue0)
        {
            clue0Nodes.Add(new TreeNode(element, clue1Nodes, clue1Odds));
        }
    }

    public void InitializeEscapeTree()
    {
        string[] escape0, escape1, escape2, escape3;

        escape0 = File.ReadAllText("TextFiles\\Escape\\escape_0.txt").Split(':');
        escape1 = File.ReadAllText("TextFiles\\Escape\\escape_1.txt").Split(':');
        escape2 = File.ReadAllText("TextFiles\\Escape\\escape_2.txt").Split(':');
        escape3 = File.ReadAllText("TextFiles\\Escape\\escape_3.txt").Split(':');

        foreach(string element in escape3)
        {
            escape3Nodes.Add(new TreeNode(element));
        }

        float odds3 = 1.00f / escape3Nodes.Count;
        List<float> escape3Odds = new List<float>();

        for(int i = 0; i < escape3Nodes.Count; i++)
        {
            escape3Odds.Add(odds3 * (i + 1));
        }

        foreach(string element in escape2)
        {
            escape2Nodes.Add(new TreeNode(element, escape3Nodes, escape3Odds));
        }

        float odds2 = 1.00f / escape2Nodes.Count;
        List<float> escape2Odds = new List<float>();

        for(int i = 0; i < escape2Nodes.Count; i++)
        {
            escape2Odds.Add(odds2 * (i + 1));
        }


        foreach(string element in escape1)
        {
            escape1Nodes.Add(new TreeNode(element, escape2Nodes, escape2Odds));
        }

        float odds1 = 1.00f / escape1Nodes.Count;
        List<float> escape1Odds = new List<float>();

        for(int i = 0; i < escape1Nodes.Count; i++)
        {
            escape1Odds.Add(odds1 * (i + 1));
        }


        foreach(string element in escape0)
        {
            escape0Nodes.Add(new TreeNode(element, escape1Nodes, escape1Odds));
        }
    }

    public void InitializeStealTree()
    {
        string[] steal0, steal1, steal2;

        steal0 = File.ReadAllText("TextFiles\\Steal\\steal_0.txt").Split(':');
        steal1 = File.ReadAllText("TextFiles\\Steal\\steal_1.txt").Split(':');
        steal2 = File.ReadAllText("TextFiles\\Steal\\steal_2.txt").Split(':');


        foreach(string element in steal2)
        {
            steal2Nodes.Add(new TreeNode(element));
        }

        float odds2 = 1.00f / steal2Nodes.Count;
        List<float> steal2Odds = new List<float>();

        for(int i = 0; i < steal2Nodes.Count; i++)
        {
            steal2Odds.Add(odds2 * (i + 1));
        }


        foreach(string element in steal1)
        {
            steal1Nodes.Add(new TreeNode(element, steal2Nodes, steal2Odds));
        }

        float odds1 = 1.00f / steal1Nodes.Count;
        List<float> steal1Odds = new List<float>();

        for(int i = 0; i < steal1Nodes.Count; i++)
        {
            steal1Odds.Add(odds1 * (i + 1));
        }


        foreach(string element in steal0)
        {
            steal0Nodes.Add(new TreeNode(element, steal1Nodes, steal1Odds));
        }
    }

    List<TreeNode> Enter = new List<TreeNode>();
    List<TreeNode> GetWeapon = new List<TreeNode>();
    List<TreeNode> CommitMurder = new List<TreeNode>();
    List<TreeNode> HideWeapon = new List<TreeNode>();
    List<TreeNode> LeaveClue = new List<TreeNode>();
    List<TreeNode> Escape = new List<TreeNode>();
    List<TreeNode> StealValuable = new List<TreeNode>();

    List<TreeNode> enter0Nodes = new List<TreeNode>();
    List<TreeNode> enter1Nodes = new List<TreeNode>();
    List<TreeNode> enter2Nodes = new List<TreeNode>();
    List<TreeNode> enter3Nodes = new List<TreeNode>();
    List<TreeNode> enter4Nodes = new List<TreeNode>();

    List<TreeNode> weapon0Nodes = new List<TreeNode>();
    List<TreeNode> weapon1Nodes = new List<TreeNode>();
    List<TreeNode> weapon2Nodes = new List<TreeNode>();
    List<TreeNode> weapon3Nodes = new List<TreeNode>();
    List<TreeNode> weapon4Nodes = new List<TreeNode>();

    List<TreeNode> murder0Nodes = new List<TreeNode>();
    List<TreeNode> murder1Nodes = new List<TreeNode>();
    List<TreeNode> murder2Nodes = new List<TreeNode>();
    List<TreeNode> murder3Nodes = new List<TreeNode>();
    List<TreeNode> murder4Nodes = new List<TreeNode>();

    List<TreeNode> hide0Nodes = new List<TreeNode>();
    List<TreeNode> hide1Nodes = new List<TreeNode>();
    List<TreeNode> hide2Nodes = new List<TreeNode>();
    List<TreeNode> hide3Nodes = new List<TreeNode>();
    List<TreeNode> hide4Nodes = new List<TreeNode>();

    List<TreeNode> clue0Nodes = new List<TreeNode>();
    List<TreeNode> clue1Nodes = new List<TreeNode>();
    List<TreeNode> clue2Nodes = new List<TreeNode>();
    List<TreeNode> clue3Nodes = new List<TreeNode>();
    List<TreeNode> clue4Nodes = new List<TreeNode>();

    List<TreeNode> escape0Nodes = new List<TreeNode>();
    List<TreeNode> escape1Nodes = new List<TreeNode>();
    List<TreeNode> escape2Nodes = new List<TreeNode>();
    List<TreeNode> escape3Nodes = new List<TreeNode>();
    List<TreeNode> escape4Nodes = new List<TreeNode>();

    List<TreeNode> steal0Nodes = new List<TreeNode>();
    List<TreeNode> steal1Nodes = new List<TreeNode>();
    List<TreeNode> steal2Nodes = new List<TreeNode>();
    List<TreeNode> steal3Nodes = new List<TreeNode>();
    List<TreeNode> steal4Nodes = new List<TreeNode>();
}
