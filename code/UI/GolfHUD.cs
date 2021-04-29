using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Minigolf
{
	[Library]
	public partial class GolfHUD : Hud
	{
		private bool _fade;
		public bool Fade {
			get { return _fade; }
			set {
				if (value)
					RootPanel.AddClass("fade");
				else
					RootPanel.RemoveClass("fade");

				_fade = value;
			}
		}

		public GolfHUD()
		{
			if (!IsClient) return;

			RootPanel.StyleSheet.Load("/ui/GolfHUD.scss");

			RootPanel.AddChild<Sandbox.UI.ChatBox>();
			RootPanel.AddChild<PowerBar>();
			RootPanel.AddChild<EndScore>();
			RootPanel.AddChild<OutOfBounds>();
		}
	}
}