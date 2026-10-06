using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public int roomsStarted;
    public GameObject EnemyParent;
    public GameObject WinScreen;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void CheckWin()
    {
        if (EnemyParent.transform.childCount <= 1)
        {
            Debug.Log("All enemies defeated! You win!");
            // Implement your win logic here (e.g., load next level, show win screen, etc.)
            WinScreen.SetActive(true);
        }
    }
}
