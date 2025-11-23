using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BattleLog : MonoBehaviour
{
    [Header("UI References")]
    public Text logText;                    // Single scrolling text
    public int maxLogEntries = 20;          // Max lines before clearing old ones

    private Queue<string> logEntries = new Queue<string>();

    public void AddEntry(string message)
    {
        // Add timestamp or turn number if desired
        string entry = $"• {message}";
        
        logEntries.Enqueue(entry);
        
        // Remove oldest entry if too many
        if (logEntries.Count > maxLogEntries)
        {
            logEntries.Dequeue();
        }
        
        RefreshDisplay();
    }

    public void AddColoredEntry(string message, Color color)
    {
        string colorHex = ColorUtility.ToHtmlStringRGB(color);
        string entry = $"• <color=#{colorHex}>{message}</color>";
        
        logEntries.Enqueue(entry);
        
        if (logEntries.Count > maxLogEntries)
        {
            logEntries.Dequeue();
        }
        
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (logText == null) return;
        
        // Combine all entries with line breaks
        logText.text = string.Join("\n", logEntries);
    }

    public void Clear()
    {
        logEntries.Clear();
        if (logText) logText.text = "";
    }
}

// Updated BattleManager with logging
public partial class BattleManager
{
    [Header("Battle Log")]
    public BattleLog battleLog;

    // Call this whenever something happens in combat
    public void LogAction(string message)
    {
        if (battleLog != null)
        {
            battleLog.AddEntry(message);
        }
        Debug.Log($"[BATTLE] {message}");
    }

    public void LogDamage(string attacker, string target, int damage)
    {
        string msg = $"{attacker} deals {damage} damage to {target}!";
        if (battleLog != null)
        {
            battleLog.AddColoredEntry(msg, new Color(1f, 0.4f, 0.4f)); // Red for damage
        }
    }

    public void LogHeal(string healer, string target, int amount)
    {
        string msg = $"{healer} heals {target} for {amount} HP!";
        if (battleLog != null)
        {
            battleLog.AddColoredEntry(msg, new Color(0.4f, 1f, 0.4f)); // Green for healing
        }
    }

    public void LogBuff(string source, string target, string buffName)
    {
        string msg = $"{source} applies {buffName} to {target}!";
        if (battleLog != null)
        {
            battleLog.AddColoredEntry(msg, new Color(0.4f, 0.7f, 1f)); // Blue for buffs
        }
    }

}