// PlayerController.cs
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Referências")]
    private CharacterController controller;
    private Camera playerCamera;

    [Header("Movimentação")]
    public float moveSpeed = 5f;
    public float sprintSpeedMultiplier = 1.02f; // 2% mais rápido
    public float gravity = -9.81f;
    private Vector3 velocity;

    [Header("Controle da Câmera")]
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;

    [Header("Vida")]
    public float maxHealth = 5f; // 5 barras de vida
    public float currentHealth;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        playerCamera = Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        currentHealth = maxHealth;
        HUDManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void HandleMovement()
    {
        // Resetar gravidade se estiver no chão
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        float currentSpeed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed *= sprintSpeedMultiplier;
        }

        controller.Move(move * currentSpeed * Time.deltaTime);

        // Aplicar gravidade
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        HUDManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        Debug.Log("Player tomou dano! Vida atual: " + currentHealth);
    }

    void Die()
    {
        Debug.Log("Jogador Morreu!");
        // Aqui você pode adicionar lógica de fim de jogo, como reiniciar a cena.
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f; // Pausa o jogo
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Exemplo de reinício
    }

    // --- Funções para Upgrades ---
    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth += amount; // Cura o jogador ao receber o upgrade
        HUDManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
    }

    public void IncreaseSpeed(float multiplier)
    {
        moveSpeed *= multiplier;
    }
}