﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

[RequireComponent(typeof(TextMeshProUGUI))]
public class DebugText : MonoBehaviour
{
    private TextMeshProUGUI txt;
    private int[] last100FPS = new int[100];
    private int currentIndex = 0;
    public int averageFPS {get; private set;}
    public int count {get; private set;}

    private void Start()
    {
        txt = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        int currentFPS = Mathf.RoundToInt(1f / Time.unscaledDeltaTime);
        if (currentIndex < last100FPS.Length)
        {
            last100FPS[currentIndex] = currentFPS;
            currentIndex++;
        }
        else
        {
            currentIndex = 0;
            last100FPS[currentIndex] = currentFPS;
        }
        averageFPS = Average(last100FPS);

        count = GameObject.FindObjectsOfType<GameObject>().Length;
        
        txt.text = $"Object count: {count}\nFPS: {averageFPS}";
    }

    ///<summary>
    /// Compute the average value of an array.
    ///</summary>
    ///<param name="_array">The array to compute</param>
    ///<returns>The average og the array</returns>
    private int Average(int[] _array)
    {
        int sum = 0;
        for(int i = 0; i < _array.Length; i++)
        {
            sum += _array[i];
        }
        return sum / _array.Length;
    }
}
