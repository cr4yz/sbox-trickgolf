
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Threading.Tasks;

namespace Trickgolf
{
	public partial class HoleScore : Panel
	{
		public static HoleScore Current;

		private Label strokeLabel;

		public HoleScore()
		{
			Current = this;

			StyleSheet.Load("/ui/HoleScore.scss");

			var strokeContainer = Add.Panel("stroke");
			strokeContainer.Add.Label("STROKE");
			strokeLabel = strokeContainer.Add.Label("0");
		}

		public override void Tick()
		{
			if(Player.Local is not GolfPlayer player)
            {
				return;
            }

			strokeLabel.Text = $"{player.Strokes}";
		}
	}

}