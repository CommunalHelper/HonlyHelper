using Celeste;
using Microsoft.Xna.Framework;
using Monocle;
using System;
using System.Collections.Generic;
using Celeste.Mod.Entities;
using System.Linq;

namespace Celeste.Mod.HonlyHelper
{
    static class StaticExtensions
    {
        static public List<T> GetWithinRadius<T>(this Tracker tracker, Vector2 nearestTo, float radius) where T : Entity
        {
            List<T> entityList = new List<T>();
            float radiusSquared = (float)Math.Pow(radius, 2);

            foreach (T entity in tracker.GetEntities<T>())
            {
                if(Vector2.DistanceSquared(nearestTo, entity.Position) < radiusSquared)
                {
                    entityList.Add(entity);
                }
            }
                return entityList;
        }

    }
}
