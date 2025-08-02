using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchingTrainTrack : TrackBase
{
    [SerializeField] private Transform guarenteedExit;
    [SerializeField] private MeshRenderer leftMesh;
    [SerializeField] private MeshRenderer rightMesh;
    [SerializeField] private MeshRenderer leftIndicator;
    [SerializeField] private MeshRenderer rightIndicator;
    private MusicPlayer musicPlayer; 

    [Header("Set Me! Set Me!")]
    [SerializeField] private bool exitLeft = true;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material unselectedMaterial;

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
        //flips the value of exitLeft
        exitLeft = !exitLeft;
        
        //plays the sound
        musicPlayer.PlaySnap();

        SetActiveTrackMaterial();
    }

    private void Start()
    {
        musicPlayer = FindObjectOfType<MusicPlayer>();
        
        leftMesh.material = selectedMaterial;
        rightMesh.material = selectedMaterial;
        
        SetActiveTrackMaterial();
    }

    public void SetActiveTrackMaterial()
    {
        
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

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
        {
            FlipExit();
        }
    }
}
