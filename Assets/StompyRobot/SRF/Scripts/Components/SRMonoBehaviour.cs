namespace SRF
{
    using System.Diagnostics;
    using UnityEngine;

    public abstract class SRMonoBehaviour : MonoBehaviour
    {
        public Transform CachedTransform
        {
            [DebuggerStepThrough]
            [DebuggerNonUserCode]
            get
            {
                if (_transform == null)
                {
                    _transform = base.transform;
                }

                return _transform;
            }
        }

        public Collider CachedCollider
        {
            [DebuggerStepThrough]
            [DebuggerNonUserCode]
            get
            {
                if (_collider == null)
                {
                    _collider = GetComponent<Collider>();
                }

                return _collider;
            }
        }

        public Collider2D CachedCollider2D
        {
            [DebuggerStepThrough]
            [DebuggerNonUserCode]
            get
            {
                if (_collider2D == null)
                {
                    _collider2D = GetComponent<Collider2D>();
                }

                return _collider2D;
            }
        }

        public Rigidbody CachedRigidBody
        {
            [DebuggerStepThrough]
            [DebuggerNonUserCode]
            get
            {
                if (_rigidBody == null)
                {
                    _rigidBody = GetComponent<Rigidbody>();
                }

                return _rigidBody;
            }
        }

        public Rigidbody2D CachedRigidBody2D
        {
            [DebuggerStepThrough]
            [DebuggerNonUserCode]
            get
            {
                if (_rigidbody2D == null)
                {
                    _rigidbody2D = GetComponent<Rigidbody2D>();
                }

                return _rigidbody2D;
            }
        }

        public GameObject CachedGameObject
        {
            [DebuggerStepThrough]
            [DebuggerNonUserCode]
            get
            {
                if (_gameObject == null)
                {
                    _gameObject = base.gameObject;
                }

                return _gameObject;
            }
        }

        public new Transform transform
        {
            get { return CachedTransform; }
        }

#if !UNITY_5

		public new Collider collider
		{
			get { return CachedCollider; }
		}
		public new Collider2D collider2D
		{
			get { return CachedCollider2D; }
		}
		public new Rigidbody rigidbody
		{
			get { return CachedRigidBody; }
		}
		public new Rigidbody2D rigidbody2D
		{
			get { return CachedRigidBody2D; }
		}
		public new GameObject gameObject
		{
			get { return CachedGameObject; }
		}

#endif


        private Collider _collider;
        private Transform _transform;
        private Rigidbody _rigidBody;
        private GameObject _gameObject;
        private Rigidbody2D _rigidbody2D;
        private Collider2D _collider2D;

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        protected void AssertNotNull(object value, string fieldName = null)
        {
        }

        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        protected void Assert(bool condition, string message = null)
        {
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        protected void EditorAssertNotNull(object value, string fieldName = null)
        {
            AssertNotNull(value, fieldName);
        }

        [Conditional("UNITY_EDITOR")]
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        protected void EditorAssert(bool condition, string message = null)
        {
            Assert(condition, message);
        }
    }
}
