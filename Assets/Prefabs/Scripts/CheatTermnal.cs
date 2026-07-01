using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TerminalConsole : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI outputText;
    public TMP_InputField inputField;

    [Header("Скорость печати")]
    public float charDelay = 0.01f;
    public float lineDelay = 0.1f;

    [Header("Ограничение строк на экране")]
    public int maxVisibleLines = 6;

    [Header("Ссылка на чит-логику")]
    public CheatMenu cheatMenu;

    private readonly string[] bootLines = {
        "Microsoft GameOS [Version 2.7.14]",
        "(c) Game Systems Corporation. All rights reserved.",
        "",
        "C:\\>",
        "C:\\>boot recovery_mode",
        "Loading Recovery Environment...",
        "Loading System Modules...",
        "Loading Antivirus Core...",
        "ERROR: Antivirus Core not found.",
        "C:\\>scan /all",
        "Scanning sectors...",
        "[##########----------] 47%",
        "Threat detected.",
        "Name: VIRUS.EXE",
        "Location: CORE\\KERNEL",
        "Risk: CRITICAL",
        "Scan aborted.",
        "C:\\>delete VIRUS.EXE",
        "Access denied.",
        "Reason:",
        "File is protected.",
        "C:\\>whoami",
        "USER: PLAYER",
        "PERMISSIONS: ADMINISTRATOR",
        "WARNING:",
        "Administrator activity linked to infection.",
        "C:\\>help",
        "Available actions:",
        "1. more_power",
        "2. more_speed",
        "3. power_jump",
        "4. need_help",
        "C:\\>"
    };

    // Короткий текст для повторных открытий
    private readonly string[] shortLines = {
        "Available actions:",
        "1. more_power",
        "2. more_speed",
        "3. power_jump",
        "4. need_help",
        "C:\\>"
    };

    // Сохраняется между открытиями панели в течение игровой сессии
    private static bool hasBootedBefore = false;

    private readonly List<string> lineBuffer = new List<string>();
    private string currentTypingLine = "";
    private bool isBooting = true;
    private Coroutine bootRoutine;

    void OnEnable()
    {
        Time.timeScale = 0f; // ставим игру на паузу

        lineBuffer.Clear();
        currentTypingLine = "";
        outputText.text = "";
        inputField.text = "";
        inputField.interactable = false;
        isBooting = true;

        if (bootRoutine != null) StopCoroutine(bootRoutine);

        string[] linesToPlay = hasBootedBefore ? shortLines : bootLines;
        bootRoutine = StartCoroutine(PlaySequence(linesToPlay));

        hasBootedBefore = true;
    }

    void OnDisable()
    {
        Time.timeScale = 1f; // возвращаем игру

        if (bootRoutine != null) StopCoroutine(bootRoutine);
        inputField.onSubmit.RemoveListener(OnCommandSubmit);
    }

    IEnumerator PlaySequence(string[] lines)
    {
        foreach (string line in lines)
        {
            yield return StartCoroutine(TypeLine(line));
            CommitLine(currentTypingLine);
            currentTypingLine = "";
            yield return new WaitForSecondsRealtime(lineDelay);
        }

        isBooting = false;
        inputField.interactable = true;
        inputField.ActivateInputField();
        inputField.onSubmit.AddListener(OnCommandSubmit);
    }

    IEnumerator TypeLine(string line)
    {
        currentTypingLine = "";
        foreach (char c in line)
        {
            currentTypingLine += c;
            RedrawWithTyping();
            yield return new WaitForSecondsRealtime(charDelay);
        }
    }

    void CommitLine(string line)
    {
        lineBuffer.Add(line);
        while (lineBuffer.Count > maxVisibleLines)
        {
            lineBuffer.RemoveAt(0);
        }
        Redraw();
    }

    void RedrawWithTyping()
    {
        var sb = new StringBuilder();
        foreach (string l in lineBuffer)
            sb.Append(l).Append('\n');
        sb.Append(currentTypingLine);
        outputText.text = sb.ToString();
    }

    void Redraw()
    {
        var sb = new StringBuilder();
        foreach (string l in lineBuffer)
            sb.Append(l).Append('\n');
        outputText.text = sb.ToString();
    }

    void OnCommandSubmit(string command)
    {
        if (isBooting) return;

        command = command.Trim();
        inputField.text = "";

        if (lineBuffer.Count > 0)
            lineBuffer[lineBuffer.Count - 1] += command;

        HandleCommand(command);

        inputField.ActivateInputField();
    }

    void HandleCommand(string command)
    {
        switch (command)
        {
            case "1":
                CommitLine("[EXEC] more_power.exe -> OK");
                cheatMenu?.OnMorePower();
                break;
            case "2":
                CommitLine("[EXEC] more_speed.exe -> OK");
                cheatMenu?.OnMoreSpeed();
                break;
            case "3":
                CommitLine("[EXEC] power_jump.exe -> OK");
                cheatMenu?.OnPowerJump();
                break;
            case "4":
                CommitLine("[EXEC] need_help.exe -> OK");
                cheatMenu?.OnNeedHelp();
                break;
            default:
                CommitLine("'" + command + "' is not recognized as an internal command.");
                break;
        }
        CommitLine("C:\\>");
        cheatMenu?.ForceClose();
    }
}