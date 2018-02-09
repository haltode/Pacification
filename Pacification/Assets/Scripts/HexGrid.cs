using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
	public int width = 6;
	public int height = 6;

	public HexCell cellPrefab;
	public Text cellLabelPrefab;

	Canvas gridCanvas;
	HexMesh hexMesh;

	HexCell[] cells;

	void Awake()
	{
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[height * width];
		int id_cell = 0;
		for(int z = 0; z < height; ++z)
			for(int x = 0; x < width; ++x)
				CreateCell(x, z, id_cell++);
	}

	void Start()
	{
		hexMesh.Triangulate(cells);
	}

	void CreateCell(int x, int z, int id_cell)
	{
		Vector3 position;
		// Hexagonal spacing
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.InnerRadius * 2f);
		position.y = 0f;
		position.z = z * (HexMetrics.OuterRadius * 1.5f);

		HexCell cell = cells[id_cell] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);

		Text label = Instantiate<Text>(cellLabelPrefab);
		label.rectTransform.SetParent(gridCanvas.transform, false);
		label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
		label.text = cell.coordinates.ToStringOnSeparateLines();
	}

	void Update()
	{
		if(Input.GetMouseButton(0))
			HandleInput();
	}

	void HandleInput()
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(inputRay, out hit))
			TouchCell(hit.point);
	}

	void TouchCell(Vector3 position)
	{
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		Debug.Log("Touched at " + coordinates.ToString());
	}
}