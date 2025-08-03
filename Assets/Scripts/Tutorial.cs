using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Tutorial : MonoBehaviour
{
    [SerializeField] private string[] scriptedMessages;
    private int popUpIndex = 0;

    [Header("UI")] 
    [SerializeField] private TextMeshProUGUI textPopUp;
    [SerializeField] private GameObject nextButton;

    [Header("Camera")] 
    [SerializeField] private CameraMovement camMovement;

    private bool camActive = false;
    
    [Header("Objects")]
    [SerializeField] private Train train;
    [SerializeField] private Station topStation;
    [SerializeField] private Station bottomStation;

    private bool hasSwitch1BeenFlicked = false;
    // Start is called before the first frame update
    void Start()
    {
        //Time.timeScale = 0.0f;
        //train.enabled = false;
        camMovement.enabled = false;
        camActive = false;
        ShowPopUp();
    }

    // Update is called once per frame
    void Update()
    {
        if (camActive)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (popUpIndex <= 8)
                {
                    popUpIndex = 8;
                    ShowPopUp();
                }
             
            }
            
            if (Input.mouseScrollDelta.magnitude > 0)
            {
                if (popUpIndex == 9)
                {
                    ShowPopUp();
                }
            }
            
            if (Input.GetMouseButtonDown(2))
            {
                if (popUpIndex == 10)
                {
                    ShowPopUp();
                }
            }
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (popUpIndex == 11)
                {
                    ShowPopUp();
                }
            }
            
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (popUpIndex == 12)
                {
                    nextButton.SetActive(true);
                    ShowPopUp();
                }
            }
        }
        
        
    }

    public void ShowPopUp()
    {
        if (popUpIndex >= scriptedMessages.Length)
        {
            SceneManager.LoadScene(0);
        }
        
        //special stuff
        switch (popUpIndex)
        {
            case 2:
                nextButton.SetActive(false);
                bottomStation.ActuallySpawnTheGuy();
                break;
            case 7:
                camMovement.enabled = true;
                camActive = true;
                nextButton.SetActive(false);
                break;
        }
        
        textPopUp.text = scriptedMessages[popUpIndex];
        popUpIndex++;
        
        
    }

    public void Switch1Flicked()
    {
        if (!hasSwitch1BeenFlicked)
        {
            hasSwitch1BeenFlicked = true;
            popUpIndex = 3;
            nextButton.SetActive(true);
            ShowPopUp();
        }
    }
    

}
