
namespace Fox2.Interfaces
{
    using Foundation;

    public interface IButtonOverlayDelegate : INSObjectProtocol
    {
        void WillPress(ButtonOverlay button);

        void DidPress(ButtonOverlay button);
    }
}
