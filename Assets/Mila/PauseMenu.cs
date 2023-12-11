using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] GameObject resumeButton;


    void Start()
    {
        
        EventSystem.current.SetSelectedGameObject(resumeButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
 
}
