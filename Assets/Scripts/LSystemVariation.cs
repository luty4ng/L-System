using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

[System.Serializable]
public struct RulePairVariation
{
    public Rule ruleKey;
    public string Value;
    public float Possibility;
}

[System.Serializable]
public struct Rule
{
    public string ruleStr;
    public string preContext;
    public string postContext;
}

[ExecuteInEditMode]
public class LSystemVariation : MonoBehaviour
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
    public List<RulePairVariation> rules;
    public int iteration;
    public int seed = 0727;

    private string currentString;
    private string updateString;
    private int currentStep;
    private Stack<TransformData> transformStack;
    private System.Random random;

    private void Update()
    {
        if (IsUpdate)
            GenerateSystem();
    }

    public void GenerateSystem()
    {
        Debug.ClearDeveloperConsole();
        currentStep = 0;
        random = new System.Random(seed);
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

    public virtual void Generate(int iter)
    {
        updateString = "";
        char[] stringChar = currentString.ToCharArray();
        for (int i = 0; i < stringChar.Length; i++)
        {
            if (!variables.Contains(stringChar[i]) && !constants.Contains(stringChar[i]))
            {
                Debug.LogWarning("Invalid variable or constants in rules.");
                return;
            }

            char currentChar = stringChar[i];
            List<KeyValuePair<string, float>> currentSubstitude = new List<KeyValuePair<string, float>>();
            // List<string> optionalRule = (from rule in rules where rule.ruleKey.ruleStr == currentChar.ToString() select rule.Value).ToList();
            foreach (var rule in rules)
            {
                bool fitPreContext = true;
                bool fitPostContext = true;
                if (rule.ruleKey.preContext != "" && i > 0)
                {
                    if (stringChar[i - 1] != rule.ruleKey.preContext.ToCharArray()[0])
                        fitPreContext = false;
                }
                if (rule.ruleKey.postContext != "" && i < stringChar.Length - 1)
                {
                    if (stringChar[i + 1] != rule.ruleKey.postContext.ToCharArray()[0])
                        fitPostContext = false;
                }
                if (rule.ruleKey.ruleStr == currentChar.ToString() && fitPreContext && fitPostContext)
                    currentSubstitude.Add(new KeyValuePair<string, float>(rule.Value, rule.Possibility));
            }

            
            if (currentSubstitude.Count == 1)
            {
                updateString += random.NextDouble() < currentSubstitude[0].Value ? currentSubstitude[0].Key : "";
            }
            else if (currentSubstitude.Count > 1)
            {
                if(currentSubstitude[0].Value+currentSubstitude[1].Value!=1)
                    Debug.LogWarning("Possibility setting inappropriate. ");
                updateString += random.NextDouble() < currentSubstitude[0].Value ? currentSubstitude[0].Key : currentSubstitude[1].Key;
            }
            else
                updateString += currentChar.ToString();
        }
        currentString = updateString;
    }

    public virtual void Render(char[] renderChars)
    {
        for (int i = 0; i < renderChars.Length; i++)
        {
            if (currentStep > renderStep && IsUpdate)
                return;
            currentStep += 1;
            char currentChar = renderChars[i];
            if (currentChar == 'F' || currentChar == 'G')
            {
                Vector3 initialPosition = transform.position;
                transform.Translate(Vector3.forward * lens);
                Debug.DrawLine(initialPosition, transform.position, Color.white, 1f, false);
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

