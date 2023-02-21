using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel���� �̵��� ����� (Scene �̵��� LobbyManager�� ���)
/// </summary>
public class PanelManager : MonoBehaviour
{
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject banPickPanel;
    [SerializeField] private GameObject deckPanel;

    [SerializeField] private Text loginIDText;
    [SerializeField] private Text loginPWText;

    [SerializeField] private Text registerIDText;
    [SerializeField] private Text registerPWText;

    private GameObject currentPanel;

    private void Awake()
    {
        currentPanel = loginPanel;
    }

    /// <summary>
    /// �α��� ȭ������ ���ư���
    /// </summary>
    public void SetLoginPanel(bool isRegister)
    {
        if (isRegister)
        {
            if (string.IsNullOrEmpty(registerIDText.text) || string.IsNullOrEmpty(registerPWText.text))
            {
                return;
            }
        }
        currentPanel.SetActive(false);
        loginPanel.SetActive(true);
        currentPanel = loginPanel;
    }

    /// <summary>
    /// ȸ������ ȭ������ ����
    /// </summary>
    public void SetRegisterPanel()
    {
        currentPanel.SetActive(false);
        registerPanel.SetActive(true);
        currentPanel = registerPanel;
    }

    /// <summary>
    /// �κ� ȭ�� ����
    /// </summary>
    public void SetLobbyPanel(bool isLogin)
    {
        if (isLogin)
        {
            if (string.IsNullOrEmpty(loginIDText.text) || string.IsNullOrEmpty(loginPWText.text))
            {
                return;
            }
        }
        currentPanel.SetActive(false);
        lobbyPanel.SetActive(true);
        currentPanel = lobbyPanel;
    }

    /// <summary>
    /// ��Ī ���� �� ���� ȭ�� ����
    /// </summary>
    public void SetBanPickPanel()
    {
        currentPanel.SetActive(false);
        banPickPanel.SetActive(true);
        currentPanel = banPickPanel;
    }


}
