using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Trickgolf.UI
{
	public partial class BetterChatboxEntry : Panel
	{
		public Label NameLabel { get; internal set; }
		public Label Message { get; internal set; }
		public Image Avatar { get; internal set; }

		public RealTimeSince TimeSinceBorn = 0;

		public bool Faded => TimeSinceBorn > 12;
		private bool _faded;

		public BetterChatboxEntry()
		{
			Avatar = Add.Image();
			NameLabel = Add.Label("Name", "name");
			Message = Add.Label("Message", "message");
		}

		public override void Tick()
		{
			base.Tick();

			if (!_faded && Faded)
			{
				AddClass("faded");
				_faded = true;
			}
		}

	}
}
