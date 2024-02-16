namespace Mapbox.Examples
{
	using UnityEngine;
	using Mapbox.Utils;
	using Mapbox.Unity.Map;
	using Mapbox.Unity.MeshGeneration.Factories;
	using Mapbox.Unity.Utilities;
	using System.Collections.Generic;

	public class SpawnOnMap : MonoBehaviour
	{
		[SerializeField]
		private AbstractMap _map;

		[SerializeField]
		[Geocode]
		private string[] _locationCoordinate;	// datos cargados de los locations existentes
		private Vector2d[] _locations;	// nuevo vector para almacenar las informaciones de _locationCoordinate

		[SerializeField]
		private float _spawnScale = 3f;

		[SerializeField]
		private GameObject _markerPrefab;	// icono de spawn de locationPoint

		private List<GameObject> _spawnedObjects;	// lista para guardar los locations instanciados

		void Start()
		{
			_locations = new Vector2d[_locationCoordinate.Length];
			_spawnedObjects = new List<GameObject>();
			for (int i = 0; i < _locationCoordinate.Length; i++)
			{
				var locationString = _locationCoordinate[i];
				_locations[i] = Conversions.StringToLatLon(locationString);	// pasa de formato string a Vector2D
				var instance = Instantiate(_markerPrefab);

				instance.GetComponent<LocationPointInformation>().setActualCoordinate(_locations[i]);
				instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
				instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
				_spawnedObjects.Add(instance);
			}
		}

		private void Update()
		{
			for (int i = 0; i < _spawnedObjects.Count; i++)
			{
				var spawnedObject = _spawnedObjects[i];
				var location = _locations[i];
				spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
				spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			}
		}
	}
}