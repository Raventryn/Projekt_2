using System;
using System.Collections.Generic;
using Ink.Parsed;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

//[ExecuteInEditMode]
public class HUDCodeLines : MonoBehaviour
{
    [SerializeField] TMP_Text Textfield;
    [SerializeField] String allSymbols;

    List<Char> symbols = new List<char>();

    void Start()
    {
        AddCharsToList();

        InvokeRepeating("GenerateSymbolSequences", 0, 0.1f);
    }

    void AddCharsToList()
    {
        foreach(char symbol in allSymbols)
        {
            symbols.Add(symbol);
        }
    }

    int GenerateRandomIndex()
    {
        int randomIndex = UnityEngine.Random.Range(0, symbols.Count);

        return randomIndex;
    }

    void GenerateSymbolSequences()
    {
        Textfield.text = "";

        for(int i = 0; i < UnityEngine.Random.Range(5,12); i++)
        {
            int index = GenerateRandomIndex();

            Textfield.text += symbols[index];
        }    

        Textfield.text += "\n";

        for(int i = 0; i < UnityEngine.Random.Range(2,7); i++)
        {
            int index = GenerateRandomIndex();

            Textfield.text += symbols[index];
        }

        Textfield.text += "\n";

        for(int i = 0; i < UnityEngine.Random.Range(3,8); i++)
        {
            int index = GenerateRandomIndex();

            Textfield.text += symbols[index];
        }

        Textfield.text += "\n";

        for(int i = 0; i < UnityEngine.Random.Range(2,5); i++)
        {
            int index = GenerateRandomIndex();

            Textfield.text += symbols[index];
        }
  
    }
}
