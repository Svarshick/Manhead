namespace CustomMath
{
    public static class SpaceUtility
    {
        public static Direction GetGlobalDirection(Direction direction, Direction lookDirection)
        {
            return (Direction)(((int)direction + (int)lookDirection) % 4);
        }

        public static Direction GetFacingSideDirection(Direction fromDirection, Direction objectLookDirection)
        {
            //fromDirection+2 get the global direction of object's side.
            //-objectLookDirection transform global to local (we expect local side direction) 
            return (Direction)(((int)fromDirection + 2 - (int)objectLookDirection) % 4);
        }
    }
}