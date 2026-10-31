
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.Tests.CoreCLR.TestJobs;
using UnityEngine;

public sealed class world
{
    private static readonly world instance = new world();
    private static GameObject[] hidingSpots;

    static world()
    {
        hidingSpots = GameObject.FindGameObjectsWithTag("hide");
    }

    private world() { }


    public static world Instance
    {
        get { return instance; }
    }

    public GameObject[] GetHidingSpots()
    {
        return hidingSpots;
    }
}
