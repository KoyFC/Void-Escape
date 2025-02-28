using UnityEngine;
using System.Collections;

public class PlayerMovementScript : MonoBehaviour
{
    private int m_CurrentIndex = 0;

    #region Main Methods
    private void OnEnable()
    {
        PlayerInputScript.Instance.OnMovementPressed += HandleMovement;
        InGameManager.Instance.OnPerspectiveChanged += ResetPosition;
    }

    private void OnDisable()
    {
        PlayerInputScript.Instance.OnMovementPressed -= HandleMovement;
        InGameManager.Instance.OnPerspectiveChanged -= ResetPosition;
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
        }

        StopAllCoroutines();

        Vector3 newPosition;
        if (InGameManager.Instance.m_IsHorizontal)
        {
            newPosition = UpdatePlayerPositionHorizontal();
            StartCoroutine(LerpToPosition(newPosition));
        }
        else
        {
            newPosition = UpdatePlayerPositionVertical();
            StartCoroutine(LerpToPosition(newPosition));
        }
    }
    #endregion

    #region Helper Methods
    private void ResetPosition()
    {
        m_CurrentIndex = 0;
        Vector3 centerPoint = PointManager.Instance.m_Points.CenterPoint.position;
        StartCoroutine(LerpToPosition(centerPoint));
    }

    private Vector3 UpdatePlayerPositionHorizontal()
    {
        Vector3 newPosition = new Vector3(0, 0, 10);

        switch (m_CurrentIndex)
        {
            case -1:
                newPosition.x = PointManager.Instance.m_Points.LeftPoint.position.x;
                break;
            case 0:
                newPosition.x = PointManager.Instance.m_Points.CenterPoint.position.x;
                break;
            case 1:
                newPosition.x = PointManager.Instance.m_Points.RightPoint.position.x;
                break;
        }

        return newPosition;
    }

    private Vector3 UpdatePlayerPositionVertical()
    {
        Vector3 newPosition = new Vector3(0, 0, 10);

        switch (m_CurrentIndex)
        {
            case 1:
                newPosition.y = PointManager.Instance.m_Points.BottomPoint.position.y;
                break;
            case 0:
                newPosition.y = PointManager.Instance.m_Points.CenterPoint.position.y;
                break;
            case -1:
                newPosition.y = PointManager.Instance.m_Points.TopPoint.position.y;
                break;
        }

        return newPosition;
    }

    private IEnumerator LerpToPosition(Vector3 targetPosition)
    {
        float time = 0;
        float duration = InGameManager.Instance.m_LerpDuration;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
    #endregion
}