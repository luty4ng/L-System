using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public struct RulePair
{
    public char Key;
    public string Value;
}

public class TransformData
{
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
}


[ExecuteInEditMode]
public class LSystem : MonoBehaviour
{
    [Header("Auto Generate")]
    public bool IsUpdate;
    public int renderStep;

    [Header("Params")]
    public char[] variables;
    public char[] constants;
    public char axiom;
    public float angle;
    public float lens;
    public List<RulePair> rules;
    public int iteration;

    private string currentString;
    private string updateString;
    private int currentStep;
    private Stack<TransformData> transformStack;
    private List<char> tempChars = new List<char>();
    private void Awake()
    {
        Debug.ClearDeveloperConsole();
    }
    private void Update()
    {
        if (IsUpdate)
            GenerateSystem();
    }

    public void GenerateSystem()
    {
        Debug.ClearDeveloperConsole();
        currentStep = 0;
        this.transform.position = Vector3.zero;
        this.transform.eulerAngles = new Vector3(-90, 0, 0);
        if (currentString != axiom.ToString())
            currentString = axiom.ToString();
        transformStack = new Stack<TransformData>();
        for (int i = 0; i < iteration; i++)
        {
            Generate(i);
        }

        Render(currentString.ToCharArray());
    }

    public void Generate(int iter)
    {
        updateString = "";
        char[] stringChar = currentString.ToCharArray();
        for (int i = 0; i < stringChar.Length; i++)
        {
            // Debug.Log(stringChar[i]);
            if (!variables.Contains(stringChar[i]) && !constants.Contains(stringChar[i]))
            {
                Debug.Log(stringChar[i]);
                Debug.LogWarning("Invalid variable or constants in rules.");
                return;
            }

            char currentChar = stringChar[i];
            List<string> optionalRule = (from rule in rules where rule.Key == currentChar select rule.Value).ToList();
            if (optionalRule.Count > 0)
                updateString += optionalRule[0];
            else
                updateString += currentChar.ToString();
        }
        currentString = updateString;

    }

    public void Render(char[] renderChars)
    {
        // Debug.Log(transformStack.Count);
        for (int i = 0; i < renderChars.Length; i++)
        {
            if (currentStep > renderStep && IsUpdate)
                return;
            currentStep += 1;
            char currentChar = renderChars[i];
            if (currentChar == 'F' || currentChar == 'G' || currentChar == 'A' || currentChar == 'B')
            {
                Vector3 initialPosition = transform.position;
                transform.Translate(Vector3.forward * lens);
                Debug.DrawLine(initialPosition, transform.position, Color.white, 0.01f, false);
            }
            else if (currentChar == '+')
            {
                transform.Rotate(Vector3.up * angle);
            }
            else if (currentChar == '-')
            {
                transform.Rotate(Vector3.up * -angle);
            }
            else if (currentChar == '[')
            {
                TransformData data = new TransformData();
                data.position = transform.position;
                data.scale = transform.localScale;
                data.rotation = transform.rotation;
                transformStack.Push(data);
            }
            else if (currentChar == ']')
            {
                TransformData data = transformStack.Pop();
                transform.position = data.position;
                transform.localScale = data.scale;
                transform.rotation = data.rotation;
            }
        }
    }
}

