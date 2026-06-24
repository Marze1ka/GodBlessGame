using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerModel _model;
    private PlayerView _view;
    private CharacterController _cc;
    private bool _hasPendingSavedHealth;
    private float _pendingSavedHealth;

    [Header("Combat settings")]
    public GameObject magicPrefab;
    public Transform firePoint;
    public Transform swordPoint;
    public float attackRadius = 1.5f;

    private Vector3 _velocity;
    public bool canMove = true;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _view = GetComponent<PlayerView>();
    }

    private void Start()
    {
        EnsureInitialized();
    }

    public void Initialize(PlayerModel model, PlayerView view)
    {
        if (_model != null)
        {
            return;
        }

        _model = model ?? new PlayerModel();
        _view = view != null ? view : GetComponent<PlayerView>();
        _cc = _cc != null ? _cc : GetComponent<CharacterController>();

        if (_view != null)
        {
            _model.OnHealthChanged += _view.SetHealth;
            _model.OnMagicTimerChanged += _view.SetMagic;
            _view.Initialize(_model.MaxHealth, _model.MagicCooldown);
            _view.SetHealth(_model.Health);
        }

        _model.OnDeath += HandleDeath;

        if (_hasPendingSavedHealth)
        {
            ApplySavedHealth(_pendingSavedHealth);
            _hasPendingSavedHealth = false;
        }
    }

    public void EnsureInitialized()
    {
        if (_model != null)
        {
            return;
        }

        Initialize(new PlayerModel(), _view != null ? _view : GetComponent<PlayerView>());
    }

    public PlayerModel GetModel()
    {
        EnsureInitialized();
        return _model;
    }

    public void SetHealthFromSave(float hp)
    {
        if (_model == null)
        {
            _pendingSavedHealth = hp;
            _hasPendingSavedHealth = true;
            EnsureInitialized();
            return;
        }

        ApplySavedHealth(hp);
    }

    private void ApplySavedHealth(float hp)
    {
        EnsureInitialized();
        _model.Health = Mathf.Clamp(hp, 0, _model.MaxHealth);

        if (_view != null)
        {
            _view.SetHealth(_model.Health);
        }
    }

    private void Update()
    {
        EnsureInitialized();

        if (_cc == null || !_cc.enabled) return;
        if (Time.timeScale == 0) return;

        _model.UpdateMagicTimer(Time.deltaTime);

        if (canMove)
        {
            HandleMovement();
            HandleCombat();
        }
    }

    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        float currentSpeed = _model.MoveSpeed;

        bool sprinting = Input.GetKey(KeyCode.LeftShift) && move.magnitude > 0.1f;
        if (sprinting)
        {
            currentSpeed *= _model.SprintMultiplier;
        }

        _cc.Move(move * currentSpeed * Time.deltaTime);

        if (_cc.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += -9.81f * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);

        float inputMagnitude = new Vector2(x, z).magnitude;
        float speedForAnim = inputMagnitude * (sprinting ? 5f : 2f);

        if (_view != null)
        {
            _view.UpdateMoveAnimation(speedForAnim);
        }
    }

    private void HandleCombat()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(PhysicalAttackRoutine());
        }

        if (Input.GetMouseButtonDown(1) && _model.IsMagicReady)
        {
            StartCoroutine(MagicAttackRoutine());
        }
    }

    private IEnumerator PhysicalAttackRoutine()
    {
        canMove = false;

        if (_view != null)
        {
            _view.PlayAnimation("Attack");
        }

        yield return new WaitForSeconds(0.4f);

        if (swordPoint != null)
        {
            Collider[] hits = Physics.OverlapSphere(swordPoint.position, attackRadius);
            foreach (Collider hit in hits)
            {
                if (hit.gameObject == gameObject) continue;

                Health health = hit.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(20, DamageType.Physical);
                    break;
                }
            }
        }

        yield return new WaitForSeconds(0.4f);
        canMove = true;
    }

    private IEnumerator MagicAttackRoutine()
    {
        _model.ResetMagicTimer();
        canMove = false;

        if (_view != null)
        {
            _view.PlayAnimation("Magic");
        }

        yield return new WaitForSeconds(0.5f);

        if (magicPrefab != null && firePoint != null)
        {
            Instantiate(magicPrefab, firePoint.position, transform.rotation);
        }

        yield return new WaitForSeconds(0.5f);
        canMove = true;
    }

    public void ApplyDamage(float amount)
    {
        EnsureInitialized();

        if (_model.Health <= 0) return;

        _model.ChangeHealth(-amount);

        if (_view != null)
        {
            _view.PlayAnimation("Hit");
        }

        Debug.Log($"Player damaged: -{amount}, HP={_model.Health}");
    }

    private void HandleDeath()
    {
        if (!enabled) return;
        StartCoroutine(DeathSequenceRoutine());
    }

    private IEnumerator DeathSequenceRoutine()
    {
        canMove = false;
        if (_cc != null) _cc.enabled = false;

        if (_view != null)
        {
            _view.PlayAnimation("Death");
        }

        yield return new WaitForSecondsRealtime(3f);

        GameManager gameManager = Object.FindAnyObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ShowGameOverScreen();
        }

        enabled = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (swordPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(swordPoint.position, attackRadius);
        }
    }
}
