using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public GameObject[] gridObjectsPrefabs; // ������ �������� ������� ��������
    public int gridSize = 5; // ������ �������� ����

    private GameObject[,] gridObjects; // ������ �������� �� ������� ����
    private GameObject selectedObject; // ��������� ������

    void Start()
    {
        gridObjects = new GameObject[gridSize, gridSize];
        GenerateObjects();
    }

    void GenerateObjects()
    {
        for (int i = 0; i < gridSize; i++) // ��������� 5 �����
        {
            for (int j = 0; j < gridSize; j++) // ��������� 5 ��������
            {
                GameObject newObject = GetNewObjectWithUniqueNeighbors(i, j);
                Vector3 position = new Vector3(i, 0, j); // ������� ������� �������
                newObject.transform.position = position; // ������������� ������� �������

                gridObjects[i, j] = newObject; // ��������� ��������� ������ � ������
            }
        }
    }

    GameObject GetNewObjectWithUniqueNeighbors(int x, int y)
    {
        int randomIndex = Random.Range(0, gridObjectsPrefabs.Length); // ��������� ������ �� ������� ��������

        // ������� ����� ������ � ��������� ��� �������
        while (true)
        {
            GameObject newObject = Instantiate(gridObjectsPrefabs[randomIndex]);

            bool hasMatchingNeighbors = HasMatchingNeighbors(x, y, newObject);

            if (!hasMatchingNeighbors)
            {
                return newObject;
            }
            else
            {
                // ������� ������ � �������� ������� ����� �����
                Destroy(newObject);
                randomIndex = Random.Range(0, gridObjectsPrefabs.Length);
            }
        }
    }

    bool HasMatchingNeighbors(int x, int y, GameObject newObject)
    {
        // ��������� ���� ������� �� ������� �������� � ����� �� �����
        return HasMatchingNeighbor(x, y, -1, 0, newObject) || // ����
               HasMatchingNeighbor(x, y, 1, 0, newObject) || // ���
               HasMatchingNeighbor(x, y, 0, -1, newObject) || // ����
               HasMatchingNeighbor(x, y, 0, 1, newObject); // �����
    }

    bool HasMatchingNeighbor(int x, int y, int offsetX, int offsetY, GameObject newObject)
    {
        int newX = x + offsetX;
        int newY = y + offsetY;

        if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize)
        {
            GameObject neighborObject = gridObjects[newX, newY];

            if (neighborObject != null && newObject.CompareTag(neighborObject.tag))
            {
                return true;
            }
        }

        return false;
    }


    void Update()
    {
        HandleInput();
        // CheckMatches(); // ��� ������� ����������� ���� �����
    }

    void HandleInput()
    {
        // ��������� ����� ���� (����)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("������ ������");

                // ���������, ��� �� ������ �� ������� �� ������� ����
                if (hit.collider != null && IsGamePiece(hit.collider.gameObject))
                {
                    Debug.Log("�� ������� ������");

                    GameObject clickedObject = hit.collider.gameObject;

                    // ���� ������ ��� ������, �� ������ ��� ������� � ���������
                    if (selectedObject != null)
                    {
                        Debug.Log("������ � ���������");
                        MoveObject(selectedObject, clickedObject);
                        selectedObject = null; // ���������� ��������� ������ ����� ������
                    }
                    else
                    {
                        selectedObject = clickedObject; // �������� ������ �� �����
                    }
                }
            }
        }
    }

    bool IsGamePiece(GameObject obj)
    {
        // ���������, �������� �� ������ ������� ���������, ��������� ��� ������ gridObjectsPrefabs
        foreach (GameObject prefab in gridObjectsPrefabs)
        {
            if (obj.CompareTag(prefab.tag))
            {
                return true;
            }
        }
        return false;
    }

    void MoveObject(GameObject selected, GameObject clicked)
    {
        // �������� ������� ��������� ��������
        Vector2Int selectedPos = GetObjectPosition(selected);
        Vector2Int clickedPos = GetObjectPosition(clicked);

        // ����� ��������� � ������� gridObjects
        GameObject temp = gridObjects[selectedPos.x, selectedPos.y];
        gridObjects[selectedPos.x, selectedPos.y] = gridObjects[clickedPos.x, clickedPos.y];
        gridObjects[clickedPos.x, clickedPos.y] = temp;

        // ��������� ������� ��������
        Vector3 selectedPosition = selected.transform.position;
        selected.transform.position = clicked.transform.position;
        clicked.transform.position = selectedPosition;

        // ��������� ������� ����� ����� ������
        CheckMatches();
    }

    bool HasMatchingNeighbors(int x, int y)
    {
        // �������� �������� �������� ������ �� ��������� � �����������
        return HasMatchingNeighbor(x, y, -1, 0) || // ����
               HasMatchingNeighbor(x, y, 1, 0) || // ���
               HasMatchingNeighbor(x, y, 0, -1) || // ����
               HasMatchingNeighbor(x, y, 0, 1); // �����
    }

    bool HasMatchingNeighbor(int x, int y, int offsetX, int offsetY)
    {
        int newX = x + offsetX;
        int newY = y + offsetY;

        if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize)
        {
            GameObject currentObject = gridObjects[x, y];
            GameObject neighborObject = gridObjects[newX, newY];
            GameObject secondNeighborObject = null;

            int secondX = x + 2 * offsetX;
            int secondY = y + 2 * offsetY;

            if (secondX >= 0 && secondX < gridSize && secondY >= 0 && secondY < gridSize)
            {
                secondNeighborObject = gridObjects[secondX, secondY];
            }

            if (currentObject != null && neighborObject != null && secondNeighborObject != null)
            {
                if (currentObject.CompareTag(neighborObject.tag) && currentObject.CompareTag(secondNeighborObject.tag))
                {
                    return true;
                }
            }
        }

        return false;
    }

    Vector2Int GetObjectPosition(GameObject obj)
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (gridObjects[i, j] == obj)
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        return new Vector2Int(-1, -1);
    }

    void CheckMatches()
    {
        StartCoroutine(CheckAndDestroyMatches());
    }

    IEnumerator MoveObjectToPosition(GameObject obj, Vector3 targetPosition, float duration)
    {
        if (obj == null) yield break; // ���������, �� ��� �� ������ ���������

        float elapsedTime = 0;
        Vector3 startPosition = obj.transform.position;

        while (elapsedTime < duration)
        {
            if (obj == null) yield break; // ���������, �� ��� �� ������ ��������� �� ����� ��������
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (obj != null) // ���������, �� ��� �� ������ ��������� �� ����� ��������
            obj.transform.position = targetPosition;
    }


    IEnumerator CheckAndDestroyMatches()
    {
        List<Vector2Int> destroyedPositions = new List<Vector2Int>(); // ������ ������� ������������ ��������

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject currentObject = gridObjects[i, j];
                if (currentObject != null)
                {
                    // �������� �������������� �����
                    if (i < gridSize - 2)
                    {
                        if (currentObject.CompareTag(gridObjects[i + 1, j].tag) && currentObject.CompareTag(gridObjects[i + 2, j].tag))
                        {
                            destroyedPositions.Add(new Vector2Int(i, j));
                            destroyedPositions.Add(new Vector2Int(i + 1, j));
                            destroyedPositions.Add(new Vector2Int(i + 2, j));
                        }
                    }
                    // ���������� ��� ������������ �����
                    if (j < gridSize - 2)
                    {
                        if (currentObject.CompareTag(gridObjects[i, j + 1].tag) && currentObject.CompareTag(gridObjects[i, j + 2].tag))
                        {
                            destroyedPositions.Add(new Vector2Int(i, j));
                            destroyedPositions.Add(new Vector2Int(i, j + 1));
                            destroyedPositions.Add(new Vector2Int(i, j + 2));
                        }
                    }
                }
            }
        }

        // �������� ����� ��������� ��������
        yield return new WaitForSeconds(1f);

        // ��������� � ������� �������
        foreach (Vector2Int pos in destroyedPositions)
        {
            GameObject objectToDestroy = gridObjects[pos.x, pos.y];
            gridObjects[pos.x, pos.y] = null;

            // ��������� ������ �� ������� ������� (������� ������� � ����� "Posgeroi")
            Vector3 targetPosition = GameObject.FindGameObjectWithTag("Posgeroi").transform.position;
            StartCoroutine(MoveObjectToPosition(objectToDestroy, targetPosition, 1f));

            // �������� ����� ���������
            yield return new WaitForSeconds(1f);

            Destroy(objectToDestroy); // ������� ������
        }

        // ����� �������� ��������, ������� ����� ������� �� �� �����
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (gridObjects[i, j] == null)
                {
                    GameObject newObject = GetNewObjectWithUniqueNeighbors(i, j);
                    Vector3 position = new Vector3(i, 0, j); // ������� ������� �������
                    newObject.transform.position = position; // ������������� ������� �������

                    gridObjects[i, j] = newObject; // ��������� ��������� ������ � ������
                }
            }
        }
    }



    void CheckNeighbors(int x, int y, Vector2Int movedObjectPos)
    {
        // �������� �������� �������� ������ �� ��������� � �����������
        CheckNeighbor(x, y, -1, 0, movedObjectPos); // ����
        CheckNeighbor(x, y, 1, 0, movedObjectPos); // ���
        CheckNeighbor(x, y, 0, -1, movedObjectPos); // ����
        CheckNeighbor(x, y, 0, 1, movedObjectPos); // �����
    }

    void CheckNeighbor(int x, int y, int offsetX, int offsetY, Vector2Int movedObjectPos)
    {
        int newX = x + offsetX;
        int newY = y + offsetY;

        if (newX >= 0 && newX < gridSize && newY >= 0 && newY < gridSize)
        {
            GameObject currentObject = gridObjects[x, y];
            GameObject neighborObject = gridObjects[newX, newY];
            GameObject secondNeighborObject = null;

            int secondX = x + 2 * offsetX;
            int secondY = y + 2 * offsetY;

            if (secondX >= 0 && secondX < gridSize && secondY >= 0 && secondY < gridSize)
            {
                secondNeighborObject = gridObjects[secondX, secondY];
            }

            if (currentObject != null && neighborObject != null && secondNeighborObject != null)
            {
                if (currentObject.CompareTag(neighborObject.tag) && currentObject.CompareTag(secondNeighborObject.tag))
                {
                    Destroy(currentObject);
                    Destroy(neighborObject);
                    Destroy(secondNeighborObject);

                    Vector3 newPosition = currentObject.transform.position;
                    GameObject newObject1 = Instantiate(gridObjectsPrefabs[Random.Range(0, gridObjectsPrefabs.Length)], newPosition, Quaternion.identity);
                    gridObjects[x, y] = newObject1;

                    newPosition = neighborObject.transform.position;
                    GameObject newObject2 = Instantiate(gridObjectsPrefabs[Random.Range(0, gridObjectsPrefabs.Length)], newPosition, Quaternion.identity);
                    gridObjects[newX, newY] = newObject2;

                    newPosition = secondNeighborObject.transform.position;
                    GameObject newObject3 = Instantiate(gridObjectsPrefabs[Random.Range(0, gridObjectsPrefabs.Length)], newPosition, Quaternion.identity);
                    gridObjects[secondX, secondY] = newObject3;
                }
            }
        }
    }
}
