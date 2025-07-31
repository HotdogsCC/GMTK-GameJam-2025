using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

public class TrackBuilder : EditorWindow
{
    //have we subscribed to the delegate
    private bool delegateSubbed = false;
    
    //whether selection mode is on
    private bool isBuilding = false;
    
    //whether buttons are currently held
    private bool isControlDown = false;
    private bool isAltDown = false;
    
    //prefabs of trains
    private GameObject regularTrainTrackPrefab;
    
    //what prefab is selected?
    private GameObject selectedPrefab;
    //current selector instance
    private GameObject currentSelectionInstance;
    //tracks parent
    private GameObject tracksParent;

    [MenuItem("Train Tools/Track Builder")]
    public static void ShowWindow()
    {
        GetWindow(typeof(TrackBuilder));
    }

    private void OnFocus()
    {
        if (delegateSubbed) return;
        
        SceneView.duringSceneGui += OnSceneGUI;
        delegateSubbed = true;
    }

    private void OnLostFocus()
    {
        if (!delegateSubbed) return;
        
        SceneView.duringSceneGui -= OnSceneGUI;
        delegateSubbed = false;
    }
    

    private void OnGUI()
    {
        GUILayout.Label("Controls:");
        GUILayout.Label("Place: Control");
        GUILayout.Label("Rotate: Alt");
        GUILayout.Label(" ");
        
        GUILayout.Label("Prefabs");
        regularTrainTrackPrefab = EditorGUILayout.ObjectField("Regular Train Track Prefab", 
            regularTrainTrackPrefab, typeof(GameObject), false) as GameObject;

        GUILayout.Label("Debugger");
        if (GUILayout.Button("View Debug Tracks"))
        {
            TrackBase[] tracks = FindObjectsOfType<TrackBase>();
            List<Vector3> trackPositions = new List<Vector3>();
            foreach (TrackBase track in tracks)
            {
                //check for overlaps
                foreach (Vector3 pos in trackPositions)
                {
                    if (track.GetPos() == pos)
                    {
                        Debug.LogWarning("You have an overlapping train track at: " + pos);
                        
                        Vector3 TexitPos1 = track.GetExit1Pos();
                        Vector3 TexitPos2 = track.GetExit2Pos();

                        TexitPos1.y += 3;
                        TexitPos2.y += 3;
                        
                        Debug.DrawLine(TexitPos1, TexitPos2, Color.red, 5.0f);
                        return;
                    }
                }

                trackPositions.Add(track.GetPos());
                
                Vector3 exitPos1 = track.GetExit1Pos();
                Vector3 exitPos2 = track.GetExit2Pos();

                exitPos1.y += 2;
                exitPos2.y += 2;
                
                Debug.DrawLine(exitPos1, exitPos2, Color.green, 5.0f);
                
            }
        }

        GUILayout.Label("On and Off");
        if (GUILayout.Button("Start Building"))
        {
            isBuilding = true;
            
            //see if there is a parent "Tracks" game object
            GameObject tracksTemp = GameObject.Find("Tracks");
            if (tracksTemp)
            {
                tracksParent = tracksTemp;
            }
            else
            {
                tracksParent = new GameObject();
                tracksParent.name = "Tracks";
            }
            
            if (!currentSelectionInstance)
            {
                ChangeSelection(regularTrainTrackPrefab);
            }
        }
        if (GUILayout.Button("Stop Building"))
        {
            isBuilding = false;

            if (currentSelectionInstance)
            {
                DestroyImmediate(currentSelectionInstance);
            }
        }
        
        GUILayout.Label("Select Train Track");
        if (GUILayout.Button("Regular Track"))
        {
            ChangeSelection(regularTrainTrackPrefab);
        }
         
        
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!isBuilding) return;

        Event e = Event.current;
        
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, 31))
        {
            MoveTrackSelector(hit);
        }

        if (e.control)
        {
            //dont do anything if control is still head
            if (isControlDown) return;

            isControlDown = true;
            
            PlaceTrack();
        }
        else
        {
            isControlDown = false;
        }
        

    }

    private void ChangeSelection(GameObject newSelection)
    {
        if (!isBuilding)
        {
            return;
        }
        
        if (currentSelectionInstance)
        {
            DestroyImmediate(currentSelectionInstance);
        }

        currentSelectionInstance = Instantiate(newSelection);
        currentSelectionInstance.name = "temp (quentin has a smelly bum)";
    }

    private void PlaceTrack()
    {
        GameObject placedTrack = Instantiate(currentSelectionInstance, tracksParent.transform);
        placedTrack.name = "Track";
    }

    private void MoveTrackSelector(RaycastHit hit)
    {
        if (currentSelectionInstance)
        {
            Vector3 roundedPosition = hit.point;
            roundedPosition.x = Mathf.Round(roundedPosition.x);
            roundedPosition.y = Mathf.Round(roundedPosition.y);
            roundedPosition.z = Mathf.Round(roundedPosition.z);

            currentSelectionInstance.transform.position = roundedPosition;
        }
    }
}


