using UnityEngine;
using UnityEngine.EventSystems; // Tambahkan ini untuk pengecekan UI

public class DragMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    public float dragSpeed = 10f; // Semakin tinggi, semakin responsif
    private bool isDragging = false;

    void Update()
    {
        // Cek apakah kursor berada di atas UI, jika iya maka abaikan input
        if (EventSystem.current.IsPointerOverGameObject()) return;

        // Jika tombol kiri mouse ditekan, mulai dragging
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
        }
        // Jika tombol kiri mouse dilepas, hentikan dragging
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            // Mendapatkan posisi kursor dalam koordinat dunia
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetPosition = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
        }

        // Gerakan lebih smooth menggunakan Lerp
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * dragSpeed);
    }
}
