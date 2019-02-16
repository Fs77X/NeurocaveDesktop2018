using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public class DataLoader : MonoBehaviour {

    public string m_Path;
    public Dictionary<string, string[][]> connectomeData;

    private List<string> _topologiesList = new List<string> { "anatomy", "isomap", "tsne", "mds" };
    //substring char count to cut the path from 'Assets/Resources/Data/' to 'Data/' for Resources
    private const int subStringStartChar = 17;
    private string[] _connectomeFolders;
    //private List<string> _topologies = new List<string>();
    

    void Awake()
    {
        _connectomeFolders = System.IO.Directory.GetDirectories(m_Path, "*", System.IO.SearchOption.TopDirectoryOnly);
        Debug.Log("first "+_connectomeFolders.Length);

    }

    void ReadCSVFile(string connectomePath, string filePath, string FileType)
    {
        filePath = filePath.Substring(subStringStartChar);
        TextAsset initialdata = Resources.Load<TextAsset>(filePath);
     
        string[] data = initialdata.ToString().Split('\n' );
        string[][] dataMatrix = new string[data.Length][];

        for (int i = 1; i < data.Length; i++)
            dataMatrix[i] = data[i].Split(',');

        connectomeData.Add(FileType, dataMatrix);
    }

    public string[] GetConnectomeFolderName()
    {

        string[] connectomeFolderName =new string[_connectomeFolders.Length];

        for(int i=0; i< _connectomeFolders.Length;i++)
            connectomeFolderName[i] = new DirectoryInfo(_connectomeFolders[i]).Name;
      
        return connectomeFolderName;
    }

    // Use this for initialization
    public List<Dictionary<string, string[][]>> LoadConnectomes(out Dictionary<string, List<string>> _connectomeTopologiesDictionary)
    {
        List<Dictionary<string, string[][]>> connectomeList = new List<Dictionary<string, string[][]>>();
        //Dictionary<string, string[]> _connectomeTopologiesDictionary = new Dictionary<string, string[]>();
        _connectomeTopologiesDictionary = new Dictionary<string, List<string>>();

        Debug.Log("SECOBD " + _connectomeFolders.Length);

        foreach (string connectome in _connectomeFolders)
        {
            Dictionary<string, string> filePath = new Dictionary<string, string>();
            //Dictionary<string, string[]> dataArray = new Dictionary<string, string[]>();
            List<string> _connectomeTopologies = new List<string>();


            connectomeData = new Dictionary<string, string[][]>();

            filePath.Add("anatomy", connectome + "/topologies/anatomy");
            filePath.Add("isomap", connectome + "/topologies/isomap");
            filePath.Add("tsne", connectome + "/topologies/tsne");
            filePath.Add("mds", connectome + "/topologies/mds");
            filePath.Add("label", connectome + "/labels/label");
            filePath.Add("place", connectome + "/labels/place");
            filePath.Add("Q", connectome + "/labels/Q");
            filePath.Add("LookupTable", connectome + "/atlas/LookupTable_freesurfer");
            filePath.Add("NW", connectome + "/edges/NW");

            //_topologies = new List<string>();
            foreach (KeyValuePair<string, string> file in filePath)
                if (System.IO.File.Exists(file.Value + ".csv"))
                { 
                    ReadCSVFile(connectome, file.Value, file.Key);
                    if (_topologiesList.Contains(file.Key))
                    {
                        _connectomeTopologies.Add(file.Key);
                        
                    }
                       
                }
            //Debug.Log(new DirectoryInfo(connectome).Name);
            _connectomeTopologiesDictionary.Add(new DirectoryInfo(connectome).Name, _connectomeTopologies);
            
            connectomeList.Add(connectomeData); 
        }
        return connectomeList;
    }
}
