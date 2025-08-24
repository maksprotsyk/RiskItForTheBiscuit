using UnityEngine;
using Characters.Player;

namespace Managers
{

    public class GameplayManager : MonoBehaviour
    {

        private PlayerController _playerController;
		
        public PlayerController PlayerController
		{
            get
            {
                if (_playerController == null)
                {
                    _playerController = FindObjectOfType<PlayerController>();
                }
                return _playerController;
            }
        }
    }

}
