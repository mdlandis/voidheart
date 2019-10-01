using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Voidheart {
    // dictionary where you can insert things with string tags
    public class TagDictionary<T> {
        Dictionary<string, HashSet<T>> _innerStorage = new Dictionary<string, HashSet<T>>();

        public void Add(IEnumerable<string> tags, T value) {
            foreach (var tag in tags) {
                if (_innerStorage.ContainsKey(tag)) {
                    var hash = _innerStorage[tag];
                    if (!hash.Contains(value)) {
                        hash.Add(value);
                    }
                } else {
                    _innerStorage[tag] = new HashSet<T> {
                        value
                    };
                }
            }
        }

        public IEnumerable<T> GetValuesWithAnyTags(IEnumerable<string> tags) {
            var result = new HashSet<T>();
            foreach (var tag in tags) {
                if (_innerStorage.ContainsKey(tag)) {
                    result.UnionWith(_innerStorage[tag]);
                }
            }
            return result;
        }

        // <summary>Removes all objects that have all specfied tags.</summary
        public IEnumerable<T> GetValuesWithAllTags(IEnumerable<string> tags) {
            bool isFirst = true;
            HashSet<T> result = new HashSet<T>();
            foreach (var tag in tags) {
                if (_innerStorage.ContainsKey(tag)) {
                    if (isFirst) {
                        result.UnionWith(_innerStorage[tag]);
                        isFirst = false;
                    } else {
                        result.IntersectWith(_innerStorage[tag]);
                    }

                }
            }

            return result;
        }

        // <summary>Removes all objects given.</summary
        public void RemoveObjects(IEnumerable<T> objects) {
            foreach (string tag in _innerStorage.Keys) {
                _innerStorage[tag].ExceptWith(objects);
            }
        }

        // <summary>Removes all objects with the tags passed in, and returns objects.</summary
        public HashSet<T> RemoveTags(IEnumerable<string> tags) {
            var result = new HashSet<T>();
            foreach (string tag in tags) {
                if (_innerStorage.ContainsKey(tag)) {
                    result.UnionWith(_innerStorage[tag]);
                    _innerStorage.Remove(tag);
                }
            }

            return result;
        }

    }

}