using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VectorEngine;

namespace VectorEngineWPFGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            systemsListBox.ItemsSource = EntityAdmin.Instance.Systems;
            entitiesTreeView.DataContext = EntityAdmin.Instance.Entities;
            sceneGraphTreeView.DataContext = EntityAdmin.Instance.RootTransforms;
        }

        private void SceneGraphTreeViewSelectionChanged(object sender, RoutedPropertyChangedEventArgs<Object> e)
        {
            var newTransform = e.NewValue as Transform;
            if(newTransform != null)
            {
                var newEntity = newTransform.Entity;
                var tvi = Util.Util.FindTviFromObjectRecursive(entitiesTreeView, newEntity);
                if (tvi != null)
                {
                    tvi.IsExpanded = true;

                    // It takes time to expand. And we can't get the TVI until it's finished expanding.
                    // So wait a bit and do this on another thread.
                    var sc = SynchronizationContext.Current;
                    new Thread(new ThreadStart(() =>
                    {
                        Thread.Sleep(50);
                        sc.Send(o =>
                        {
                            var transTvi = Util.Util.FindTviFromObjectRecursive(tvi, newTransform);
                            if (transTvi != null)
                            {
                                transTvi.IsSelected = true;
                                transTvi.BringIntoView();
                            }
                        }, null);
                    })).Start();
                }
            }
        }

        Transform draggedItem;
        TreeViewItem _target;

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    draggedItem = sceneGraphTreeView.SelectedItem as Transform;
                    if (draggedItem != null)
                    {
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(sceneGraphTreeView, sceneGraphTreeView.SelectedValue, DragDropEffects.Move);
                        //Checking target is not null and item is dragging(moving)
                        if (finalDropEffect == DragDropEffects.Move)
                        {
                            if (_target != null)
                            {
                                // A Move drop was accepted
                                var parent = _target.Header as Transform;
                                Transform.AssignParent(draggedItem, parent, true);
                                _target = null;
                                draggedItem = null;
                            }
                            else
                            {
                                Transform.AssignParent(draggedItem, null, true);
                                draggedItem = null;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                // Verify that this is a valid drop and then store the drop target
                TreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);
                if (CheckDropTarget(draggedItem, item))
                {
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                TreeViewItem TargetItem = GetNearestContainer(e.OriginalSource as UIElement);
                if (TargetItem != null && draggedItem != null)
                {
                    _target = TargetItem;
                    e.Effects = DragDropEffects.Move;
                }
            }
            catch (Exception)
            {
            }
        }

        private bool CheckDropTarget(Transform _sourceItem, TreeViewItem _targetItem)
        {
            // Can only drag onto other ones of the same type
            return _targetItem.Header.GetType() == _sourceItem.GetType();
        }

        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = System.Windows.Media.VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }
    }
}
