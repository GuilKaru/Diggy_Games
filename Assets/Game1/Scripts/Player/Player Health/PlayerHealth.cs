using Diggy_MiniGame_1;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Diggy_MiniGame_1
{
	public class PlayerHealth : MonoBehaviour
	{
		//Serialize Fields
		#region Serialize Fields
		[Header("Player Hearts")]
		public int maxHearts = 6; // Maximum number of hearts
		public int currentHearts = 6; // Current number of hearts
		public GameObject heartContainerObject;

		[Header("Health Bar")]
		[SerializeField]
		private HealthBar _healthBar; // Reference to the health bar component

		[Header("Player Hit")]
		[SerializeField]
		private SpriteRenderer _spriteRenderer;

		[Header("Player Collider")]
		[SerializeField]
		private Collider2D _collider2D;


		[Header("Boilers")]
		[SerializeField]
		private GameObject[] boiler1Objects; // Array of GameObjects for Boiler 1
		[SerializeField]
		private GameObject[] boiler2Objects; // Array of GameObjects for Boiler 2
		[SerializeField]
		private GameObject[] boiler3Objects; // Array of GameObjects for Boiler 3
		[SerializeField]
		private GameObject[] boiler4Objects; // Array of GameObjects for Boiler 4
		[SerializeField]
		private GameObject[] boiler5Objects; // Array of GameObjects for Boiler 5
		[SerializeField]
		private GameObject[] boiler6Objects; // Array of GameObjects for Boiler 6

		#endregion

		//Private Variables
		#region Private Variables
		public bool IsDead { get; private set; } = false;
		private HeartContainer _heartContainer;
		private GameManager _gameManager;
		private Timer _timer;
		public KeyCode invincibilityToggleKey = KeyCode.I; // Key to toggle invincibility
		private bool _isInvincible = false; // Whether the player is currently invincible
		private Vector3 _initialPosition;

		#endregion

		//Initialization
		#region Initialization
		void Start()
		{
			_heartContainer = heartContainerObject.GetComponent<HeartContainer>();
			_gameManager = FindAnyObjectByType<GameManager>();
			_timer = FindAnyObjectByType<Timer>();
			_heartContainer.SetMaxHearts(maxHearts);
			_healthBar.SetMaxHealth(maxHearts);
			_initialPosition = transform.position;
			UpdateBoilerStatus();
		}

		void Update()
		{
			// If the invincibilityToggleKey is pressed, toggle the _isInvincible variable
			if (Input.GetKeyDown(invincibilityToggleKey))
			{
				_isInvincible = !_isInvincible;
				Debug.Log("Invincibility toggled: " + _isInvincible);
			}
		}

		#endregion

		//Health System
		#region Health System
		public void SetPlayerHealth(int hearts)
		{
			currentHearts = hearts;
			_heartContainer.SetHearts(currentHearts);
			_healthBar.UpdateHealth(currentHearts);
			UpdateBoilerStatus();
		}

		public void Damage(int heartAmount)
		{
			// Check if the player is invincible
			if (_isInvincible)
			{
				Debug.Log("Player is invincible, no damage taken.");
				return; // Exit the method without applying damage
			}

			// Deduct the health based on heartAmount (1 heart = 2 health points)
			currentHearts -= heartAmount; // Multiply by 2 to reflect the hearts

			_healthBar.UpdateHealth(currentHearts);

			// Clamp the current health to ensure it does not go below 0
			currentHearts = Mathf.Clamp(currentHearts, 0, maxHearts * 2); // 10 max health

			// Update the UI
			_heartContainer.SetHearts(currentHearts);
			UpdateBoilerStatus();

			// If health reaches 0, trigger GameOver
			if (currentHearts <= 0 && !IsDead)
			{
				IsDead = true; // Set IsDead to true
				_gameManager.GameOver();
				_timer.ResetTimer();
			}
			StartCoroutine(ToggleSpriteAndCollider(0.2f));
		}

		private void UpdateBoilerStatus()
		{
			// Toggle boiler GameObjects based on currentHearts
			ToggleBoilerObjects(boiler1Objects, currentHearts > 5);
			ToggleBoilerObjects(boiler2Objects, currentHearts > 4);
			ToggleBoilerObjects(boiler3Objects, currentHearts > 3);
			ToggleBoilerObjects(boiler4Objects, currentHearts > 2);
			ToggleBoilerObjects(boiler5Objects, currentHearts > 1);
			ToggleBoilerObjects(boiler6Objects, currentHearts > 0);
		}

		private void ToggleBoilerObjects(GameObject[] boilerObjects, bool isActive)
		{
			foreach (var obj in boilerObjects)
			{
				obj.SetActive(isActive);
			}
		}


		private IEnumerator ToggleSpriteAndCollider(float delay)
		{
			_collider2D.enabled = !_collider2D.enabled;
			yield return new WaitForSeconds(delay);

			float _toggleDuration = 2.0f; 
			float _toggleInterval = 0.2f;

			float endTime = Time.time + _toggleDuration;

			while (Time.time < endTime)
			{
				_spriteRenderer.enabled = !_spriteRenderer.enabled;
				yield return new WaitForSeconds(_toggleInterval);
			}
			_spriteRenderer.enabled = true;
			_collider2D.enabled = true;

		}






		#endregion

	}

}

