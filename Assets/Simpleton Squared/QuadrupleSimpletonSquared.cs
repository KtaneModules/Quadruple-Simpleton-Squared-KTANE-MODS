using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuadrupleSimpletonSquared : MonoBehaviour {

    KMAudio Audio;
    public AudioClip solveSound;

    static int ModuleIdCounter = 1;
    int ModuleId;

    List<Button> buttons;
    void Awake () {
        Audio = GetComponent<KMAudio>();
        ModuleId = ModuleIdCounter++;
        buttons = new List<Button>();

        KMSelectable selectable = GetComponent<KMSelectable>();
        foreach (KMSelectable qudrant in selectable.Children) 
        {
            foreach (KMSelectable button in qudrant.Children) 
            {
                Button b = new Button(button);
                button.OnInteract += delegate () { ButtonPressed(b); return false; };
                buttons.Add(b);
            }
        }

        Log("Press the buttons");
    }


    private void ButtonPressed(Button button)
    {
        if (button.Pressed)
        {
            return;
        }

        KMSelectable selectable = button.Selectable;
        selectable.AddInteractionPunch();
        
        Audio.PlaySoundAtTransform(solveSound.name, transform);

        selectable.transform.Find("Text").GetComponent<TextMesh>().text = "Victory!";

        button.Pressed = true;

        if (buttons.All(b => b.Pressed))
        {
            Solve();
        }
    }

    private void Solve()
    {
        Log("You pressed all the buttons");
        GetComponent<KMBombModule>().HandlePass();
    }

    private void Log(string s)
    {
        Debug.Log($"[Quadruple Simpleton Squared #{ModuleId}] {s}");
    }
    
    private class Button
    {
        public KMSelectable Selectable { get; private set; }
        public bool Pressed { get; set; }

        public Button(KMSelectable selectable)
        {
            Selectable = selectable;
            Pressed = false;
        }
    }


#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        yield return null;
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return null;
    }
}
