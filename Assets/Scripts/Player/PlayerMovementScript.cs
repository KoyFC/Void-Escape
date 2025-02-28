using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    private int m_CurrentIndex = 0;

    #region Main Methods
    private void OnEnable()
    {
        PlayerInputScript.Instance.OnMovementPressed += HandleMovement;
        PlayerInputScript.Instance.OnPerspectiveChanged += ResetPosition;
    }

    private void OnDisable()
    {
        PlayerInputScript.Instance.OnMovementPressed -= HandleMovement;
        PlayerInputScript.Instance.OnPerspectiveChanged -= ResetPosition;
    }
    #endregion

    #region Handling Methods
    private void HandleMovement()
    {
        if (PlayerInputScript.Instance.m_NextPressed && m_CurrentIndex < 1)
        {
            m_CurrentIndex++;
        }
        if (PlayerInputScript.Instance.m_PreviousPressed && m_CurrentIndex > -1)
        {
            m_CurrentIndex--;
            UpdatePlayerPositionHorizontal();
        }

        if (PlayerInputScript.Instance.m_IsHorizontal)
        {
            UpdatePlayerPositionHorizontal();
        }
        else
        {
            UpdatePlayerPositionVertical();
        }
    }
    #endregion

    #region Helper Methods
    private void ResetPosition()
    {
        m_CurrentIndex = 0;
        transform.position = PointManager.Instance.m_Points.CenterPoint.position;
    }

    private void UpdatePlayerPositionHorizontal()
    {
        Vector3 newPosition = new Vector3(0, 0, 10);

        switch (m_CurrentIndex)
        {
            case -1:
                newPosition.x = PointManager.Instance.m_Points.LeftPoint.position.x;
                transform.position = newPosition;
                break;
            case 0:
                newPosition.x = PointManager.Instance.m_Points.CenterPoint.position.x;
                transform.position = newPosition;
                break;
            case 1:
                newPosition.x = PointManager.Instance.m_Points.RightPoint.position.x;
                transform.position = newPosition;
                break;
        }
    }

    private void UpdatePlayerPositionVertical()
    {
        Vector3 newPosition = new Vector3(0, 0, 10);

        switch (m_CurrentIndex)
        {
            case 1:
                newPosition.y = PointManager.Instance.m_Points.BottomPoint.position.y;
                transform.position = newPosition;
                break;
            case 0:
                newPosition.y = PointManager.Instance.m_Points.CenterPoint.position.y;
                transform.position = newPosition;
                break;
            case -1:
                newPosition.y = PointManager.Instance.m_Points.TopPoint.position.y;
                transform.position = newPosition;
                break;
        }
    }
    #endregion
}