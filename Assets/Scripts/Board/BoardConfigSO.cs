using UnityEngine;

[CreateAssetMenu(fileName = "BoardConfigSO", menuName = "Scriptable Objects/BoardConfigSO")]
public class BoardConfigSO : ScriptableObject
{
    //Guarda configurações da dificuldade do jogo
    public int Width;
    public int Height;
    public int MineCount;
    [Tooltip("Largura da vizinhança de tiles vazios em volta do primeiro click")]
    public int PoolSize;
}
