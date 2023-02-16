
namespace XamarinShot.Models {
	using AudioUnit;
	using AVFoundation;

	public class AUSamplerNode : AVAudioUnitMidiInstrument {
		private static AudioComponentDescription auSamplerDescription = AudioComponentDescription.CreateGeneric (AudioComponentType.MusicDevice,
																												(int) AudioUnitSubType.Sampler);

		public AUSamplerNode () : this (auSamplerDescription)
		{
			auSamplerDescription.ComponentFlags = 0;
			auSamplerDescription.ComponentFlagsMask = 0;
			auSamplerDescription.ComponentManufacturer = AudioComponentManufacturerType.Apple;
		}

		public AUSamplerNode (AudioComponentDescription auSamplerDescription) : base (auSamplerDescription) { }
	}
}
