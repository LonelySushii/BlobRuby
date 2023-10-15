using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    #region Public Variables
    public Animator fadeBG = default;
    public Button[] menuButtons = default;
    #endregion

    #region Private Variables

    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void OnDestroy()
    {

    }
    #endregion

    void Start()
    {
        fadeBG.Play("Fade_In");
    }
    #endregion

    #region My Functions
    public void OnClick_PlayGame()
    {
        DisableButtons();
        StartCoroutine(StartGameDelay());
    }

    public void OnClick_QuitGame()
    {
        DisableButtons();
        StartCoroutine(QuitGameDelay());
    }

    void DisableButtons()
    {
        for (int i = 0; i < menuButtons.Length; i++)
            menuButtons[i].interactable = false;
    }
    #endregion

    #region Coroutines
    /// <summary>
    /// Starts game with a Delay;
    /// </summary>
    /// <returns> Float Delay; </returns>
    IEnumerator StartGameDelay()
    {
        fadeBG.Play("Fade_Out");
        yield return new WaitForSeconds(0.5f);
        Application.LoadLevel(1);
    }

    /// <summary>
    /// Quits game with a Delay;
    /// </summary>
    /// <returns> Float Delay; </returns>
    IEnumerator QuitGameDelay()
    {
        fadeBG.Play("Fade_Out");
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
    }
    #endregion

    #region Events

    #endregion
}