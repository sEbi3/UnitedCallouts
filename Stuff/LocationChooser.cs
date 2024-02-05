namespace UnitedCallouts.Stuff;

public static class LocationChooser
{

    internal static Vector3 ChooseNearestLocation(List<Vector3> list)
    {
        Vector3 closestLocation = list[0];
        float closestDistance = Vector3.Distance(MainPlayer.Position, list[0]);
        for (int i = 1; i < list.Count; i++)
        {
            if (Vector3.Distance(MainPlayer.Position, list[i]) <= closestDistance)
            {
                closestDistance = Vector3.Distance(MainPlayer.Position, list[i]);
                closestLocation = list[i];
            }
        }
        return closestLocation;
    }
    internal static int NearestLocationIndex(List<Vector3> list)
    {
        int closestLocationIndex = 0;
        float closestDistance = Vector3.Distance(MainPlayer.Position, list[0]);
        for (int i = 1; i < list.Count; i++)
        {
            if (Vector3.Distance(MainPlayer.Position, list[i]) <= closestDistance)
            {
                closestDistance = Vector3.Distance(MainPlayer.Position, list[i]);
                closestLocationIndex = i;
            }
        }
        return closestLocationIndex;
    }
}