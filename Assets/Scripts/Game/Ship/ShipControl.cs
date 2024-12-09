using Summoner.Game.Ship.States;
using Summoner.Utilities.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using Input = NPTP.InputSystemWrapper.Input;
using Summoner.Systems.Data.Persistent;
using Summoner.Systems.StateMachines;
using Summoner.Utilities;
using Summoner.Systems.ObjectPooling;
using Summoner.Utilities.Attributes;

namespace Summoner.Game.Ship
{
    public class ShipControl : MonoBehaviour
    {
        public static Collider2D PlayerCollider2DReference { get; private set; }

        [SerializeField][Required] private Rigidbody2D shipRigidbody;
        public Rigidbody2D ShipRigidBody => shipRigidbody;
        
        [SerializeField][Required] private Collider2D playerCollider2D;
        public Collider2D PlayerCollider2D => playerCollider2D;
        
        [SerializeField][Required] private StateMachine stateMachine;
        [Space]
        [SerializeField] private SpriteShapeRenderer jetsRenderer;
        public SpriteShapeRenderer JetsRenderer => jetsRenderer;
        
        public float LastShotTime { get; set; }
        public float FlickerElapsed { get; set; }
        
        public SetOnce<float> SqrMaxVelocityMagnitude { get; private set; }
        public bool Turning { get; private set; }
        public bool Thrusting { get; private set; }
        public Transform Transform { get; private set; }
        public float Direction { get; private set; }

        private void Awake()
        {
            Transform = transform;
            PlayerCollider2DReference = playerCollider2D;
            SqrMaxVelocityMagnitude = PersistentData.Player.MaxVelocityMagnitude.Squared();
            ObjectPooler.PrePopulatePool(PD.Player.BulletPrefab, 10);
            ObjectPooler.PrePopulatePool(PD.Player.BulletTrailPrefab, 100);
        }

        private void Start()
        {
            stateMachine.Queue(new ShipFlyState(this));
            jetsRenderer.enabled = false;
            AddInputListeners();
        }
        
        private void OnDestroy()
        {
            RemoveInputListeners();
        }

        private void AddInputListeners()
        {
            Input.Gameplay.Thrust.OnEvent += OnThrust;
            Input.Gameplay.Shoot.OnEvent += OnShoot;
            Input.Gameplay.Turn.OnEvent += OnTurn;
            Input.Gameplay.Hyperspace.OnEvent += OnHyperspace;
        }

        private void RemoveInputListeners()
        {
            Input.Gameplay.Thrust.OnEvent -= OnThrust;
            Input.Gameplay.Shoot.OnEvent -= OnShoot;
            Input.Gameplay.Turn.OnEvent -= OnTurn;
            Input.Gameplay.Hyperspace.OnEvent -= OnHyperspace;
        }
        
        private void OnThrust(InputAction.CallbackContext context)
        {
            Thrusting = context.started || context.performed;
        }


        private void OnShoot(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                stateMachine.Queue(new ShipShootState(this));
                return;
            }
        }
        
        private void OnTurn(InputAction.CallbackContext context)
        {
            Turning = context.started || context.performed;
            Direction = context.ReadValue<float>();
        }

        private void OnHyperspace(InputAction.CallbackContext context)
        {
            if (context.started && !stateMachine.CurrentStateIs<ShipHyperspaceState>())
            {
                stateMachine.Queue(new ShipHyperspaceState(this));
            }
        }
    }
}