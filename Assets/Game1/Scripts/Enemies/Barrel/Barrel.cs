using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Diggy_MiniGame_1
{
	public class Barrel : MonoBehaviour
	{
		// Serialize Fields
		#region SerializeField
		[Header("Barrel Properties Settings")]
		[SerializeField]
		private float _speed = 2f;
		[SerializeField]
		private float _destroyXPosition = -10f;
		[SerializeField]
		private int _scoreValue = 10;

		[Header("Barrel Components Settings")]
		[SerializeField]
		SpriteRenderer _spriteRenderer;
		[SerializeField]
		Collider2D _collider;

		[Header("Barrel Audio")]
		[SerializeField]
		private AudioSource _barrelAudioSource;
		[SerializeField]
		private AudioClip[] _barrelClips;
		#endregion

		//Private Variables
		#region Private Variables
		private float _originalSpeed;
		private ScoreManager _scoreManager;
		private PlayerHealth _playerHealth;
		private PlayerController _playerController;
		private Rock _rock;
		#endregion

		// Initialization
		#region Initialization

		private void Start()
		{
			// Find the ScoreManager script in the scene
			_scoreManager = FindObjectOfType<ScoreManager>();
			_playerHealth = FindObjectOfType<PlayerHealth>();
			_playerController = FindObjectOfType<PlayerController>();
			_barrelAudioSource = GetComponent<AudioSource>();
			_collider = GetComponent<Collider2D>();
			_spriteRenderer = GetComponent<SpriteRenderer>();
			_rock = FindObjectOfType<Rock>();
			_originalSpeed = _speed;
			if (_scoreManager == null)
			{
				Debug.LogError("ScoreManager not found in the scene. Ensure there is a GameObject with the ScoreManager script.");
			}
		}

		private void Update()
		{
			MoveLeft();

			if (transform.position.x <= _destroyXPosition)
			{
				DestroyBarrel();
			}
		}

		#endregion

		// Movement Methods
		#region Movement Methods
		private void MoveLeft()
		{
			transform.Translate(Vector3.left * _speed * Time.deltaTime);
		}
		#endregion

		// Speed Modification Method (for external control)
		#region Speed Control
		public void SetSpeed(float newSpeed)
		{
			_speed = newSpeed;
		}

		// Method to restore speed to its original value
		public void RestoreSpeed()
		{
			_speed = _originalSpeed; // Restore the speed
		}
		#endregion

		//Collision
		#region Collision

		private void OnCollisionEnter2D(Collision2D collision)
		{
			if (collision.gameObject.CompareTag("AttackPoint"))
			{
				_scoreManager.AddScore(_scoreValue); 
				DestroyBarrel();
			}

			if (collision.gameObject.CompareTag("Player"))
			{
				_playerController.PlayAudioPlayerHitClip(0);
				_playerHealth.Damage(1);
				DestroyBarrel();
			}

			if (collision.gameObject.CompareTag("Boomerang"))
			{
				_scoreManager.AddScore(_scoreValue);
				PlayAudioBarrelClip(0);
				StartCoroutine(DestroyBarrelWithShovel());
			}

			if (collision.gameObject.CompareTag("Rock"))
			{
				DestroyBarrel();
			}

		}
		#endregion

		// Utility Methods
		#region Utility Methods
		private void DestroyBarrel()
		{
			Destroy(gameObject);
		}

		private IEnumerator DestroyBarrelWithShovel()
		{
			PlayAudioBarrelClip(0);
			_spriteRenderer.enabled = false;
			_collider.enabled = false;

			yield return new WaitForSeconds(1);
			Destroy(gameObject);
		}


		private void PlayAudioBarrelClip(int clipIndex)
		{
			if (clipIndex >= 0 && clipIndex < _barrelClips.Length)
			{
				_barrelAudioSource.clip = _barrelClips[clipIndex];
				_barrelAudioSource.Play();
			}
		}

		#endregion
	}
}

