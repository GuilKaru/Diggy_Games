using UnityEngine;
using System.Collections.Generic;

namespace Diggy_MiniGame_1
{
	public static class SpawnManager
	{
		private static readonly HashSet<Vector2> _occupiedPositions = new HashSet<Vector2>();

		// Register a position
		public static bool TryRegisterPosition(Vector2 position)
		{
			if (_occupiedPositions.Contains(position))
			{
				return false; // Position is already occupied
			}

			_occupiedPositions.Add(position);
			return true;
		}

		// Unregister a position (if needed, e.g., when an object is destroyed)
		public static void UnregisterPosition(Vector2 position)
		{
			_occupiedPositions.Remove(position);
		}

		public static void ClearAllPositions()
		{
			_occupiedPositions.Clear();
		}
	}
}

