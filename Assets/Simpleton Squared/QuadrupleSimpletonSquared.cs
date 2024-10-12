using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class QuadrupleSimpletonSquared : MonoBehaviour {

    KMAudio Audio;
    public AudioClip solveSound;

    static int ModuleIdCounter = 1;
    int ModuleId;
    bool ModuleSolved;

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
        ModuleSolved = true;
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


    string[] validCoordinates = new[] {     "A1", "B1",
                                            "A2", "B2",

                                            "C1", "D1",
                                            "C2", "D2",

                                            "A3", "B3",
                                            "A4", "B4",

                                            "C3", "D3",
                                            "C4", "D4"};
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} followed by a Battleship coordinate to press that button. You can chain buttons presses with a "","" between the coordinates";
#pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string Command)
    {
        Command = Command.ToUpper().Trim();
        yield return null;
        

        Match match = Regex.Match(Command, @"^([A-D][1-4],?\s?)+$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        if (match.Success) 
        {
            string[] coordinates = match.Groups[0].Value.Split(',').Select(coor => coor.Trim()).ToArray();

            foreach (string coor in coordinates)
            {
                Debug.Log(coor);
                int ix = Array.IndexOf(validCoordinates, coor);
                buttons[ix].Selectable.OnInteract();
                yield return new WaitForSeconds(.1f);
            }
        }

        else
        {
            yield return $"sendtochaterror Could not recongize that command";
            yield break;
        }

    }

    IEnumerator TwitchHandleForcedSolve()
    {
        yield return ProcessTwitchCommand(string.Join(",", validCoordinates));
        while (!ModuleSolved)
        {
            yield return null;
        }
    }
}
