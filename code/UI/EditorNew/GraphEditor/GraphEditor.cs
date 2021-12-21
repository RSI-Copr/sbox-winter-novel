using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TerryNovel.Editor
{
	public partial class GraphEditor : Panel
	{
		private static Vector2 FieldSize = new( 10000 );
		private const float GridSize = 16f;
		private static GraphEditor Instance;
		private static int CurNodeId = 0;
		public GraphEditor()
		{
			Instance = this;
			Style.SetBackgroundImage( "UI/background-grid.png" );
			Style.PointerEvents = "all";
			Style.Position = PositionMode.Absolute;

			Style.Width = Length.Percent( FieldSize.x );
			Style.Height = Length.Percent( FieldSize.y );

			AcceptsFocus = true;

			Center();
			CreateDefault();
			Focus();
		}


		private List<BaseNode> Nodes = new();
		private List<Connection> Connections = new();
		private IEnumerable<Panel> FindPanelsUnderMouse( float rect_size = 10, bool full_inside = false )
		{
			var pos = Mouse.Position;
			float half_size = rect_size / 2;
			return FindInRect( new Rect( pos.x - half_size, pos.y - half_size, rect_size, rect_size ), full_inside );
		}

		private Vector2 GetRelativePosition(BaseNode node )
		{
			return node.GetPosition() - FieldSize/2;
		}


		private void ShowNodesCreation()
		{
			var ops = new OptionsPanel();
			foreach ( var kv in BaseNode.NodesNames )
			{
				ops.AddOption( kv.Key, () => CreateNode( kv.Value ) );
			}
		}
		private void CreateNode( Type type )
		{

			var node = Library.Create<BaseNode>( type );
			AddNode( node );
			node.SetPosition( MousePosition.SnapToGrid( GridSize ) );
		}
		private void CreateNode<T>() where T : BaseNode
		{
			var node = Library.Create<T>();
			AddNode( node );
			node.SetPosition( FieldSize / 2 );
		}
		private BaseNode CreateNode(string classname, int id, Vector2 relativepos )
		{
			var node = Library.Create<BaseNode>( classname );
			AddChild( node );
			Nodes.Add( node );

			node.SetPosition( FieldSize / 2 + relativepos );
			node.Id = id;

			return node;
		}

		[ClientCmd("node_debug")]
		private static void DebugNodes()
		{
			foreach(var node in Instance.Nodes )
			{
				Log.Info( $"{node} {node.BaseInput?.Id} {node.BaseOutput?.Id}" );
			}
		}

		private void AddNode( BaseNode node )
		{
			node.Id = CurNodeId;
			CurNodeId++;
			AddChild( node );
			Nodes.Add( node );
		}
		private void CreateDefault()
		{
			CreateNode<InfoNode>();
			CreateNode<StartNode>();
		}
		private void Center()
		{
			
			this.SetPosition( -FieldSize / 2 + Screen.Size / 2 );

		}

		private void Clear()
		{
			foreach ( var node in Nodes )
			{
				node.Delete( true );

			}
			Nodes.Clear();
			Connections.Clear();
			CurNodeId = 0;
			Plug.CurrentId = 0;
			Center();
		}

		public static void Reset()
		{
			Plug.AutoIdAssignment = true;
			Instance.Clear();
			Instance.CreateDefault();
		}

		

		private Vector2 MoveDir = 0;
		
		private void AddDirection(Vector2 dir, bool pressed )
		{
			if ( pressed )
			{
				MoveDir += dir;
			}
			else
			{
				MoveDir -= dir;
			}
		}
		public override void OnButtonEvent( ButtonEvent e )
		{
			if ( InputFocus.Current != this ) return;

			bool pressed = e.Pressed;

			switch ( e.Button )
			{
				case "w":
					AddDirection( Vector2.Up, pressed );
					break;
				case "s":
					AddDirection( Vector2.Down, pressed );
					break;
				case "a":
					AddDirection( Vector2.Left, pressed );
					break;
				case "d":
					AddDirection( Vector2.Right, pressed );
					break;
				case "mousemiddle":
					SetMoving( pressed );


					break;

			}
			base.OnButtonEvent( e );
		}

		Vector2 MouseMoveOffset;
		private bool FieldMoving;

		private void SetMoving(bool value)
		{

			if ( value )
			{
				MouseMoveOffset = this.GetPosition();
			}
			FieldMoving = value;
		}

		public BaseNode GetNodeById( int id ) => Nodes.FirstOrDefault( n => n.Id == id );


		private Vector2 NodeMoveOffset;
		private BaseNode CurNode;
		protected override void OnMouseDown( MousePanelEvent e )
		{
			OptionsPanel.CloseAll();

			if ( e.Target is PlugOut pnl && e.Button == "mouseleft" )
			{
				StartConnectFrom( pnl );
				return;
			}

			if ( e.Target is BaseNode node )
			{
				CurNode = node;
				NodeMoveOffset = MousePosition - CurNode.GetPosition();
				return;
			}

			
		}
		
	
		protected override void OnMouseUp( MousePanelEvent e )
		{
			if ( CurNode is not null )
			{
				CurNode = null;
			}


			if ( WantConnect )
			{
				WantConnect = false;

				if ( FindPanelsUnderMouse().FirstOrDefault( p => p is PlugIn ) is PlugIn pnl )
				{
					ConnectTo( pnl );
				}

				return;
			}


			if ( e.Target != this ) return;
			switch ( e.Button )
			{
				case "mouseright":
					ShowNodesCreation();
					break;
			}
		}




		private bool WantConnect;
		private PlugOut CurPlug;

		private void StartConnectFrom( PlugOut output )
		{
			WantConnect = true;
			CurPlug = output;
		}
		private void ConnectTo( PlugIn input )
		{
			if ( CurPlug.Node == input.Node ) return;

			var conn = new Connection()
			{
				Output = CurPlug,
				Input = input,
			};

			Connections.Add( conn );
		}

	
		public override void Tick()
		{
		    if(MoveDir != Vector2.Zero )
			{
				this.SetPosition(this.GetPosition() + MoveDir * 5);
			}

			if ( CurNode is not null )
			{
				CurNode.SetPosition( (MousePosition - NodeMoveOffset).SnapToGrid( GridSize ) );
			}
			if ( FieldMoving )
			{
				MouseMoveOffset += Mouse.Delta;
				this.SetPosition( MouseMoveOffset );
			}
		}

		public override void DrawBackground( ref RenderState state )
		{
			if ( WantConnect )
			{
				Lines.DrawBroken( CurPlug.GetScreenPosition(), Mouse.Position, FindPanelsUnderMouse().Any( p => p is PlugIn ) ? Color.Yellow : Color.Red );
			}

			for ( int i = Connections.Count - 1; i >= 0; i-- )
			{
				var con = Connections[i];
				if ( !con.IsValid )
				{
					Connections.Remove( con );
					continue;
				}

				Lines.DrawBroken( con.Output.GetScreenPosition(), con.Input.GetScreenPosition(), con.Output.Color );
			}

			

			base.DrawBackground( ref state );
		}
		

	}
}
