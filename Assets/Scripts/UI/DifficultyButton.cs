using UnityEngine;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] private BoardConfigSO _config;

    public void OnClick()
    {
        GameManager.Instance.StartGame(_config);
    }
}
