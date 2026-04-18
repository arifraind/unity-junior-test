using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpIconsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpringJumpController jumpController;
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Transform iconParent;
    [SerializeField] private Image iconPrefab;

    [Header("Visuals")]
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.25f;

    private readonly List<Image> spawnedIcons = new List<Image>();
    private int lastTotalJumps = -1;

    private void Start()
    {
        RebuildIconsIfNeeded();
        UpdateIconsVisual();
    }

    private void Update()
    {
        RebuildIconsIfNeeded();
        UpdateIconsVisual();
    }

    private void RebuildIconsIfNeeded()
    {
        if (jumpController == null || playerStats == null || iconParent == null || iconPrefab == null)
            return;

        int totalJumps = 1 + playerStats.ExtraAirJumps;

        if (totalJumps == lastTotalJumps)
            return;

        lastTotalJumps = totalJumps;

        ClearIcons();

        for (int i = 0; i < totalJumps; i++)
        {
            Image newIcon = Instantiate(iconPrefab, iconParent);
            newIcon.gameObject.SetActive(true);
            spawnedIcons.Add(newIcon);
        }
    }

    private void UpdateIconsVisual()
    {
        if (jumpController == null)
            return;

        int jumpsRemaining = jumpController.JumpsRemaining;
        int totalJumps = spawnedIcons.Count;

        for (int i = 0; i < spawnedIcons.Count; i++)
        {
            bool isActive = i >= (totalJumps - jumpsRemaining);
            SetIconAlpha(spawnedIcons[i], isActive ? activeAlpha : inactiveAlpha);
        }
    }

    private void SetIconAlpha(Image icon, float alpha)
    {
        if (icon == null)
            return;

        Color c = icon.color;
        c.a = alpha;
        icon.color = c;
    }

    private void ClearIcons()
    {
        for (int i = 0; i < spawnedIcons.Count; i++)
        {
            if (spawnedIcons[i] != null)
                Destroy(spawnedIcons[i].gameObject);
        }

        spawnedIcons.Clear();
    }
}