using Verse;

namespace StoneCampFire
{
	public class CompProperties_SmokeSignalComms : CompProperties
	{
        public bool debug = false;

		public CompProperties_SmokeSignalComms()
		{
			compClass = typeof(CompSmokeSignalComms);
		}
	}
}