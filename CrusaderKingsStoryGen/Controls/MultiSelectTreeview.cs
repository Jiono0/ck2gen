// <copyright file="MultiSelectTreeview.cs" company="Yemmlie - 252afh fork">
// Copyright policies set by https://github.com/yemmlie
// </copyright>

namespace MultiSelectTreeview
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Forms;

	public class MultiSelectTreeview : TreeView
	{
		#region Selected Node(s) PropertyForm

		private List<TreeNode> m_SelectedNodes = null;

		public List<TreeNode> SelectedNodes
		{
			get
			{
				return this.m_SelectedNodes;
			}

			set
			{
				this.ClearSelectedNodes();
				if( value != null )
				{
					foreach( TreeNode node in value )
					{
						this.ToggleNode( node, true );
					}
				}
			}
		}

		// Note we use the new keyword to Hide the native treeview's SelectedNode property.
		private TreeNode m_SelectedNode;

		public new TreeNode SelectedNode
		{
			get { return this.m_SelectedNode; }

			set
			{
				this.ClearSelectedNodes();
				if( value != null )
				{
					this.SelectNode( value );
				}
			}
		}

		#endregion

		public MultiSelectTreeview()
		{
			this.m_SelectedNodes = new List<TreeNode>();
			base.SelectedNode = null;
		}

		#region Overridden Events

		protected override void OnGotFocus( EventArgs e )
		{
			// Make sure at least one node has a selection
			// this way we can tab to the ctrl and use the
			// keyboard to select nodes
			try
			{
				if( this.m_SelectedNode == null && this.TopNode != null )
				{
					this.ToggleNode( this.TopNode, true );
				}

				base.OnGotFocus( e );
			}
			catch( Exception ex )
			{
				this.HandleException( ex );
			}
		}

		protected override void OnMouseDown( MouseEventArgs e )
		{
			// If the user clicks on a node that was not
			// previously selected, select it now.

			try
			{
				base.SelectedNode = null;

				TreeNode node = this.GetNodeAt( e.Location );
				if( node != null )
				{
					int leftBound = node.Bounds.X; // - 20; // Allow user to click on image
					int rightBound = node.Bounds.Right + 10; // Give a little extra room
					if( e.Location.X > leftBound && e.Location.X < rightBound )
					{
						if( ModifierKeys == Keys.None && ( this.m_SelectedNodes.Contains( node ) ) )
						{
							// Potential Drag Operation
							// Let Mouse Up do select
						}
						else
						{
							this.SelectNode( node );
						}
					}
				}

				base.OnMouseDown( e );
			}
			catch( Exception ex )
			{
				this.HandleException( ex );
			}
		}

		protected override void OnMouseUp( MouseEventArgs e )
		{
			// If the clicked on a node that WAS previously
			// selected then, reselect it now. This will clear
			// any other selected nodes. e.g. A B C D are selected
			// the user clicks on B, now A C & D are no longer selected.
			try
			{
				// Check to see if a node was clicked on
				TreeNode node = this.GetNodeAt( e.Location );
				if( node != null )
				{
					if( ModifierKeys == Keys.None && this.m_SelectedNodes.Contains( node ) )
					{
						int leftBound = node.Bounds.X; // -20; // Allow user to click on image
						int rightBound = node.Bounds.Right + 10; // Give a little extra room
						if( e.Location.X > leftBound && e.Location.X < rightBound )
						{
							this.SelectNode( node );
						}
					}
				}

				base.OnMouseUp( e );
			}
			catch( Exception ex )
			{
				this.HandleException( ex );
			}
		}

		protected override void OnItemDrag( ItemDragEventArgs e )
		{
			// If the user drags a node and the node being dragged is NOT
			// selected, then clear the active selection, select the
			// node being dragged and drag it. Otherwise if the node being
			// dragged is selected, drag the entire selection.
			try
			{
				TreeNode node = e.Item as TreeNode;

				if( node != null )
				{
					if( !this.m_SelectedNodes.Contains( node ) )
					{
						this.SelectSingleNode( node );
						this.ToggleNode( node, true );
					}
				}

				base.OnItemDrag( e );
			}
			catch( Exception ex )
			{
				this.HandleException( ex );
			}
		}

		protected override void OnBeforeSelect( TreeViewCancelEventArgs e )
		{
			// Never allow base.SelectedNode to be set!
			try
			{
				base.SelectedNode = null;
				e.Cancel = true;

				base.OnBeforeSelect( e );
			}
			catch( Exception ex )
			{
				this.HandleException( ex );
			}
		}

		protected override void OnAfterSelect( TreeViewEventArgs e )
		{
			// Never allow base.SelectedNode to be set!
			try
			{
				base.OnAfterSelect( e );
				base.SelectedNode = null;
			}
			catch( Exception ex )
			{
				this.HandleException( ex );
			}
		}

		protected override void OnKeyDown( KeyEventArgs e )
		{
			// Handle all possible key strokes for the control.
			// including navigation, selection, etc.

			base.OnKeyDown( e );

			if( e.KeyCode == Keys.ShiftKey )
            {
                return;
            }

            //this.BeginUpdate();
            bool bShift = ( ModifierKeys == Keys.Shift );

			try
			{
				// Nothing is selected in the tree, this isn't a good state
				// select the top node
				if( this.m_SelectedNode == null && this.TopNode != null )
				{
					this.ToggleNode( this.TopNode, true );
				}

				// Nothing is still selected in the tree, this isn't a good state, leave.
				if( this.m_SelectedNode == null )
                {
                    return;
                }

                if ( e.KeyCode == Keys.Left )
				{
					if( this.m_SelectedNode.IsExpanded && this.m_SelectedNode.Nodes.Count > 0 )
					{
						// Collapse an expanded node that has children
						this.m_SelectedNode.Collapse();
					}
					else if( this.m_SelectedNode.Parent != null )
					{
						// Node is already collapsed, try to select its parent.
						this.SelectSingleNode( this.m_SelectedNode.Parent );
					}
				}
				else if( e.KeyCode == Keys.Right )
				{
					if( !this.m_SelectedNode.IsExpanded )
					{
						// Expand a collpased node's children
						this.m_SelectedNode.Expand();
					}
					else
					{
						// Node was already expanded, select the first child
						this.SelectSingleNode( this.m_SelectedNode.FirstNode );
					}
				}
				else if( e.KeyCode == Keys.Up )
				{
					// Select the previous node
					if( this.m_SelectedNode.PrevVisibleNode != null )
					{
						this.SelectNode( this.m_SelectedNode.PrevVisibleNode );
					}
				}
				else if( e.KeyCode == Keys.Down )
				{
					// Select the next node
					if( this.m_SelectedNode.NextVisibleNode != null )
					{
						this.SelectNode( this.m_SelectedNode.NextVisibleNode );
					}
				}
				else if( e.KeyCode == Keys.Home )
				{
					if( bShift )
					{
						if( this.m_SelectedNode.Parent == null )
						{
							// Select all of the root nodes up to this point
							if( this.Nodes.Count > 0 )
							{
								this.SelectNode( this.Nodes[0] );
							}
						}
						else
						{
							// Select all of the nodes up to this point under this nodes parent
							this.SelectNode( this.m_SelectedNode.Parent.FirstNode );
						}
					}
					else
					{
						// Select this first node in the tree
						if( this.Nodes.Count > 0 )
						{
							this.SelectSingleNode( this.Nodes[0] );
						}
					}
				}
				else if( e.KeyCode == Keys.End )
				{
					if( bShift )
					{
						if( this.m_SelectedNode.Parent == null )
						{
							// Select the last ROOT node in the tree
							if( this.Nodes.Count > 0 )
							{
								this.SelectNode( this.Nodes[this.Nodes.Count - 1] );
							}
						}
						else
						{
							// Select the last node in this branch
							this.SelectNode( this.m_SelectedNode.Parent.LastNode );
						}
					}
					else
					{
						if( this.Nodes.Count > 0 )
						{
							// Select the last node visible node in the tree.
							// Don't expand branches incase the tree is virtual
							TreeNode ndLast = this.Nodes[0].LastNode;
							while( ndLast.IsExpanded && ( ndLast.LastNode != null ) )
							{
								ndLast = ndLast.LastNode;
							}

							this.SelectSingleNode( ndLast );
						}
					}
				}
				else if( e.KeyCode == Keys.PageUp )
				{
					// Select the highest node in the display
					int nCount = this.VisibleCount;
					TreeNode ndCurrent = this.m_SelectedNode;
					while( ( nCount ) > 0 && ( ndCurrent.PrevVisibleNode != null ) )
					{
						ndCurrent = ndCurrent.PrevVisibleNode;
						nCount--;
					}

					this.SelectSingleNode( ndCurrent );
				}
				else if( e.KeyCode == Keys.PageDown )
				{
					// Select the lowest node in the display
					int nCount = this.VisibleCount;
					TreeNode ndCurrent = this.m_SelectedNode;
					while( ( nCount ) > 0 && ( ndCurrent.NextVisibleNode != null ) )
					{
						ndCurrent = ndCurrent.NextVisibleNode;
						nCount--;
					}

					this.SelectSingleNode( ndCurrent );
				}
				else
				{
					// Assume this is a search character a-z, A-Z, 0-9, etc.
					// Select the first node after the current node that
					// starts with this character
					string sSearch = ( (char) e.KeyValue ).ToString();

					TreeNode ndCurrent = this.m_SelectedNode;
					while( ( ndCurrent.NextVisibleNode != null ) )
					{
						ndCurrent = ndCurrent.NextVisibleNode;
						if( ndCurrent.Text.StartsWith( sSearch ) )
						{
							this.SelectSingleNode( ndCurrent );
							break;
						}
					}
				}
			}
			catch( Exception ex )
			{
				this.HandleException( ex );
			}
			finally
			{
				this.EndUpdate();
			}
		}

		#endregion

		#region Helper Methods

		private void SelectNode( TreeNode node )
		{
			try
			{
				this.BeginUpdate();

				if( this.m_SelectedNode == null || ModifierKeys == Keys.Control )
				{
					// Ctrl+Click selects an unselected node, or unselects a selected node.
					bool bIsSelected = this.m_SelectedNodes.Contains( node );
					this.ToggleNode( node, !bIsSelected );
				}
				else if( ModifierKeys == Keys.Shift )
				{
					// Shift+Click selects nodes between the selected node and here.
					TreeNode ndStart = this.m_SelectedNode;
					TreeNode ndEnd = node;

					if( ndStart.Parent == ndEnd.Parent )
					{
						// Selected node and clicked node have same parent, easy case.
						if( ndStart.Index < ndEnd.Index )
						{
							// If the selected node is beneath the clicked node walk down
							// selecting each Visible node until we reach the end.
							while( ndStart != ndEnd )
							{
								ndStart = ndStart.NextVisibleNode;
								if( ndStart == null )
                                {
                                    break;
                                }

                                this.ToggleNode( ndStart, true );
							}
						}
						else if( ndStart.Index == ndEnd.Index )
						{
							// Clicked same node, do nothing
						}
						else
						{
							// If the selected node is above the clicked node walk up
							// selecting each Visible node until we reach the end.
							while( ndStart != ndEnd )
							{
								ndStart = ndStart.PrevVisibleNode;
								if( ndStart == null )
                                {
                                    break;
                                }

                                this.ToggleNode( ndStart, true );
							}
						}
					}
					else
					{
						// Selected node and clicked node have same parent, hard case.
						// We need to find a common parent to determine if we need
						// to walk down selecting, or walk up selecting.

						TreeNode ndStartP = ndStart;
						TreeNode ndEndP = ndEnd;
						int startDepth = Math.Min( ndStartP.Level, ndEndP.Level );

						// Bring lower node up to common depth
						while( ndStartP.Level > startDepth )
						{
							ndStartP = ndStartP.Parent;
						}

						// Bring lower node up to common depth
						while( ndEndP.Level > startDepth )
						{
							ndEndP = ndEndP.Parent;
						}

						// Walk up the tree until we find the common parent
						while( ndStartP.Parent != ndEndP.Parent )
						{
							ndStartP = ndStartP.Parent;
							ndEndP = ndEndP.Parent;
						}

						// Select the node
						if( ndStartP.Index < ndEndP.Index )
						{
							// If the selected node is beneath the clicked node walk down
							// selecting each Visible node until we reach the end.
							while( ndStart != ndEnd )
							{
								ndStart = ndStart.NextVisibleNode;
								if( ndStart == null )
                                {
                                    break;
                                }

                                this.ToggleNode( ndStart, true );
							}
						}
						else if( ndStartP.Index == ndEndP.Index )
						{
							if( ndStart.Level < ndEnd.Level )
							{
								while( ndStart != ndEnd )
								{
									ndStart = ndStart.NextVisibleNode;
									if( ndStart == null )
                                    {
                                        break;
                                    }

                                    this.ToggleNode( ndStart, true );
								}
							}
							else
							{
								while( ndStart != ndEnd )
								{
									ndStart = ndStart.PrevVisibleNode;
									if( ndStart == null )
                                    {
                                        break;
                                    }

                                    this.ToggleNode( ndStart, true );
								}
							}
						}
						else
						{
							// If the selected node is above the clicked node walk up
							// selecting each Visible node until we reach the end.
							while( ndStart != ndEnd )
							{
								ndStart = ndStart.PrevVisibleNode;
								if( ndStart == null )
                                {
                                    break;
                                }

                                this.ToggleNode( ndStart, true );
							}
						}
					}
				}
				else
				{
					// Just clicked a node, select it
					this.SelectSingleNode( node );
				}

				this.OnAfterSelect( new TreeViewEventArgs( this.m_SelectedNode ) );
			}
			finally
			{
				this.EndUpdate();
			}
		}

		private void ClearSelectedNodes()
		{
			try
			{
				foreach( TreeNode node in this.m_SelectedNodes )
				{
					node.BackColor = this.BackColor;
					node.ForeColor = this.ForeColor;
				}
			}
			finally
			{
				this.m_SelectedNodes.Clear();
				this.m_SelectedNode = null;
			}
		}

		private void SelectSingleNode( TreeNode node )
		{
			if( node == null )
			{
				return;
			}

			this.ClearSelectedNodes();
			this.ToggleNode( node, true );
			node.EnsureVisible();
		}

		private void ToggleNode( TreeNode node, bool bSelectNode )
		{
			if( bSelectNode )
			{
				this.m_SelectedNode = node;
				if( !this.m_SelectedNodes.Contains( node ) )
				{
					this.m_SelectedNodes.Add( node );
				}

				node.BackColor = SystemColors.Highlight;
				node.ForeColor = SystemColors.HighlightText;
			}
			else
			{
				this.m_SelectedNodes.Remove( node );
				node.BackColor = this.BackColor;
				node.ForeColor = this.ForeColor;
			}
		}

		private void HandleException( Exception ex )
		{
			// Perform some error handling here.
			// We don't want to bubble errors to the CLR.
			MessageBox.Show( ex.Message );
		}

		#endregion
	}
}
