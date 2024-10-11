using UnityEngine;

public class GameSetting : MonoBehaviour
{
    public int MaxMove;
    void Start()
    {
        GameManager.Instance.MaxMove = MaxMove;
        GameManager.Instance.MoveLeft = MaxMove;
    }
}
