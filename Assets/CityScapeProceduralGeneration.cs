using System.Collections;
using UnityEngine;

public class CityScapeProceduralGeneration : MonoBehaviour
{
    public GameObject buildingPrefab;
    public Material roadMaterial;

    public int gridSize = 20;
    public float buildingSize = 1f;
    public float updateInterval = 0.01f;
    public int roadSpacing = 3;

    private GameObject[,] cityGrid;
    private bool isBuildingCity = false;

    private string currentPhase = ""; 

    GUIStyle guiStyle = new GUIStyle();

    private void Start()
    {
        guiStyle.fontSize = 24;
        guiStyle.normal.textColor = Color.white;
        guiStyle.alignment = TextAnchor.UpperLeft;
        GenerateCity();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !isBuildingCity)
        {
            GenerateCity();
        }
    }

    public void GenerateCity()
    {
        ClearCity();
        StartCoroutine(GenerateCityPhases());
    }

    private void ClearCity()
    {
        if (cityGrid != null)
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    if (cityGrid[x, z] != null)
                    {
                        Destroy(cityGrid[x, z]);
                        cityGrid[x, z] = null;
                    }
                }
            }
        }

        cityGrid = new GameObject[gridSize, gridSize];
    }

    private IEnumerator GenerateCityPhases()
    {
        isBuildingCity = true;

        Vector3 origin = transform.position;

        currentPhase = "Phase 1: Generating Buildings";
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                Vector3 position = origin + new Vector3(x * buildingSize, 0, z * buildingSize);
                GameObject obj = Instantiate(buildingPrefab, position, Quaternion.identity);
                cityGrid[x, z] = obj;

                float initialHeight = Random.Range(1, 4);
                obj.transform.localScale = new Vector3(buildingSize, initialHeight, buildingSize);

                yield return new WaitForSeconds(updateInterval);
            }
        }

        currentPhase = "Phase 2: Clearing Roads";
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (x % roadSpacing == 0 || z % roadSpacing == 0)
                {
                    GameObject road = cityGrid[x, z];
                    if (road != null)
                    {
                        road.GetComponent<MeshRenderer>().material = roadMaterial;
                        road.transform.localScale = new Vector3(buildingSize, 0.1f, buildingSize);
                    }
                }

                yield return new WaitForSeconds(updateInterval);
            }
        }

        currentPhase = "Phase 3: Adjusting Building Heights";
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (cityGrid[x, z] != null && (x % roadSpacing != 0 && z % roadSpacing != 0))
                {
                    GameObject building = cityGrid[x, z];
                    float newHeight = (x + z) % 5 < 3 ? Random.Range(2, 4) : Random.Range(4, 6);
                    building.transform.localScale = new Vector3(buildingSize, newHeight, buildingSize);
                }

                yield return new WaitForSeconds(updateInterval);
            }
        }

        currentPhase = "Generation Complete!";
        isBuildingCity = false;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 50), "Press B to Rebuild", guiStyle);
        GUI.Label(new Rect(10, 40, 300, 50), currentPhase, guiStyle);
    }
}
