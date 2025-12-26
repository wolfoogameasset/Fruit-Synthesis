using System.Linq;

namespace SRF
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public sealed class RequiredFieldAttribute : Attribute
    {
        private bool _autoCreate;
        private bool _autoSearch;
        private bool _editorOnly = true;

        public RequiredFieldAttribute(bool autoSearch)
        {
            AutoSearch = autoSearch;
        }

        public RequiredFieldAttribute() { }

        public bool AutoSearch
        {
            get { return _autoSearch; }
            set { _autoSearch = value; }
        }

        public bool AutoCreate
        {
            get { return _autoCreate; }
            set { _autoCreate = value; }
        }

        [Obsolete]
        public bool EditorOnly
        {
            get { return _editorOnly; }
            set { _editorOnly = value; }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class ImportAttribute : Attribute
    {
        public readonly Type Service;
        public ImportAttribute() { }

        public ImportAttribute(Type serviceType)
        {
            Service = serviceType;
        }
    }

    public abstract class SRMonoBehaviourEx : SRMonoBehaviour
    {
        private static Dictionary<Type, IList<FieldInfo>> _checkedFields;

        private static void CheckFields(SRMonoBehaviourEx instance, bool justSet = false)
        {
            if (_checkedFields == null)
            {
                _checkedFields = new Dictionary<Type, IList<FieldInfo>>();
            }

            var t = instance.GetType();

            IList<FieldInfo> cache;

            if (!_checkedFields.TryGetValue(instance.GetType(), out cache))
            {
                cache = ScanType(t);

                _checkedFields.Add(t, cache);
            }

            PopulateObject(cache, instance, justSet);
        }

        private static void PopulateObject(IList<FieldInfo> cache, SRMonoBehaviourEx instance, bool justSet)
        {
            for (var i = 0; i < cache.Count; i++)
            {
                var f = cache[i];

                if (!EqualityComparer<object>.Default.Equals(f.Field.GetValue(instance), null))
                {
                    continue;
                }

                if (f.Import)
                {
                    var t = f.ImportType ?? f.Field.FieldType;

                    continue;
                }

                if (f.AutoSet)
                {
                    var newValue = instance.GetComponent(f.Field.FieldType);

                    if (!EqualityComparer<object>.Default.Equals(newValue, null))
                    {
                        f.Field.SetValue(instance, newValue);
                        continue;
                    }
                }

                if (justSet)
                {
                    continue;
                }

                if (f.AutoCreate)
                {
                    var newValue = instance.CachedGameObject.AddComponent(f.Field.FieldType);
                    f.Field.SetValue(instance, newValue);
                }
            }
        }

        private static List<FieldInfo> ScanType(Type t)
        {
            var cache = new List<FieldInfo>();

#if NETFX_CORE
		var fields = t.GetTypeInfo().DeclaredFields.Where(f => !f.IsStatic);
#else
            var fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
#endif

            foreach (var f in fields)
            {


                var info = new FieldInfo();
                info.Field = f;



                cache.Add(info);
            }

            return cache;
        }

        protected virtual void Awake()
        {
            CheckFields(this);
        }

        private struct FieldInfo
        {
            public bool AutoCreate;
            public bool AutoSet;
            public System.Reflection.FieldInfo Field;
            public bool Import;
            public Type ImportType;
        }
    }
}
