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
		private List<Vector2d> _normalLocationsCoordinateList = new List<Vector2d>();   // nuevo vector para almacenar las informaciones de _locationCoordinate
		private List<Vector2d> _likedLocationsCoordinateList = new List<Vector2d>();   // nuevo vector para almacenar las informaciones de _locationCoordinate
		private List<Vector2d> _myLocationLocationsCoordinateList = new List<Vector2d>();   // nuevo vector para almacenar las informaciones de _locationCoordinate

		[SerializeField]
		private float _spawnScale = 3f;

		[SerializeField]
		private GameObject _normalLocationPrefab;   // icono de spawn de locationPoint
		[SerializeField]
		private GameObject _likedLocationPrefab;   // icono de spawn de locationPoint gustado
		[SerializeField]
		private GameObject _myLocationPrefab;   // icono de spawn de locationPoint añadidos por el usuario

		private List<GameObject> _normalLocationPrefabList = new List<GameObject>();  // lista para guardar los locations instanciados
		private List<GameObject> _likedLocationPrefabList = new List<GameObject>();  // lista para guardar los locations gustado instanciados
		private List<GameObject> _myLocationPrefabList = new List<GameObject>();  // lista para guardar los locations añadidos instanciados

		void Start()
		{

		}

		private void Update()
		{
			this.InstantiatePrefabsOnMap(this._normalLocationPrefabList, _normalLocationsCoordinateList);
			this.InstantiatePrefabsOnMap(this._likedLocationPrefabList, _likedLocationsCoordinateList);
			this.InstantiatePrefabsOnMap(this._myLocationPrefabList, _myLocationLocationsCoordinateList);
		}

		// crea el GameObject de la locationPoint normal
		public void InstantiateNormalLocationPointOnMap(LocationPoint point)
		{
			Debug.Log("EN MAP NORMAL -> " + "ID: " + point.Id + ", Latitud-Longitud: " + point.ConcatenarLatitudLongitud() + ", Altitud: " + point.Altitud + ", Creado por Usuario ID: " + point.CreatedByUserID + ", ID de Información: " + point.InformationId);

			// guardar la coordenada
			Vector2d locationIn2D = Conversions.StringToLatLon(point.ConcatenarLatitudLongitud());
			this._normalLocationsCoordinateList.Add(locationIn2D);

			// instanciar el gameobject
			var instance = Instantiate(_normalLocationPrefab);

			this.InitializeGameObject(instance, point);

			_normalLocationPrefabList.Add(instance);
		}
		public void InstantiateLikedLocationPointOnMap(LocationPoint point)
		{
			Debug.Log("EN MAP LIKED -> " + "ID: " + point.Id + ", Latitud-Longitud: " + point.ConcatenarLatitudLongitud() + ", Altitud: " + point.Altitud + ", Creado por Usuario ID: " + point.CreatedByUserID + ", ID de Información: " + point.InformationId);

			// guardar la coordenada
			Vector2d locationIn2D = Conversions.StringToLatLon(point.ConcatenarLatitudLongitud());
			this._likedLocationsCoordinateList.Add(locationIn2D);

			// instanciar el gameobject
			var instance = Instantiate(_likedLocationPrefab);

			this.InitializeGameObject(instance, point);

			_likedLocationPrefabList.Add(instance);
		}
		public void InstantiateMyLocationPointOnMap(LocationPoint point)
		{
			Debug.Log("EN MAP MY -> " + "ID: " + point.Id + ", Latitud-Longitud: " + point.ConcatenarLatitudLongitud() + ", Altitud: " + point.Altitud + ", Creado por Usuario ID: " + point.CreatedByUserID + ", ID de Información: " + point.InformationId);

			// guardar la coordenada
			Vector2d locationIn2D = Conversions.StringToLatLon(point.ConcatenarLatitudLongitud());
			this._myLocationLocationsCoordinateList.Add(locationIn2D);

			// instanciar el gameobject
			var instance = Instantiate(_myLocationPrefab);

			this.InitializeGameObject(instance, point);

			_myLocationPrefabList.Add(instance);
		}

		// settea la información al gameObject
		private void InitializeGameObject(GameObject locationObject, LocationPoint point)
		{
			LocationPointInformation locationInfo = locationObject.GetComponent<LocationPointInformation>();
			Vector2d actualCoordinate = Conversions.StringToLatLon(point.ConcatenarLatitudLongitud());

			locationInfo.ActualCoordinate = actualCoordinate;
			locationInfo.Id = point.Id;
			locationInfo.Altitud = point.Altitud;
			locationInfo.CreatedByUserID = point.CreatedByUserID;
			locationInfo.InformationId = point.InformationId;
		}
		// Método generalizado para instanciar prefabs en el mapa
		private void InstantiatePrefabsOnMap(List<GameObject> prefabList, List<Vector2d> coordinateList)
		{
			for (int i = 0; i < prefabList.Count; i++)
			{
				var spawnedObject = prefabList[i];
				var location = coordinateList[i];
				spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
				spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			}
		}
	}
}