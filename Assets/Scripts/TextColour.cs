using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextColour : MonoBehaviour
{
    public Text displayText;
    public Text comboCounter;

    string display;
    string original;

    public int charIndex = 0;
    public int stringIndex = 0;
    public int combo = 0;

    public List<string> sourceStrings;

    // Start is called before the first frame update
    void Start()
    {
        original = sourceStrings[stringIndex];

        UnityEngine.InputSystem.Keyboard.current.onTextInput += OnTextInput;
    }

    private void OnTextInput(char obj)
    {
        if (obj == original[charIndex])
        {
            charIndex += 1;
            combo += 1;
            if (charIndex == original.Length)
            {
                charIndex = 0;
                stringIndex += 1;
                stringIndex = stringIndex % sourceStrings.Count;
                original = sourceStrings[stringIndex];
            }
        }
        else
        {
            combo = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        string left = original.Substring(0, charIndex);
        string right = original.Substring(charIndex, original.Length - charIndex);

        display = string.Format("<color=red>{0}</color>{1}", left, right);
        displayText.text = display;

        comboCounter.text = "Combo: " + combo.ToString();
    }
}
