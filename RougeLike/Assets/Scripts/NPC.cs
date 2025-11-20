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

    void Update()
    {

        UpgradebutttonAppear();
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialoguePanel.activeInHierarchy)
            {
                NextLine(); // now also hides panel and stops typing

            }
            else
            {
                dialoguePanel.SetActive(true);
                // start typing fresh
                if (typingRoutine != null) { StopCoroutine(typingRoutine); }
                typingRoutine = StartCoroutine(TypeLine());
            }
        }

        // Remove reliance on string equality; TypeLine will show the continue button when done
        // if (dialogueText.text == dialogueLines[currentLineIndex]) contButton.SetActive(true);
    }

    private void ZeroText()
    {
        // stop typing and fully close the dialogue
        if (typingRoutine != null) { StopCoroutine(typingRoutine); typingRoutine = null; }
        isTyping = false;
        contButton.SetActive(false);
        upgradeButton.SetActive(false);
        dialogueText.text = "";
        dialoguePanel.SetActive(false);
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        contButton.SetActive(false);
        upgradeButton.SetActive(false);
        dialogueText.text = "";

        // safety check
        if (dialogueLines == null || dialogueLines.Length == 0)
        {
            isTyping = false;
            contButton.SetActive(false);
            upgradeButton.SetActive(false);
            yield break;
        }

        foreach (char letter in dialogueLines[currentLineIndex].ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }

        isTyping = false;
        contButton.SetActive(true);
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
        if (currentLineIndex == dialogueLines.Length - 1 && isTyping == false)
        {
            upgradeButton.SetActive(true);
            if (upgradeButton.activeInHierarchy == true && Input.GetKeyDown(KeyCode.R))
            {
                UpgradeMenu();
            }
        }

    }

    public void UpgradeMenu()
    {
        ZeroText();
        upgradeMenuPanel.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            currentLineIndex = 0;
            dialoguePanel.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            ZeroText();
            upgradeMenuPanel.SetActive(false);
        }
    }
}
