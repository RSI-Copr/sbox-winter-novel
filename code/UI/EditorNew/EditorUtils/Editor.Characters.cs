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
			public readonly List<CharacterNode> AllNodes = new();
			private readonly List<CharacterPanel> Panels = new();
			public void AddNode( CharacterNode node )
			{
				AllNodes.Add( node );
				CharactersDict[node.Id] = "";
			}

			public IEnumerable<CharacterPanel> GetAttachedPanels( CharacterNode node )
			{
				return Panels.Where( p => p.CharacterId == node.Id );
			}

			public void Update( CharacterNode node , string name )
			{
				CharactersDict[node.Id] = name;
				foreach (var p in GetAttachedPanels(node) )
				{
					p.UpdateText( name );
				}
			}

			public void Remove( CharacterNode node )
			{
				CharactersDict.Remove(node.Id);
				foreach ( var p in GetAttachedPanels( node ).ToList() )
				{
					p.Delete(true);
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
			public string GetName(int id )
			{
				return CharactersDict[id];
			}

			public IReadOnlyDictionary<int,string> Get()
			{
				return CharactersDict;
			}

			public void Clear()
			{
				AllNodes.Clear();
				CharactersDict.Clear();
				Panels.Clear();
			}
		}
	}

	public class CharacterPanel : Panel
	{
		public int CharacterId;
		private readonly Label label;
		public CharacterPanel( int id )
		{
			AddClass( "character" );
			label = Add.Label();

			Editor.Characters.RegisterPanel( this );
			UpdateFrom( id );
		}
		public void UpdateText(string name)
		{
			label.Text = name;
		}

		public void UpdateFrom( int id )
		{
			CharacterId = id;
			label.Text = Editor.Characters.GetName( id );
		}
		public override void OnDeleted()
		{
			Editor.Characters.UnregisterPanel( this );
		}
	}
}
