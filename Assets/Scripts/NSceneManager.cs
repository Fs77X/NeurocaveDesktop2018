using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSceneManager : MonoBehaviour
{

    public DataLoader connectomeDataLoader;
    public ConnectomeBuilder connectomeBuilder;

    private string[] connectomeFolderNames;
    private List<Dictionary<string, string[][]>> connectomeList = new List<Dictionary<string, string[][]>>();
    Dictionary<string, List<string>> _connectomeTopologiesDictionary = new Dictionary<string, List<string>>();
    // Use this for initialization
    void Start()
    {

        connectomeList = connectomeDataLoader.LoadConnectomes(out _connectomeTopologiesDictionary);
        connectomeFolderNames = connectomeDataLoader.GetConnectomeFolderName();
        connectomeBuilder.Build(connectomeList, connectomeFolderNames, false, _connectomeTopologiesDictionary);
    }
   
}
