using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    public GameObject dialoguePanel;
    public GameObject upgradeMenuPanel;
    public TMPro.TextMeshProUGUI dialogueText;
    public string[] dialogueLines;
    private int currentLineIndex;
    public GameObject contButton;
    public GameObject upgradeButton;
    public float wordSpeed;
    public bool playerInRange;

    private Coroutine typingRoutine;
    private bool isTyping;

    void Start()
    {
        // Find UI elements if not assigned (for when returning from other scenes)

    }

    void Update()
    {

        UpgradebutttonAppear();
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialoguePanel != null && dialoguePanel.activeInHierarchy)
            {
                NextLine(); // now also hides panel and stops typing

            }
            else if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
                // start typing fresh
                if (typingRoutine != null) { StopCoroutine(typingRoutine); }
                typingRoutine = StartCoroutine(TypeLine());
            }
        }
        if (dialoguePanel == null)
        {
            dialoguePanel = GameObject.FindWithTag("DiaPanel");
            dialoguePanel.SetActive(false);
            if (dialoguePanel != null) Debug.Log("DialoguePanel found");
        }
        if (upgradeMenuPanel == null)
        {
            upgradeMenuPanel = GameObject.FindWithTag("UpgMenu");
            upgradeMenuPanel.SetActive(false);
            if (upgradeMenuPanel != null) Debug.Log("UpgradeMenuPanel found");
        }
        if (dialogueText == null && dialoguePanel != null)
        {
            // Search for TextMeshPro component in children
            dialogueText = GameObject.FindWithTag("DiaText")?.GetComponent<TMPro.TextMeshProUGUI>();
            if (dialogueText != null) Debug.Log("DialogueText found");
        }
        if (contButton == null)
        {
            contButton = GameObject.FindWithTag("ContButton");
            contButton.SetActive(false);
            if (contButton != null) Debug.Log("ContButton found");
        }
        if (upgradeButton == null)
        {
            upgradeButton = GameObject.FindWithTag("UpgButton");
            upgradeButton.SetActive(false);
            if (upgradeButton != null) Debug.Log("UpgradeButton found");
        }

        // Remove reliance on string equality; TypeLine will show the continue button when done
        // if (dialogueText.text == dialogueLines[currentLineIndex]) contButton.SetActive(true);
    }

    private void ZeroText()
    {
        // stop typing and fully close the dialogue
        if (typingRoutine != null) { StopCoroutine(typingRoutine); typingRoutine = null; }
        isTyping = false;
        if (contButton != null) contButton.SetActive(false);
        if (upgradeButton != null) upgradeButton.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        if (contButton != null) contButton.SetActive(false);
        if (upgradeButton != null) upgradeButton.SetActive(false);
        if (dialogueText != null) dialogueText.text = "";

        // safety check
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            isTyping = false;
            if (contButton != null) contButton.SetActive(false);
            if (upgradeButton != null) upgradeButton.SetActive(false);
            yield break;
        }

        foreach (char letter in dialogueLines[currentLineIndex].ToCharArray())
        {
            if (dialogueText != null) dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
        if (contButton != null) contButton.SetActive(true);
    }

    public void NextLine()
    {
        if (isTyping) return;

        if (currentLineIndex < dialogueLines.Length - 1)
        {
            currentLineIndex++;
            if (typingRoutine != null) { StopCoroutine(typingRoutine); }
            typingRoutine = StartCoroutine(TypeLine());
        }
        else
        {
            ZeroText(); // close when finished
        }
    }

    public void UpgradebutttonAppear()
    {
        if (currentLineIndex == dialogueLines.Length - 1 && isTyping == false && dialoguePanel != null && dialoguePanel.activeInHierarchy)
        {
            if (upgradeButton != null)
                upgradeButton.SetActive(true);

            if (upgradeButton != null && upgradeButton.activeInHierarchy == true && Input.GetKeyDown(KeyCode.R))
            {
                UpgradeMenu();
            }
        }
        else
        {
            // Hide upgrade button when dialogue panel is closed or not on last line
            if (upgradeButton != null && !dialoguePanel.activeInHierarchy)
                upgradeButton.SetActive(false);
        }
    }

    public void UpgradeMenu()
    {
        ZeroText();
        if (upgradeMenuPanel != null) upgradeMenuPanel.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentLineIndex = 0;
            if (dialoguePanel != null) dialoguePanel.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            ZeroText();
            if (upgradeMenuPanel != null) upgradeMenuPanel.SetActive(false);
        }
    }


}
