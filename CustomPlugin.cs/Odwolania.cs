using UnityEngine;
using PluginAPI.Core;
using Mirror;
using System.Collections.Generic;

/*
public class DoubleJumpController : MonoBehaviour
{
    private Player player;
    private int jumpsRemaining = 2;
    private PlayerMovementSync movement;

    void Start()
    {
        player = Player.Get(gameObject);
        movement = player.GameObject.GetComponent<PlayerMovementSync>();
    }

    void Update()
    {
        if (jumpsRemaining > 0 && movement.IsJumping)
        {
            movement.ForceJump(1.5f);
            jumpsRemaining--;
        }
    }
}
*/

public class TimeShiftController : MonoBehaviour
{
    private float timeShift;

    public void Initialize(float shift) => timeShift = shift;

    void Start()
    {
        Player player = Player.Get(gameObject);
        player.SendBroadcast($"Przesunięcie czasowe: {timeShift}s!", 5);
        player.Position += new Vector3(0, timeShift / 10f, 0);
    }
}

public class VoiceModulator : MonoBehaviour
{
    public void SetRandomVoice()
    {
        // Placeholder: Implementacja modyfikacji głosu
    }
}

public class DecoyController : MonoBehaviour
{
    private Player owner;
    private float duration;

    public void Initialize(Player player, float time)
    {
        owner = player;
        duration = time;
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }
}

public class ForceFieldController : MonoBehaviour
{
    private float duration;

    public void Initialize(float time)
    {
        duration = time;
    }

    void Start()
    {
        Destroy(gameObject, duration);
    }
}

public class CustomItemHandler
{
    // Placeholder: Obsługa niestandardowych przedmiotów
}

/*
public class ItemSpawnManager
{
    public static void SpawnItem(ItemType itemType, Vector3 position, Quaternion rotation)
    {
        var item = InventorySystem.ItemFactory.CreateItem(itemType);
        item.transform.position = position;
        item.transform.rotation = rotation;
        NetworkServer.Spawn(item.gameObject);
    }
}
*/

public static class ExtensionsStatic
{
    public static void ShuffleList<T>(this IList<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}