using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SymbolsGenerator : MonoBehaviour
{
    public string Symbols;

    public GameObject DataObject;

    public GameObject Canvas;

    List<char> _symbols = new List<char>();

    List<string> _generatedSets = new List<string>();

    List<GameObject> _instances = new List<GameObject>();

    string _solutionSet;

    //Converts Inspector string to List of single chars
    void ConvertToList()
    {
        foreach(char symbol in Symbols)
        {
            _symbols.Add(symbol);
        }
    }

//Generates randomized char sequences and saves to string list
    void GenerateSymbolSets()
    {
        for(int i = 0; i < 8; i++)
        {
            string symbolSet = "";
            for(int e = 0; e < 8; e++)
            {
                symbolSet += _symbols[Random.Range(0, _symbols.Count)];    
            }

            _generatedSets.Add(symbolSet);

            Debug.Log(symbolSet);
        }       
    }

    //Generates solution based on chars for each string in generated list
    void GenerateSolution()
    {
        int i = 0;

        foreach(string set in _generatedSets)
        {
            _solutionSet += set.Substring(i, 1);

            i++;
        }

        //Debug.Log(_solutionSet);
    }

    //Instantiates UI Elements in ellipsis with symbol sequences
    void GenerateObjects()
    {
        RectTransform canvasTransform = Canvas.GetComponent<RectTransform>();

        float radiusX = canvasTransform.rect.width/2;
        float radiusY = canvasTransform.rect.height/2;
        Vector3 centre = Canvas.transform.position;
        
        List<string> sequences = new List<string>();
        sequences = _generatedSets;

        bool solutionSpawned = false;

        for(int pointNum = 0; pointNum < 9; pointNum++)
        {
            float i = (float)pointNum / 9;

            //Debug.Log(i);

            float angle = i * Mathf.PI * 2;

            float posX = Mathf.Sin(angle) * radiusX;
            float posY = Mathf.Cos(angle) * radiusY;

            Vector3 position = new Vector3(posX, posY, 0) + centre;

            GameObject dataObject = Instantiate(DataObject, position, Quaternion.Euler(Vector3.zero), Canvas.transform);

            _instances.Add(dataObject);

            TMP_Text dataText = dataObject.GetComponentInChildren<TMP_Text>();

            if(Random.Range(0,4) == 2 && !solutionSpawned)
            {
                dataText.text = _solutionSet;
                dataText.color = Color.springGreen;
                solutionSpawned = true;
            }
            else if(pointNum == 8 && !solutionSpawned)
            {
                dataText.text = _solutionSet;
                dataText.color = Color.springGreen;
                solutionSpawned = true;
            }
            else
            {
                int randomIndex = Random.Range(0, 8 - pointNum);
                dataText.text = sequences[randomIndex];
                sequences.Remove(_generatedSets[randomIndex]);
            }
        }
    }

    public void Randomize()
    {
        ConvertToList();
        GenerateSymbolSets();
        GenerateSolution();
        GenerateObjects();
    }

    public void Clean()
    {
        foreach(GameObject instance in _instances)
        {
            Destroy(instance);
        }

        _symbols = new List<char>();
        _generatedSets = new List<string>();
        _solutionSet = "";
        _instances = new List<GameObject>();
    }
}



/*Vector2 randomScreenPosition = new Vector2(Random.Range(-Screen.width/2, Screen.width/2), Random.Range(-Screen.height/2, Screen.height/2));

            GameObject dataObject = Instantiate(DataObject, new Vector3(randomScreenPosition.x, randomScreenPosition.y, 0), Quaternion.Euler(Vector3.zero), Canvas.transform);
            TMP_Text dataText = dataObject.GetComponentInChildren<TMP_Text>();

            dataText.text = _generatedSets[i];*/