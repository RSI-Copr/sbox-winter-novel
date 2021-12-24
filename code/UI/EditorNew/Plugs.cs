using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TerryNovel.Editor
{
	public class Plug : Panel
	{
		public static bool AutoIdAssignment { get; set; } = true;
		public static int CurrentId { get; set; }

		public readonly static Dictionary<int, Plug> Dict = new();

		public static Plug GetById(int id )
		{
			return Dict.GetValueOrDefault( id );
		}


		public int Id { get; private set; }

		public void SetId(int id )
		{
			Dict.Add( id, this );
			Id = id;
		}

		public readonly BaseNode Node;
		public Plug( BaseNode node)
		{
			if ( AutoIdAssignment )
			{
				SetId(CurrentId);
				CurrentId++;
			}

			//Log.Info( $"{this} {Id} {AutoIdAssignment}" );

			Node = node;
			AddClass( "plug" );
			Add.Label( "navigate_next" );
		}
		public static void Clear()
		{
			Dict.Clear();
		}

		public override void OnDeleted()
		{
			Dict.Remove( Id );
			Connection.RemoveWithPlug( this );
		}
	}

	public class PlugIn : Plug
	{

		public PlugIn( BaseNode node ):base(node)
		{
			AddClass( "input" );
			
		}
	}

	public class PlugOut : Plug
	{
		public IEnumerable<PlugIn> NextInputs => Connection.All.Where(c=>c.Output == this).Select(c=>c.Input);
		public IEnumerable<BaseNode> NextNodes => NextInputs.Select( p => p.Node );
		public readonly Color Color;
		private Color GenerateRandomColor()
		{
			return Color.Random;
		}
		public PlugOut( BaseNode node ) : base( node )
		{
			Color = GenerateRandomColor();
			Style.BackgroundColor = Color;
			AddClass( "output" );
		}
	}

	class Connection
	{
		public static readonly List<Connection> All = new();
		public static Connection Create(PlugOut output, PlugIn input )
		{
			//output.NextInputs.Add( input );

			var conn =  new Connection()
			{
				Output = output,
				Input = input,
			};

			All.Add( conn );

			return conn;
		}

		public static void RemoveWithPlug(Plug plug)
		{
			All.RemoveAll( c => c.Input == plug || c.Output == plug );
		}


		public PlugIn Input;
		public PlugOut Output;

		public bool IsValid => Input.IsValid() && Output.IsValid();
	}
}
