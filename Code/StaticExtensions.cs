using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;

namespace Celeste.Mod.HonlyHelper {
    internal static class StaticExtensions {
        public static List<T> GetWithinRadius<T>(this Tracker tracker, Vector2 nearestTo, float radius) where T : Entity {
            List<T> entityList = new();
            float radiusSquared = (float)Math.Pow(radius, 2);

            foreach (T entity in tracker.GetEntities<T>()) {
                if (Vector2.DistanceSquared(nearestTo, entity.Position) < radiusSquared) {
                    entityList.Add(entity);
                }
            }

            return entityList;
        }
    }
}
