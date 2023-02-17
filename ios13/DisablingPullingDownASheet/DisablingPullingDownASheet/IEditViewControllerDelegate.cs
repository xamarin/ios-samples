using System;
namespace DisablingPullingDownASheet {
	public interface IEditViewControllerDelegate {
		void EditViewControllerDidCancel (EditViewController editViewController);
		void EditViewControllerDidFinish (EditViewController editViewController);
	}
}
