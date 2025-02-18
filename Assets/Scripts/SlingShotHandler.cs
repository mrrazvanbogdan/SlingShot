using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class SlingShotHandler : MonoBehaviour
{
    [Header("Line Renderers")]
    [SerializeField] private LineRenderer leftLineRenderer;
    [SerializeField] private LineRenderer rightLineRenderer;

    [Header("Transform References")]
    [SerializeField] private Transform leftStartPosition;
    [SerializeField] private Transform rightStartPosition;
    [SerializeField] private Transform centerPosition;
    [SerializeField] private Transform idlePosition;
    [SerializeField] private Transform elasticTransform;

    [Header("Slingshot Stats")]
    [SerializeField] private float maxDistance = 3.5f;
    [SerializeField] private float shotForce = 5f;
    [SerializeField] private float timeBetweenPlayerRespawns = 2f;
    [SerializeField] private float elasticDivider = 1.2f;
    [SerializeField] private AnimationCurve elasticCurve;

    [Header("Scripts")]
    [SerializeField] private SlingShotArea slingShotArea;
    [SerializeField] private CameraManager cameraManager;

    [Header("Player")]
    [SerializeField] private Player playerPrefab;
    [SerializeField] private float playerPositionOffset = 2f;

    private Camera mainCamera;
    private Vector2 slingShotLinesPosition;
    private Vector2 directionNormalized;

    private bool clickWithinArea;
    private bool playerOnSlingshot;

    private Player spawnedPlayer;

    private void Awake()
    {
        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;

        SpawnPlayer();
    }

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && slingShotArea.IsWithinSlingshotArea())
        {
            clickWithinArea = true;
        }

        if (Mouse.current.leftButton.isPressed && clickWithinArea && playerOnSlingshot)
        {
            DrawSlingShot();
            PositionAndRotatePlayer();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && clickWithinArea && playerOnSlingshot)
        {
            if (GameManager.instance.HasEnoughShots())
            {
                ReleaseSlingshot();
                clickWithinArea = false;

                if (spawnedPlayer != null)
                {
                    spawnedPlayer.LunchPlayer(directionNormalized, shotForce);
                }

                GameManager.instance.UseShot();
                playerOnSlingshot = false;

                AnimateSlingshot();

                if (GameManager.instance.HasEnoughShots())
                {
                    StartCoroutine(SpawnPlayerAfterTime());
                }
            }
        }
    }

    #region SlingShot Methods
    private void DrawSlingShot()
    {
        Vector3 touchPosition = mainCamera.ScreenToWorldPoint(new Vector3(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y, 0));
        touchPosition.z = 0;

        slingShotLinesPosition = centerPosition.position + Vector3.ClampMagnitude(touchPosition - centerPosition.position, maxDistance);

        SetLines(slingShotLinesPosition);

        directionNormalized = ((Vector2)centerPosition.position - slingShotLinesPosition).normalized;
    }

    private void SetLines(Vector3 position)
    {
        if (!leftLineRenderer.enabled && !rightLineRenderer.enabled)
        {
            leftLineRenderer.enabled = true;
            rightLineRenderer.enabled = true;
        }

        leftLineRenderer.SetPosition(0, leftStartPosition.position);
        leftLineRenderer.SetPosition(1, position);

        rightLineRenderer.SetPosition(0, rightStartPosition.position);
        rightLineRenderer.SetPosition(1, position);
    }

    private void ReleaseSlingshot()
    {
        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;

        ResetLines();
    }

    private void ResetLines()
    {
      
        leftLineRenderer.SetPosition(0, leftStartPosition.position);
        leftLineRenderer.SetPosition(1, idlePosition.position);

        rightLineRenderer.SetPosition(0, rightStartPosition.position);
        rightLineRenderer.SetPosition(1, idlePosition.position);
    }
    #endregion

    #region Player Methods
    private void SpawnPlayer()
    {
       
        Vector2 dir = (centerPosition.position - idlePosition.position).normalized;
        Vector2 spawnPosition = (Vector2)idlePosition.position + dir * playerPositionOffset;

        spawnedPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        spawnedPlayer.transform.right = dir;

        playerOnSlingshot = true;

        CameraManager cameraManager = FindAnyObjectByType<CameraManager>();
        if (cameraManager != null)
        {
            cameraManager.SetTarget(spawnedPlayer.transform);
        }

        leftLineRenderer.enabled = true;
        rightLineRenderer.enabled = true;
        ResetLines();

        
    }

    private void PositionAndRotatePlayer()
    {
        spawnedPlayer.transform.position = slingShotLinesPosition + directionNormalized * playerPositionOffset;
        float angle = Mathf.Atan2(directionNormalized.y, directionNormalized.x) * Mathf.Rad2Deg;
        spawnedPlayer.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private IEnumerator SpawnPlayerAfterTime()
    {
        yield return new WaitForSeconds(timeBetweenPlayerRespawns);
        SpawnPlayer();
        
    }
    #endregion

    #region Animated Slingshot
    private void AnimateSlingshot()
    {
        elasticTransform.position = slingShotLinesPosition;
        float dist = Vector2.Distance(elasticTransform.position, centerPosition.position);

        float time = dist / elasticDivider;

        elasticTransform.DOMove(centerPosition.position, time).SetEase(elasticCurve);
        StartCoroutine(AnimateSlingshotLines(elasticTransform, time));
    }

    private IEnumerator AnimateSlingshotLines(Transform trans, float time)
    {
        float elapsedTime = 0f;
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            SetLines(trans.position);

            yield return null;
        }
    }
    #endregion
}
