using System.Collections;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    #region Public Fields + Properties + Events + Delegates

    public GameSystem Instance { get { return m_Instance; } }

    #endregion Public Fields + Properties + Events + Delegates

    #region Private Fields + Properties + Events + Delegates

    private GameSystem m_Instance;

    #endregion Private Fields + Properties + Events + Delegates

    #region Private Methods

    private void Awake()
    {
        m_Instance = this;
    }

    private void OnDestroy()
    {
        m_Instance = null;
    }

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #endregion Private Methods
}