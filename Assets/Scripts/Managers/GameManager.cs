using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : CustomMonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get { return instance; }
    }
    public Action<int> OnScoreChangedDelegate;
    public Action<bool> OnGameEndedDelegate;

    public PaddleController ActivePaddleController { get { return _activePaddleController; } }
    [SerializeField] private PaddleController _activePaddleController;

    public bool HasGameEnded { get { return hasGameEnded; } }
    private int _blocksAmount = 0;
    private int _score = 0;


    private bool hasGameEnded = false;
    

    public override void CustomAwake()
    {  
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public override void CustomStart()
    {
        _activePaddleController.PaddleModel.OnLifeChangedDelegate += LoseCondition;

        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
        _blocksAmount = blocks.Length;

        UpdateGameScore(_score);
    }
    
    public override void CustomOnDisable()
    {
        _activePaddleController.PaddleModel.OnLifeChangedDelegate -= LoseCondition;
    }

    private void LoseCondition(int remainingHealth)
    {
        if (remainingHealth <= 0)
        {
            hasGameEnded = true;
            OnGameEndedDelegate?.Invoke(false);
        }
    }
    
    private void WinCondition()
    {
        hasGameEnded = true;
        OnGameEndedDelegate?.Invoke(true);
    }

    public void UpdateBlocksInSceneCount()
    {
        _blocksAmount--;
        if (_blocksAmount <= 0)
        {
            WinCondition();
        }
    }
    public void UpdateGameScore(int score)
    {
        _score += score;
        OnScoreChangedDelegate?.Invoke(_score);
    }

    public int GetNumberOfActiveBallsInScene()
    {
        return _activePaddleController.PaddleModel.BallPool.GetNumberOfActivePoolObjects();
    }
}
