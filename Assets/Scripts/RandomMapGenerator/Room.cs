using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector3Int scale;
    public List<Vector3Int> doors = new List<Vector3Int>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
    
    private void OnDrawGizmosSelected()
    {
        // 상하좌우 방향 벡터
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };

        // 각 방향에 대한 화살표 그리기
        foreach (Vector3 direction in directions)
        {
            Vector3 arrowEnd = transform.position + direction * 2f; // 화살표 끝점 계산
            Debug.DrawRay(transform.position, direction, Color.red); // 화살표 그리기
        }
    }
}
