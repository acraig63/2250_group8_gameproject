using UnityEngine;

public class IntroNPC : MonoBehaviour
{
    private bool _playerInside = false;
    private bool _isShowing    = false;
    private int  _currentLine  = 0;

    private readonly string[] _lines = new string[]
    {
        "Arrr, welcome to Stormbreaker Island, ye brave soul!\nI've seen many a pirate wash up on these storm-battered rocks...\nnot many leave.",
        "These waters belong to the Storm Captain and his crew.\nThey've fortified this island with guards at every turn —\nfierce pirates who won't go down without a fight.",
        "Yer mission is clear: hunt down every last one of\nthe enemy pirates on this island. Defeat them all!\nOnly then will the path forward open to ye.",
        "The Storm Captain himself waits in the fortress to the north.\nBring him down and claim Key 3 — ye'll need it\nto rescue yer captured crewmates.",
        "The storms here are no joke either — avoid the\nlightning hazard zones or they'll drain yer health fast.\nNow go, and may the winds be in yer favour. Arrr!"
    };

    void Update()
    {
        if (!_playerInside || !_isShowing) return;

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            _currentLine++;

            if (_currentLine >= _lines.Length)
            {
                // All lines shown — close
                NPCDialoguePanel.Instance?.Hide();
                _isShowing   = false;
                _currentLine = 0;
            }
            else
            {
                ShowCurrentLine();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (_isShowing) return;

        _playerInside = true;
        _isShowing    = true;
        _currentLine  = 0;
        ShowCurrentLine();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;
        _isShowing    = false;
        _currentLine  = 0;
        NPCDialoguePanel.Instance?.Hide();
    }

    private void ShowCurrentLine()
    {
        bool isLast = _currentLine >= _lines.Length - 1;
        string prompt = isLast
            ? "\n\n<size=10>[ Press E to close ]</size>"
            : "\n\n<size=10>[ Press E to continue ]</size>";

        NPCDialoguePanel.Instance?.ShowPersistent(_lines[_currentLine] + prompt);
    }
}