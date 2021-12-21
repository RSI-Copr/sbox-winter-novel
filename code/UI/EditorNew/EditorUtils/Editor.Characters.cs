using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerryNovel.Editor
{
	static partial class Editor
	{
		public readonly static CharacterHandler Characters = new();
		public class CharacterHandler
		{
			private readonly Dictionary<int, string> CharactersDict = new();
			private readonly Dictionary<CharacterNode,string> Dict = new();
			private readonly List<CharacterPanel> Panels = new();
			public void AddNode( CharacterNode node )
			{
				Dict.Add( node, "" );
			}

			public void Update( CharacterNode node , string name )
			{
				Dict[node] = name;
				foreach(var p in Panels.Where(p=>p.Node == node) )
				{
					p.UpdateText( name );
				}
			}

			public void Remove( CharacterNode node )
			{
				Dict.Remove( node );
				foreach ( var p in Panels.Where( p => p.Node == node ).ToList() )
				{
					p.Delete(true);
					Panels.Remove( p );
				}
			}

			public void RegisterPanel(CharacterPanel pnl )
			{
				Panels.Add( pnl );
			}
			public void UnregisterPanel( CharacterPanel pnl )
			{
				Panels.Remove( pnl );
			}
			public string GetName(CharacterNode node )
			{
				return Dict[node];
			}
			public IReadOnlyDictionary<CharacterNode,string> Get()
			{
				return Dict;
			}
		}
	}

	public class CharacterPanel : Panel
	{
		public CharacterNode Node;
		private readonly Label label;
		public CharacterPanel( CharacterNode node )
		{
			AddClass( "character" );
			label = Add.Label();

			Editor.Characters.RegisterPanel( this );
			UpdateFrom( node );
		}
		public void UpdateText(string name)
		{
			label.Text = name;
		}

		public void UpdateFrom( CharacterNode node )
		{
			Node = node;
			label.Text = Editor.Characters.GetName( node );
		}
		public override void OnDeleted()
		{
			Editor.Characters.UnregisterPanel( this );
		}
	}
}
