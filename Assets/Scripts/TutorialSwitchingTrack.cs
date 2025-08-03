using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSwitchingTrack : SwitchingTrainTrack
{
    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
        {
            FlipExit();
            FindObjectOfType<Tutorial>().Switch1Flicked(); 
        }
    }
}
