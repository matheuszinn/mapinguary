using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TMPro;
// using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerBehaviour : MonoBehaviour
{
    [Header("Health Settings")]
    public GameObject healthSlider;
    public float health = 100;
    public float healthMax = 100;
    public float healValue = 5;
    public float secondToHeal = 10;

    [Header("Flashlight Battery Settings")]
    public GameObject Flashlight;
    public GameObject batterySlider;
    public float battery = 100;
    public float batteryMax = 100;
    public float removeBatteryValue = 0.05f;
    public float secondToRemoveBaterry = 5f;
    private bool isFlashlightOn = true;

    [Header("Audio Settings")]
    public AudioClip slenderNoise;
    public AudioClip cameraNoise;

    [Header("Page System Settings")]
    public List<GameObject> pages = new List<GameObject>();
    public int collectedPages;

    [Header("UI Settings")] 
    public GameObject fakeCameraRenderer;
    public GameObject inGameMenuUI;
    public GameObject pickUpUI;
    public GameObject noteContentUI;
    public GameObject finishedGameUI;
    public GameObject pagesCount;
    public bool paused;

	void Start ()
    {
        // set initial health values
        health = healthMax;
        battery = batteryMax;

        healthSlider.GetComponent<Slider>().maxValue = healthMax;
        healthSlider.GetComponent<Slider>().value = healthMax;

        // set initial battery values
        batterySlider.GetComponent<Slider>().maxValue = batteryMax;
        batterySlider.GetComponent<Slider>().value = batteryMax;

        noteContentUI.gameObject.GetComponentInChildren<Button>().onClick.AddListener(CloseNoteUI);
        // start consume flashlight battery
        StartCoroutine(RemoveBaterryCharge(removeBatteryValue, secondToRemoveBaterry));
    }

    public void CloseNoteUI()
    {
        fakeCameraRenderer.SetActive(true);
        Debug.Log("Closing Page UI");
        noteContentUI.SetActive(false);
        paused = false;
    }

    public void OpenNoteUI(GameObject noteGameObject)
    {
        var noteText = noteGameObject.gameObject.GetComponentInChildren<Text>().text;
        noteContentUI.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = noteText;
        fakeCameraRenderer.SetActive(false);
        Debug.Log("Opening Page UI");
        noteContentUI.SetActive(true);
        paused = true;
    }
    
	private void ToggleFlashlight()
	{
		Debug.Log("Toggle Flashlight");
		isFlashlightOn = !isFlashlightOn;
		
		if (isFlashlightOn)
		{
			Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 5.5f;
			return;
		}
		
		Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 0.0f;
	}
	
	void Update ()
    {
	    // enable or disable flashlight
	    if (Input.GetKeyDown(KeyCode.Tab))
	    {
		    ToggleFlashlight();
	    }
	    
        // update player health slider
        healthSlider.GetComponent<Slider>().value = health;

        // update baterry slider
        batterySlider.GetComponent<Slider>().value = battery;

        // if health is low than 20%
        if(health / healthMax * 100 <= 20 && health / healthMax * 100 != 0)
        {
            Debug.Log("You are dying.");
            this.GetComponent<AudioSource>().PlayOneShot(slenderNoise);
        }

        // if health is low than 0
        if (health / healthMax * 100 <= 0)
        {
            Debug.Log("You are dead.");
            health = 0.0f;
        }

        if (isFlashlightOn)
        {
	        // if battery is low 50%
	        if (battery / batteryMax * 100 <= 50)
	        {
		        Debug.Log("Flashlight is running out of battery.");
		        Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 2.85f;
	        }

	        // if battery is low 25%
	        if (battery / batteryMax * 100 <= 25)
	        {
		        Debug.Log("Flashlight is almost without battery.");
		        Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 2.0f;
	        }

	        // if battery is low 10%
	        if (battery / batteryMax * 100 <= 10)
	        {
		        Debug.Log("You will be out of light.");
		        Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 1.35f;           
	        }

	        // if battery out%
	        if (battery / batteryMax * 100 <= 0)
	        {
		        battery = 0.00f;
		        Debug.Log("The flashlight battery is out and you are out of the light.");
		        Flashlight.transform.Find("Spotlight").gameObject.GetComponent<Light>().intensity = 0.0f;
	        }
        }

        // page system
        pagesCount.GetComponent<Text>().text = "Notas Coletadas: " + collectedPages + "/8";

        //animations
        if (Input.GetKey(KeyCode.LeftShift))
            this.gameObject.GetComponent<Animation>().CrossFade("Run", 1);
        else
            this.gameObject.GetComponent<Animation>().CrossFade("Idle", 1);

        // collected all pages
        if (collectedPages >= 8)
        {
            Debug.Log("Você fugiu do Mapinguas!");
            Cursor.visible = true;

            // disable first person controller and show finished game UI
            this.gameObject.GetComponent<FirstPersonController>().enabled = false;
            inGameMenuUI.SetActive(false);
            finishedGameUI.SetActive(true);       

            // set play again button
            Button playAgainBtn = finishedGameUI.gameObject.transform.Find("PlayAgainBtn").GetComponent<Button>();
            playAgainBtn.onClick.AddListener(this.gameObject.GetComponent<MenuInGame>().PlayAgain);

            // set quit button
            Button quitBtn = finishedGameUI.gameObject.transform.Find("QuitBtn").GetComponent<Button>();
            quitBtn.onClick.AddListener(this.gameObject.GetComponent<MenuInGame>().QuitGame);
        } 
    }

    public IEnumerator RemoveBaterryCharge(float value, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            if (!isFlashlightOn) continue;

            Debug.Log("Removing baterry value: " + value);

            if (battery > 0)
                battery -= value;
            else
                Debug.Log("The flashlight battery is out");
        }
    }

    public IEnumerator RemovePlayerHealth(float value, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            Debug.Log("Removing player health value: " + value);

            if (health > 0)
                health -= value;
            else
            {
                Debug.Log("You're dead");
                paused = true;
                inGameMenuUI.SetActive(true);
                inGameMenuUI.transform.Find("ContinueBtn").gameObject.GetComponent<Button>().interactable = false;
                inGameMenuUI.transform.Find("PlayAgainBtn").gameObject.GetComponent<Button>().interactable = true;
            }
        }
    }

    // function to heal player
    public IEnumerator StartHealPlayer(float value, float time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);

            Debug.Log("Healling player value: " + value);

            if (health > 0 && health < healthMax)
                health += value;
            else
                health = healthMax;
        }
    }

    // page system - show UI
    private void OnTriggerEnter(Collider collider)
    {
        // start noise when reach slender
        if (collider.gameObject.transform.tag == "Slender")
        {
            if (health > 0 && paused == false)
            {
                this.GetComponent<AudioSource>().PlayOneShot(cameraNoise);
                this.GetComponent<AudioSource>().loop = true;
            }            
        }

        if (collider.gameObject.transform.tag == "Page")
        {
            Debug.Log("You Found a Page: " + collider.gameObject.name + ", Press 'E' to pickup");
            pickUpUI.SetActive(true);      
        }
    }

    // page system - pickup system
    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.transform.CompareTag("Page"))
        {       
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("You get this page: " + collider.gameObject.name);

                // disable UI
                pickUpUI.SetActive(false);
                
                // Open Note UI
                OpenNoteUI(collider.gameObject);
                
                // add page to list
                pages.Add(collider.gameObject);
                collectedPages ++;

                // disable game object
                collider.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        // remove noise sound
        if (collider.gameObject.transform.tag == "Slender")
        {
            if (health > 0 && paused == false)
            {
                this.GetComponent<AudioSource>().clip = null;
                this.GetComponent<AudioSource>().loop = false;
            }          
        }

        // disable UI
        if (collider.gameObject.transform.tag == "Page")
            pickUpUI.SetActive(false);
    }
}
