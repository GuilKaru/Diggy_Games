using UnityEngine;
namespace Diggy_MiniGame_2
{
	public class PickupSpawner : MonoBehaviour
	{
		[Header("Spawner Settings")]
		[SerializeField]
		private GameObject[] _pickupPrefabs;
		[SerializeField]
		private Transform _pickupParent;
		[SerializeField]
		private Vector2 _spawnAreaSize = new Vector2(5f, 5f);
		[SerializeField]
		private int _maxPickups = 10;
		[SerializeField]
		private float _spawnInterval = 3f;

		private void Start()
		{
			InvokeRepeating(nameof(SpawnPickup), 0f, _spawnInterval);
		}

		private void SpawnPickup()
		{
			int currentPickups = _pickupParent.childCount;
			if (currentPickups >= _maxPickups || _pickupPrefabs.Length == 0) return;

			Vector2 spawnPos = new Vector2(
				Random.Range(transform.position.x - _spawnAreaSize.x / 2, transform.position.x + _spawnAreaSize.x / 2),
				Random.Range(transform.position.y - _spawnAreaSize.y / 2, transform.position.y + _spawnAreaSize.y / 2)
			);

			GameObject randomPickup = _pickupPrefabs[Random.Range(0, _pickupPrefabs.Length)];
			Instantiate(randomPickup, spawnPos, Quaternion.identity, _pickupParent);
		}

		private void OnDrawGizmos()
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireCube(transform.position, _spawnAreaSize);
		}
	}

}

