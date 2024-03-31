using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public GameObject[] gridObjectsPrefabs; // Массив префабов игровых объектов
    public int gridSize = 5; // Размер игрового поля

    private GameObject[,] gridObjects; // Массив объектов на игровом поле
    private GameObject selectedObject; // Выбранный объект

    void Start()
    {
        gridObjects = new GameObject[gridSize, gridSize];
        GenerateObjects();
    }

    void GenerateObjects()
    {
        for (int i = 0; i < gridSize; i++) // Генерация 5 строк
        {
            for (int j = 0; j < gridSize; j++) // Генерация 5 столбцов
            {
                GameObject newObject = GetNewObjectWithUniqueNeighbors(i, j);
                Vector3 position = new Vector3(i, 0, j); // Позиция каждого объекта
                newObject.transform.position = position; // Устанавливаем позицию объекта

                gridObjects[i, j] = newObject; // Добавляем созданный объект в массив
            }
        }
    }

    GameObject GetNewObjectWithUniqueNeighbors(int x, int y)
    {
        int randomIndex = Random.Range(0, gridObjectsPrefabs.Length); // Случайный индекс из массива префабов

        // Создаем новый объект и проверяем его соседей
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
                // Удаляем объект и пытаемся создать новый снова
                Destroy(newObject);
                randomIndex = Random.Range(0, gridObjectsPrefabs.Length);
            }
        }
    }

    bool HasMatchingNeighbors(int x, int y, GameObject newObject)
    {
        // Проверяем всех соседей на наличие объектов с таким же тегом
        return HasMatchingNeighbor(x, y, -1, 0, newObject) || // Верх
               HasMatchingNeighbor(x, y, 1, 0, newObject) || // Низ
               HasMatchingNeighbor(x, y, 0, -1, newObject) || // Лево
               HasMatchingNeighbor(x, y, 0, 1, newObject); // Право
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
        // CheckMatches(); // Вам следует реализовать этот метод
    }

    void HandleInput()
    {
        // Обработка ввода мыши (клик)
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Объект выбран");

                // Проверяем, что мы попали по объекту на игровом поле
                if (hit.collider != null && IsGamePiece(hit.collider.gameObject))
                {
                    Debug.Log("По объекту попали");

                    GameObject clickedObject = hit.collider.gameObject;

                    // Если объект уже выбран, то меняем его местами с кликнутым
                    if (selectedObject != null)
                    {
                        Debug.Log("Меняем с кликнутым");
                        MoveObject(selectedObject, clickedObject);
                        selectedObject = null; // Сбрасываем выбранный объект после обмена
                    }
                    else
                    {
                        selectedObject = clickedObject; // Выбираем объект по клику
                    }
                }
            }
        }
    }

    bool IsGamePiece(GameObject obj)
    {
        // Проверяем, является ли объект игровым элементом, используя ваш массив gridObjectsPrefabs
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
        // Получаем позиции выбранных объектов
        Vector2Int selectedPos = GetObjectPosition(selected);
        Vector2Int clickedPos = GetObjectPosition(clicked);

        // Обмен объектами в массиве gridObjects
        GameObject temp = gridObjects[selectedPos.x, selectedPos.y];
        gridObjects[selectedPos.x, selectedPos.y] = gridObjects[clickedPos.x, clickedPos.y];
        gridObjects[clickedPos.x, clickedPos.y] = temp;

        // Обновляем позиции объектов
        Vector3 selectedPosition = selected.transform.position;
        selected.transform.position = clicked.transform.position;
        clicked.transform.position = selectedPosition;

        // Проверяем наличие рядов после обмена
        CheckMatches();
    }

    bool HasMatchingNeighbors(int x, int y)
    {
        // Проверка соседних объектов только по вертикали и горизонтали
        return HasMatchingNeighbor(x, y, -1, 0) || // Верх
               HasMatchingNeighbor(x, y, 1, 0) || // Низ
               HasMatchingNeighbor(x, y, 0, -1) || // Лево
               HasMatchingNeighbor(x, y, 0, 1); // Право
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
        if (obj == null) yield break; // Проверяем, не был ли объект уничтожен

        float elapsedTime = 0;
        Vector3 startPosition = obj.transform.position;

        while (elapsedTime < duration)
        {
            if (obj == null) yield break; // Проверяем, не был ли объект уничтожен во время анимации
            obj.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (obj != null) // Проверяем, не был ли объект уничтожен во время анимации
            obj.transform.position = targetPosition;
    }


    IEnumerator CheckAndDestroyMatches()
    {
        List<Vector2Int> destroyedPositions = new List<Vector2Int>(); // Список позиций уничтоженных объектов

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject currentObject = gridObjects[i, j];
                if (currentObject != null)
                {
                    // Проверка горизонтальных рядов
                    if (i < gridSize - 2)
                    {
                        if (currentObject.CompareTag(gridObjects[i + 1, j].tag) && currentObject.CompareTag(gridObjects[i + 2, j].tag))
                        {
                            destroyedPositions.Add(new Vector2Int(i, j));
                            destroyedPositions.Add(new Vector2Int(i + 1, j));
                            destroyedPositions.Add(new Vector2Int(i + 2, j));
                        }
                    }
                    // Аналогично для вертикальных рядов
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

        // Задержка перед удалением объектов
        yield return new WaitForSeconds(1f);

        // Анимируем и удаляем объекты
        foreach (Vector2Int pos in destroyedPositions)
        {
            GameObject objectToDestroy = gridObjects[pos.x, pos.y];
            gridObjects[pos.x, pos.y] = null;

            // Анимируем объект до целевой позиции (позиция объекта с тегом "Posgeroi")
            Vector3 targetPosition = GameObject.FindGameObjectWithTag("Posgeroi").transform.position;
            StartCoroutine(MoveObjectToPosition(objectToDestroy, targetPosition, 1f));

            // Задержка перед удалением
            yield return new WaitForSeconds(1f);

            Destroy(objectToDestroy); // Удаляем объект
        }

        // После удаления объектов, создаем новые объекты на их месте
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (gridObjects[i, j] == null)
                {
                    GameObject newObject = GetNewObjectWithUniqueNeighbors(i, j);
                    Vector3 position = new Vector3(i, 0, j); // Позиция каждого объекта
                    newObject.transform.position = position; // Устанавливаем позицию объекта

                    gridObjects[i, j] = newObject; // Добавляем созданный объект в массив
                }
            }
        }
    }



    void CheckNeighbors(int x, int y, Vector2Int movedObjectPos)
    {
        // Проверка соседних объектов только по вертикали и горизонтали
        CheckNeighbor(x, y, -1, 0, movedObjectPos); // Верх
        CheckNeighbor(x, y, 1, 0, movedObjectPos); // Низ
        CheckNeighbor(x, y, 0, -1, movedObjectPos); // Лево
        CheckNeighbor(x, y, 0, 1, movedObjectPos); // Право
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
