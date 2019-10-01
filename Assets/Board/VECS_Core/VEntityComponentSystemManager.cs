using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Voidheart {
    public class VEntityComponentSystemManager : SerializedSingleton<VEntityComponentSystemManager> {
        [SerializeField]
        private List<VEntity> gameEntities = new List<VEntity>();

        [SerializeField]
        [ListDrawerSettings(ShowPaging = false)]
        [ValidateInput("StaticValidateFunction", "Something is wrong with the system list! Check the debug log for more info.")]
        private List<VSystem> gameSystems = new List<VSystem>();

        private static bool StaticValidateFunction(List<VSystem> systemList) {
            HashSet<Type> types = new HashSet<Type>();
            int index = 0;
            foreach (VSystem sys in systemList) {
                if (sys == null) {
                    Debug.Log("System should not be null! Index: " + index);
                    return false;
                }
                if (types.Contains(sys.GetType())) {
                    Debug.Log(sys.GetType().FullName + " is in the list twice! Index: " + index);
                    return false;
                }
                types.Add(sys.GetType());
                index++;
            }
            return true;
        }

        [SerializeField]
        private List<VAnimationSystem> animationSystems = new List<VAnimationSystem>();

        private HashSet<VEntity> entitiesToDelete;

        [ShowInInspector]
        public GameplayEventQueue gameplayEventQueue = null;
        [SerializeField]
        public AnimationQueue animationQueue = null;

        [SerializeField]
        public List<VEntity> removedEntitiesForAnimation = new List<VEntity>();

        private Dictionary<string, VEntity> entityLookupTable = new Dictionary<string, VEntity>();
        private Dictionary<Type, VSystem> systemLookupTable = new Dictionary<Type, VSystem>();

        void Start() {
            Init();
        }

        // Should be true for debugging purposes only
        public bool updateManually = false;

        void Update() {
            if (!updateManually) {
                Tick();
            }
        }

        private void Init() {
            // initialize objects
            gameEntities = new List<VEntity>();
            entitiesToDelete = new HashSet<VEntity>();
            gameplayEventQueue = new GameplayEventQueue(this);

            // add self to systems
            foreach (VAnimationSystem aniSys in animationSystems) {
                aniSys.Init(this);
            }
            foreach (VSystem s in gameSystems) {
                s.Init(this);
            }

            // create special start game event
            VEntity startGameEvent = CreateEntity("StartGame");
            AddComponents(startGameEvent, new VComponent[] {
                new StartGameComponent()
            });
            gameplayEventQueue.Push(startGameEvent);
        }

        [Button]
        private void Step() {
            gameplayEventQueue.Step();
        }

        private void Tick() {
            gameplayEventQueue.Flush();
            // animationQueue.Flush();
        }

        public void RunSystems(VLifecycle lifecycle, bool shouldFlush, IEnumerable<VEntity> entities = null) {
            if (entities == null) {
                entities = gameEntities;
            }
            foreach (VEntity entity in entities) {
                foreach (VSystem system in gameSystems) {
                    if (system.ShouldOperate(entity)) {
                        system.OnLifecycle(entity, lifecycle);
                    }

                    if (shouldFlush) {
                        gameplayEventQueue.Flush();
                    }
                }
            }
        }

        // public interface into game monobehaviour
        // non-optimized
        public VEntity GetVEntityById(string id) {
            if (id == null) {
                return null;
            }

            if (entityLookupTable.ContainsKey(id)) {
                return entityLookupTable[id];
            }
            VEntity returnVEntity = gameEntities.Find((entity) => entity.id == id);
            entityLookupTable[id] = returnVEntity;
            return returnVEntity;
        }

        public VEntity GetVEntityByIdIncludingRemoved(string id) {
            var a = GetVEntityById(id);
            if (a != null) {
                return a;
            }

            return removedEntitiesForAnimation.Find((entity) => entity.id == id);
        }

        public IList<VEntity> GetEntitiesFromIds(IEnumerable<string> ids) {
            List<VEntity> entities = new List<VEntity>();
            foreach (string id in ids) {
                entities.Add(GetVEntityById(id));
            }
            return entities;
        }

        public List<VEntity> FilterEntities(IEnumerable<VEntity> entities = null, bool removeNull = true, Predicate<VEntity> test = null) {
            if (entities == null) {
                entities = this.gameEntities;
            }

            List<VEntity> returnList = new List<VEntity>();
            foreach (VEntity entity in entities) {
                if ((test == null || test.Invoke(entity)) && (removeNull == false || entity != null)) {
                    returnList.Add(entity);
                }
            }
            return returnList;
        }

        public T GetVComponent<T>(string id) where T : VComponent {
            return GetVComponent<T>(GetVEntityById(id));
        }

        public static T GetVComponent<T>(VEntity entity) where T : VComponent {
            foreach (VComponent comp in entity.Components) {
                if (comp is T) {
                    return (T) comp;
                }
            }
            return null;
        }

        // <summary> Passes in a list of ids that identify entities, can optionally say removeNull for synchronizing lists, and can pass in a test to filter the list more.</summary>
        public List<T> GetVComponentsFromList<T>(IEnumerable<string> ids, bool removeNull = true, Predicate<VEntity> test = null) where T : VComponent {
            List<T> returnList = new List<T>();
            foreach (string entityId in ids) {
                var comp = GetVComponent<T>(entityId);
                if ((test == null || test.Invoke(GetVEntityById(entityId))) && (!removeNull || comp != null)) {
                    returnList.Add(comp);
                }
            }
            return returnList;
        }

        // <summary> Passes in a list of entities, can optionally say removeNull for synchronizing lists, and can pass in a test to filter the list more.</summary>
        public static List<T> GetVComponentsFromList<T>(IEnumerable<VEntity> entities, bool removeNull = true, Predicate<VEntity> test = null) where T : VComponent {
            List<T> returnList = new List<T>();
            foreach (VEntity entity in entities) {
                var comp = GetVComponent<T>(entity);
                if ((test == null || test.Invoke(entity)) && (!removeNull || comp != null)) {
                    returnList.Add(comp);
                }
            }
            return returnList;
        }

        public List<T> GetAllVComponents<T>() where T : VComponent {
            return VEntityComponentSystemManager.GetVComponentsFromList<T>(gameEntities);
        }

        public static bool HasVComponent<T>(VEntity entity) {
            if (entity == null) {
                return false;
            }

            for (int i = 0; i < entity.Components.Count; ++i) {
                if (entity.Components[i] is T) {
                    return true;
                }
            }
            return false;
        }

        public T GetVSingletonComponent<T>() where T : VComponentSingleton {
            foreach (VEntity entity in gameEntities) {
                T singletonComponent = GetVComponent<T>(entity);
                if (singletonComponent != null) {
                    return singletonComponent;
                }
            }
            return null;
        }

        public List<VEntity> GetAllEntities() {
            return gameEntities;
        }
        public List<VSystem> GetAllSystems() {
            return gameSystems;
        }

        public List<VAnimationSystem> GetAllAnimationSystems() {
            return animationSystems;
        }
        public GameplayEventQueue GetGameplayEventQueue() {
            return gameplayEventQueue;
        }

        public VEntity CreateEntity(string prefix, IList<VComponent> components = null, VComponent component = null) {
            if (component != null) {
            components = new List<VComponent> {
            component
                };
            }
            VEntity v = new VEntity {
                id = prefix + "_" + Guid.NewGuid().ToString(),
                Components = components == null ? new List<VComponent>() : new List<VComponent>(components)
            };

            gameEntities.Add(v);
            return v;
        }

        // helper function for creating an entity that will be put on the event stack. 
        public VEntity CreateEvent(string prefix, IList<VComponent> components = null, VComponent component = null) {
            VEntity ent = CreateEntity(prefix, components : components, component : component);
            StackEvent(ent);
            return ent;
        }

        // helper function for creating an entity that will be put on the event stack. 
        public VEntity ExecuteImmediateEvent(string prefix, IList<VComponent> components = null, VComponent component = null) {
            VEntity ent = CreateEntity(prefix, components : components, component : component);
            gameplayEventQueue.ImmediateExecute(ent);
            return ent;
        }

        // doesn't immediately remove entity, but marks it for removal
        public void MarkRemovalEntity(string entityId) {
            entitiesToDelete.Add(GetVEntityById(entityId));
        }

        public void MarkRemovalEntity(VEntity entity) {
            entitiesToDelete.Add(entity);
        }

        public void StackEvent(VEntity entity) {
            gameplayEventQueue.Push(entity);
        }

        public void QueueAnimationEvent(VEntity entity) {
            animationQueue.Add(entity);
        }

        public void QueueAnimationEvent(string prefix, IList<VComponent> components = null, VComponent component = null) {
            animationQueue.Add(CreateEvent(prefix, components, component));
        }

        public void AddComponent(VEntity entity, VComponent component) {
            entity.Components.Add(component);
        }
        public void AddComponents(VEntity entity, IEnumerable<VComponent> components) {
            foreach (VComponent component in components) {
                entity.Components.Add(component);
            }
        }

        public static T DeepClone<T>(T obj) {
            byte[] bytes = SerializationUtility.SerializeValue(obj, DataFormat.Binary);
            return SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
        }

        public VComponent CloneComponent(VComponent component) {
            return DeepClone<VComponent>(component);
        }

        public List<VComponent> CloneComponents(IEnumerable<VComponent> components) {
            List<VComponent> list = new List<VComponent>();
            foreach (VComponent component in components) {
                if (component is ICloneable cloneable) {
                    list.Add((VComponent) cloneable.Clone());
                } else {
                    list.Add(DeepClone(component));
                }
            }
            return list;
        }

        public VEntity InsantiateEntityFromBlueprint(VEntity entity) {
            return CreateEntity(entity.id.Split('_') [0], components : CloneComponents(entity.Components));
        }

        //Removes a component from an entity that
        public bool RemoveComponent<T>(VEntity entity) {
            int removeComponentIndex = -1;
            for (int i = 0; i < entity.Components.Count; ++i) {
                if (entity.Components[i] is T) {
                    removeComponentIndex = i;
                }
            }

            if (removeComponentIndex == -1) {
                return false;
            } else {
                entity.Components.RemoveAt(removeComponentIndex);
                return true;
            }
        }

        public T GetSystem<T>() where T : VSystem {
            if (systemLookupTable.ContainsKey(typeof(T))) {
                return (T) systemLookupTable[typeof(T)];
            }

            foreach (VSystem system in gameSystems) {
                if (system is T returnSystem) {
                    systemLookupTable[typeof(T)] = returnSystem;
                    return returnSystem;
                }
            }

            Debug.LogFormat("SYSTEM {0} NOT FOUND. Did you remember to add it to the VECS Manager?", typeof(T).FullName);
            return null;
        }

        public T GetAnimationSystem<T>() where T : VAnimationSystem {
            foreach (VAnimationSystem system in animationSystems) {
                if (system is T returnSystem) {
                    return returnSystem;
                }
            }

            Debug.LogFormat("ANIMATION SYSTEM {0} NOT FOUND. Did you remember to add it to the VECS Manager?", typeof(T).FullName);
            return null;
        }

        public void DoRemove() {
            removedEntitiesForAnimation.AddRange(entitiesToDelete);
            gameEntities.RemoveAll((entity) => entitiesToDelete.Contains(entity));
            foreach (VEntity entity in entitiesToDelete) {
                entityLookupTable.Remove(entity.id);
            }

            entitiesToDelete.Clear();
        }
    }
}