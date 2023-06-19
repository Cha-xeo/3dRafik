using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GameProject
{
    public class PlayerControls : NetworkBehaviour
    {
        [SerializeField] float _speed;
        [SerializeField] float _rotationSpeed;
        [SerializeField] float _accumulatedRotation;
        [SerializeField] Animator _anim;
        [SerializeField] CinemachineVirtualCamera _vcam;
        [SerializeField] Transform _CameraFollowTarget;
        [SerializeField] Transform _FreeCameraFollowTarget;
        [SerializeField] AudioListener _audioListener;
        [SerializeField] Transform _playerTransform;
        [SerializeField] CharacterController _charachterController;
        int aimValue = 1;
        public float rotationPower = 3f;
        public float rotationLerp = 0.5f;


        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                _audioListener.enabled = true;
                _vcam.Priority = 1;
                _vcam.Follow = _playerTransform;
                //_vcam.Follow = _CameraFollowTarget;
            }
            else
            {
                _vcam.Priority = 0;
            }
        }

        void Update()
        {
            if (!IsOwner) return;

            // look around
            if (Input.GetMouseButton(0)) 
            {
                _vcam.Follow = _CameraFollowTarget;
                Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

                if (IsServer && IsLocalPlayer)
                {
                    LookAround(targetMouseDelta);
                }
                else if (IsClient && IsLocalPlayer)
                {
                    LookAroundServerRpc(targetMouseDelta);
                }
            } // look around and change direction
            else if (Input.GetMouseButton(1))
            {
                _vcam.Follow = _playerTransform;
                Vector2 targetMouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                if (IsServer && IsLocalPlayer)
                {
                    LookAround2(targetMouseDelta);
                }
                else if (IsClient && IsLocalPlayer)
                {
                    LookAround2ServerRpc(targetMouseDelta);
                }
            }
            else
            {
               _vcam.Follow = _playerTransform;
                _CameraFollowTarget.rotation = _playerTransform.rotation;
            }

            float xInput = Input.GetAxis("Horizontal");
            float yInput = Input.GetAxis("Vertical");

            Vector3 moveDirection = new Vector3(xInput, 0, yInput).normalized;
            if (IsServer && IsLocalPlayer)
            {
               // Move(moveDirection.x, moveDirection.z);
                Move(xInput, yInput);
            }else if (IsClient && IsLocalPlayer)
            {
               // MovePlayerServerRpc(moveDirection.x, moveDirection.z);
                MovePlayerServerRpc(xInput, yInput);
            }

        }

        void Move(float deltaX, float deltaZ)
        {
            Vector3 move = deltaX * _playerTransform.right + deltaZ * _playerTransform.forward;
            _charachterController.Move(move * _speed * Time.deltaTime);
        }

        void LookAround2(Vector2 _input)
        {
            float rotationAmountx = _input.x * rotationPower;
            _playerTransform.rotation *= Quaternion.AngleAxis(rotationAmountx, Vector3.up);
            LookAround(_input);
        }

        void LookAround(Vector2 _input)
        {
            float rotationAmountx = _input.x * rotationPower;
            float rotationAmounty = _input.y * rotationPower;
            
            /*_accumulatedRotation += rotationAmount;
            _CameraFollowTarget.rotation = Quaternion.Euler(0, _accumulatedRotation, 0);*/
            _CameraFollowTarget.rotation *= Quaternion.AngleAxis(rotationAmountx, Vector3.up);

            _CameraFollowTarget.rotation *= Quaternion.AngleAxis(rotationAmounty, Vector3.right);
            var angles = _CameraFollowTarget.localEulerAngles;
            angles.z = 0;
            var angle = _CameraFollowTarget.localEulerAngles.x;
            if(angle > 180 && angle < 340) 
            {
                angles.x = 340;
            }
            else if (angle < 180 && angle > 40)
            { 
                angles.x = 40;
            }
            _CameraFollowTarget.localEulerAngles = angles;
        }

        [ServerRpc]
        private void LookAroundServerRpc(Vector2 _input)
        {
            LookAround(_input);
        }

        [ServerRpc]
        private void LookAround2ServerRpc(Vector2 _input)
        {
            LookAround2(_input);
        }

        [ServerRpc]
        private void MovePlayerServerRpc(float deltaX, float deltaZ)
        {
            Move(deltaX, deltaZ);
        }
    }
}