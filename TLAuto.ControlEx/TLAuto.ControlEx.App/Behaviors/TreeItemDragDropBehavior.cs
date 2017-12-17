// --------------------------
//   Author => Lex XIAO
// --------------------------

#region
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

using TLAuto.BaseEx.Extensions;
using TLAuto.ControlEx.App.Models;
using TLAuto.ControlEx.App.Models.TreeModels;
#endregion

namespace TLAuto.ControlEx.App.Behaviors
{
    public class TreeItemDragDropBehavior : Behavior<TreeView>
    {
        private TreeItemBase _draggedItem;
        private bool _isDrag;
        private Point _lastMouseDown;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.AllowDrop = true;
            AssociatedObject.PreviewMouseLeftButtonDown += AssociatedObject_MouseLeftButtonDown;
            AssociatedObject.MouseLeave += AssociatedObject_MouseLeave;
            AssociatedObject.MouseMove += AssociatedObject_MouseMove;
            AssociatedObject.DragOver += AssociatedObject_DragOver;
            AssociatedObject.Drop += AssociatedObject_Drop;
        }

        private void AssociatedObject_Drop(object sender, DragEventArgs e)
        {
            var source = e.OriginalSource as UIElement;
            if (source != null)
            {
                var targetItem = source.FindVisualParent<TreeViewItem>();
                if ((targetItem != null) && targetItem.DataContext is TreeItemBase && (_draggedItem != null))
                {
                    var target = (TreeItemBase)targetItem.DataContext;
                    _draggedItem.MoveTo(target);
                }
            }
            e.Handled = true;
        }

        private void AssociatedObject_DragOver(object sender, DragEventArgs e)
        {
            var isMoveEffects = false;
            var currentPosition = e.GetPosition(AssociatedObject);
            if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) || (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
            {
                var source = e.OriginalSource as UIElement;
                if (source != null)
                {
                    var item = source.FindVisualParent<TreeViewItem>();
                    if ((item != null) && item.DataContext is TreeItemBase)
                    {
                        //var treeItemBase = (TreeItemBase) item.DataContext;
                        //if (AssociatedObject.SelectedItem != null)
                        //{
                        //    _draggedItem = (TreeItemBase)AssociatedObject.SelectedItem;
                        //    if (_draggedItem != treeItemBase)
                        //    {
                        //        if (!(_draggedItem is ControllerInfo && treeItemBase is ControllerInfo && _draggedItem.Parent == treeItemBase.Parent))
                        //        {
                        e.Effects = DragDropEffects.Move;
                        isMoveEffects = true;
                        //}
                        //}
                        //}
                    }
                }
            }
            if (!isMoveEffects)
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void AssociatedObject_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.LeftButton == MouseButtonState.Pressed) && _isDrag)
            {
                var currentPosition = e.GetPosition(AssociatedObject);
                if ((Math.Abs(currentPosition.X - _lastMouseDown.X) > 10.0) || (Math.Abs(currentPosition.Y - _lastMouseDown.Y) > 10.0))
                {
                    if (AssociatedObject.SelectedItem != null)
                    {
                        _draggedItem = (TreeItemBase)AssociatedObject.SelectedItem;
                        if (!(_draggedItem is ProjectInfo))
                        {
                            DragDrop.DoDragDrop(AssociatedObject, _draggedItem, DragDropEffects.Move);
                        }
                    }
                }
            }
            else
            {
                _isDrag = false;
            }
        }

        private void AssociatedObject_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                _isDrag = false;
            }
        }

        private void AssociatedObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDrag = true;
            _lastMouseDown = e.GetPosition(AssociatedObject);
        }
    }
}