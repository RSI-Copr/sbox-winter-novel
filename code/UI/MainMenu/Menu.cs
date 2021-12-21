using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using TerryNovel.Editor;

namespace TerryNovel
{
	public class MainMenu:Panel
	{
		public MainMenu()
		{
			this.LoadDefaultStyleSheet();
			Add.Button( "Start novel", () => Novel.Load("") );
			Add.Button( "Create own story", () => EditorWindow.Open() ); ;
		}
	}
}
