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
			// Instead of checking each position every time, we will use a simple register-unregister system
			if (_occupiedPositions.Contains(position))
			{
				return false; // Position already occupied
			}

			_occupiedPositions.Add(position);
			return true;
		}

		// Unregister a position when an object is destroyed or moved
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

