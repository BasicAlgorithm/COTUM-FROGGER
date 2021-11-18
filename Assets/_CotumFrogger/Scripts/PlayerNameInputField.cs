using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    #region Private Constants

    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";

    #endregion


    #region MonoBehaviour CallBacks

    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        //string defaultName = string.Empty;
        string defaultName = "mobile" + Random.Range(1.0f, 10.0f).ToString();
        InputField _inputField = this.GetComponent<InputField>();
        if (_inputField != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                _inputField.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
    }

    #endregion

    #region Public Methods

    // Sets the name of the player, and save it in the PlayerPrefs for future sessions.
    // <param name="value">The name of the Player</param>
    public void SetPlayerName(string value)
    {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");

            // When with mobile will be possible set a name
            //return;

            // while using mobile is not possible put a name, we generate a random
            string id_random = "mobile " + Random.Range(1, 10).ToString();
            value = id_random;

            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
        else
        {
            PhotonNetwork.NickName = value;
            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
    }

    #endregion
}
