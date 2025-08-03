using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;



public class TrackBuilder : EditorWindow
{
    private enum ESelectedPrefab
    {
        Horizontal,
        Diagonal,
        EdgeToLeftCorner,
        EdgeToRightCorner,
        CornerToY,
        EdgeToY,
        LeftY,
        RightY,
        DiaLeftY,
        DiaRightY,
        Plus,
        X,
        FlatXLeft,
        FlatXRight,
        SIZE
    }
    
    //have we subscribed to the delegate
    private bool delegateSubbed = false;
    
    //whether selection mode is on
    private bool isBuilding = false;
    
    //whether buttons are currently held
    private bool isControlDown = false;
    private bool isAltDown = false;
    private bool isShiftDown = false;
    
    //prefabs of trains
    private GameObject regularTrainTrackPrefab;
    private GameObject diagonalTrainTrackPrefab;
    private GameObject edgeToLeftCornerPrefab;
    private GameObject edgeToRightCornerPrefab;
    private GameObject cornerToYPrefab;
    private GameObject edgeToYPrefab;
    private GameObject leftYPrefab;
    private GameObject rightYPrefab;
    private GameObject diaLeftYPrefab;
    private GameObject diaRightYPrefab;
    private GameObject plusPrefab;
    private GameObject xPrefab;
    private GameObject flatXLeftPrefab;
    private GameObject flatXRightPrefab;
    
    //what prefab is selected?
    private ESelectedPrefab selectedPrefab;
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
        //list the controls
        GUILayout.Label("Controls:");
        GUILayout.Label("Place: Control");
        GUILayout.Label("Rotate: Alt");
        GUILayout.Label("Change Selection: Shift");
        GUILayout.Label(" ");
        
        //list the prefabs
        GUILayout.Label("Prefabs");
        if (GUILayout.Button("Auto Load Prefabs"))
        {
            regularTrainTrackPrefab = Resources.Load("Regular Train Track").GameObject();
            diagonalTrainTrackPrefab = Resources.Load("Diagonal Train Track").GameObject();
            edgeToLeftCornerPrefab = Resources.Load("EdgeToLeftCorner").GameObject();
            edgeToRightCornerPrefab = Resources.Load("EdgeToRightCorner").GameObject();
            cornerToYPrefab = Resources.Load("CornerToY").GameObject();
            edgeToYPrefab = Resources.Load("EdgeToY").GameObject();
            leftYPrefab = Resources.Load("LeftY").GameObject();
            rightYPrefab = Resources.Load("RightY").GameObject();
            diaLeftYPrefab = Resources.Load("DiaLeftY").GameObject();
            diaRightYPrefab = Resources.Load("DiaRightY").GameObject();
            plusPrefab = Resources.Load("Plus").GameObject();
            xPrefab = Resources.Load("X").GameObject();
            flatXLeftPrefab = Resources.Load("FlatXLeft").GameObject();
            flatXRightPrefab = Resources.Load("FlatXRight").GameObject();
            
        }
        regularTrainTrackPrefab = EditorGUILayout.ObjectField("Regular Train Track Prefab", 
            regularTrainTrackPrefab, typeof(GameObject), false) as GameObject;
        diagonalTrainTrackPrefab = EditorGUILayout.ObjectField("Diagonal Train Track Prefab", 
            diagonalTrainTrackPrefab, typeof(GameObject), false) as GameObject;
        edgeToLeftCornerPrefab = EditorGUILayout.ObjectField("Edge To Left Corner Prefab", 
            edgeToLeftCornerPrefab, typeof(GameObject), false) as GameObject;
        edgeToRightCornerPrefab = EditorGUILayout.ObjectField("Edge To Right Corner Prefab", 
            edgeToRightCornerPrefab, typeof(GameObject), false) as GameObject;
        cornerToYPrefab = EditorGUILayout.ObjectField("Corner To Y Prefab", 
            cornerToYPrefab, typeof(GameObject), false) as GameObject;
        edgeToYPrefab = EditorGUILayout.ObjectField("Edge To Y Prefab", 
            edgeToYPrefab, typeof(GameObject), false) as GameObject;
        leftYPrefab = EditorGUILayout.ObjectField("Left Y Prefab", 
            leftYPrefab, typeof(GameObject), false) as GameObject;
        rightYPrefab = EditorGUILayout.ObjectField("Right Y Prefab", 
            rightYPrefab, typeof(GameObject), false) as GameObject;
        diaLeftYPrefab = EditorGUILayout.ObjectField("Dia Left Y Prefab", 
            diaLeftYPrefab, typeof(GameObject), false) as GameObject;
        diaRightYPrefab = EditorGUILayout.ObjectField("Dia Right Y Prefab", 
            diaRightYPrefab, typeof(GameObject), false) as GameObject;
        plusPrefab = EditorGUILayout.ObjectField("Plus Prefab", 
            plusPrefab, typeof(GameObject), false) as GameObject;
        xPrefab = EditorGUILayout.ObjectField("X Prefab", 
            xPrefab, typeof(GameObject), false) as GameObject;
        flatXLeftPrefab = EditorGUILayout.ObjectField("Flat X Left Prefab", 
            flatXLeftPrefab, typeof(GameObject), false) as GameObject;
        flatXRightPrefab = EditorGUILayout.ObjectField("Flat X Right Prefab", 
            flatXRightPrefab, typeof(GameObject), false) as GameObject;
        GUILayout.Label(" ");

        //list the debuggers
        GUILayout.Label("Debugger");
        if (GUILayout.Button("View Debug Tracks"))
        {
            DebugTracks();
        }
        if (GUILayout.Button("Set Materials on Switching Tracks"))
        {
            SwitchingTrainTrack[] tracks = FindObjectsOfType<SwitchingTrainTrack>();
            foreach (SwitchingTrainTrack track in tracks)
            {
                track.SetActiveTrackMaterial();
            }
        }
        GUILayout.Label(" ");

        //list the build start and stop
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
                ChangeSelection(ESelectedPrefab.Horizontal);
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
        GUILayout.Label(" ");
        
        //list the train track types
        GUILayout.Label("Select Train Track");
        if (GUILayout.Button("Regular Track"))
        {
            ChangeSelection(ESelectedPrefab.Horizontal);
        }
        if (GUILayout.Button("Diagonal Track"))
        {
            ChangeSelection(ESelectedPrefab.Diagonal);
        }
        if (GUILayout.Button("Edge to Left Corner Track"))
        {
            ChangeSelection(ESelectedPrefab.EdgeToLeftCorner);
        }
        if (GUILayout.Button("Edge to Right Corner Track"))
        {
            ChangeSelection(ESelectedPrefab.EdgeToRightCorner);
        }
        if (GUILayout.Button("Corner to Y Track"))
        {
            ChangeSelection(ESelectedPrefab.CornerToY);
        }
        if (GUILayout.Button("Edge to Y Track"))
        {
            ChangeSelection(ESelectedPrefab.EdgeToY);
        }
        if (GUILayout.Button("Left Y Track"))
        {
            ChangeSelection(ESelectedPrefab.LeftY);
        }
        if (GUILayout.Button("Right Y Track"))
        {
            ChangeSelection(ESelectedPrefab.RightY);
        }
        if (GUILayout.Button("Dia Left Y Track"))
        {
            ChangeSelection(ESelectedPrefab.DiaLeftY);
        }
        if (GUILayout.Button("Dia Right Y Track"))
        {
            ChangeSelection(ESelectedPrefab.DiaRightY);
        }
        if (GUILayout.Button("Plus Track"))
        {
            ChangeSelection(ESelectedPrefab.Plus);
        }
        if (GUILayout.Button("X Track"))
        {
            ChangeSelection(ESelectedPrefab.X);
        }
        if (GUILayout.Button("Flat X Left"))
        {
            ChangeSelection(ESelectedPrefab.FlatXLeft);
        }
        if (GUILayout.Button("Flat X Right"))
        {
            ChangeSelection(ESelectedPrefab.FlatXRight);
        }
         
        
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        //if we arent building, dont do anything
        if (!isBuilding) return;

        //get where the mouse is
        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, 31))
        {
            MoveTrackSelector(hit);
        }

        //is control pressed?
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

        //is alt pressed?
        if (e.alt)
        {
            if (isAltDown) return;

            isAltDown = true;
            
            RotateTrack();
        }
        else
        {
            isAltDown = false;
        }
        
        //is shift pressed?
        if (e.shift)
        {
            if (isShiftDown) return;

            isShiftDown = true;
            
            IncrementSelection();
        }
        else
        {
            isShiftDown = false;
        }

    }

    private void ChangeSelection(ESelectedPrefab selection)
    {
        if (!isBuilding)
        {
            return;
        }
        
        selectedPrefab = selection;
        
        if (currentSelectionInstance)
        {
            DestroyImmediate(currentSelectionInstance);
        }
        
        
        currentSelectionInstance = Instantiate(GetCurrentPrefab());
        
        currentSelectionInstance.name = "temp (quentin has a smelly bum)";
        
        
    }

    private void IncrementSelection()
    {
        selectedPrefab++;
        if (selectedPrefab == ESelectedPrefab.SIZE)
        {
            selectedPrefab = 0;
        }
        
        ChangeSelection(selectedPrefab);
    }

    private void PlaceTrack()
    {
        Object goat = PrefabUtility.InstantiatePrefab(GetCurrentPrefab(), tracksParent.transform);
        
        
       
        goat.GameObject().transform.position = currentSelectionInstance.transform.position;
        goat.GameObject().transform.rotation = currentSelectionInstance.transform.rotation;
    }

    private void RotateTrack()
    {
        currentSelectionInstance.transform.Rotate(new Vector3(0, 90.0f, 0));
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

    private GameObject GetCurrentPrefab()
    {
        GameObject toReturn = null;
        switch(selectedPrefab)
        {
            case ESelectedPrefab.Horizontal:
                toReturn = regularTrainTrackPrefab;
                break;
            
            case ESelectedPrefab.Diagonal:
                toReturn = diagonalTrainTrackPrefab;
                break;
            
            case ESelectedPrefab.EdgeToLeftCorner:
                toReturn = edgeToLeftCornerPrefab;
                break;
            
            case ESelectedPrefab.EdgeToRightCorner:
                toReturn = edgeToRightCornerPrefab;
                break;
            
            case ESelectedPrefab.CornerToY:
                toReturn = cornerToYPrefab;
                break;
            
            case ESelectedPrefab.EdgeToY:
                toReturn = edgeToYPrefab;
                break;
            
            case ESelectedPrefab.LeftY:
                toReturn = leftYPrefab;
                break;
            
            case ESelectedPrefab.DiaLeftY:
                toReturn = diaLeftYPrefab;
                break;
            
            case ESelectedPrefab.DiaRightY:
                toReturn = diaRightYPrefab;
                break;
            
            case ESelectedPrefab.RightY:
                toReturn = rightYPrefab;
                break;
            
            case ESelectedPrefab.Plus:
                toReturn = plusPrefab;
                break;
            
            case ESelectedPrefab.X:
                toReturn = xPrefab;
                break;
            
            case ESelectedPrefab.FlatXLeft:
                toReturn = flatXLeftPrefab;
                break;
            
            case ESelectedPrefab.FlatXRight:
                toReturn = flatXRightPrefab;
                break;
        }

        if (!toReturn) 
        {
            Debug.Log("Hello Designer! Click on Auto Load Prefabs, and this error should fix itself! :)");
        }

        return toReturn;
    }

    private void DebugTracks()
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

                    TexitPos1.y += 0.2f;
                    TexitPos2.y += 0.2f;

                    Debug.DrawLine(TexitPos1, TexitPos2, Color.red, 5.0f);
                    return;
                }
            }

            trackPositions.Add(track.GetPos());

            Vector3 exitPos1 = track.GetExit1Pos();
            Vector3 exitPos2 = track.GetExit2Pos();

            exitPos1.y += 0.1f;
            exitPos2.y += 0.1f;

            Debug.DrawLine(exitPos1, exitPos2, Color.green, 5.0f);

            //is it a switching track?
            SwitchingTrainTrack switchingTrack;
            if(track.TryGetComponent<SwitchingTrainTrack>(out switchingTrack))
            {
                exitPos2 = switchingTrack.GetInactiveExit();
                exitPos2.y += 0.1f;

                Debug.DrawLine(exitPos1, exitPos2, Color.magenta, 5.0f);
                
            }
            

        }
    }
}


