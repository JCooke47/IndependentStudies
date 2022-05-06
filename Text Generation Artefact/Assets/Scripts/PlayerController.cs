using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float speed = 0;
    private Rigidbody rb;
    private float movementX;
    private float movementY;

    public GameObject scenarioUI;
    public GameObject tasksUI;
    public GameObject cluePrefab;
    public TextMeshProUGUI currentLocation;
    public TextMeshProUGUI generatedScenario;
    public TextMeshProUGUI generatedQuests;
    
    GenTimeline genTimeline;
    GenQuests genQuests;
    AssetManager assetManager;

    void Update()
    {
        if(Keyboard.current[Key.G].wasPressedThisFrame)
        {
            cluePrefab.SetActive(false);
            assetManager.DisableAllClues();
            genTimeline.GenerateTask();
            genQuests.GenerateQuests();
            assetManager.SetAssetOrder();
        }

        if(Keyboard.current[Key.J].wasPressedThisFrame)
        {
            genQuests.GenerateQuests();
        }

        if(Keyboard.current[Key.Tab].wasPressedThisFrame)
        {
            scenarioUI.SetActive(!scenarioUI.activeInHierarchy);
            tasksUI.SetActive(!tasksUI.activeInHierarchy);
        }
    }

    void SetLocationText(string location)
    {
        currentLocation.text = "Location: " + location;
    }

    void OnTriggerEnter(Collider other)
    {  
        if(other.gameObject.CompareTag("Pickup"))
        {
            other.gameObject.SetActive(false);
            assetManager.ClueCollected();
        }
        else
        {
            SetLocationText(other.name);
        }     
    }


    /* ---------- Player controller stuff ---------- */

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        scenarioUI.SetActive(false);
        tasksUI.SetActive(false);

        genTimeline = GameObject.FindGameObjectWithTag("timeline").GetComponent<GenTimeline>();
        genQuests = GameObject.FindGameObjectWithTag("quests").GetComponent<GenQuests>();
        assetManager = GameObject.FindGameObjectWithTag("assetmanager").GetComponent<AssetManager>();
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

}