using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleConnectome : MonoBehaviour
{

    public enum representationTypes { None, Anatomy, Isomap, Tsne, MDS };
    public enum classificationTypes { None, Anatomy, Embeddness, RichClub };

    public representationTypes _representationType;
    public classificationTypes _classificationType;

    private representationTypes _prevRepresentationType = representationTypes.None;
    private classificationTypes _prevClassificationType = classificationTypes.None;

    private List<GameObject> Nodes = new List<GameObject>();
    private List<GameObject> Edges = new List<GameObject>();
    private GameObject _nodePrefab;
    private GameObject _edgePrefab;
    private GameObject _connectomeLabels;
    private GameObject _HUD;
    private GameObject labelText;
    private GameObject HUDpanel;

    private bool changeRepresentationEnum = false;
    private bool changeClassificationEnum = false;
    private string _connectomeName = string.Empty;
    private string fileName = string.Empty;
    private string classificationName = string.Empty;
    private string connectomeRegionName = string.Empty;
    private string[][] _edgesMatrix;
    private float _thresholdMax;
    private float _thresholdMin;
    private float _rotationAngle;


    private List<int> edgesExisted = new List<int>();

    private Dictionary<int, List<GameObject>> edgeStatus = new Dictionary<int, List<GameObject>>();
    private Dictionary<string, string[][]> _connectomeData = new Dictionary<string, string[][]>();
    private Dictionary<string, int> classificationColumn = new Dictionary<string, int>();
    private Dictionary<int, int> labelDictionary = new Dictionary<int, int>();
    private Dictionary<string, Material> _connectomeMaterials = new Dictionary<string, Material>();
    private Dictionary<string, List<string>> _connectomeTopologiesDictionary = new Dictionary<string, List<string>>();
    private Dictionary<string, representationTypes> _representationList = new Dictionary<string, representationTypes>();

    // Use this for initialization
    void Start()
    {
        _connectomeLabels = Resources.Load("Prefabs/ConnectomeLabel") as GameObject;
        gameObject.layer = 2;

        _HUD = Resources.Load("Prefabs/HUD") as GameObject;


        _representationList = new Dictionary<string, representationTypes>()
        {
            {"anatomy",representationTypes.Anatomy},
            {"isomap",representationTypes.Isomap},
            {"tsne",representationTypes.Tsne},
            {"mds",representationTypes.MDS}
        };

        _representationType = _representationList[_connectomeTopologiesDictionary[_connectomeName][0]];
        _classificationType = classificationTypes.Anatomy;
        classificationName = "anatomy";
        connectomeRegionName = "leftCerebellum";

        classificationColumn.Add("anatomy", 1);
        classificationColumn.Add("embeddness", 2);
        classificationColumn.Add("richclub", 3);


    }

    // Update is called once per frame
    void Update()
    {
        if (DetectRepresentationEnum() || DetectClassificationEnum())
        {
            DrawConnectome(_connectomeData);
            edgesExisted = new List<int>();
            foreach (int i in edgeStatus.Keys)
                edgesExisted.Add(i);
            edgeStatus = new Dictionary<int, List<GameObject>>();
            foreach (int i in edgesExisted)
            {
                DrawEdges(i);
            }
        }

    }

    //loading data from connectomeBuilder
    public void attachConnectomeData(Dictionary<string, string[][]> connectomeData, string connectomeName, GameObject nodePrefab, GameObject edgePrefab, float thresholdMax, float thresholdMin, Dictionary<string, Material> connectomeMaterials, float rotationAngle, bool isConnectomeReset, Dictionary<string, List<string>> connectomeTopologiesDictionary)
    {
        _connectomeData = connectomeData;
        _nodePrefab = nodePrefab;
        _edgePrefab = edgePrefab;
        _connectomeMaterials = connectomeMaterials;
        _thresholdMax = thresholdMax;
        _thresholdMin = thresholdMin;
        _rotationAngle = rotationAngle;
        _connectomeName = connectomeName;
        _connectomeTopologiesDictionary = connectomeTopologiesDictionary; //_connectomeTopologiesDictionary include all the topologies name under each connectome folders.
        _edgesMatrix = connectomeData["NW"];

    }

    public representationTypes representationType
    {
        get
        {
            return _representationType;
        }
        set
        {
            _representationType = value;
        }
    }

    public classificationTypes classificationType
    {
        get
        {
            return _classificationType;
        }
        set
        {
            _classificationType = value;
        }
    }

    //detecting the change of representation enum
    public bool DetectRepresentationEnum()
    {
        changeRepresentationEnum = false;

        if (_representationType != _prevRepresentationType)
        {
            if (labelText != null)
                Destroy(labelText);
            if (HUDpanel != null)
                Destroy(HUDpanel);

            foreach (GameObject node in Nodes)
                if (node != null)
                    Destroy(node);
            foreach (GameObject edge in Edges)
                if (edge != null)
                    Destroy(edge);

            switch (_representationType)
            {
                case representationTypes.Anatomy:

                    fileName = "anatomy";
                    _prevRepresentationType = representationTypes.Anatomy;
                    break;

                case representationTypes.Isomap:

                    fileName = "isomap";
                    _prevRepresentationType = representationTypes.Isomap;
                    break;

                case representationTypes.Tsne:

                    fileName = "tsne";
                    _prevRepresentationType = representationTypes.Tsne;
                    break;

                case representationTypes.MDS:

                    fileName = "mds";
                    _prevRepresentationType = representationTypes.MDS;
                    break;

                case representationTypes.None:

                    fileName = string.Empty;
                    _prevRepresentationType = representationTypes.None;
                    break;
            }

            changeRepresentationEnum = true;
        }
        return changeRepresentationEnum;
    }

    public bool DetectClassificationEnum()
    {
        changeClassificationEnum = false;

        if (_classificationType != _prevClassificationType)
        {
            if (labelText != null)
                Destroy(labelText);
            if (HUDpanel != null)
                Destroy(HUDpanel);

            foreach (GameObject node in Nodes)
                if (node != null)
                    Destroy(node);
            foreach (GameObject edge in Edges)
                if (edge != null)
                    Destroy(edge);

            switch (_classificationType)
            {
                case classificationTypes.Anatomy:

                    classificationName = "anatomy";
                    //changeClassificationEnum = true;
                    _prevClassificationType = classificationTypes.Anatomy;
                    break;

                case classificationTypes.Embeddness:

                    classificationName = "embeddness";
                    //changeClassificationEnum = true;
                    _prevClassificationType = classificationTypes.Embeddness;
                    break;

                case classificationTypes.RichClub:

                    classificationName = "richclub";
                    //changeClassificationEnum = true;
                    _prevClassificationType = classificationTypes.RichClub;
                    break;
                case classificationTypes.None:

                    classificationName = string.Empty;
                    _prevClassificationType = classificationTypes.None;
                    break;

            }
            changeClassificationEnum = true;
        }
        return changeClassificationEnum;
    }

    public void DrawConnectome(Dictionary<string, string[][]> connectomeData)
    {
        int labelKey = 0;
        int label = 0;
        int regionColumn = 0;
        string regionName = string.Empty;

        string[][] edgesMatrix = connectomeData["NW"];

        Nodes = new List<GameObject>();

        //creating a dictionary for ID and label
        for (int i = 1; i < connectomeData["LookupTable"].Length - 1; i++)
        {
            labelKey = int.Parse(connectomeData["LookupTable"][i][0]);

            if (!labelDictionary.ContainsKey(labelKey))
                labelDictionary.Add(labelKey, i);
        }


        if (fileName != string.Empty)
        {


            for (int row = 1; row < connectomeData[fileName].Length; row++)
            //for (int row = 1; row < 5; row++)
            {

                //get the label for the node ID
                //get the column number of coresponding label
                //getting the classification name
                //get the material from the material dictionary and add it the node


                try
                {
                    GameObject node = Instantiate(_nodePrefab, new Vector3(float.Parse(connectomeData[fileName][row][1]), float.Parse(connectomeData[fileName][row][2]), float.Parse(connectomeData[fileName][row][3])), Quaternion.identity);
                    label = int.Parse(connectomeData["label"][row][1]);
                    regionColumn = classificationColumn[classificationName];
                    connectomeRegionName = connectomeData["LookupTable"][labelDictionary[label]][regionColumn];

                    node.GetComponent<Renderer>().material = _connectomeMaterials[connectomeRegionName];


                    Vector3 tempPosition = node.transform.position;
                    node.transform.parent = this.transform;
                    node.transform.localPosition = tempPosition;
                    node.name = row.ToString();

                    Nodes.Add(node);
                }
                catch (System.IndexOutOfRangeException e)
                {
                    System.Console.WriteLine(e.Message);
                }
            }

            labelText = Instantiate(_connectomeLabels, new Vector3(-0.5f, 1.5f, -0.3f), Quaternion.Euler(new Vector3(0, _rotationAngle * 40, 0)));
            Vector3 labeltexttempPosition = labelText.transform.position;
            labelText.transform.parent = this.transform.parent.transform;
            labelText.transform.localPosition = labeltexttempPosition;
            labelText.GetComponent<TextMesh>().text = _connectomeName;


            HUDpanel = Instantiate(_HUD, new Vector3(-0.3f, -1.15f, -0.9f), Quaternion.Euler(new Vector3(0, _rotationAngle * 40, 0)));
            Vector3 HUDtempPosition = HUDpanel.transform.position;
            HUDpanel.transform.parent = this.transform.parent.transform;
            HUDpanel.transform.localPosition = HUDtempPosition;
            // HUDpanel.layer = 2;


        }
        changeRepresentationEnum = false;
    }

    public void DrawEdges(int nodeNumber)
    {
        int label = 0;
        int regionColumn = 0;
        string regionName = string.Empty;

        Vector3 startPosition = new Vector3();
        Vector3 endPosition = new Vector3();
        Vector3 scale = new Vector3();
        Vector3 offset = new Vector3();
        Vector3 edgePosition = new Vector3();

        List<GameObject> edgeTemp = new List<GameObject>();

        if (edgeStatus.ContainsKey(nodeNumber))
        {
            foreach (GameObject node in edgeStatus[nodeNumber])
                if (node != null)
                    Destroy(node);
            edgeStatus.Remove(nodeNumber);
        }
        else
        {
            for (int i = 0; i < _edgesMatrix[nodeNumber].Length; i++)
            {
                if (float.Parse(_edgesMatrix[nodeNumber][i]) < _thresholdMax && float.Parse(_edgesMatrix[nodeNumber][i]) > _thresholdMin && float.Parse(_edgesMatrix[nodeNumber][i]) > 0)
                {
                    //starting point is node ID, end point is node ID i+1
                    //the position of node is float.Parse(connectomeData[fileName][i/j+1][1]), float.Parse(connectomeData[fileName][i/j+1][2]), float.Parse(connectomeData[i/j+1][row][3])
                    if (fileName != string.Empty)
                    {
                        //get the column number of coresponding label
                        //getting the classification name
                        //get the material from the material dictionary and add it the node
                        label = int.Parse(_connectomeData["label"][nodeNumber][1]);
                        regionColumn = classificationColumn[classificationName];
                        connectomeRegionName = _connectomeData["LookupTable"][labelDictionary[label]][regionColumn];

                        startPosition = new Vector3(Nodes[nodeNumber - 1].transform.localPosition.x, Nodes[nodeNumber - 1].transform.localPosition.y, Nodes[nodeNumber - 1].transform.localPosition.z);
                        endPosition = new Vector3(Nodes[i].transform.localPosition.x, Nodes[i].transform.localPosition.y, Nodes[i].transform.localPosition.z);
                        offset = endPosition - startPosition;
                        edgePosition = startPosition + offset / 2.0f;
                        scale = new Vector3(0.003f, offset.magnitude / 2.0f, 0.003f);

                        GameObject edge = Instantiate(_edgePrefab, edgePosition, Quaternion.identity);
                        edge.GetComponent<Renderer>().material.SetColor("_Color1", Nodes[nodeNumber - 1].transform.GetComponent<Renderer>().material.GetColor("_Color"));
                        edge.GetComponent<Renderer>().material.SetColor("_Color2", Nodes[i].transform.GetComponent<Renderer>().material.GetColor("_Color"));

                        Vector3 tempPosition = edge.transform.position;
                        edge.transform.parent = this.transform;
                        edge.transform.localPosition = tempPosition;
                        edge.transform.up = offset;
                        edge.transform.localScale = scale;
                        edgeTemp.Add(edge);
                        Edges.Add(edge);

                    }
                }
            }
            edgeStatus.Add(nodeNumber, edgeTemp);
        }
    }
}
