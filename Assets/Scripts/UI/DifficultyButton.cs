using UnityEngine;

public class DifficultyButton : MonoBehaviour
{
    [SerializeField] BoardConfigSO _config;

    public void SetDifficulty()
    {
        GameManager.Instance.StartGame(_config);
    }
}
