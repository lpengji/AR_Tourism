using System.Numerics;
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
		private List<Vector2d> _locations = new List<Vector2d>();   // nuevo vector para almacenar las informaciones de _locationCoordinate

		[SerializeField]
		private float _spawnScale = 3f;

		[SerializeField]
		private GameObject _markerPrefab;   // icono de spawn de locationPoint

		private List<GameObject> _spawnedObjects = new List<GameObject>();  // lista para guardar los locations instanciados

		void Start()
		{

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

		public void InstantiateLocationPointOnMap(LocationPoint point)
		{
			Debug.Log("EN MAP " + "ID: " + point.Id + ", Latitud-Longitud: " + point.ConcatenarLatitudLongitud() + ", Altitud: " + point.Altitud + ", Creado por Usuario ID: " + point.CreatedByUserID + ", ID de Información: " + point.InformationId);

			// guardar la coordenada
			Vector2d locationIn2D = Conversions.StringToLatLon(point.ConcatenarLatitudLongitud());
			this._locations.Add(locationIn2D);

			// instanciar el gameobject
			var instance = Instantiate(_markerPrefab);

			this.InitializeGameObject(instance, point);

			_spawnedObjects.Add(instance);
		}

		private void InitializeGameObject(GameObject locationObject, LocationPoint point)
		{
			LocationPointInformation locationInfo = locationObject.GetComponent<LocationPointInformation>();
			Vector2d actualCoordinate = Conversions.StringToLatLon(point.ConcatenarLatitudLongitud());

			locationInfo.ActualCoordinate = actualCoordinate;
			locationInfo.Id = point.Id;
			locationInfo.Altitud = point.Altitud;
			locationInfo.CreatedByUserID = point.CreatedByUserID;
			locationInfo.InformationId = point.InformationId;

			locationObject.transform.localPosition = _map.GeoToWorldPosition(actualCoordinate, true);
			locationObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
		}
	}
}