using UnityEngine;
using UnityEngine.SceneManagement;
public class Invaders : MonoBehaviour
{
    public Invader[] invaderPrefabs;
    public int rows = 5;

    public int columns = 11;

    private Vector3 _direction = Vector2.right;

    public AnimationCurve speed;
    public Projectile missilePrefab;
    public float missileAttackRate = 1.0f;

    public int invaderKilled { get; private set; }
    public int amountAlive => this.totalInvaders - this.invaderKilled;
    public int totalInvaders => this.rows * this.columns;
    public float percentKilled => (float)this.invaderKilled / (float)this.totalInvaders;

    private void Awake()
    {
        for (int row = 0; row < this.rows; row++)
        {

            float width = 4.0f * (this.columns - 1);
            float height = 4.0f * (this.rows - 1);

            Vector2 centering = new Vector2(-width / 2.0f, -height / 2.0f);
            Vector3 rowPosition = new Vector3(centering.x, centering.y + row * 4.0f, 0.0f);

            for (int column = 0; column < this.columns; column++)
            {
                Invader invader = Instantiate(this.invaderPrefabs[row], this.transform);
                invader.killed += InvaderKilled;
                Vector3 position = rowPosition;
                position.x += column * 4.0f;
                invader.transform.localPosition = position;
            }
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), this.missileAttackRate, this.missileAttackRate);
    }

    private void Update()
    {
        this.transform.position += this.speed.Evaluate(this.percentKilled) * Time.deltaTime * _direction;

        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (_direction == Vector3.right && invader.position.x >= (rightEdge.x - 1.0f))
            {
                AdvanceRow();
            }
            else if (_direction == Vector3.left && invader.position.x <= (leftEdge.x + 1.0f))
            {
                AdvanceRow();
            }
        }
    }

    private void AdvanceRow()
    {
        _direction.x *= -1.0f;

        Vector3 position = this.transform.position;
        position.y -= 1.0f;
        this.transform.position = position;
    }

    /// <summary>
    /// Performs a missile attack by instantiating a missile prefab at a random active invader's position.
    /// </summary>
    private void MissileAttack()
    {
        foreach (Transform invader in this.transform)
        {
            if (!invader.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (Random.value < (1.0f / this.amountAlive))
            {
                Instantiate(this.missilePrefab, invader.position, Quaternion.identity);
                break;
            }
        }
    }

    /// <summary>
    /// Increases the count of killed invaders.
    /// </summary>
    private void InvaderKilled()
    {
        this.invaderKilled++;

        if(this.amountAlive >= this.totalInvaders)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}