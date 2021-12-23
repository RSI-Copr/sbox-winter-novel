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
		public static readonly SpritesHandler Sprites = new();
		public class SpritesHandler
		{
			private readonly Dictionary<int, string> SpritesDict = new();

			public void Register(SpriteNode node )
			{
				SpritesDict[node.Id] = "";
			}
			public void Unregister( SpriteNode node )
			{
				SpritesDict.Remove( node.Id );
				foreach ( var spriteNode in GetAttachedNodes( node ) )
				{
					spriteNode.Name = "";
					spriteNode.SpriteId = -1;
				}

			}
			public void Update( SpriteNode node)
			{
				var name = node.Name;

				SpritesDict[node.Id] = name;
				foreach ( var spriteNode in GetAttachedNodes( node ) )
				{
					spriteNode.Name = name;
				}


			}

			public int GetSpriteGeneratedId(int nodeid)
			{
				var sprite = (BaseNode.GetNodeById(nodeid) as SpriteNode).File;
				var item = Editor.Info.Sprites.Where( s => s.Name == sprite ).First();

				return Editor.Info.Sprites.IndexOf( item );
			}

			public void Clear()
			{
				SpritesDict.Clear();
			}
			private IEnumerable<SpriteEventNode> GetAttachedNodes( SpriteNode node )
			{
				return BaseNode.All.OfType<SpriteEventNode>().Where( n => n.SpriteId == node.Id );
			}
			public IReadOnlyDictionary<int, string> Get() => SpritesDict;


		}
	}
}
