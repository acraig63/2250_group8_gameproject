using System.Collections;
using UnityEngine;

public class IntroNPC : MonoBehaviour
{
    private bool _isShowing  = false;
    private int  _currentLine = 0;
    private bool _isOnCooldown = false;
    
    [SerializeField] private float cooldown = 5f;

    private readonly string[] _lines = new string[]
    {
        "Arrr, welcome to Stormbreaker Island, ye brave soul!\nI've seen many a pirate wash up on these storm-battered rocks...\nnot many leave.",
        "These waters belong to the Storm Captain Triplets and their crew.\nThey've fortified this island with guards at every turn —\nfierce pirates who won't go down without a fight.",
        "Yer mission is clear: hunt down every last one of\nthe enemy pirates on this island. Defeat them all!\nOnly then will the path forward open to ye.",
        "The Storm Captain himself waits in the fortress to the east.\nBring the triplets down and claim Key 3 — ye'll need it\nto rescue yer captured crewmates.",
        "The storms here are no joke either — look for the\nhealing monks they'll heal yer health fast.\nNow go, and may the winds be in yer favour. Arrr!"
    };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        
        if (_isOnCooldown || _isShowing)
        {
            NPCDialoguePanel.Instance?.ShowPersistent(
                "Whoo, let me catch me breath young one!\nCome back in a moment and I'll have more to say. Arrr!");
            return;
        }
        
        _isShowing    = true;
        _currentLine  = 0;
        ShowCurrentLine();
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        NPCDialoguePanel.Instance?.Hide();
        //StartCoroutine(ResetAfterDelay());
    }

    void Update()
    {
        if (!_isShowing) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            _currentLine++;
            if (_currentLine >= _lines.Length)
            {
                NPCDialoguePanel.Instance?.Hide();
                StartCoroutine(ResetAfterDelay()); // same as monk's cooldown
            }
            else
            {
                ShowCurrentLine();
            }
        }
    }
    
    

    private IEnumerator ResetAfterDelay()
    {
        _isOnCooldown = true;
        yield return new WaitForSeconds(cooldown); 
        _isShowing   = false;
        _currentLine = 0;
    }

    private void ShowCurrentLine()
    {
        bool isLast = _currentLine >= _lines.Length - 1;
        string prompt = isLast
            ? "\n\n<size=10>[ Press Space to close ]</size>"
            : "\n\n<size=10>[ Press Space to continue ]</size>";
        NPCDialoguePanel.Instance?.ShowPersistent(_lines[_currentLine] + prompt);
    }
    
    
}