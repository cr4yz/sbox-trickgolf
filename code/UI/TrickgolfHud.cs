using Sandbox;
using Sandbox.UI;
using Trickgolf.UI;

namespace Trickgolf
{
	[Library]
	public partial class TrickgolfHud : Hud
	{
		public bool Fade 
		{
			get => RootPanel.HasClass("fade");
			set => RootPanel.SetClass("fade", value);
		}

		public TrickgolfHud()
		{
			if (!IsClient) return;

			RootPanel.StyleSheet.Load("/ui/TrickgolfHud.scss");

			RootPanel.AddChild<BetterChatbox>();
			RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
			RootPanel.AddChild<PowerBar>();
			RootPanel.AddChild<HoleScore>();
			RootPanel.AddChild<EndScore>();
			RootPanel.AddChild<OutOfBounds>();
		}
	}
}