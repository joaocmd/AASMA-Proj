using UnityEngine;

public interface IShip
{
    void OnKilled(string key);
    void OnNotifyKill(string key);
    void UpdateShip(GameObject ship, Intention intention);
    void RemoveShip(GameObject ship);
    void RemoveAllShips();
    void CoordinateHuntWhale(string key, Vector2 shipPos, Vector2 whalePos);
    void NotifyWhaleSighted(Vector2 whalePos);
}