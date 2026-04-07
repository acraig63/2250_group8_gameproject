using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DefaultNamespace;
using System.Collections.Generic;


public class BattleManager4 : MonoBehaviour
{
   [Header("Health Bars")]
   [SerializeField] private Slider    playerHealthBar;
   [SerializeField] private Slider    enemyHealthBar;
   [SerializeField] private TMP_Text  playerHealthText;
   [SerializeField] private TMP_Text  enemyHealthText;


   [Header("Unit Display")]
   [SerializeField] private TMP_Text  enemyNameText;
   [SerializeField] private Image     enemyImage;
   [SerializeField] private Image     playerImage;


   [Header("Status")]
   [SerializeField] private TMP_Text  statusText;


   [Header("Quiz Panel")]
   [SerializeField] private GameObject quizPanel;
   [SerializeField] private TMP_Text   questionText;
   [SerializeField] private TMP_Text   feedbackText;
   [SerializeField] private Button[]   answerButtons;
  
   [Header("Animation Controllers")]
   [SerializeField] private BattleAnimationController playerAnim;
   [SerializeField] private BattleAnimationController enemyAnim;
   [SerializeField] private CharacterSpriteData       characterSpriteData;


   // Battle state
   private int _playerHP;
   private int _playerMaxHP;
   private int _playerAttack;


   private int _enemyHP;
   private int _enemyMaxHP;
   private int _enemyAttack;


   private bool _battleOver = false;


   void Start()
   {
      
       // Read data written by EnemySpawner
       _playerMaxHP   = BattleData.PlayerMaxHealth;
       _playerHP      = _playerMaxHP;
       _playerAttack  = BattleData.PlayerAttackPower;


       _enemyMaxHP    = BattleData.EnemyMaxHealth;
       _enemyHP       = _enemyMaxHP;
       _enemyAttack   = BattleData.EnemyAttackPower;
      
       if (playerImage != null && BattleData.PlayerSprite != null)
           playerImage.sprite = BattleData.PlayerSprite;
       if (enemyImage != null && BattleData.EnemySprite != null)
           enemyImage.sprite = BattleData.EnemySprite;


       // Set up UI labels
       if (enemyNameText  != null) enemyNameText.text = BattleData.EnemyName;
       if (statusText     != null) statusText.text    = "Your turn — answer the question to attack!";


       // Hide quiz panel until player's turn begins
       if (quizPanel != null) quizPanel.SetActive(false);


       RefreshHealthBars();
       // Load the correct colour variant for the selected character
       // Load the correct colour variant for the selected character
       if (characterSpriteData != null && playerAnim != null)
       {
           if (characterSpriteData.TryGetFrames(CharacterSelectManager.selectedCharacter,
                   out Sprite[] idle, out Sprite[] attack))
           {
               playerAnim.SetFrames(idle, attack);
           }
       }


       // Load the correct enemy type animation
       if (characterSpriteData != null && enemyAnim != null)
       {
           if (characterSpriteData.TryGetFrames(BattleData.EnemyType,
                   out Sprite[] idle, out Sprite[] attack))
           {
               enemyAnim.SetFrames(idle, attack);
           }
       }


// Start the first player turn after a short delay
       StartCoroutine(StartPlayerTurn(0.5f));
   }
  
   // PLAYER TURN
   private IEnumerator StartPlayerTurn(float delay)
   {
       yield return new WaitForSeconds(delay);
       if (_battleOver) yield break;


       if (statusText != null)
           statusText.text = "Your turn — answer correctly to deal damage!";


       ShowQuestion();
   }


   private void ShowQuestion()
   {
       SetButtonsInteractable(true);
       List<MultipleChoiceQuestion> pool = GetQuestions(BattleData.QuestionLevel);
       if (pool == null || pool.Count == 0) return;


       MultipleChoiceQuestion q = pool[Random.Range(0, pool.Count)];


       questionText.text  = q.questionText;
       feedbackText.text  = "";
       quizPanel.SetActive(true);


       for (int i = 0; i < answerButtons.Length; i++)
       {
           if (i < q.options.Count)
           {
               answerButtons[i].gameObject.SetActive(true);
               TMP_Text label = answerButtons[i].GetComponentInChildren<TMP_Text>();
               if (label != null) label.text = q.options[i];


               string captured = q.options[i];
               answerButtons[i].onClick.RemoveAllListeners();
               answerButtons[i].onClick.AddListener(() => OnAnswerSelected(captured, q));
           }
           else
           {
               answerButtons[i].gameObject.SetActive(false);
           }
       }
   }


   private void OnAnswerSelected(string answer, MultipleChoiceQuestion question)
   {
       // Disable buttons immediately so the player can't click twice
       SetButtonsInteractable(false);


       bool correct = question.Evaluate(answer);


       if (correct)
       {
           feedbackText.text  = "✓ Correct! You dealt damage!";
           feedbackText.color = Color.green;
           StartCoroutine(PlayerAttack());
       }
       else
       {
           feedbackText.text  = "✗ Wrong! No damage dealt.";
           feedbackText.color = Color.red;
           // No damage — go straight to enemy turn
           StartCoroutine(EnemyTurn(1.2f));
       }
   }


   private IEnumerator PlayerAttack()
   {
       yield return new WaitForSeconds(0.3f);
       quizPanel.SetActive(false);


       // Play player attack animation
       if (playerAnim != null)
           yield return StartCoroutine(playerAnim.PlayAttack());


       _enemyHP -= _playerAttack;
       _enemyHP  = Mathf.Max(0, _enemyHP);
       RefreshHealthBars();


       // Play enemy hit effect
       if (enemyAnim != null)
           StartCoroutine(enemyAnim.PlayHit(isPlayer: false));


       if (statusText != null)
           statusText.text = $"You dealt {_playerAttack} damage!";


       yield return new WaitForSeconds(0.5f);


       if (_enemyHP <= 0)
       {
           if (enemyAnim != null)
               yield return StartCoroutine(enemyAnim.PlayDeath());
           EndBattle(playerWon: true);
       }
       else
       {
           StartCoroutine(EnemyTurn(0.3f));
       }
   }
  
   // ENEMY TURN


   private IEnumerator EnemyTurn(float delay)
   {
       yield return new WaitForSeconds(delay);
       quizPanel.SetActive(false);
       if (_battleOver) yield break;


       if (statusText != null)
           statusText.text = $"{BattleData.EnemyName} attacks!";


       // Play enemy attack animation
       if (enemyAnim != null)
           yield return StartCoroutine(enemyAnim.PlayAttack());


       _playerHP -= _enemyAttack;
       _playerHP  = Mathf.Max(0, _playerHP);
       RefreshHealthBars();


       // Play player hit effect
       if (playerAnim != null)
           StartCoroutine(playerAnim.PlayHit(isPlayer: true));


       if (statusText != null)
           statusText.text = $"{BattleData.EnemyName} dealt {_enemyAttack} damage!";


       yield return new WaitForSeconds(0.5f);


       if (_playerHP <= 0)
       {
           if (playerAnim != null)
               yield return StartCoroutine(playerAnim.PlayDeath());
           EndBattle(playerWon: false);
       }
       else
       {
           StartCoroutine(StartPlayerTurn(0.3f));
       }
   }
  
   // BATTLE END


   private void EndBattle(bool playerWon)
   {
       _battleOver = true;
       quizPanel.SetActive(false);


       if (playerWon)
       {
           if (statusText != null)
               statusText.text = $"You defeated {BattleData.EnemyName}!";
           StartCoroutine(ReturnToOverworld(2.0f, won: true));
       }
       else
       {
           if (statusText != null)
               statusText.text = "You were defeated...";
           StartCoroutine(ReturnToOverworld(2.0f, won: false));
       }
   }


   private IEnumerator ReturnToOverworld(float delay, bool won)
   {
       yield return new WaitForSeconds(delay);


       // Tell the overworld whether the player won so the NPC can be destroyed
       BattleData.PlayerWon     = won;
       BattleData.PlayerCurrentHealth = _playerHP; // carry HP back
       BattleData.ReturningFromBattle  = true;


       if (!won) BattleData.HasReturnPosition = false;


       SceneManager.LoadScene(BattleData.ReturnScene);
   }




   // UI HELPERS


   private void RefreshHealthBars()
   {
       if (playerHealthBar != null)
       {
           playerHealthBar.maxValue = _playerMaxHP;
           playerHealthBar.value    = _playerHP;
       }
       if (enemyHealthBar != null)
       {
           enemyHealthBar.maxValue = _enemyMaxHP;
           enemyHealthBar.value    = _enemyHP;
       }
       if (playerHealthText != null)
           playerHealthText.text = $"HP: {_playerHP} / {_playerMaxHP}";
       if (enemyHealthText != null)
           enemyHealthText.text  = $"HP: {_enemyHP} / {_enemyMaxHP}";
   }


   private void SetButtonsInteractable(bool interactable)
   {
       foreach (Button b in answerButtons)
           b.interactable = interactable;
   }
  
   // QUESTION BANK
   private List<MultipleChoiceQuestion> GetQuestions(int level)
   {
       switch (level)
       {
           case 1:  return QuestionBank.GetLevel1Questions();
           case 2:  return QuestionBank.GetLevel2Questions();
           case 3:  return QuestionBank.GetLevel3Questions();
           case 4:  return QuestionBank.GetLevel4Questions();
           case 5:  return QuestionBank.GetLevel5Questions();
           default: return QuestionBank.GetLevel1Questions();
       }
   }
}

