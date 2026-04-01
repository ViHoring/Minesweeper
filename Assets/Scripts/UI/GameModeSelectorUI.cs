using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode
{
    Classic = 0,
    NoLuck = 1
}

public class GameModeSelectorUI : MonoBehaviour
{
    [SerializeField] Toggle _toggle;
    [SerializeField] TMP_Text _onOffText;
    [SerializeField] TMP_Text _modeDescriptionText;

    [SerializeField] string _classicDescription = "Clássico: pode exigir chute.";
    [SerializeField] string _noLuckDescription = "Sem sorte: tabuleiro gerado para ser resolvível sem chute.";

    [SerializeField] bool _toggleOnMeansNoLuck = true;
    [SerializeField] GameMode _defaultMode = GameMode.NoLuck;
    const string PrefKey = "GameMode";

    void Start()
    {
        var savedMode = GetSavedMode(_defaultMode);

        _toggle.isOn = _toggleOnMeansNoLuck
            ? savedMode == GameMode.NoLuck
            : savedMode == GameMode.Classic;

        RefreshTexts(CurrentModeFromToggle());

        _toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    void OnDestroy()
    {
        if (_toggle != null) _toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }

    void OnToggleChanged(bool isOn)
    {
        var mode = CurrentModeFromToggle();
        SaveMode(mode);
        RefreshTexts(mode);
    }

    GameMode CurrentModeFromToggle()
    {
        if (_toggleOnMeansNoLuck) return _toggle.isOn ? GameMode.NoLuck : GameMode.Classic;

        return _toggle.isOn ? GameMode.Classic : GameMode.NoLuck;
    }

    void RefreshTexts(GameMode mode)
    {
        if (_onOffText != null) _onOffText.text = _toggle.isOn ? "ON" : "OFF";

        if (_modeDescriptionText != null) _modeDescriptionText.text = mode == GameMode.Classic ? _classicDescription : _noLuckDescription;
    }

    static void SaveMode(GameMode mode)
    {
        PlayerPrefs.SetInt(PrefKey, (int)mode);
        PlayerPrefs.Save();
    }

    public static GameMode GetSavedMode(GameMode defaultMode = GameMode.NoLuck)
    {
        return (GameMode)PlayerPrefs.GetInt(PrefKey, (int)defaultMode);
    }
}