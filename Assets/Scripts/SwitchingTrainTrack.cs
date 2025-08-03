using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class SwitchingTrainTrack : TrackBase
{
    [SerializeField] private Transform guarenteedExit;
    [SerializeField] private MeshRenderer leftMesh;
    [SerializeField] private MeshRenderer rightMesh;
    
    [Header("Indicator Control")]
    [SerializeField] private SwitchIndicator switchIndicator;

    [Header("Old Indicator Renderers")]
    [SerializeField] private MeshRenderer leftIndicator;
    [SerializeField] private MeshRenderer rightIndicator;
    
    private MusicPlayer musicPlayer; 

    [Header("Set Me! Set Me!")]
    [SerializeField] private bool exitLeft = true;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material unselectedMaterial;

    public void SetActiveTrackMaterial()
    {
        if (leftIndicator == null || rightIndicator == null) return;

        if(exitLeft)
        {
            leftIndicator.enabled = true;
            rightIndicator.enabled = false;
        }
        else
        {
            leftIndicator.enabled = false;
            rightIndicator.enabled = true;
        }
    }
    
    public override Vector3 GetExit1Pos()
    {
        return guarenteedExit.position;
    }

    public override Vector3 GetExit2Pos()
    {
        if(exitLeft)
        {
            return exitTransform1.position;
        }
        else
        {
            return exitTransform2.position;
        }
    }

    public Vector3 GetInactiveExit()
    {
        if(exitLeft)
        {
            return exitTransform2.position;
        }
        else
        {
            return exitTransform1.position;
        }
    }

    public void FlipExit()
    {
        exitLeft = !exitLeft;
        
        if(musicPlayer != null)
            musicPlayer.PlaySnap();

        UpdateIndicatorState();
    }

    private void Start()
    {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        
        leftMesh.material = selectedMaterial;
        rightMesh.material = selectedMaterial;
        
        // Set the initial state of the indicator without animation
        if (switchIndicator != null)
        {
            if (exitLeft)
            {
                switchIndicator.SetLeft();
            }
            else
            {
                switchIndicator.SetRight();
            }
        }
    }
    
    public void UpdateIndicatorState()
    {
        if(switchIndicator == null) return;
        
        if(exitLeft)
        {
            switchIndicator.SwitchToLeft();
        }
        else
        {
            switchIndicator.SwitchToRight();
        }
    }

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
        {
            FlipExit();
        }
    }
}
