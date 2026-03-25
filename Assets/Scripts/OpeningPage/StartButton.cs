using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnStartClicked);
    }

    public void OnStartClicked()
    {
        SceneManager.LoadScene("CharacterCustomization");
    }
}