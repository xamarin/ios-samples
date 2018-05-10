
namespace Fox2
{
    [System.Flags]
    public enum Bitmask : uint
    {
        Character = 1 << 0,// the main character

        Collision = 1 << 1, // the ground and walls

        Enemy = 1 << 2,// the enemies

        Trigger = 1 << 3, // the box that triggers camera changes and other actions

        Collectable = 1 << 4, // the collectables (gems and key)
    }
}
