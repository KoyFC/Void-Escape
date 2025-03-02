using UnityEngine;
using System.Collections;

public class PlayerMovementScript : MonoBehaviour
{
    private PlayerController m_PlayerController = null;

    private int m_CurrentIndex = 0;
    private int m_PreviousIndex = 0;
    [HideInInspector] public bool m_IsMoving = false;

    #region Main Methods
    private void Start()
    {
        m_PlayerController = GetComponent<PlayerController>();
    }

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
        if (m_IsMoving) return;

        m_PreviousIndex = m_CurrentIndex;

        if (PlayerInputScript.Instance.m_NextPressed && m_CurrentIndex < 1)
        {
            m_CurrentIndex++;
        }
        if (PlayerInputScript.Instance.m_PreviousPressed && m_CurrentIndex > -1)
        {
            m_CurrentIndex--;
        }

        // If we try to move to the same position (if we're already at the extremes) we don't continue.
        // If we did continue, player movement would get disabled for the duration of the movement,
        // even if we didn't physically move. Anyway, just leave this line here.
        if (m_PreviousIndex == m_CurrentIndex) return; 

        StopAllCoroutines();

        Vector3 newPosition;
        Quaternion newRotation;
        int indexDifference = m_PreviousIndex - m_CurrentIndex; // Determining the multiplier that we apply to the rotation

        if (InGameManager.Instance.m_IsHorizontal)
        {
            newPosition = UpdatePlayerPositionHorizontal();
            newRotation = Quaternion.Euler(0, 0, indexDifference * 45);
        }
        else
        {
            newPosition = UpdatePlayerPositionVertical();
            newRotation = Quaternion.Euler(-indexDifference * 30, 0, 0);
        }

        StartCoroutine(LerpToPosition(newPosition, true));
        StartCoroutine(LerpRotation(newRotation));
    }
    #endregion

    #region Helper Methods
    private void ResetPosition()
    {
        m_CurrentIndex = 0;
        Vector3 centerPoint = PointManager.Instance.m_PlayerPoints.CenterPoint.position;
        StartCoroutine(LerpToPosition(centerPoint, false));
    }

    private Vector3 UpdatePlayerPositionHorizontal()
    {
        Vector3 newPosition = new Vector3(0, 0, 10);

        switch (m_CurrentIndex)
        {
            case -1:
                newPosition.x = PointManager.Instance.m_PlayerPoints.LeftPoint.position.x;
                break;
            case 0:
                newPosition.x = PointManager.Instance.m_PlayerPoints.CenterPoint.position.x;
                break;
            case 1:
                newPosition.x = PointManager.Instance.m_PlayerPoints.RightPoint.position.x;
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
                newPosition.y = PointManager.Instance.m_PlayerPoints.BottomPoint.position.y;
                break;
            case 0:
                newPosition.y = PointManager.Instance.m_PlayerPoints.CenterPoint.position.y;
                break;
            case -1:
                newPosition.y = PointManager.Instance.m_PlayerPoints.TopPoint.position.y;
                break;
        }

        return newPosition;
    }

    private IEnumerator LerpToPosition(Vector3 targetPosition, bool blockMovement)
    {
        if (blockMovement) m_IsMoving = true;

        float time = 0;
        float duration = InGameManager.Instance.m_MovementLerpDuration;
        Vector3 startPosition = transform.position;

        while (time < duration)
        {
            if (m_PlayerController.m_PlayerHealth.m_Hit) yield break;

            transform.position = Vector3.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        if (blockMovement) m_IsMoving = false;
    }

    private IEnumerator LerpRotation(Quaternion targetRotation)
    {
        float time = 0;
        float duration = InGameManager.Instance.m_RotationLerpDuration;
        Quaternion originalRotation = transform.rotation;

        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(originalRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;

        time = 0;
        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(targetRotation, Quaternion.identity, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = Quaternion.identity;
    }
    #endregion
}

