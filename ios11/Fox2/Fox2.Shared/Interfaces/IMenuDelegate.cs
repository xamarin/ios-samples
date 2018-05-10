
namespace Fox2.Interfaces
{
    using Foundation;

    public interface IMenuDelegate : INSObjectProtocol
    {
        void FStopChanged(float value);

        void FocusDistanceChanged(float value);

        void DebugMenuSelectCameraAtIndex(int index);
    }
}