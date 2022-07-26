using System;
using Rage;
using System.Collections.Generic;

namespace UnitedCallouts.Stuff
{
    public static class Vector3Extension
    {
        public static Vector3 ExtensionAround(this Vector3 start, float radius)
        {
            Vector3 direction = ExtensionRandomXY();
            Vector3 around = start + (direction * radius);
            return around;
        }

        public static float ExtensionDistanceTo(this Vector3 start, Vector3 end)
        {
            return (end - start).Length();
        }

        public static Vector3 ExtensionRandomXY()
        {
            Random random = new Random(Environment.TickCount);

            Vector3 vector3 = new Vector3();
            vector3.X = (float)(random.NextDouble() - 0.5);
            vector3.Y = (float)(random.NextDouble() - 0.5);
            vector3.Z = 0.0f;
            vector3.Normalize();
            return vector3;
        }
         public static Vector3 chooseNearestLocation(List<Vector3> list)
        {
            Vector3 closestLocation = list[0];
            float closestDistance = Vector3.Distance(Game.LocalPlayer.Character.Position, list[0]);
            for (int i = 1; i < list.Count; i++)
            {
                if(Vector3.Distance(Game.LocalPlayer.Character.Position, list[i]) <= closestDistance)
                {
                    closestDistance = Vector3.Distance(Game.LocalPlayer.Character.Position, list[i]);
                    closestLocation = list[i];
                }
            }
            return closestLocation;
        }
    }
}