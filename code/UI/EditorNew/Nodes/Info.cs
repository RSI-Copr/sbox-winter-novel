using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.IO;

namespace TerryNovel.Editor
{
	[Node(Title = "Novel information", CanCreate = false, HasInput = false, HasOutput = false)]
    public class InfoNode:BaseNode
	{
		public string Title {
			get => title.Text;
			set => title.Text = value;
		} 
		public string Desc
		{
			get => desc.Text;
			set => desc.Text = value;
		}

		private TextEntry title;
		private TextEntry desc;
	    public InfoNode()
		{
			title = Canvas.Add.TextEntryWithPlaceHolder("Novel title");
			desc = Canvas.Add.TextEntryWithPlaceHolder( "Description");
			desc.Multiline = true;
		}

		public override void Write( BinaryWriter writer )
		{
			writer.Write( Title );
			writer.Write( Desc );
		}

		public override void Read( BinaryReader reader )
		{
			Title = reader.ReadString();
			Desc = reader.ReadString();
		}
	}
}
