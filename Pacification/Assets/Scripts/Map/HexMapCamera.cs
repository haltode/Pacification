using UnityEngine;
using System.Collections;

public class HexMapCamera : MonoBehaviour
{
    static HexMapCamera instance;
    public static bool Locked
    {
        set { instance.enabled = !value; }
    }

    const float TimeToMove = 0.4f;

    Transform swivel, stick;

    // 0 = fully zoomed out, 1 = fully zoomed in
    float zoom = 1f;

    public float stickMinZoom;
    public float stickMaxZoom;
    public float swivelMinZoom;
    public float swivelMaxZoom;

    public float moveSpeedMinZoom;
    public float moveSpeedMaxZoom;

    public HexGrid grid;

    Client client;
    Player player;
    int lastUnit;
    int lastCity;

    void Awake()
    {
        grid = FindObjectOfType<HexGrid>();
        if(GameManager.Instance.gamemode != GameManager.Gamemode.EDITOR)
        {
            client = FindObjectOfType<Client>();
            player = client.player;   
        }
        instance = this;
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if(zoomDelta != 0f)
        {
            AdjustZoom(zoomDelta);
            lastUnit = lastCity = 0;
        }

        if(GameManager.Instance.gamemode != GameManager.Gamemode.EDITOR &&
            (client.chat == null || client.chat.input.isFocused))
            return;

        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if(xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
            lastUnit = lastCity = 0;
        }
    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);
        
        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    void AdjustPosition(float xDelta, float zDelta)
    {
        Vector3 direction = new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    Vector3 ClampPosition(Vector3 position)
    {
        float xMax = (grid.cellCountX - 0.5f) * (2f * HexMetrics.InnerRadius);
        position.x = Mathf.Clamp(position.x, 0f, xMax);

        float zMax = (grid.cellCountZ - 1f) * (1.5f * HexMetrics.OuterRadius);
        position.z = Mathf.Clamp(position.z, 0f, zMax);

        return position;
    }

    public static void ValidatePosition()
    {
        instance.AdjustPosition(0f, 0f);
    }

    public static void FocusOnPosition(Vector3 position)
    {
        instance.transform.localPosition = position;
    }

    public IEnumerator FocusSmoothTransition(Vector3 position)
    {
        Vector3 currentPos = instance.transform.position;
        float t = 0f;
        while(t < 1)
        {
             t += Time.deltaTime / TimeToMove;
             instance.transform.position = Vector3.Lerp(currentPos, position, t);
             yield return null;
        }
    }

    public int CycleBetweenCities()
    {
        if(player.playerCities.Count == 0)
            return -1;
        if(lastCity >= player.playerCities.Count)
            lastCity = 0;
        Vector3 cityPos = player.playerCities[lastCity].Location.Position;
        StartCoroutine(FocusSmoothTransition(cityPos));
        int ret = lastCity;
        lastCity = (lastCity + 1) % player.playerCities.Count;
        lastUnit = 0;
        return ret;
    }

    public int CycleBetweenUnits()
    {
        if(player.playerUnits.Count == 0)
            return -1;
        if(lastUnit >= player.playerUnits.Count)
            lastUnit = 0;
        Vector3 unitPos = player.playerUnits[lastUnit].HexUnit.location.Position;
        StartCoroutine(FocusSmoothTransition(unitPos));
        int ret = lastUnit;
        lastUnit = (lastUnit + 1) % player.playerUnits.Count;
        lastCity = 0;
        return ret;
    }
}