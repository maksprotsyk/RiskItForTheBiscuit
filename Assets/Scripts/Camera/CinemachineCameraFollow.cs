using Characters.Player;
using Cinemachine;
using Managers;
using UnityEngine;

namespace Camera
{
    public class CinemachineCameraFollow : MonoBehaviour
    {
        //private GameObject _pointToFollow;
        private Transform _playerTransform;

        void Start()
        {
            var player = ManagersOwner.GetManager<GameplayManager>().PlayerController;
            _playerTransform = player.transform;
            //_pointToFollow = new GameObject("PointToFollow");
            
            var virtualCamera = GetComponent<CinemachineVirtualCamera>();
            virtualCamera.Follow = _playerTransform;
        }

        // Update is called once per frame
        void Update()
        {
            //Vector3 playerPosition = _playerTransform.position;
            //Vector3 lookPosition = _movementComponent.LookPosition;
            //var targetPosition = Vector3.Lerp(playerPosition, lookPosition, 1/3f);

            //_pointToFollow.transform.position = targetPosition;
        }
    }
}
