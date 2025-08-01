using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegTrainTracks : TrackBase
{
    public override Vector3 GetExit1Pos()
    {
        return exitTransform1.position;
    }

    public override Vector3 GetExit2Pos()
    {
        return exitTransform2.position;
    }
}
