
namespace Fox2.Interfaces
{
    using Foundation;

    public interface IPadOverlayDelegate : INSObjectProtocol
    {
        void PadOverlayVirtualStickInteractionDidStart(PadOverlay padNode);

        void PadOverlayVirtualStickInteractionDidChange(PadOverlay padNode);

        void PadOverlayVirtualStickInteractionDidEnd(PadOverlay padNode);
    }
}
