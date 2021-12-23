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

		public static GraphEditor Instance { get; private set; }
	
		public GraphEditor()
		{
			Instance = this;
			Style.SetBackgroundImage( "UI/background-grid.png" );
			Style.PointerEvents = "all";
			Style.Position = PositionMode.Absolute;

			Style.Width = Length.Percent( FieldSize.x );
			Style.Height = Length.Percent( FieldSize.y );

			AcceptsFocus = true;

			Clear();
			//CreateDefault();
			Focus();
		}


		private List<BaseNode> Nodes = new();
		
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
			foreach ( var g in BaseNode.NodesTypes.Where(kv=>kv.Value.CanCreate).GroupBy(kv=> kv.Value.Group ) )
			{
				ops.AddSpacer( g.Key );

				foreach(var kv in g )
				{
					ops.AddOption( kv.Value.Title, () => CreateNode( kv.Key ) );
				}
				
			}
		}
		private void CreateNode( Type type )
		{

			var node = Library.Create<BaseNode>( type );
			AddChild( node );
			node.SetPosition( MousePosition.SnapToGrid( GridSize ) );
		}
		private void CreateNode<T>() where T : BaseNode
		{
			var node = Library.Create<T>();
			AddChild( node );
			node.SetPosition( FieldSize / 2 );
		}
		private BaseNode CreateNode(string classname, int id, Vector2 relativepos )
		{
			var node = Library.Create<BaseNode>( classname );
			AddChild( node );
			
			node.SetPosition( FieldSize / 2 + relativepos );
			//node.Id = id;

			return node;
		}

		[ClientCmd("node_debug")]
		private static void DebugNodes()
		{
			foreach(var node in BaseNode.All )
			{
				Log.Info( $"{node} {node.Id}" );
			}
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
			for (int i = BaseNode.All.Count -1; i>= 0;i-- )
			{
				BaseNode.All[i]?.Delete( true );
			}
			 
			BaseNode.All.Clear();
			Connection.All.Clear();
			

			BaseNode.AutoIdAssignment = true;
			BaseNode.CurrentId = 0;

			Plug.Clear();
			Plug.CurrentId = 0;
			Plug.AutoIdAssignment = true;
			Editor.Characters.Clear();
			Editor.Sprites.Clear();

			Center();
		}

		public static void Reset()
		{
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
			Connection.Create(CurPlug,input);
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

			foreach ( var con in Connection.All )
			{
				Lines.DrawBroken( con.Output.GetScreenPosition(), con.Input.GetScreenPosition(), con.Output.Color );
			}
			
			base.DrawBackground( ref state );
		}
		

	}
}
